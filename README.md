# AltTester SDK - Cloud Services Examples (C# / TrashCat)

Run automated Unity game tests on real devices in the cloud using [AltTester Unity SDK](https://alttester.com/). This repository contains examples for multiple cloud device providers, each on its own branch.

All examples use the [TrashCat](https://github.com/AltTester/TrashCat) sample game with C#/NUnit tests and the Page Object Model pattern.

## Cloud Provider Examples

| Cloud Provider | Branch | Description |
|---|---|---|
| **BrowserStack** | [`browserstack-example`](../../tree/browserstack-example) | Run tests on BrowserStack real devices using BrowserStackLocal tunnel |
| **BrowserStack + App Percy** | [`browserstack-app-percy-example`](../../tree/browserstack-app-percy-example) | BrowserStack with visual regression testing via App Percy |
| **BrowserStack WebGL** | [`browserstack_webGL`](../../tree/browserstack_webGL) | Run WebGL build tests on BrowserStack |
| **Sauce Labs** | [`saucelabs_example`](../../tree/saucelabs_example) | Run tests on Sauce Labs real devices |
| **TestMu AI (LambdaTest)** | [`testmu-ai-example`](../../tree/testmu-ai-example) | Run tests on LambdaTest real devices with automatic tunnel management |
| **TestMu AI - Python** | [`testmu-ai-python-example`](../../tree/testmu-ai-python-example) | Python/pytest version of the TestMu AI example |

## How It Works

```
┌──────────────┐       Secure Tunnel         ┌──────────────────────┐
│  Your Machine│◄──────────────────────────► │  Cloud Provider      │
│              │                             │                      │
│  NUnit Tests │   Appium (launch/manage)    │  Real Device         │
│  AltDriver   │───────────────────────────► │  TrashCat app        │
│  Tunnel      │   AltTester (port 13000)    │  (instrumented with  │
│              │◄──────────────────────────► │   AltTester SDK)     │
└──────────────┘                             └──────────────────────┘
```

- **Appium** installs and launches the app on a cloud device
- A **secure tunnel** connects your machine to the cloud device so AltDriver can communicate with the AltTester SDK inside the app over port 13000
- **AltDriver** drives the game UI: find objects, tap, swipe, check state, etc.

## Getting Started

1. Pick a cloud provider from the table above
2. Switch to that branch
3. Follow the README in that branch for setup instructions

Each branch contains:
- Full test suite (pages + tests)
- Cloud-specific `BaseTest.cs` with tunnel and driver setup
- GitHub Actions CI workflow
- Provider-specific README with prerequisites and quick start guide

## Useful Links

- [AltTester Documentation](https://alttester.com/docs/sdk/)
- [AltTester Desktop Downloads](https://alttester.com/downloads/)
- [TrashCat Sample Game](https://github.com/AltTester/TrashCat)
