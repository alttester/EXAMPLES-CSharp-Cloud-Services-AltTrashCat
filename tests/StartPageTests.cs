
namespace alttrashcat_tests_csharp.tests
{
    public class StartPageTests : BaseTest
    {
        private AltDriver altDriver;
        private MainMenuPage mainMenuPage;
        private StartPage startPage;
        [SetUp]
        public void Setup()
        {   
            String HOST_ALT_SERVER = Environment.GetEnvironmentVariable("HOST_ALT_SERVER");
            altDriver = new AltDriver(HOST_ALT_SERVER, port: 13000, connectTimeout: 3000);
            startPage = new StartPage(altDriver);
            startPage.Load();
            mainMenuPage = new MainMenuPage(altDriver);

        }
        [Test]
        public void TestStartPageLoadedCorrectly()
        {
            Assert.True(startPage.IsDisplayed());
        }
        [Test]
        public void TestStartButtonLoadMainMenu()
        {
            startPage.PressStart();
            Assert.True(mainMenuPage.IsDisplayed());
        }

        [TearDown]
        public void Dispose()
        {
            altDriver.Stop();
            Thread.Sleep(1000);
        }
    }
}