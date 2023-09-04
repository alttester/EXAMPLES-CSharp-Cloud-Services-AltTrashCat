namespace alttrashcat_tests_csharp.pages
{
    public class StorePage : BasePage
    {
        public StorePage(AltDriver driver) : base(driver)
        {
        }
        public void LoadScene()
        {
            Driver.LoadScene("Shop");
        }
        public AltObject CloseButton { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Button"); }
        public AltObject StoreTitleMoneyButton { get => Driver.WaitForObject(By.NAME, "StoreTitle", timeout: 5); }
        public AltObject ItemsTab { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/TabsSwitch/Item"); }
        public AltObject CharactersTab { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/TabsSwitch/Character"); }
        public AltObject AccessoriesTab { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/TabsSwitch/Accesories"); }
        public AltObject ThemesTab { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/TabsSwitch/Themes"); }
        public AltObject BuyButton { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/ItemsList/Container/ItemEntry(Clone)/NamePriceButtonZone/PriceButtonZone/BuyButton"); }
        public AltObject CoinsCounter { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Coin/CoinsCounter"); }
        public AltObject CoinImage { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Coin/Image"); }
        public AltObject PremiumPlusButton { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Premium/Button"); }
        public AltObject PremiumCoinImage { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Premium/Image"); }
        public AltObject PremiumCounter { get => Driver.WaitForObject(By.PATH, "/Canvas/Background/Premium/PremiumCounter"); }
        public AltObject PremiumButtonAtCoordinates { get => Driver.FindObjectAtCoordinates(new AltVector2(PremiumCoinImage.x - 46, PremiumCoinImage.y)); }
        public List<AltObject> ItemCount {get => Driver.FindObjectsWhichContain(By.PATH, "/Canvas/Background/ItemsList/Container/ItemEntry(Clone)/Icon/Count");}
        public bool StoreIsDisplayed()
        {
            if (StoreTitleMoneyButton != null && CloseButton != null && ItemsTab != null && CharactersTab != null && AccessoriesTab != null && ThemesTab != null && BuyButton != null && PremiumPlusButton != null && CoinImage != null && PremiumCoinImage != null)
                return true;
            return false;
        }
        public bool BuyButtonsState()
        {
            bool BuyMagnetState = ButtonObjectState(GetObjectsBuyButton("Items", 0));
            bool BuyMultiplierState = ButtonObjectState(GetObjectsBuyButton("Items", 1));
            bool BuyInvincibleState = ButtonObjectState(GetObjectsBuyButton("Items", 2));
            bool BuyLifeState = ButtonObjectState(GetObjectsBuyButton("Items", 3));
            if (BuyMagnetState && BuyInvincibleState && BuyMultiplierState && BuyLifeState)
                return true;
            else
                return false;
        }
        /// <summary>
        /// tabName = Items, Accesories, Character, Themes
        /// </summary>
        public void Buy(string tabName, int index)
        {
            GetObjectsBuyButton(tabName, index).Tap();
        }
        /// <summary>
        /// tabName = Items, Accesories, Character, Themes
        /// </summary>
        public void BuyAllFromTab(string tabName)
        {
            int index = 0;
            bool goOn = true;
            while (goOn == true)
            {
                try
                {
                    GetObjectsBuyButton(tabName, index).Tap();
                    index += 1;

                }
                catch
                {
                    goOn = false;
                }
            }
        }
        /// <summary>
        /// tabName = Items, Accesories, Character, Themes
        /// </summary>
        public AltObject GetObjectsBuyButton(string tabName, int index, string endPath = "")
        {
            string tabNamePath = GetPathByTabName(tabName);
            Driver.WaitForObjectWhichContains(By.PATH, $"/Canvas/Background/{tabNamePath}/Container/ItemEntry(Clone)/NamePriceButtonZone/PriceButtonZone/BuyButton{endPath}", timeout:50);
            var Objects = Driver.FindObjectsWhichContain(By.PATH, $"/Canvas/Background/{tabNamePath}/Container/ItemEntry(Clone)/NamePriceButtonZone/PriceButtonZone/BuyButton{endPath}");
            return Objects[index];
        }
        /// <summary>
        /// tabName = Item, Character, Accesories, Themes
        /// </summary>
        public void GoToTab(string tabName)
        {
            Driver.WaitForObject(By.NAME, tabName, timeout: 5).Tap();
        }
        /// <summary>
        /// tabName = Items, Accesories, Character, Themes
        /// </summary>
        public string GetPathByTabName(string tabName)
        {
            if (tabName == "Item") return "ItemsList";
            if (tabName == "Character") return "CharacterList";
            if (tabName == "Accesories") return "CharacterAccessoriesList";
            if (tabName == "Themes") return "ThemeList";
            return "";
        }
        public bool CountersReset()
        {
            if (GetTotalAmountOfCoins() == 0 && GetTotalAmountOfPremiumCoins() == 0)
                return true;
            return false;
        }
        public void ReloadItems()
        {
            ItemsTab.Tap();
        }
        public void CloseStore()
        {
            CloseButton.Tap();
        }
        public void GetMoreMoney()
        {
            StoreTitleMoneyButton.Click();
        }
        /// <summary>
        /// indexInPage represents the item's position in the list of items from the page
        /// </summary>
        public int GetNumberOf(int indexInPage)
        {
            return int.Parse(ItemCount[indexInPage].GetText());
        }
        public AltObject GetNameObjectByIndexInPage(string tabName, int index)
        {
            string tabNamePath = GetPathByTabName(tabName);
            Driver.WaitForObjectWhichContains(By.PATH, $"/Canvas/Background/{tabNamePath}/Container/ItemEntry(Clone)/NamePriceButtonZone/Name");
            var Objects = Driver.FindObjectsWhichContain(By.PATH, $"/Canvas/Background/{tabNamePath}/Container/ItemEntry(Clone)/NamePriceButtonZone/Name");
            return Objects[index];
        }
        public int GetPriceOf(string tabName, int index)
        {
            string tabNamePath = GetPathByTabName(tabName);
            var Objects = Driver.FindObjectsWhichContain(By.PATH, $"/Canvas/Background/{tabNamePath}/Container/ItemEntry(Clone)/NamePriceButtonZone/PriceButtonZone/PriceZone/PriceCoin/Amount");
            return int.Parse(Objects[index].GetText());
        }
        public bool DifferentStateWhenPressingBtn()
        {
            int state1 = StoreTitleMoneyButton.CallComponentMethod<int>("UnityEngine.UI.Button", "get_currentSelectionState", "UnityEngine.UI", new object[] { });
            var state2 = StoreTitleMoneyButton.PointerDownFromObject().CallComponentMethod<int>("UnityEngine.UI.Button", "get_currentSelectionState", "UnityEngine.UI", new object[] { });
            if (state1 == state2) return false;
            return true;
        }
        /// <summary>
        /// tabName = Items, Character, Accessories, Themes
        /// index = the position of the element in the list
        /// newName = the value for renaming the element
        /// </summary>
        public string ChangeItemName(string tabName, int index, string newName)
        {

            const string propertyName = "text";
            AltObject NewObject = GetNameObjectByIndexInPage(tabName, index);
            NewObject.SetText(newName, true);

            var propertyValue = NewObject.GetComponentProperty<string>("UnityEngine.UI.Text", propertyName, "UnityEngine.UI");
            return propertyValue;
        }
        public int GetTotalAmountOfCoins()
        {
            string coins = CoinsCounter.GetText();
            return int.Parse(coins);
        }

        public int GetTotalAmountOfPremiumCoins()
        {
            string premiumCoins= PremiumCounter.GetText();
            return int.Parse(premiumCoins);
        }
        public bool AssertOwning(string tabName, int index)
        {
            Thread.Sleep(500);
            var buyBtnText = GetObjectsBuyButton(tabName, index, "/Text");
            if (buyBtnText.GetText() == "Owned")
                return true;
            return false;
        }
        public bool EnableButtonObject(AltObject button)
        {
            button.SetComponentProperty("UnityEngine.UI.Button", "interactable", "True", "UnityEngine.UI");
            return button.GetComponentProperty<bool>("UnityEngine.UI.Button", "interactable", "UnityEngine.UI");
        }
        public bool ButtonObjectState(AltObject button)
        {
            return button.GetComponentProperty<bool>("UnityEngine.UI.Button", "interactable", "UnityEngine.UI");
        }
    }
}
