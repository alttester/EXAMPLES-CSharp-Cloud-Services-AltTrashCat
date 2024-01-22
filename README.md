This is an example repository for running tests using `AltTester Unity SDK 2.0.2` and BrowserStack Web Automate on WebGL projects. 

## Executing tests using `AltTester Unity SDK 2.0.2` (without BrowserStack).
### Prerequisite

1. Download and install [.NET SDK](https://dotnet.microsoft.com/en-us/download)
2. Have a WebGL build [instrumented with AltTester SDK 2.0.2].
3. Have [AltTester Desktop app, 2.0.2](https://alttester.com/alttester/#pricing) installed (to be able to inspect game).
- For SDK v2.0.2 => need to use AltTester Desktop 2.0.2
4. Add AltTester package:
```
dotnet add package AltTester-Driver --version 2.0.2
```

#### Specific for running on BrowserStack
5. Start http server on port 5500
```
python -m http.server 5500
```
6. Run tests with dotnet
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