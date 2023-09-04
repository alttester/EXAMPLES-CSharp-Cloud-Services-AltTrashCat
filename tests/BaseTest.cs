using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace alttrashcat_tests_csharp.tests
{
    public class BaseTest
    {
        AltDriver altDriver;
        AndroidDriver<AndroidElement> appiumDriver;

        [OneTimeSetUp]
        public void SetupAppium()
        {
            String SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
            String SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "Android");
            options.AddAdditionalCapability("appium:app", "storage:filename=TrashCatNoTut20.107.70.8.apk");
            options.AddAdditionalCapability("appium:deviceName", "Samsung Galaxy S10 WQHD GoogleAPI Emulator");
            options.AddAdditionalCapability("appium:newCommandTimeout", 2000);

            options.AddAdditionalCapability("appium:platformVersion", "11.0");

            options.AddAdditionalCapability("appium:deviceOrientation", "portrait");
            options.AddAdditionalCapability("appium:automationName", "UiAutomator2");

            var sauceOptions = new Dictionary<string, object>();
            sauceOptions.Add("appiumVersion", "2.0.0");
            sauceOptions.Add("username", SAUCE_USERNAME);
            sauceOptions.Add("accessKey", SAUCE_ACCESS_KEY);
            sauceOptions.Add("build", "TrashCat_20.107.70.8_notutorial");
            sauceOptions.Add("name", "Test " + DateTime.Now.ToString("dd.MM - HH:mm"));
            options.AddAdditionalCapability("sauce:options", sauceOptions);
            Console.WriteLine("WebDriver request initiated. Waiting for response, this typically takes 2-3 mins");
            appiumDriver = new AndroidDriver<AndroidElement>(new Uri("https://ondemand.eu-central-1.saucelabs.com:443/wd/hub"), options);
            Thread.Sleep(30000);
            Console.WriteLine("Appium driver started");
        }

        [TearDown]
        public void KeepAppiumAlive()
        {
            appiumDriver.GetDisplayDensity();
        }

        [OneTimeTearDown]
        public void DisposeAppium()
        {
            Console.WriteLine("Ending");
            appiumDriver.Quit();
        }
    }
}