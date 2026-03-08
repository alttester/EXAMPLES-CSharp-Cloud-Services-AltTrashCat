using System.Runtime.InteropServices;
using BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace alttrashcat_tests_csharp.tests
{
    public class BaseTest
    {
        public AltDriver altDriver;
        Local browserStackLocal;
        AndroidDriver<AndroidElement> appiumDriver;
        // IOSDriver<IOSElement> appiumDriver;

        [OneTimeSetUp]
        public void SetupAppium()
        {
            String BROWSERSTACK_USERNAME = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME")?.Trim();
            String BROWSERSTACK_ACCESS_KEY = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY")?.Trim();
            String BROWSERSTACK_APP_ID_SDK_202 =
                Environment.GetEnvironmentVariable("BROWSERSTACK_APP_ID_SDK_202")?.Trim() ??
                Environment.GetEnvironmentVariable("BROWSERSTACK_APP_ID_SDK_201")?.Trim();

            if (string.IsNullOrWhiteSpace(BROWSERSTACK_USERNAME) || string.IsNullOrWhiteSpace(BROWSERSTACK_ACCESS_KEY))
            {
                throw new InvalidOperationException("BrowserStack credentials are missing. Set BROWSERSTACK_USERNAME and BROWSERSTACK_ACCESS_KEY.");
            }

            if (string.IsNullOrWhiteSpace(BROWSERSTACK_APP_ID_SDK_202))
            {
                throw new InvalidOperationException("BrowserStack app id is missing. Set BROWSERSTACK_APP_ID_SDK_202 (or BROWSERSTACK_APP_ID_SDK_201) to a valid app_url (bs://...), custom_id, or shareable_id.");
            }

            // Use dot net bindings v4.0.0 or above
            AppiumOptions capabilities = new AppiumOptions();
            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            browserstackOptions.Add("projectName", "TrashCat");
            string buildName = BROWSERSTACK_APP_ID_SDK_202.StartsWith("#file:", StringComparison.OrdinalIgnoreCase)
                ? BROWSERSTACK_APP_ID_SDK_202.Substring("#file:".Length)
                : BROWSERSTACK_APP_ID_SDK_202;
            browserstackOptions.Add("buildName", buildName);
            browserstackOptions.Add("sessionName", "tests - " + DateTime.Now.ToString("MMMM dd - HH:mm"));
            browserstackOptions.Add("local", "true");
            browserstackOptions.Add("idleTimeout", "300");
            browserstackOptions.Add("userName", BROWSERSTACK_USERNAME);
            browserstackOptions.Add("accessKey", BROWSERSTACK_ACCESS_KEY);
            capabilities.AddAdditionalCapability("bstack:options", browserstackOptions);
            capabilities.AddAdditionalCapability("platformName", "android");
            capabilities.AddAdditionalCapability("platformVersion", "11.0");
            capabilities.AddAdditionalCapability("appium:deviceName", "Samsung Galaxy S21");
            // capabilities.AddAdditionalCapability("platformName", "ios");
            // capabilities.AddAdditionalCapability("platformVersion", "16");
            // capabilities.AddAdditionalCapability("appium:deviceName", "iPhone 14");
            capabilities.AddAdditionalCapability("appium:app", BROWSERSTACK_APP_ID_SDK_202);

            browserStackLocal = new Local();
            List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() {
                        new KeyValuePair<string, string>("key", BROWSERSTACK_ACCESS_KEY)
                };

            string localBinaryPath = Environment.GetEnvironmentVariable("BROWSERSTACK_LOCAL_BINARY");
            if (string.IsNullOrWhiteSpace(localBinaryPath) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                localBinaryPath = "/usr/local/bin/BrowserStackLocal";
            }

            if (!string.IsNullOrWhiteSpace(localBinaryPath))
            {
                bsLocalArgs.Add(new KeyValuePair<string, string>("binarypath", localBinaryPath));
            }

            browserStackLocal.start(bsLocalArgs);

            appiumDriver = new AndroidDriver<AndroidElement>(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
            // appiumDriver = new IOSDriver<IOSElement>(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);

            Thread.Sleep(30000);
            Console.WriteLine("Appium driver started");
            altDriver = new AltDriver("192.168.11.35", 13010);
            Console.WriteLine("AltDriver started");

            // IWebElement ll = appiumDriver.FindElement(OpenQA.Selenium.By.Id("Allow")); //iOS
            // ll.Click(); //iOS

        }

        //browserstack has an idle timeout of max 300 seconds
        //so we need to do something with the appium driver
        //to keep it alive
        [TearDown]
        public void KeepAppiumAlive()
        {
            if (appiumDriver != null)
            {
                appiumDriver.GetDisplayDensity(); //android
            }
            // appiumDriver.GetClipboardText(); //ios
        }

        [OneTimeTearDown]
        public void DisposeAppium()
        {
            Console.WriteLine("Ending");
            if (appiumDriver != null)
            {
                appiumDriver.Quit();
            }

            if (altDriver != null)
            {
                altDriver.Stop();
            }

            if (browserStackLocal != null)
            {
                browserStackLocal.stop();
            }
        }
    }
}
