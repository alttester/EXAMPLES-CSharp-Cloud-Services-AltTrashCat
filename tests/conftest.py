import os
import subprocess
import time
from datetime import datetime

import pytest
from urllib.request import urlopen
from urllib.error import URLError
from appium import webdriver as appium_webdriver
from appium.options.common import AppiumOptions
import warnings
from alttester import AltDriver
from dotenv import load_dotenv

from pages import BasePage

load_dotenv()

TUNNEL_INFO_PORT = 8000
TUNNEL_NAME = "AltTester"


def start_tunnel(username, access_key):
    """Start the TestMu AI (formerly LambdaTest) tunnel binary."""
    print("Starting TestMu AI tunnel...")
    process = subprocess.Popen(
        [
            "LT",
            "--user", username,
            "--key", access_key,
            "--tunnelName", TUNNEL_NAME,
            "--infoAPIPort", str(TUNNEL_INFO_PORT),
        ],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    wait_for_tunnel_ready()
    print("TestMu AI tunnel is running")
    return process


def wait_for_tunnel_ready(timeout_seconds=60):
    """Poll the tunnel info API until it responds successfully."""
    deadline = time.time() + timeout_seconds
    while time.time() < deadline:
        try:
            with urlopen(
                f"http://127.0.0.1:{TUNNEL_INFO_PORT}/api/v1.0/info",
                timeout=5,
            ) as resp:
                if resp.status == 200:
                    return
        except (URLError, OSError):
            pass
        time.sleep(2)
    raise RuntimeError("TestMu AI tunnel did not start within timeout")


def stop_tunnel(process):
    """Stop the TestMu AI tunnel process."""
    if process and process.poll() is None:
        print("Stopping TestMu AI tunnel...")
        process.kill()
        process.wait(timeout=10)
        print("TestMu AI tunnel stopped")


def annotate(appium_driver, message, level="info"):
    """Send a step-context annotation to TestMu AI via lambdatest_executor."""
    escaped = message.replace("\\", "\\\\").replace('"', '\\"')
    appium_driver.execute_script(
        f'lambdatest_executor: {{"action": "stepcontext", '
        f'"arguments": {{"data": "{escaped}", "level": "{level}"}}}}'
    )


@pytest.fixture(scope="session")
def lt_tunnel():
    """Session-scoped fixture: start/stop TestMu AI tunnel."""
    username = os.environ["LT_USERNAME"]
    access_key = os.environ["LT_ACCESS_KEY"]
    process = start_tunnel(username, access_key)
    yield process
    stop_tunnel(process)


@pytest.fixture(scope="class")
def setup(request, lt_tunnel):
    """Class-scoped fixture: Appium driver + AltDriver setup/teardown."""
    username = os.environ["LT_USERNAME"]
    access_key = os.environ["LT_ACCESS_KEY"]
    app_url = os.environ["LT_APP_URL"]

    lt_options = {
        "user": username,
        "accessKey": access_key,
        "app": app_url,
        "deviceName": "Pixel 8",
        "platformVersion": "14",
        "platformName": "android",
        # "deviceName": "iPhone 14",
        # "platformVersion": "16",
        # "platformName": "ios",
        "build": "TrashCat",
        "name": f"tests - {datetime.now().strftime('%B %d - %H:%M')}",
        "isRealMobile": True,
        "idleTimeout": 300,
        "tunnel": True,
        "tunnelName": TUNNEL_NAME,
    }

    options = AppiumOptions()
    options.set_capability("lt:options", lt_options)
    options.set_capability("platformName", "android")

    with warnings.catch_warnings():
        warnings.simplefilter("ignore", UserWarning)
        appium_driver = appium_webdriver.Remote(
            command_executor=f"https://{username}:{access_key}@mobile-hub.lambdatest.com/wd/hub",
            options=options,
        )

    annotate(appium_driver, "Waiting for app to start...")
    time.sleep(30)
    print("Appium driver started")

    annotate(appium_driver, "Connecting AltDriver to AltTester Server...")
    alt_driver = AltDriver()
    annotate(appium_driver, "AltDriver connected")
    print("AltDriver started")

    # Wire up annotation callback for page objects
    BasePage.annotate_callback = lambda msg, lvl="info": annotate(
        appium_driver, msg, lvl
    )

    request.cls.alt_driver = alt_driver
    request.cls.appium_driver = appium_driver

    yield alt_driver

    # Teardown
    try:
        appium_driver.execute_script("lambda-status=passed")
    except Exception as e:
        print(f"Error reporting test status: {e}")

    appium_driver.quit()
    alt_driver.stop()


@pytest.fixture(autouse=True)
def per_test_annotation(request, setup):
    """Per-test fixture: annotate start/end of each test, keep Appium alive."""
    appium_driver = getattr(request.cls, "appium_driver", None)
    test_name = request.node.name

    if appium_driver:
        annotate(appium_driver, f"Starting test: {test_name}")

    yield

    if appium_driver:
        passed = request.node.rep_call.passed if hasattr(request.node, "rep_call") else True
        level = "info" if passed else "error"
        status = "passed" if passed else "failed"
        annotate(appium_driver, f"Finished test: {test_name} - {status}", level)
        # Keep Appium alive (idle timeout is 300s)
        try:
            appium_driver.get_display_density()  # android
            # appium_driver.get_clipboard_text()  # ios
        except Exception:
            pass


@pytest.hookimpl(tryfirst=True, hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """Store test result on the item for the per_test_annotation fixture."""
    outcome = yield
    rep = outcome.get_result()
    setattr(item, f"rep_{rep.when}", rep)
