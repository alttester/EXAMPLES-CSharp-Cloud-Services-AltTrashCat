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
        private const int TunnelInfoPort = 8000;
        private const string TunnelName = "AltTester";

        [OneTimeSetUp]
        public void SetupAppium()
        {
            string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME");
            string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");
            string LT_APP_URL = Environment.GetEnvironmentVariable("LT_APP_URL");

            StartTunnel(LT_USERNAME, LT_ACCESS_KEY);

            AppiumOptions capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability("user", LT_USERNAME);
            capabilities.AddAdditionalCapability("accessKey", LT_ACCESS_KEY);
            capabilities.AddAdditionalCapability("app", LT_APP_URL);
            capabilities.AddAdditionalCapability("deviceName", "Pixel 8");
            capabilities.AddAdditionalCapability("platformVersion", "14");
            capabilities.AddAdditionalCapability("platformName", "android");
            // capabilities.AddAdditionalCapability("deviceName", "iPhone 14");
            // capabilities.AddAdditionalCapability("platformVersion", "16");
            // capabilities.AddAdditionalCapability("platformName", "ios");
            capabilities.AddAdditionalCapability("build", "TrashCat");
            capabilities.AddAdditionalCapability("name", "tests - " + DateTime.Now.ToString("MMMM dd - HH:mm"));
            capabilities.AddAdditionalCapability("isRealMobile", true);
            capabilities.AddAdditionalCapability("idleTimeout", 300);
            capabilities.AddAdditionalCapability("tunnel", true);
            capabilities.AddAdditionalCapability("tunnelName", TunnelName);

            appiumDriver = new AndroidDriver<AndroidElement>(new Uri("https://mobile-hub.lambdatest.com:443/wd/hub"), capabilities);
            // appiumDriver = new IOSDriver<IOSElement>(new Uri("https://mobile-hub.lambdatest.com:443/wd/hub"), capabilities);

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
            ((IJavaScriptExecutor)appiumDriver).ExecuteScript(
                $"lambdatest_executor: {{\"action\": \"stepcontext\", \"arguments\": {{\"data\": \"{escaped}\", \"level\": \"{level}\"}}}}");
        }

        private void StartTunnel(string user, string accessKey)
        {
            Console.WriteLine("Starting LambdaTest tunnel...");
            tunnelProcess = new Process();
            tunnelProcess.StartInfo.FileName = "LT";
            tunnelProcess.StartInfo.Arguments = $"--user {user} --key {accessKey} --tunnelName {TunnelName} --infoAPIPort {TunnelInfoPort}";
            tunnelProcess.StartInfo.UseShellExecute = false;
            tunnelProcess.StartInfo.RedirectStandardOutput = true;
            tunnelProcess.StartInfo.RedirectStandardError = true;
            tunnelProcess.Start();

            WaitForTunnelReady();
            Console.WriteLine("LambdaTest tunnel is running");
        }

        private void WaitForTunnelReady(int timeoutSeconds = 60)
        {
            using var client = new HttpClient();
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var response = client.GetAsync($"http://127.0.0.1:{TunnelInfoPort}/api/v1.0/info").Result;
                    if (response.IsSuccessStatusCode)
                        return;
                }
                catch { }
                Thread.Sleep(2000);
            }
            throw new Exception("LambdaTest tunnel did not start within timeout");
        }

        private void StopTunnel()
        {
            if (tunnelProcess != null && !tunnelProcess.HasExited)
            {
                Console.WriteLine("Stopping LambdaTest tunnel...");
                tunnelProcess.Kill();
                tunnelProcess.WaitForExit(10000);
                tunnelProcess.Dispose();
                tunnelProcess = null;
                Console.WriteLine("LambdaTest tunnel stopped");
            }
        }

        [SetUp]
        public void TestSetUp()
        {
            Annotate($"Starting test: {TestContext.CurrentContext.Test.Name}");
        }

        // LambdaTest has an idle timeout of max 300 seconds
        // so we need to do something with the appium driver
        // to keep it alive
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
                ((IJavaScriptExecutor)appiumDriver).ExecuteScript("lambda-status=" + status);
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
