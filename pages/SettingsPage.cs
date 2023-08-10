namespace alttrashcat_tests_csharp.pages
{
    public class SettingsPage : BasePage
    {
        public SettingsPage(AltDriver driver) : base(driver)
        {
        }
        public AltObject SettingsButton { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/SettingButton", timeout: 10); }
        public AltObject DeleteDataButton { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/SettingPopup/Background/DeleteData", timeout: 10); }
        public AltObject ConfirmYesButton { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/SettingPopup/ConfirmPopup/Image/YESButton", timeout: 10); }
        public AltObject ClosePopUpButton { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/SettingPopup/Background/CloseButton"); }
        public float GetSliderValue(string sliderName)
        {
            var slider = Driver.WaitForObject(By.NAME, sliderName);
            float getSliderValue = slider.GetComponentProperty<float>("UnityEngine.UI.Slider", "value", "UnityEngine.UI");
            return getSliderValue;
        }
        public void MoveSlider(string sliderName, int moveByNumber)
        {
            var sliderHandle = Driver.WaitForObject(By.PATH, "/UICamera/Loadout/SettingPopup/Background/" + sliderName + "/Handle Slide Area/Handle");
            Driver.Swipe(new AltVector2(sliderHandle.x, sliderHandle.y), new AltVector2(sliderHandle.x + moveByNumber, sliderHandle.y));
        }
        public void DeleteData()
        {
            SettingsButton.Tap();
            DeleteDataButton.Tap();
            ConfirmYesButton.Tap();
            ClosePopUpButton.Tap();
        }
    }
}