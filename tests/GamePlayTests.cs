namespace alttrashcat_tests_csharp.tests
{
    public class GamePlayTests : BaseTest
    {
        AltDriver altDriver;
        MainMenuPage mainMenuPage;
        GamePlay gamePlayPage;
        PauseOverlayPage pauseOverlayPage;
        GetAnotherChancePage getAnotherChancePage;
        GameOverScreen gameOverScreen;
        SettingsPage settingsPage;
        StorePage storePage;
        [SetUp]
        public void Setup()
        {
            String HOST_ALT_SERVER = Environment.GetEnvironmentVariable("HOST_ALT_SERVER");
            altDriver = new AltDriver(HOST_ALT_SERVER, port: 13000, connectTimeout: 3000);
            mainMenuPage = new MainMenuPage(altDriver);
            gamePlayPage = new GamePlay(altDriver);
            pauseOverlayPage = new PauseOverlayPage(altDriver);
            getAnotherChancePage = new GetAnotherChancePage(altDriver);
            gameOverScreen = new GameOverScreen(altDriver);
            settingsPage = new SettingsPage(altDriver);
            storePage = new StorePage(altDriver);

            mainMenuPage.LoadScene();
            mainMenuPage.PressRun();
        }

        [Test]
        public void TestGamePlayDisplayedCorrectly()
        {
            Assert.True(gamePlayPage.IsDisplayed());
        }

        [Test]
        public void TestGameCanBePausedAndResumed()
        {
            gamePlayPage.PressPause();
            Assert.True(pauseOverlayPage.IsDisplayed());
            pauseOverlayPage.PressResume();
            Assert.True(gamePlayPage.IsDisplayed());
            mainMenuPage.LoadScene();
        }

        [Test]
        public void TestGameCanBePausedAndStopped()
        {
            gamePlayPage.PressPause();
            pauseOverlayPage.PressMainMenu();
            Assert.True(mainMenuPage.IsDisplayed());
        }

        [Test]
        public void TestAvoidingObstacles()
        {
            gamePlayPage.AvoidObstacles(5);
            Assert.True(gamePlayPage.GetCurrentLife() > 0);
            mainMenuPage.LoadScene();
        }

        [Test]
        public void TestPlayerDiesWhenObstacleNotAvoided()
        {
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
        }

        [Test]
        public void TestGameOverScreenIsAccesible()
        {
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
            getAnotherChancePage.PressGameOver();
            Assert.True(gameOverScreen.IsDisplayed());
        }

        [Test]
        public void TestGetAnotherChanceDisabledWhenNotEnoughCoins()
        {
            gamePlayPage.PressPause();
            pauseOverlayPage.PressMainMenu();
            settingsPage.DeleteData();
            mainMenuPage.PressRun();
            //play game until get another chance is displayed
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
            Assert.IsFalse(getAnotherChancePage.GetAnotherChangeObjectState);
        }

        [Test]
        public void TestThatTrashCatBecomesInvincible()
        {
            Assert.Multiple(() =>
            {
                gamePlayPage.SetCharacterInvincible("True");
                Thread.Sleep(20000);
                altDriver.WaitForObjectNotBePresent(By.NAME, "GameOver");
                //if this fails, at timeout of 20, it means that the object is displayed, thus exit with a timeout
                gamePlayPage.SetCharacterInvincible("False");
                Thread.Sleep(10000);
                Assert.True(getAnotherChancePage.IsDisplayed());
            });
        }
        [Test]
        public void TestAssertCharacterIsMoving()
        {
            Assert.True(gamePlayPage.CharacterIsMoving());
        }

        [Test]
        public void TestMagnetIsUsedInGameplay()
        {
            LoadMainSceneAndGoToStore();
            bool buttonState = storePage.ButtonObjectState(storePage.GetObjectsBuyButton("Items", 0));
            if (buttonState == true)
                storePage.Buy("Items", 0); //buy magnet

            else
            {
                GetMoneyAndGoToTab("Character");
                storePage.ReloadItems();
                storePage.Buy("Items", 0);//buy magnet
            }
            int numOfMagnets = storePage.GetNumberOf(0);

            storePage.CloseStore();

            mainMenuPage.TapArrowButton("power", "Left");
            mainMenuPage.PressRun();
            gamePlayPage.ActivateInGamePowerUp();
            Assert.NotNull(gamePlayPage.PowerUpIcon);

            storePage.LoadScene();
            storePage.GoToTab("Item");
            Assert.True(numOfMagnets - storePage.GetNumberOf(0) == 1);
        }


        [Test]
        public void TestThatLifePowerUpAddsALife()
        {
            LoadMainSceneAndGoToStore();
            bool buttonState = storePage.BuyButtonsState();
            if (buttonState == true)
                storePage.Buy("Items", 3);//buy life
            else
            {
                GetMoneyAndGoToTab("Character");
                storePage.ReloadItems();
                storePage.Buy("Items", 3);//buy life
            }
            storePage.CloseStore();
            mainMenuPage.TapArrowButton("power", "Left");
            mainMenuPage.PressRun();

            while (gamePlayPage.GetCurrentLife() > 1)
            { Thread.Sleep(5); }
            gamePlayPage.ActivateInGamePowerUp();
            gamePlayPage.GetCurrentLife();
            Assert.AreEqual(gamePlayPage.GetCurrentLife(), 2);
        }


        [Test]
        public void TestTheUserCanPlayWithRaccoon()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            mainMenuPage.PressStore();
            GetMoneyAndGoToTab("Character");
            //buys Raccon character
            storePage.Buy("Character", 1);
            storePage.CloseStore();
            mainMenuPage.SelectRaccoonCharacter();
            mainMenuPage.PressRun();
            Thread.Sleep(20);
            Assert.NotNull(gamePlayPage.RacconMesh);
        }

        [Test]
        public void TestThatTheCharacterCanWearAccessories()
        {

            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            mainMenuPage.PressStore();
            GetMoneyAndGoToTab("Accesories");
            storePage.BuyAllFromTab("Accesories");
            storePage.CloseStore();
            mainMenuPage.ChangeAccessory();
            mainMenuPage.PressRun();
            Thread.Sleep(10);
            Assert.NotNull(gamePlayPage.RacconConstructionGear);
        }

        [Test]
        public void TestNightTimeThemeisApplied()
        {
            LoadMainSceneAndGoToStore();
            bool buttonState = storePage.BuyButtonsState();
            if (buttonState == true)
            {
                storePage.GoToTab("Themes");
                storePage.Buy("Themes", 1);
            }
            else
            {
                GetMoneyAndGoToTab("Themes");
                storePage.Buy("Themes", 1);
            }

            storePage.CloseStore();
            Thread.Sleep(100);
            Assert.NotNull(mainMenuPage.ThemeSelectorRight);
            mainMenuPage.TapArrowButton("theme", "Right");
            Thread.Sleep(100);
            mainMenuPage.PressRun();
            Assert.NotNull(gamePlayPage.NightLights);
        }

        [Test]
        public void TestPremiumButtonColorChangesAsExpectedPerState()
        {
            LoadMainSceneAndGoToStore();
            storePage.GetMoreMoney();
            storePage.CloseStore();
            mainMenuPage.PressRun();

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
            Thread.Sleep(1000);

            AltObject PremiumButton = getAnotherChancePage.GetAnotherChanceButton();
            getAnotherChancePage.CompareObjectColorByState(PremiumButton);

            getAnotherChancePage.PremiumButton.PointerDownFromObject();
            Thread.Sleep(1000);

            getAnotherChancePage.CompareObjectColorByState(PremiumButton);
            getAnotherChancePage.PremiumButton.PointerUpFromObject();
            Thread.Sleep(1000);

            getAnotherChancePage.CompareObjectColorByState(PremiumButton);
        }

        [Test]
        public void TestGetWorldPositionTrashCat()
        {
            var Character = gamePlayPage.Character;
            AltVector3 worldPositionCharacter = Character.GetWorldPosition();
            Thread.Sleep(20000);
            AltVector3 worlPositionUpdateObject = Character.UpdateObject().GetWorldPosition();
            Assert.AreNotEqual(worldPositionCharacter.z, worlPositionUpdateObject.z);
        }

        [Test]
        public void TestGetScreenPositionTrashCat()
        {
            var Character = altDriver.FindObject(By.NAME, "CharacterSlot");
            AltVector2 screenPositionCharacter = Character.GetScreenPosition();
            Thread.Sleep(5000);
            gamePlayPage.MoveLeft(gamePlayPage.Character);
            Thread.Sleep(1000);

            AltVector2 screenPositionCharacteraAfterSomeTime = Character.UpdateObject().GetScreenPosition();
            Assert.AreNotEqual(screenPositionCharacter.x, screenPositionCharacteraAfterSomeTime.x);
        }

        [Test]
        public void TestFindObjectWhichContainsWithCamera()
        {
            var characterName = gamePlayPage.CharacterFoundByWhichContainsWithCamera.name;
            Assert.AreEqual(characterName, "CharacterSlot");
        }

        [Test]
        public void TestTimeScale()
        {
            altDriver.SetTimeScale(0.1f);
            Thread.Sleep(1000);
            var timeScaleFromGame = altDriver.GetTimeScale();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0.1f, timeScaleFromGame);
                altDriver.SetTimeScale(1f);
            });
        }

        [Test]
        public void TestDisplayAllEnabledElementsFromAnotherChancePage()
        {
            gamePlayPage.AvoidObstacles(3);
            getAnotherChancePage.DisplayAllEnabledElements();
        }

        [TearDown]
        public void Dispose()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            altDriver.Stop();
            Thread.Sleep(1000);
        }
        //// helping methods
        public void GetMoneyAndGoToTab(string tabName)
        {
            storePage.GetMoreMoney();
            storePage.GoToTab(tabName);
        }
        public void LoadMainSceneAndGoToStore()
        {
            mainMenuPage.LoadScene();
            mainMenuPage.MoveObject(mainMenuPage.AltTesterLogo);
            mainMenuPage.PressStore();
            Thread.Sleep(500);
        }
    }
}