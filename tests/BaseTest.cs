using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium;
using System.Diagnostics;
using System.Net.Http;

namespace alttrashcat_tests_csharp.tests
{
    public class BaseTest
    {
        public AltDriver altDriver;
        AndroidDriver<AndroidElement> appiumDriver;
        // IOSDriver<IOSElement> appiumDriver;
        private static Process tunnelProcess;
        private const int TunnelApiPort = 8032;
        private const string TunnelName = "alttester-tunnel";

        [OneTimeSetUp]
        public void SetupAppium()
        {
            string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
            string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
            string SAUCE_APP_URL = Environment.GetEnvironmentVariable("SAUCE_APP_URL");
            string SAUCE_REGION = Environment.GetEnvironmentVariable("SAUCE_REGION") ?? "eu-central-1";

            StartTunnel(SAUCE_USERNAME, SAUCE_ACCESS_KEY, SAUCE_REGION);

            AppiumOptions capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability("platformName", "Android");
            // capabilities.AddAdditionalCapability("platformName", "iOS");
            capabilities.AddAdditionalCapability("appium:app", SAUCE_APP_URL);
            capabilities.AddAdditionalCapability("appium:deviceName", "Samsung.*");
            capabilities.AddAdditionalCapability("appium:platformVersion", "");
            // capabilities.AddAdditionalCapability("appium:deviceName", "iPhone 14");
            // capabilities.AddAdditionalCapability("appium:platformVersion", "16");
            capabilities.AddAdditionalCapability("appium:deviceOrientation", "portrait");
            capabilities.AddAdditionalCapability("appium:automationName", "UiAutomator2");
            // capabilities.AddAdditionalCapability("appium:automationName", "XCUITest");
            capabilities.AddAdditionalCapability("appium:newCommandTimeout", 2000);
            capabilities.AddAdditionalCapability("appium:autoGrantPermissions", true);

            var sauceOptions = new Dictionary<string, object>();
            sauceOptions.Add("username", SAUCE_USERNAME);
            sauceOptions.Add("accessKey", SAUCE_ACCESS_KEY);
            sauceOptions.Add("build", "TrashCat");
            sauceOptions.Add("name", "tests - " + DateTime.Now.ToString("MMMM dd - HH:mm"));
            sauceOptions.Add("tunnelIdentifier", TunnelName);
            sauceOptions.Add("tunnelOwner", SAUCE_USERNAME);
            sauceOptions.Add("appiumVersion", "latest");
            capabilities.AddAdditionalCapability("sauce:options", sauceOptions);

            string hubUrl = $"https://ondemand.{SAUCE_REGION}.saucelabs.com:443/wd/hub";
            Console.WriteLine($"Connecting to Sauce Labs at {hubUrl}");
            appiumDriver = new AndroidDriver<AndroidElement>(new Uri(hubUrl), capabilities);
            // appiumDriver = new IOSDriver<IOSElement>(new Uri(hubUrl), capabilities);

            Annotate("Waiting for app to start...");
            Thread.Sleep(30000);
            Console.WriteLine("Appium driver started");
            Annotate("Connecting AltDriver to AltTester Server...");
            altDriver = new AltDriver();
            Annotate("AltDriver connected");
            Console.WriteLine("AltDriver started");
            alttrashcat_tests_csharp.pages.BasePage.AnnotateCallback = Annotate;

            // IWebElement ll = appiumDriver.FindElement(OpenQA.Selenium.By.Id("Allow")); //iOS
            // ll.Click(); //iOS
        }

        public void Annotate(string message, string level = "info")
        {
            var escaped = message.Replace("\\", "\\\\").Replace("\"", "\\\"");
            ((IJavaScriptExecutor)appiumDriver).ExecuteScript($"sauce:context={escaped}");
        }

        private void StartTunnel(string user, string accessKey, string region)
        {
            Console.WriteLine("Starting Sauce Connect tunnel...");
            tunnelProcess = new Process();
            tunnelProcess.StartInfo.FileName = "sc";
            tunnelProcess.StartInfo.Arguments = $"run --username {user} --access-key {accessKey} --region {region} --tunnel-name {TunnelName} --api-address :{TunnelApiPort} --proxy-localhost allow";
            tunnelProcess.StartInfo.UseShellExecute = false;
            tunnelProcess.StartInfo.RedirectStandardOutput = true;
            tunnelProcess.StartInfo.RedirectStandardError = true;
            tunnelProcess.Start();

            WaitForTunnelReady();
            Console.WriteLine("Sauce Connect tunnel is running");
        }

        private void WaitForTunnelReady(int timeoutSeconds = 90)
        {
            using var client = new HttpClient();
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var response = client.GetAsync($"http://127.0.0.1:{TunnelApiPort}/readyz").Result;
                    if (response.IsSuccessStatusCode)
                        return;
                }
                catch { }
                Thread.Sleep(2000);
            }
            throw new Exception("Sauce Connect tunnel did not start within timeout");
        }

        private void StopTunnel()
        {
            if (tunnelProcess != null && !tunnelProcess.HasExited)
            {
                Console.WriteLine("Stopping Sauce Connect tunnel...");
                tunnelProcess.Kill();
                tunnelProcess.WaitForExit(10000);
                tunnelProcess.Dispose();
                tunnelProcess = null;
                Console.WriteLine("Sauce Connect tunnel stopped");
            }
        }

        [SetUp]
        public void TestSetUp()
        {
            Annotate($"Starting test: {TestContext.CurrentContext.Test.Name}");
        }

        [TearDown]
        public void KeepAppiumAlive()
        {
            var testResult = TestContext.CurrentContext.Result.Outcome.Status;
            var level = testResult == NUnit.Framework.Interfaces.TestStatus.Passed ? "info" : "error";
            Annotate($"Finished test: {TestContext.CurrentContext.Test.Name} - {testResult}", level);
            appiumDriver.GetDisplayDensity(); //android
            // appiumDriver.GetClipboardText(); //ios
        }

        [OneTimeTearDown]
        public void DisposeAppium()
        {
            Console.WriteLine("Ending");
            try
            {
                var testResult = TestContext.CurrentContext.Result.Outcome.Status;
                var status = testResult == NUnit.Framework.Interfaces.TestStatus.Passed ? "passed" : "failed";
                ((IJavaScriptExecutor)appiumDriver).ExecuteScript("sauce:job-result=" + status);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reporting test status: " + e.Message);
            }
            appiumDriver.Quit();
            altDriver.Stop();
            StopTunnel();
        }
    }
}
