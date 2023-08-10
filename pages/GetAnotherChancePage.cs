namespace alttrashcat_tests_csharp.pages
{
    public class GetAnotherChancePage : BasePage
    {
        public GetAnotherChancePage(AltDriver driver) : base(driver)
        {
        }
        public AltObject GameOverButton { get => Driver.WaitForObject(By.NAME, "GameOver"); }
        public AltObject PremiumButton { get => Driver.WaitForObject(By.NAME, "Premium Button"); }
        public AltObject AvailableCurrency { get => Driver.WaitForObject(By.NAME, "PremiumOwnCount"); }
        public bool GetAnotherChangeObjectState { get => PremiumButton.GetComponentProperty<bool>("UnityEngine.UI.Button", "interactable", "UnityEngine.UI"); }
        public bool IsDisplayed()
        {
            if (GameOverButton != null && PremiumButton != null && AvailableCurrency != null)
                return true;
            return false;
        }
        public void PressGameOver()
        {
            GameOverButton.Tap();
        }
        public void PressPremiumButton()
        {
            PremiumButton.Tap();
        }
        public AltObject GetAnotherChanceButton()
        {
            return PremiumButton;
        }
        public void DisplayAllEnabledElements()
        {
            List<AltObject> altObjects = Driver.GetAllElements(enabled: true);
        }
        public int GetCurrentStateNumber(AltObject button)
        {
            int state = button.CallComponentMethod<int>("UnityEngine.UI.Button", "get_currentSelectionState", "UnityEngine.UI", new object[] { });
            return state;
        }
        public string GetStateReference(int index)
        {
            switch (index)
            {
                case 0:
                    return "normalColor";
                case 1:
                    return "highlightedColor";
                case 2:
                    return "pressedColor";
                case 3:
                    return "selectedColor";
                case 4:
                    return "disabledColor";
                default:
                    return "";
            }
        }
        public float GetPremiumButtonCurrentColorRGB(string colorChannel)
        {
            object PremiumCurrentColor = PremiumButton.CallComponentMethod<object>("UnityEngine.CanvasRenderer", "GetColor", "UnityEngine.UIModule", new object[] { });
            string json = JsonConvert.SerializeObject(PremiumCurrentColor);
            dynamic colorData = JsonConvert.DeserializeObject(json);
            float rValue = colorData[colorChannel];
            return rValue;
        }
        public float GetPremiumButtonStateColorRGB(string state, string colorStateChannel)
        {

            float PremiumButtonStateColorRGB = PremiumButton.GetComponentProperty<float>("UnityEngine.UI.Button", "colors." + state + "." + colorStateChannel, "UnityEngine.UI");
            return PremiumButtonStateColorRGB;
        }
        public bool CompareObjectColorByState(AltObject button)
        {
            var stateNumber = GetCurrentStateNumber(button);
            var stateName = GetStateReference(stateNumber);

            var initialButtonColorR = GetPremiumButtonCurrentColorRGB("r");
            var initialButtonColorG = GetPremiumButtonCurrentColorRGB("g");
            var initialButtonColorB = GetPremiumButtonCurrentColorRGB("b");

            var normalColorR = GetPremiumButtonStateColorRGB(stateName, "r");
            var normalColorG = GetPremiumButtonStateColorRGB(stateName, "g");
            var normalColorB = GetPremiumButtonStateColorRGB(stateName, "b");
            try
            {
                Assert.AreEqual(initialButtonColorR, normalColorR);
                Assert.AreEqual(initialButtonColorG, normalColorG);
                Assert.AreEqual(initialButtonColorB, normalColorB);
                return true;
            }
            catch
            {
                return false;
            };
        }

    }
}