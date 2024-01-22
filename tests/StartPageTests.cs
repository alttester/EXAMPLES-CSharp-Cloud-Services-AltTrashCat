
namespace alttrashcat_tests_csharp.tests
{
    public class StartPageTests : BaseTest
    {
        private MainMenuPage mainMenuPage;
        private StartPage startPage;
        [SetUp]
        public void Setup()
        {
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
            Thread.Sleep(1000);
        }
    }
}