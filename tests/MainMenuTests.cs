namespace alttrashcat_tests_csharp.tests
{
    public class MainMenuTests : BaseTest
    {
        MainMenuPage mainMenuPage;
        StorePage storePage;
        GamePlay gamePlayPage;
        SettingsPage settingsPage;
        GetAnotherChancePage getAnotherChancePage;

        [SetUp]
        public void Setup()
        {
            mainMenuPage = new MainMenuPage(altDriver);
            gamePlayPage = new GamePlay(altDriver);
            settingsPage = new SettingsPage(altDriver);
            storePage = new StorePage(altDriver);
            getAnotherChancePage = new GetAnotherChancePage(altDriver);
            mainMenuPage.LoadScene();
        }

        [Test]
        public void TestMainMenuPageLoadedCorrectly()
        {
            Assert.True(mainMenuPage.IsDisplayed());
        }

        [Test]
        public void TestNamesOfAllButtonsFromPage()
        {
            List<string> buttonsNames = new List<string>(mainMenuPage.GetAllButtons());
            Assert.True(buttonsNames.Contains("OpenLeaderboard"));
            Assert.True(buttonsNames.Contains("StoreButton"));
            Assert.True(buttonsNames.Contains("MissionButton"));
            Assert.True(buttonsNames.Contains("SettingButton"));
            Assert.True(buttonsNames.Contains("StoreButton"));
            Assert.True(buttonsNames.Contains("StartButton"));
        }
        [Test]
        public void TestButtonsAreCorrectlyDisplayed()
        {
            Assert.True(mainMenuPage.ButtonsAndTextDisplayedCorrectly());
        }
        [Test]
        public void TestDeleteData()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            mainMenuPage.PressStore();
            Assert.True(storePage.CountersReset());
        }
        [Test]
        public void TestLeaderBoardNameHighScoreChanges()
        {
            mainMenuPage.LoadScene();
            mainMenuPage.SelectLeaderBoard();
            mainMenuPage.SetHighScoreName();
            Assert.AreEqual(mainMenuPage.LeaderboardHighScoreName.GetText(), "HighScore");
        }
        [TestCase("MasterSlider")]
        [TestCase("MusicSlider")]
        [TestCase("MasterSFXSlider")]
        ///<summary> 
        ///SliderName can be one of the next three string values: Master, Music,SFX
        ///</summary> 
        public void SliderValuesChangeAsExpected(string sliderName)
        {

            mainMenuPage.LoadScene();
            mainMenuPage.PressSettings();
            settingsPage.MoveSlider(sliderName, -1000); //moves slider to start

            float initialSliderValue = settingsPage.GetSliderValue(sliderName);
            settingsPage.MoveSlider(sliderName, 20);

            float finalSliderValue = settingsPage.GetSliderValue(sliderName);
            Assert.AreNotEqual(initialSliderValue, finalSliderValue);
        }

        [Test]
        public void TestGetParent()
        {
            mainMenuPage.LoadScene();
            Thread.Sleep(100);
            var altObjectParent = mainMenuPage.ThemeZoneCamera.GetParent();
            Assert.AreEqual("Loadout", altObjectParent.name);
        }

        [Test]
        public void TestGetTimeScaleinGame()
        {
            var timeScaleFromGame = altDriver.GetTimeScale();
            altDriver.SetTimeScale(0.1f);
            mainMenuPage.PressRun();
            Thread.Sleep(1000);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0.1f, altDriver.GetTimeScale());
                Thread.Sleep(1000);
                altDriver.SetTimeScale(1);
            });
        }

        [Test]
        public void TestGetCurrentSceneIsMain()
        {
            Assert.AreEqual("Main", mainMenuPage.Driver.GetCurrentScene());
        }

        [Test]
        public void TestGetApplicationScreenSize()
        {
            string initialScreenSizeX = altDriver.GetApplicationScreenSize().x.ToString();
            string initialScreenSizeY = altDriver.GetApplicationScreenSize().y.ToString();
            string resolutionX = "375";
            string resolutionY = "667";

            mainMenuPage.SetResolution(resolutionX, resolutionY, "false");
            var screensize = altDriver.GetApplicationScreenSize();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(resolutionX, screensize.x.ToString());
                mainMenuPage.SetResolution(initialScreenSizeX, initialScreenSizeY, "false");
            });
        }

        [Test]
        public void TestStringKeyPlayerPref()
        {
            string setStringPref = "stringPlayerPrefInTrashcat";
            var stringPlayerPref = mainMenuPage.GetKeyPlayerPref("test", setStringPref);
            Assert.That(stringPlayerPref, Is.EqualTo(setStringPref));
        }
        [Test]
        public void TestDeleteKey()
        {
            mainMenuPage.Driver.DeletePlayerPref();
            mainMenuPage.Driver.SetKeyPlayerPref("test", 1);
            var val = mainMenuPage.Driver.GetIntKeyPlayerPref("test");
            Assert.AreEqual(1, val);
            mainMenuPage.Driver.DeleteKeyPlayerPref("test");
            try
            {
                mainMenuPage.Driver.GetIntKeyPlayerPref("test");
                Assert.Fail();
            }
            catch (NotFoundException exception)
            {
                Assert.AreEqual("PlayerPrefs key test not found", exception.Message);
            }
        }

        [Test]
        public void TestGetServerVersion()
        {
            var serverVersion = altDriver.GetServerVersion();
            Console.WriteLine("App was instrumented with server version: " + serverVersion);
            Assert.That(serverVersion, Is.EqualTo("2.2.5"));
        }

        [Test]
        public void TestGetActiveCameras()
        {
            var listActiveCameras = altDriver.GetAllActiveCameras();
            Assert.That(listActiveCameras.Count, Is.EqualTo(1));
            Assert.That(listActiveCameras[0].name, Is.EqualTo("Main Camera"));
        }

        [Test]
        public void TestGetAllComponents()
        {
            var expectedComponents = ListOfComponentNamesForStoreButton();
            var storeBtnComponentsList = mainMenuPage.StoreButton.GetAllComponents();

            Assert.IsNotEmpty(storeBtnComponentsList);
            for (int index = 0; index <= storeBtnComponentsList.Count - 1; index++)
            {
                Assert.That(storeBtnComponentsList[index].componentName, Is.EqualTo(expectedComponents[index]));
            }
        }
        [Test]
        public void TestGetAllProperties()
        {
            AltComponent testComponent = new AltComponent("UnityEngine.CanvasRenderer", "UnityEngine.UIModule");
            var storeBtnProperties = mainMenuPage.StoreButton.GetAllProperties(testComponent);

            Assert.That(storeBtnProperties.Count, Is.GreaterThan(12));
            Assert.That(storeBtnProperties[0].name, Is.EqualTo("hasPopInstruction"));
            Assert.That(storeBtnProperties[0].value, Is.EqualTo("False"));
        }
        [Test]
        public void TestGetAllFields()
        {
            AltComponent testComponent = new AltComponent("UnityEngine.UI.Button", "UnityEngine.UI");
            var storeBtnFields = mainMenuPage.StoreButton.GetAllFields(testComponent);

            Assert.That(storeBtnFields.Count, Is.EqualTo(2));
            Assert.That(storeBtnFields[0].name, Is.EqualTo("m_OnClick"));
            Assert.That(storeBtnFields[1].name, Is.EqualTo("m_CurrentIndex"));
        }
        [Test]
        public void TestGetAllMethods()
        {
            AltComponent testComponent = new AltComponent("UnityEngine.CanvasRenderer", "UnityEngine.UIModule");
            var storeBtnMethods = mainMenuPage.StoreButton.GetAllMethods(testComponent);

            Assert.That(storeBtnMethods.Count, Is.GreaterThan(80));
            Assert.That(storeBtnMethods[0], Is.EqualTo("Boolean get_hasPopInstruction()"));
            Assert.That(storeBtnMethods[1], Is.EqualTo("Void set_hasPopInstruction(Boolean)"));
        }

        [Test]
        public void TestGetScreenshot()
        {
            var path = "../../../test-screenshot.png";
            altDriver.GetPNGScreenshot(path);
            FileAssert.Exists(path);
        }

        [TearDown]
        public void Dispose()
        {
            Thread.Sleep(1000);
        }

        public static List<string> ListOfComponentNamesForStoreButton()
        {
            var listComponents = new List<string>()
            {
                "UnityEngine.RectTransform",
                "UnityEngine.CanvasRenderer",
                "UnityEngine.UI.Image",
                "UnityEngine.UI.Button",
                "LevelLoader",
                "UnityEngine.AudioSource"
            };
            return listComponents;
        }
    }
}
