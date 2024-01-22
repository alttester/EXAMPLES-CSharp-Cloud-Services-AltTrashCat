using OpenQA.Selenium;
using OpenQA.Selenium.Remote;


namespace alttrashcat_tests_csharp.tests
{
    [TestFixture]
    public class BaseTest
    {
        public AltDriver altDriver;
        public RemoteWebDriver driver;

        [OneTimeSetUp]
        public void Init()
        {
            DriverOptions capability = new OpenQA.Selenium.Chrome.ChromeOptions();

            capability.BrowserVersion = "latest";
            capability.AddAdditionalOption("bstack:options", capability);

            driver = new RemoteWebDriver(capability);
            driver.Navigate().GoToUrl("http://localhost:5500/");
            driver.Manage().Window.Maximize();
            altDriver = new AltDriver();
        }

        [OneTimeTearDown]
        public void DisposeAppium()
        {
            Console.WriteLine("Ending");
            altDriver.Stop();
            driver.Quit();
        }
    }
}