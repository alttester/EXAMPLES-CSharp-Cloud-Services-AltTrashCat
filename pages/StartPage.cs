namespace alttrashcat_tests_csharp.pages
{
    public class StartPage : BasePage
    {
        public StartPage(AltDriver driver) : base(driver)
        {
        }
        public void Load()
        {
            Log("StartPage: Loading scene");
            Driver.LoadScene("Start");
        }
        public AltObject StartButton { get => Driver.WaitForObject(By.NAME, "StartButton", timeout: 5); }
        public AltObject StartText { get => Driver.WaitForObject(By.NAME, "StartText", timeout: 5); }
        public AltObject LogoImage { get => Driver.WaitForObject(By.NAME, "LogoImage", timeout: 5); }
        public AltObject UnityUrlButton { get => Driver.WaitForObject(By.NAME, "UnityURLButton", timeout: 5); }
        public bool IsDisplayed()
        {
            Log("StartPage: Checking if displayed");
            if (StartButton != null && StartText != null && LogoImage != null && UnityUrlButton != null)
                return true;
            return false;
        }
        public void PressStart()
        {
            Log("StartPage: Pressing Start");
            StartButton.Tap();
        }
    }
}