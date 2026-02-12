# AltTester SDK + TestMu AI (LambdaTest) - TrashCat Example

Run automated Unity game tests on real devices in the cloud using [AltTester Unity SDK](https://alttester.com/) and [TestMu AI (LambdaTest)](https://www.lambdatest.com/).

This example uses the [TrashCat](https://github.com/AltTester/TrashCat) sample game to demonstrate how to run C#/NUnit tests on remote Android (or iOS) devices through LambdaTest's real device cloud, with AltTester handling in-game object interaction via a WebSocket tunnel.

## How it works

```
┌──────────────┐     LambdaTest Tunnel     ┌──────────────────────┐
│  Your Machine│◄─────────────────────────► │  LambdaTest Cloud    │
│              │                            │                      │
│  NUnit Tests │   Appium (launch/manage)   │  Real Device         │
│  AltDriver   │──────────────────────────► │  TrashCat app        │
│  LT Tunnel   │   AltTester (port 13000)   │  (instrumented with  │
│              │◄─────────────────────────► │   AltTester SDK)     │
└──────────────┘                            └──────────────────────┘
```

- **Appium** (via LambdaTest) installs and launches the app on a cloud device.
- **LambdaTest Tunnel** creates a secure connection between your machine and the cloud device so that the AltTester driver can communicate with the AltTester SDK inside the app over port 13000.
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
│   └── testmu-ai.yml       # GitHub Actions CI workflow
├── TestAlttrashCSharp.csproj
└── README.md
```

## Prerequisites

1. [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
2. A [LambdaTest](https://www.lambdatest.com/) account (you'll need your username and access key)
3. The [LambdaTest Tunnel binary (`LT`)](https://www.lambdatest.com/support/docs/testing-locally-hosted-pages/) installed and available on your `PATH`
4. A TrashCat `.apk` (Android) or `.ipa` (iOS) **instrumented with AltTester Unity SDK 2.2.5**
   - Follow the [instrumentation tutorial](https://alttester.com/walkthrough-tutorial-upgrading-trashcat-to-2-0-x/#Instrument%20TrashCat%20with%20AltTester%20Unity%20SDK%20v.2.0.x) to build one, or use a pre-built artifact
5. Upload the instrumented app to LambdaTest and note the returned `app_url` — see [LambdaTest app upload docs](https://www.lambdatest.com/support/docs/appium-java/#upload-your-application)

## Quick start — run on LambdaTest cloud

### 1. Set environment variables

```bash
export LT_USERNAME=<your_lambdatest_username>
export LT_ACCESS_KEY=<your_lambdatest_access_key>
export LT_APP_URL=<your_lambdatest_app_url>   # e.g. lt://APP10160251234567890
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
- Starts the LambdaTest Tunnel (waits until it's ready)
- Launches the app on a cloud device via Appium
- Waits for the app to initialize (30 seconds)
- Connects `AltDriver` to the AltTester server inside the app
- Reports pass/fail status back to LambdaTest after the tests complete
- Tears down the tunnel on exit

### 4. View results

Go to the [LambdaTest App Automation dashboard](https://appautomation.lambdatest.com/) to see your test session, device logs, and video recordings.

## Changing the target device

Edit `tests/BaseTest.cs` to change device capabilities. The defaults target a **Pixel 8 (Android 14)**:

```csharp
capabilities.AddAdditionalCapability("deviceName", "Pixel 8");
capabilities.AddAdditionalCapability("platformVersion", "14");
capabilities.AddAdditionalCapability("platformName", "android");
```

For iOS, uncomment the iOS lines and comment out the Android ones:

```csharp
capabilities.AddAdditionalCapability("deviceName", "iPhone 14");
capabilities.AddAdditionalCapability("platformVersion", "16");
capabilities.AddAdditionalCapability("platformName", "ios");
```

You also need to switch the Appium driver type and hub URL in the same file — see the commented-out `IOSDriver` lines.

Browse available devices at [LambdaTest Real Device List](https://www.lambdatest.com/list-of-real-devices-for-mobile-app-testing).

## CI/CD with GitHub Actions

The included workflow (`.github/workflows/testmu-ai.yml`) runs `StartPageTests` on every push to the `testmu-ai-example` branch.

### Required GitHub secrets

| Secret | Description |
|--------|-------------|
| `LT_USERNAME` | Your LambdaTest username |
| `LT_ACCESS_KEY` | Your LambdaTest access key |
| `LT_APP_URL` | The app URL returned after uploading your build to LambdaTest |

> **Note:** The workflow uses `self-hosted` runners. Adjust the `runs-on` value if you want to use GitHub-hosted runners instead.

## Running tests locally (without LambdaTest)

You can also run the tests against a local Android device connected via USB, using AltTester Desktop instead of the cloud.

### Prerequisites

- [AltTester Desktop 2.2.4](https://alttester.com/downloads/) installed and running
- ADB installed and device connected (`adb devices` shows your device)
- The instrumented TrashCat `.apk` installed on the device

### Steps

1. Set up port forwarding:
   ```bash
   adb reverse tcp:13000 tcp:13000
   ```

2. Launch the app:
   ```bash
   adb shell am start -n com.Altom.TrashCat/com.unity3d.player.UnityPlayerActivity
   ```

3. Run tests (you'll need to modify `BaseTest.cs` to skip the Appium/tunnel setup and connect AltDriver directly):
   ```bash
   dotnet test
   ```

4. Stop the app when done:
   ```bash
   adb shell am force-stop com.Altom.TrashCat
   ```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `LambdaTest tunnel did not start within timeout` | Make sure the `LT` binary is on your PATH and your credentials are correct. Check firewall rules. |
| `AltDriver` connection fails | The tunnel may not be forwarding port 13000. Verify the tunnel is running and the app is instrumented with AltTester SDK 2.2.5. |
| App doesn't launch on cloud device | Check that `LT_APP_URL` is valid. Re-upload the app if the URL has expired. |
| Tests time out / idle timeout | LambdaTest has a max idle timeout of 300 seconds. The `KeepAppiumAlive` teardown method pings the Appium driver to prevent this. |
| Wrong SDK / Desktop version | AltTester SDK 2.2.5 requires AltTester Desktop 2.2.4 for local inspection. |

## Useful links

- [AltTester Documentation](https://alttester.com/docs/sdk/)
- [AltTester Desktop Downloads](https://alttester.com/downloads/)
- [LambdaTest App Automation](https://www.lambdatest.com/support/docs/getting-started-with-appium-testing/)
- [LambdaTest Tunnel](https://www.lambdatest.com/support/docs/testing-locally-hosted-pages/)
- [LambdaTest Capability Generator](https://www.lambdatest.com/capabilities-generator/)
