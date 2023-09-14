This branch contains **C# tests for a TrashCat endless runner build**, utilizing the AltTester framework. These tests are set up to execute on Android devices through the **Sauce Labs cloud service**. They will be performed on a build that has been uploaded to a personal Sauce Labs account and is instrumented with a predefined IP address. This IP corresponds to a public virtual machine where AltTester Desktop is downloaded and launched before starting the test execution process.

## Prerequisite

1. Download and install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Create a public virtual machine following the rules from this article. - insert the saucelabs article here
3. Have a TrashCat build [instrumented with AltTester SDK 2.0.*](https://alttester.com/walkthrough-tutorial-upgrading-trashcat-to-2-0-x/#Instrument%20TrashCat%20with%20AltTester%20Unity%20SDK%20v.2.0.x). This build needs to have the predefined IP, the IP address of the previous virtual machine.
4. Have [AltTester Desktop app, 2.0.*](https://alttester.com/alttester/) installed on the virtual machine (to have AltServer waiting for connections from AltDriver).
- For SDK v 2.0.* => need to use AltTester Desktop 2.0.*
5. Add AltTester package:
```
dotnet add package AltTester-Driver --version 2.0.1
```
6. Install Appium WebDriver dependency:
```
dotnet add package Appium.WebDriver --version 4.4.0
```

## Tests execution

1. Execute all tests:

```
dotnet test
```

2. Kill app:
```
adb shell am force-stop com.Altom.TrashCat
```


### Run all tests from a specific class / file

```
dotnet test --filter <test_class_name>
```

### Run only one test from a class

```
dotnet test --filter <test_class_name>.<test_name>
```
Here you can read [other articles from AltTester blog](https://alttester.com/blog/).
