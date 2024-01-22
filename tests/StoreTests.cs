
namespace alttrashcat_tests_csharp.tests
{
    public class StoreTests : BaseTest
    {
        StorePage storePage;
        MainMenuPage mainMenuPage;
        SettingsPage settingsPage;
        [SetUp]
        public void Setup()
        {
            storePage = new StorePage(altDriver);
            storePage.LoadScene();
            mainMenuPage = new MainMenuPage(altDriver);
            settingsPage = new SettingsPage(altDriver);
        }

        [Test]
        public void TestStoreIsDisplayed()
        {
            Assert.True(storePage.StoreIsDisplayed());
        }

        [Test]
        public void TestGetMoreMoneyAddsSpecificSum()
        {
            int currentAmountOfFishbones = storePage.GetTotalAmountOfCoins();
            int currentAmountOfPremiumCoins = storePage.GetTotalAmountOfPremiumCoins();
            storePage.GetMoreMoney();
            Assert.True(storePage.GetTotalAmountOfCoins() - currentAmountOfFishbones == 1000000);
            Assert.True(storePage.GetTotalAmountOfPremiumCoins() - currentAmountOfPremiumCoins == 1000);
        }

        [Test]
        public void TestBuyButtonsBecomeActiveOnlyWhenEnoughCoins()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            mainMenuPage.PressStore();
            Assert.IsFalse(storePage.BuyButtonsState());
            storePage.GetMoreMoney();
            Thread.Sleep(1000);
            storePage.GoToTab("Character");
            storePage.ReloadItems();
            Thread.Sleep(1000);

            Assert.IsTrue(storePage.BuyButtonsState());
        }

        [Test]
        public void TestBuyMagnetCanBeSetInteractableWithoutEnoughCoins()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            mainMenuPage.PressStore();
            Assert.IsFalse(storePage.BuyButtonsState());
            Thread.Sleep(1000);
            storePage.EnableButtonObject(storePage.GetObjectsBuyButton("Items", 0));
            Thread.Sleep(1000);
            Assert.IsTrue(storePage.EnableButtonObject(storePage.GetObjectsBuyButton("Items", 0)));
        }

        [Test]
        public void TestBuyMagnet()
        {
            AssertBuyItem(0);
        }

        [Test]
        public void TestBuyx2()
        {
            AssertBuyItem(1);
        }

        [Test]
        public void TestBuyInvincible()
        {
            AssertBuyItem(2);
        }

        [Test]
        public void TestBuyLife()
        {
            AssertBuyItem(3);
        }
        [Test]
        public void TestBuyAllItems()
        {
            storePage.GoToTab("Character");
            storePage.GetMoreMoney();
            for (int i = 0; i < 3; i++) AssertBuyItem(i);
        }
        [Test]
        public void TestAssertOwningTrashCatCharacter()
        {
            var tabName = "Character";
            storePage.GoToTab(tabName);
            Assert.True(storePage.AssertOwning(tabName, 0));
        }

        [Test]
        public void TestBuyRaccoon()
        {
            var tabName = "Character";
            storePage.GoToTab("Item");
            storePage.GetMoreMoney();

            storePage.GoToTab(tabName);
            storePage.Buy(tabName, 1);
            Assert.True(storePage.AssertOwning(tabName, 1));
        }

        [Test]
        public void TestBuySafetyHat()
        {
            AssertBuyAccessory(0);
        }

        [Test]
        public void TestBuyPartyHat()
        {
            AssertBuyAccessory(1);
        }
        [Test]
        public void TestBuySmart()
        {
            AssertBuyAccessory(2);
        }
        [Test]
        public void TestBuyAllHats()
        {
            Assert.Multiple(() =>
            {
                AssertBuyAccessory(0);
                AssertBuyAccessory(1);
                AssertBuyAccessory(2);
            });
        }
        [Test]
        public void TestBuyRaccoonAndHats()
        {
            Assert.Multiple(() =>
            {
                TestBuyRaccoon();
                AssertBuyAccessory(3);
                AssertBuyAccessory(4);
            });
        }
        [Test]
        public void TestThatPremiumButtonAtCoordinatesIsFound()
        {
            LoadMainSceneAndGoToStore();
            Assert.AreEqual(storePage.PremiumButtonAtCoordinates.GetText(), "+");
        }
        [Test]
        public void TestKeyPreferancesInStoreMenu()
        {
            Assert.Multiple(() =>
            {
                altDriver.DeletePlayerPref();
                altDriver.SetKeyPlayerPref("test", "TestString");
                var stringVar = altDriver.GetStringKeyPlayerPref("test");
                Assert.AreEqual(stringVar, "TestString");

                altDriver.SetKeyPlayerPref("test", 1);
                var intVar = altDriver.GetIntKeyPlayerPref("test");
                Assert.AreEqual(intVar, 1);

                altDriver.SetKeyPlayerPref("test", 1.0f);
                var floatVar = altDriver.GetFloatKeyPlayerPref("test");
                Assert.AreEqual(floatVar, 1.0f);

                altDriver.DeleteKeyPlayerPref("test");
            });
        }
        [Test]
        public void TestPlayerPrefsWithStaticMethod()
        {
            altDriver.CallStaticMethod<string>("UnityEngine.PlayerPrefs", "SetInt", "UnityEngine.CoreModule", new[] { "Test", "1" });
            int a = altDriver.GetIntKeyPlayerPref("Test");
            Assert.AreEqual(1, a);
        }
        [Test]
        public void TestGetStaticPropertyBrightness()
        {
            float brightness = altDriver.GetStaticProperty<float>("UnityEngine.Screen", "brightness", "UnityEngine.CoreModule");
            Assert.AreEqual(brightness, 1);
        }
        [Test]
        public void TestNewMagnetName()
        {
            string value = "magneeeeeeet";
            string tabName = "Items";
            string newName = storePage.ChangeItemName(tabName, 0, value);
            Assert.AreEqual(value, newName);
        }
        [Test]
        public void TestDifferentColorsOnPressing()
        {
            Assert.True(storePage.DifferentStateWhenPressingBtn());
        }
        [TearDown]
        public void Dispose()
        {
            mainMenuPage.LoadScene();
            settingsPage.DeleteData();
            Thread.Sleep(1000);
        }

        /////// helper methods starting here
        public void AssertBuyItem(int index)
        {
            var tabName = "Item";
            storePage.GetMoreMoney();
            storePage.GoToTab(tabName);

            var moneyAmount = storePage.GetTotalAmountOfCoins();

            var initialNumber = storePage.GetNumberOf(index);
            storePage.Buy(tabName, index);
            Assert.Multiple(() =>
            {
                Assert.True(storePage.GetNumberOf(index) - initialNumber == 1);
                Assert.True(moneyAmount - storePage.GetTotalAmountOfCoins() == storePage.GetPriceOf(tabName, index));
            });
        }

        public void AssertBuyAccessory(int index)
        {
            var tabName = "Accesories";
            storePage.GetMoreMoney();
            storePage.GoToTab(tabName);

            var initialMoneyAmount = storePage.GetTotalAmountOfCoins();
            bool isOwned = storePage.AssertOwning(tabName, index);
            storePage.Buy(tabName, index);
            if (isOwned)
                Assert.True(initialMoneyAmount == storePage.GetTotalAmountOfCoins());

            else
            {
                Assert.True(initialMoneyAmount - storePage.GetPriceOf(tabName, index) == storePage.GetTotalAmountOfCoins());
                Assert.True(storePage.AssertOwning(tabName, index));
            }
        }
        public void LoadMainSceneAndGoToStore()
        {
            mainMenuPage.LoadScene();
            mainMenuPage.PressStore();
        }
    }
}