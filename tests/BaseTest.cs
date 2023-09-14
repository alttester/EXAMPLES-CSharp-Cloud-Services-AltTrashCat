using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace alttrashcat_tests_csharp.tests
{
    public class BaseTest
    {
        public AltDriver altDriver;
        // IOSDriver<IOSElement> appiumDriver;
        AndroidDriver<AndroidElement> appiumDriver;

        [OneTimeSetUp]
        public void SetupAppiumAndAltDriver()
        {
            String SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
            String SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "Android");
            // options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("appium:app", "storage:filename=TrashCat.apk");
            // options.AddAdditionalCapability("appium:app", "storage:filename=<builName.ipa>");
            options.AddAdditionalCapability("appium:deviceName", "Samsung Galaxy S10 WQHD GoogleAPI Emulator");
            // options.AddAdditionalCapability("appium:deviceName", "iPhone XR");
            options.AddAdditionalCapability("appium:newCommandTimeout", 2000);

            options.AddAdditionalCapability("appium:platformVersion", "11.0");
            // options.AddAdditionalCapability("appium:platformVersion", "16");

            options.AddAdditionalCapability("appium:deviceOrientation", "portrait");
            options.AddAdditionalCapability("appium:automationName", "UiAutomator2");
            // options.AddAdditionalCapability("appium:automationName", "XCUITest");

            var sauceOptions = new Dictionary<string, object>();
            sauceOptions.Add("appiumVersion", "2.0.0");
            sauceOptions.Add("username", SAUCE_USERNAME);
            sauceOptions.Add("accessKey", SAUCE_ACCESS_KEY);
            sauceOptions.Add("build", "TrashCat_build");
            sauceOptions.Add("name", "Test " + DateTime.Now.ToString("dd.MM - HH:mm"));
            options.AddAdditionalCapability("sauce:options", sauceOptions);
            Console.WriteLine("WebDriver request initiated. Waiting for response, this typically takes 2-3 mins");
            // appiumDriver = new IOSDriver<IOSElement>(new Uri("https://ondemand.eu-central-1.saucelabs.com:443/wd/hub"), options);
            appiumDriver = new AndroidDriver<AndroidElement>(new Uri("https://ondemand.eu-central-1.saucelabs.com:443/wd/hub"), options);

            Thread.Sleep(30000);
            Console.WriteLine("Appium driver started");
            
            String HOST_ALT_SERVER = Environment.GetEnvironmentVariable("HOST_ALT_SERVER");
            altDriver = new AltDriver(HOST_ALT_SERVER, connectTimeout: 3000);
            Console.WriteLine("AltDriver started");

            // IWebElement ll = appiumDriver.FindElement(OpenQA.Selenium.By.Id("Allow")); 
            // ll.Click(); 
        }

        [TearDown]
        public void KeepAppiumAlive()
        {
            appiumDriver.GetDisplayDensity();
            // appiumDriver.GetClipboardText();
        }

        [OneTimeTearDown]
        public void DisposeAppiumAndAltDriver()
        {
            Console.WriteLine("Ending");
            appiumDriver.Quit();
            altDriver.Stop();
        }
    }
}