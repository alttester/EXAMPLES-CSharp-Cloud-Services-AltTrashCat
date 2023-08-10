namespace alttrashcat_tests_csharp.pages
{
    public class MainMenuPage : BasePage
    {
        public MainMenuPage(AltDriver driver) : base(driver)
        {
        }
        public void LoadScene()
        {
            Driver.LoadScene("Main");
        }
        public AltObject CharacterName { get => Driver.WaitForObject(By.NAME, "CharName", timeout: 10); }
        public AltObject ThemeName { get => Driver.WaitForObject(By.NAME, "ThemeName", timeout: 10); }
        public AltObject ThemeZoneCamera { get => Driver.FindObject(By.NAME, "ThemeZone", By.NAME, "Main Camera"); }
        public AltObject ThemeImage { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/ThemeZone/Image", timeout: 10); }
        public AltObject ThemeSelectorRight { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/ThemeZone/ThemeSelector/ButtonRight"); }
        public AltObject AccessoriesSelectorDown { get => Driver.WaitForObject(By.PATH, "/UICamera/Loadout/AccessoriesSelector/ButtonBottom"); }
        public AltObject LeaderBoardButton { get => Driver.WaitForObject(By.NAME, "OpenLeaderboard", timeout: 10); }
        public AltObject LeaderboardHighScoreName { get => Driver.FindObjectsWhichContain(By.PATH, "/UICamera/Leaderboard/Background/Display/Score/Name")[0]; }
        public AltObject StoreButton { get => Driver.WaitForObject(By.NAME, "StoreButton", timeout: 10); }
        public AltObject MissionButton { get => Driver.WaitForObject(By.NAME, "MissionButton", timeout: 10); }
        public AltObject SettingsButton { get => Driver.WaitForObject(By.NAME, "SettingButton", timeout: 10); }
        public AltObject RunButton { get => Driver.WaitForObject(By.NAME, "StartButton", timeout: 10); }
        public AltObject AltTesterLogo { get => Driver.WaitForObject(By.PATH, "/AltTesterPrefab/AltDialog/Icon", timeout: 10); }
        public bool IsDisplayed()
        {
            if (StoreButton != null && LeaderBoardButton != null && SettingsButton != null && MissionButton != null && RunButton != null && CharacterName != null && ThemeName != null && ThemeImage != null)
                return true;
            return false;
        }
        public void SelectLeaderBoard()
        {
            LeaderBoardButton.Tap();
        }
        public void SetHighScoreName()
        {
            Driver.WaitForObjectWhichContains(By.PATH, "/UICamera/Leaderboard/Background/Display/Score/Name", timeout: 10);
            LeaderboardHighScoreName.SetText("HighScore");
        }
        public void PressStore()
        {
            StoreButton.Tap();
        }
        public void PressSettings()
        {
            SettingsButton.Tap();
        }
        public void PressRun()
        {
            RunButton.Tap();
        }
        /// <summary>
        /// section = character, power, theme
        /// direction = Right, Left
        /// </summary>
        public void TapArrowButton(string section, string direction)
        {
            string path = "/UICamera/Loadout";
            if (section == "character")
                path += $"/CharZone/CharName/CharSelector/Button{direction}";
            if (section == "power")
                path += $"/PowerupZone/Button{direction}";
            if (section == "theme")
                path += $"/ThemeZone/ThemeSelector/Button{direction}";

            Driver.WaitForObject(By.PATH, path, timeout: 5).Tap();
        }
        public void SelectRaccoonCharacter()
        {
            while (GetCharacterName() != "Rubbish Raccoon")
                TapArrowButton("character", "Right");
        }
        public void ChangeAccessory()
        {
            AccessoriesSelectorDown.Tap();
        }
        public void SetResolution(string x, string y, string fullscreen)
        {
            Driver.CallStaticMethod<string>("UnityEngine.Screen", "SetResolution", "UnityEngine.CoreModule", new string[] { x, y, fullscreen }, new string[] { "System.Int32", "System.Int32", "System.Boolean" });
        }
        public string GetCharacterName()
        {
            return CharacterName.GetComponentProperty<string>("UnityEngine.UI.Text", "text", "UnityEngine.UI");
        }
        public void MoveObject(AltObject obj, int xMoving = 20, int yMoving = 20)
        {
            AltVector2 initialPosition = obj.GetScreenPosition();
            int fingerId = Driver.BeginTouch(initialPosition);
            AltVector2 newPosition = new AltVector2(initialPosition.x - xMoving, initialPosition.y + yMoving);
            Driver.MoveTouch(fingerId, newPosition);
            Driver.EndTouch(fingerId);
        }
        public string GetKeyPlayerPref(string key, string setValue)
        {
            Driver.SetKeyPlayerPref(key, setValue);
            return Driver.GetStringKeyPlayerPref(key);
        }
        public List<string> GetAllButtons()
        {
            var allButtons = Driver.FindObjectsWhichContain(By.NAME, "Button");
            allButtons.Add(Driver.FindObjectWhichContains(By.NAME, "Leaderboard"));
            List<string> buttonsNames = new List<string>();
            foreach (var button in allButtons)
            {
                var path = Driver;
                buttonsNames.Add(button.GetComponentProperty<string>("UnityEngine.UI.Button", "name", "UnityEngine.UI"));
            }
            return buttonsNames;
        }
        public bool ButtonsAndTextDisplayedCorrectly()
        {
            bool everythingIsFine = true;
            var textFromPage = Driver.FindObjects(By.NAME, "Text");
            foreach (AltObject textObject in textFromPage)
            {
                string title = textObject.GetComponentProperty<string>("UnityEngine.UI.Text", "text", "UnityEngine.UI");
                string buttonTitle = textObject.GetParent().name;

                switch (title)
                {
                    case "LEADERBOARD":
                        if (buttonTitle != "OpenLeaderboard")
                            everythingIsFine = false;
                        break;
                    case "STORE":
                        if (buttonTitle != "StoreButton")
                            everythingIsFine = false;
                        break;
                    case "MISSIONS":
                        if (buttonTitle != "MissionButton")
                            everythingIsFine = false;
                        break;
                    case "Settings":
                        if (buttonTitle != "SettingButton")
                            everythingIsFine = false;
                        break;
                }
            }
            return everythingIsFine;
        }
    }
}