namespace alttrashcat_tests_csharp.pages
{
    public class GameOverScreen : BasePage
    {

        public GameOverScreen(AltDriver driver) : base(driver) { }
        public AltObject MainMenuButton { get => Driver.WaitForObject(By.PATH, "/UICamera/GameOver/Loadout/Text"); }
        public AltObject RunButton { get => Driver.WaitForObject(By.PATH, "/UICamera/GameOver/RunButton/RunText"); }
        public AltObject GameOverText { get => Driver.WaitForObject(By.PATH, "/UICamera/GameOver/Text"); }
        public AltObject HighscoreName { get => Driver.WaitForObject(By.PATH, "/UICamera/GameOver/Highscore/PlayerEntry/InputField/Text", timeout:20); }

        public bool IsDisplayed()
        {
            if (MainMenuButton != null && RunButton != null && GameOverText != null && HighscoreName != null)
                return true;
            return false;
        }
    }
}