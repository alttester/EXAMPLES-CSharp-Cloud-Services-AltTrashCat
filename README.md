# AltTester TrashCat Tests - Python + TestMu AI

Automated tests for the TrashCat Unity game using [AltTester SDK](https://alttester.com/alttester/) and [TestMu AI](https://www.lambdatest.com/) (formerly LambdaTest) real device cloud.

> **Note:** TestMu AI was previously known as LambdaTest. The platform's tools, APIs, and environment variables still use the LambdaTest naming (e.g. the `LT` tunnel binary, `LT_USERNAME`, `lambdatest_executor`, `mobile-hub.lambdatest.com`).

## Prerequisites

- Python 3.9+
- [TestMu AI](https://www.lambdatest.com/) account
- [TestMu AI Tunnel (LT)](https://www.lambdatest.com/support/docs/testing-locally-hosted-pages/) binary installed and available in PATH
- A TrashCat `.apk` (or `.ipa`) uploaded to TestMu AI App Automation

## Setup

1. **Clone the repository:**
   ```bash
   git clone <repo-url> -b testmu-ai-python-example
   cd EXAMPLES-CSharp-Cloud-Services-AltTrashCat
   ```

2. **Create a virtual environment and install dependencies:**
   ```bash
   python -m venv venv
   source venv/bin/activate   # or venv\Scripts\activate on Windows
   pip install -r requirements.txt
   ```

3. **Configure environment variables:**

   Copy `.env` and fill in your TestMu AI credentials:
   ```bash
   cp .env .env.local
   ```

   Edit `.env` with your values:
   ```
   LT_USERNAME=your_testmu_ai_username
   LT_ACCESS_KEY=your_testmu_ai_access_key
   LT_APP_URL=lt://your_app_url
   ```

## Running Tests

Run all tests:
```bash
pytest
```

Run a specific test file:
```bash
pytest tests/test_start_page.py -v
```

Run a specific test:
```bash
pytest tests/test_main_menu.py::TestMainMenu::test_main_menu_page_loaded_correctly -v
```

## Project Structure

```
├── pages/                    # Page Object Model
│   ├── base_page.py          # Base class with annotation support
│   ├── start_page.py         # Start screen page object
│   ├── main_menu_page.py     # Main menu page object
│   ├── game_play_page.py     # Gameplay page object (obstacle avoidance, etc.)
│   ├── pause_overlay_page.py # Pause overlay page object
│   ├── get_another_chance_page.py  # Revive screen page object
│   ├── settings_page.py      # Settings popup page object
│   ├── store_page.py         # Store/shop page object
│   └── game_over_screen_page.py    # Game over screen page object
├── tests/                    # Test suites
│   ├── conftest.py           # Pytest fixtures (TestMu AI tunnel, Appium, AltDriver)
│   ├── test_start_page.py    # Start page tests
│   ├── test_main_menu.py     # Main menu tests
│   ├── test_game_play.py     # Gameplay tests
│   ├── test_store.py         # Store tests
│   └── test_user_journey.py  # End-to-end user journey tests
├── requirements.txt
├── pytest.ini
└── .env                      # Environment variable template
```

## How It Works

1. **TestMu AI Tunnel**: The `conftest.py` session fixture starts the `LT` tunnel binary, which creates a secure connection between the local machine and TestMu AI cloud devices. This allows the AltDriver to communicate with the AltTester Server running inside the app on the device.

2. **Appium + TestMu AI**: An Appium remote session is created on TestMu AI's hub. The TrashCat app is installed and launched on a real device.

3. **AltDriver**: After the app starts (30s wait), the AltDriver connects to the AltTester Server via the tunnel and enables test automation through the AltTester protocol.

4. **Annotations**: Test steps are reported to TestMu AI's dashboard via `lambdatest_executor` JavaScript execution, providing visibility into what each test is doing.

## Device Configuration

By default, tests run on **Pixel 8 (Android 14)**. To change the device, modify the capabilities in `tests/conftest.py`:

```python
# Android
options.set_capability("deviceName", "Pixel 8")
options.set_capability("platformVersion", "14")
options.set_capability("platformName", "android")

# iOS (uncomment and adjust)
# options.set_capability("deviceName", "iPhone 14")
# options.set_capability("platformVersion", "16")
# options.set_capability("platformName", "ios")
```
