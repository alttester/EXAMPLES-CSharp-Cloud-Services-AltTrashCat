# AltTester SDK + Sauce Labs - TrashCat Example

Run automated Unity game tests on real devices in the cloud using [AltTester Unity SDK](https://alttester.com/) and [Sauce Labs](https://saucelabs.com/).

This example uses the [TrashCat](https://github.com/AltTester/TrashCat) sample game to demonstrate how to run C#/NUnit tests on remote Android (or iOS) devices through Sauce Labs' real device cloud, with AltTester handling in-game object interaction via Sauce Connect tunnel.

## How it works

```
┌──────────────┐      Sauce Connect          ┌──────────────────────┐
│  Your Machine│◄──────────────────────────► │  Sauce Labs Cloud    │
│              │                             │                      │
│  NUnit Tests │   Appium (launch/manage)    │  Real Device         │
│  AltDriver   │───────────────────────────► │  TrashCat app        │
│  sc (tunnel) │   AltTester (port 13000)    │  (instrumented with  │
│              │◄──────────────────────────► │   AltTester SDK)     │
└──────────────┘                             └──────────────────────┘
```

- **Appium** (via Sauce Labs) installs and launches the app on a cloud device.
- **Sauce Connect** (`sc`) creates a secure tunnel between your machine and the cloud device. The `--proxy-localhost allow` flag is critical — it lets the AltTester SDK inside the app reach localhost:13000 on your machine through the tunnel.
- **AltDriver** connects to the AltTester server running inside the game and drives the UI (find objects, tap, check state, etc.).

## Project structure

```
├── pages/                  # Page Object classes
│   ├── BasePage.cs
│   ├── StartPage.cs
│   ├── MainMenuPage.cs
│   ├── GamePlayPage.cs
│   ├── StorePage.cs
│   └── ...
├── tests/                  # NUnit test classes
│   ├── BaseTest.cs         # Appium + AltDriver setup/teardown, tunnel management
│   ├── StartPageTests.cs
│   ├── MainMenuTests.cs
│   ├── GamePlayTests.cs
│   ├── StoreTests.cs
│   └── UserJourneyTests.cs
├── .github/workflows/
│   └── saucelabs.yml       # GitHub Actions CI workflow
├── TestAlttrashCSharp.csproj
└── README.md
```

## Prerequisites

1. [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
2. A [Sauce Labs](https://saucelabs.com/) account (you'll need your username and access key)
3. [Sauce Connect Proxy (`sc`)](https://docs.saucelabs.com/secure-connections/sauce-connect-5/) installed and available on your `PATH`
4. A TrashCat `.apk` (Android) or `.ipa` (iOS) **instrumented with AltTester Unity SDK 2.2.5**
   - Follow the [instrumentation tutorial](https://alttester.com/walkthrough-tutorial-upgrading-trashcat-to-2-0-x/#Instrument%20TrashCat%20with%20AltTester%20Unity%20SDK%20v.2.0.x) to build one, or use a pre-built artifact
5. Upload the instrumented app to Sauce Labs — see [Sauce Labs app upload docs](https://docs.saucelabs.com/mobile-apps/app-storage/)

## Quick start — run on Sauce Labs cloud

### 1. Set environment variables

```bash
export SAUCE_USERNAME=<your_saucelabs_username>
export SAUCE_ACCESS_KEY=<your_saucelabs_access_key>
export SAUCE_APP_URL=storage:filename=TrashCat.apk
export SAUCE_REGION=eu-central-1   # or us-west-1
```

### 2. Install dependencies

```bash
dotnet restore
```

### 3. Run tests

Run all tests:
```bash
dotnet test
```

Run a specific test class:
```bash
dotnet test --filter StartPageTests
```

Run a single test:
```bash
dotnet test --filter StartPageTests.TestStartPageLoadedCorrectly
```

The `BaseTest` class automatically:
- Starts the Sauce Connect tunnel with `--proxy-localhost allow` (waits until ready)
- Launches the app on a cloud device via Appium
- Waits for the app to initialize (30 seconds)
- Connects `AltDriver` to the AltTester server inside the app
- Logs step annotations visible in the Sauce Labs dashboard (via `sauce:context`)
- Reports pass/fail status back to Sauce Labs after the tests complete
- Tears down the tunnel on exit

### 4. View results

Go to the [Sauce Labs dashboard](https://app.saucelabs.com/) to see your test session, device logs, video recordings, and step annotations.

## Changing the target device

Edit `tests/BaseTest.cs` to change device capabilities. The defaults target a **Google Pixel 8 (Android 14)**:

```csharp
capabilities.AddAdditionalCapability("appium:deviceName", "Google Pixel 8");
capabilities.AddAdditionalCapability("appium:platformVersion", "14");
```

For iOS, uncomment the iOS lines and comment out the Android ones:

```csharp
capabilities.AddAdditionalCapability("appium:deviceName", "iPhone 14");
capabilities.AddAdditionalCapability("appium:platformVersion", "16");
capabilities.AddAdditionalCapability("appium:automationName", "XCUITest");
```

You also need to switch the Appium driver type — see the commented-out `IOSDriver` lines.

## CI/CD with GitHub Actions

The included workflow (`.github/workflows/saucelabs.yml`) runs `StartPageTests` on every push to the `saucelabs_example` branch.

### Required GitHub secrets

| Secret | Description |
|--------|-------------|
| `SAUCE_USERNAME` | Your Sauce Labs username |
| `SAUCE_ACCESS_KEY` | Your Sauce Labs access key |
| `SAUCE_APP_URL` | The app storage reference (e.g. `storage:filename=TrashCat.apk`) |
| `SAUCE_REGION` | Sauce Labs region (`eu-central-1` or `us-west-1`) |

> **Note:** The workflow uses `self-hosted` runners. Adjust the `runs-on` value if you want to use GitHub-hosted runners instead.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `Sauce Connect tunnel did not start within timeout` | Make sure `sc` is on your PATH and your credentials are correct. Check firewall rules. |
| `AltDriver` connection fails | Verify the tunnel is running with `--proxy-localhost allow`. This flag is critical for AltTester. |
| App doesn't launch on cloud device | Check that `SAUCE_APP_URL` is valid. Re-upload the app if needed. |
| Tests time out | Sauce Labs has a `newCommandTimeout` of 2000 seconds by default. The `KeepAppiumAlive` teardown pings the Appium driver between tests. |

## Useful links

- [AltTester Documentation](https://alttester.com/docs/sdk/)
- [AltTester Desktop Downloads](https://alttester.com/downloads/)
- [Sauce Labs Real Device Testing](https://docs.saucelabs.com/mobile-apps/automated-testing/appium/)
- [Sauce Connect Proxy](https://docs.saucelabs.com/secure-connections/sauce-connect-5/)
- [Sauce Labs Platform Configurator](https://saucelabs.com/products/platform-configurator)
