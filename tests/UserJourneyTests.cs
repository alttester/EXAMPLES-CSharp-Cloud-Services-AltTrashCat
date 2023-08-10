

namespace alttrashcat_tests_csharp.tests
{

    public class UserJourneyTests : BaseTest
    {
        AltDriver altDriver;
        MainMenuPage mainMenuPage;
        GamePlay gamePlay;
        PauseOverlayPage pauseOverlayPage;
        GetAnotherChancePage getAnotherChancePage;
        GameOverScreen gameOverScreen;
        SettingsPage settingsPage;
        StartPage startPage;
        StorePage storePage;

        [SetUp]
        public void Setup()
        {
            altDriver = new AltDriver();
            mainMenuPage = new MainMenuPage(altDriver);
            gamePlay = new GamePlay(altDriver);
            pauseOverlayPage = new PauseOverlayPage(altDriver);
            getAnotherChancePage = new GetAnotherChancePage(altDriver);
            gameOverScreen = new GameOverScreen(altDriver);
            settingsPage = new SettingsPage(altDriver);
            startPage = new StartPage(altDriver);
            storePage = new StorePage(altDriver);
            mainMenuPage.LoadScene();
        }

        [Test]
        public void UserJourneyPlayandPause()
        {
            Assert.Multiple(() =>
       {
           //User opens the game
           mainMenuPage.PressRun();
           Assert.True(gamePlay.IsDisplayed());
           gamePlay.AvoidObstacles(5);
           Assert.True(gamePlay.GetCurrentLife() > 0);
           //User pauses the game 
           gamePlay.PressPause();
           Assert.True(pauseOverlayPage.IsDisplayed());
           pauseOverlayPage.PressResume();
           Assert.True(gamePlay.IsDisplayed());
           float timeout = 20;
           while (timeout > 0)
           {
               try
               {
                   getAnotherChancePage.IsDisplayed();
                   break;
               }
               catch (Exception)
               {
                   timeout -= 1;
               }
           }
           //Character dies and game over screen is displayed
           getAnotherChancePage.PressGameOver();
           Assert.True(gameOverScreen.IsDisplayed());
       });
        }
        [Test]
        public void UserJourneyBuyItems()
        {
            Assert.Multiple(() =>
            {
                //delete current game data
                settingsPage.DeleteData();
                mainMenuPage.PressStore();
                // verify if buttons are disabled when no money
                Assert.IsFalse(storePage.BuyButtonsState());
                storePage.GetMoreMoney();
                Thread.Sleep(1000);
                storePage.GoToTab("Character");
                storePage.ReloadItems();
                Thread.Sleep(1000);
                //get coins by pressing Store and verify buttons get enabled 
                Assert.IsTrue(storePage.BuyButtonsState());
                //buy magnet and night theme
                storePage.Buy("Items", 0);//buy magnet
                storePage.GoToTab("Themes");
                storePage.Buy("Themes", 1);
                storePage.CloseStore();
                //mainMenuPage.MovePowerUpLeft();
                mainMenuPage.TapArrowButton("power", "Left");
                Assert.NotNull(mainMenuPage.ThemeSelectorRight);
                mainMenuPage.TapArrowButton("theme", "Right");
                Thread.Sleep(100);
                //verify bought items are available in game
                mainMenuPage.PressRun();
                Assert.NotNull(gamePlay.InGamePowerUp);
                Assert.NotNull(gamePlay.NightLights);
                gamePlay.ActivateInGamePowerUp();
                Assert.NotNull(gamePlay.PowerUpIcon);
            });
        }
        [Test]
        public void UserJourneyReviveAndGetASecondChance()
        {
            Assert.Multiple(() =>
            {
                settingsPage.DeleteData();
                mainMenuPage.PressStore();
                // verify if buttons are disabled when no money
                Assert.IsFalse(storePage.BuyButtonsState());
                storePage.GetMoreMoney();
                Thread.Sleep(1000);
                storePage.GoToTab("Character");
                storePage.ReloadItems();
                Thread.Sleep(1000);
                //get coins by pressing Store and verify buttons get enabled 
                Assert.IsTrue(storePage.BuyButtonsState());
                storePage.Buy("Items", 3);//buy life
                storePage.CloseStore();
                mainMenuPage.TapArrowButton("power", "Left");
                mainMenuPage.PressRun();

                while (gamePlay.GetCurrentLife() > 1)
                { Thread.Sleep(5); }

                gamePlay.ActivateInGamePowerUp();
                Thread.Sleep(5);
                gamePlay.GetCurrentLife();
                Assert.AreEqual(gamePlay.GetCurrentLife(), 2);
                float timeout = 20;
                while (timeout > 0)
                {
                    try
                    {
                        getAnotherChancePage.IsDisplayed();
                        break;
                    }
                    catch (Exception)
                    {
                        timeout -= 1;
                    }
                }
                Assert.True(getAnotherChancePage.IsDisplayed());
                getAnotherChancePage.PressPremiumButton();
                while (timeout > 0)
                {
                    try
                    {
                        gameOverScreen.IsDisplayed();
                        break;
                    }
                    catch (Exception)
                    {
                        timeout -= 1;
                    }
                }

                Assert.True(gameOverScreen.IsDisplayed());
            });
        }
        [Test]
        public void TestTheNumberOfAllEnabledElementsFromDifferentPagesIsDifferent()
        {

            var mainMenuPageEnabledElements = altDriver.GetAllElements(enabled: true);
            mainMenuPage.PressRun();
            Thread.Sleep(1000);
            var gamePlayPageEnabledElements = altDriver.GetAllElements(enabled: true);

            Assert.AreNotEqual(mainMenuPageEnabledElements.Count, gamePlayPageEnabledElements.Count);
        }
        [Test]
        public void TestTheNumberOfAllDisabledElementsFromDifferentPagesIsDifferent()
        {
            mainMenuPage.LoadScene();
            var mainMenuPageDisabledElements = altDriver.GetAllElements(enabled: false);
            mainMenuPage.PressRun();
            Thread.Sleep(1000);
            var gamePlayPageDisabledElements = altDriver.GetAllElements(enabled: false);

            Assert.AreNotEqual(mainMenuPageDisabledElements.Count, gamePlayPageDisabledElements.Count);
        }
        [Test]
        public void TestMethodsThatHandleScenes()
        {
            System.Collections.Generic.List<string> loadedSceneNames = altDriver.GetAllLoadedScenes();
            Assert.AreEqual(loadedSceneNames[0], "Main");
            Assert.AreEqual("Main", altDriver.GetCurrentScene());

            mainMenuPage.PressStore();

            System.Collections.Generic.List<string> loadedSceneNamesAfterStore = altDriver.GetAllLoadedScenes();

            altDriver.UnloadScene("Main");

            Assert.AreEqual("Shop", altDriver.GetCurrentScene());

            altDriver.LoadScene("Shop", true);

            Assert.AreEqual("Shop", altDriver.GetCurrentScene());

            Assert.AreEqual(altDriver.GetAllLoadedScenes()[0], "Shop");
            mainMenuPage.LoadScene();
        }
        [TearDown]
        public void Dispose()
        {
            altDriver.Stop();
            Thread.Sleep(1000);
        }
    }
}