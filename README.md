This branch contains **C# tests for a TrashCat endless runner build**, utilizing the AltTester® framework. These tests are set up to execute on Android and iOS devices through the **Sauce Labs cloud service**. They will be performed on a build that has been uploaded to a personal Sauce Labs account and is instrumented with a predefined IP address. This IP corresponds to a public virtual machine where AltTester® Desktop is downloaded and launched before starting the test execution process.

## Prerequisite

1. Download and install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Have a virtual machine with AltTester® Desktop installed on it, which can either be a Windows virtual machine running AltTester Desktop in GUI mode or a Linux machine running in batch mode (note that the batch mode requires an [AltTester® license](https://alttester.com/alttester/#pricing)). 
3. Have a TrashCat build [instrumented with AltTester® Unity SDK 2.2.5](https://alttester.com/walkthrough-tutorial-upgrading-trashcat-to-2-0-x/#Instrument%20TrashCat%20with%20AltTester%20Unity%20SDK%20v.2.0.x). This build needs to have the predefined IP, the IP address of the previous virtual machine.
4. Have [AltTester® Desktop app, 2.2.4](https://alttester.com/alttester/) installed on the virtual machine (to have AltTester® Server waiting for connections from AltDriver).
- For SDK v 2.2.5 => need to use AltTester® Desktop 2.2.4
5. Add AltTester® package:
```
dotnet add package AltTester-Driver --version 2.2.5
```
6. Install Appium WebDriver dependency:
```
dotnet add package Appium.WebDriver --version 4.4.0
```

Make sure that **AltTester® Desktop is running** on the virtual machine before executing the tests.

Also, make sure that Sauce Labs credentials and VM's IP are set as environment variables:
```
set SAUCE_USERNAME "yourUsername"
set SAUCE_ACCESS_KEY "yourAccessKey"
set HOST_ALT_SERVER "your VM's IP"
```

## Tests execution

Execute all tests:

```
dotnet test
```

### Run all tests from a specific class / file

```
dotnet test --filter <test_class_name>
```

### Run only one test from a class

```
dotnet test --filter <test_class_name>.<test_name>
```
## Other:
Here you can read [other articles from AltTester® blog](https://alttester.com/blog/).
