from alttester import By

from .base_page import BasePage


class MainMenuPage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    def load_scene(self):
        self.log("MainMenu: Loading scene")
        self.driver.load_scene("Main")

    @property
    def character_name(self):
        return self.driver.wait_for_object(By.NAME, "CharName", timeout=10)

    @property
    def theme_name(self):
        return self.driver.wait_for_object(By.NAME, "ThemeName", timeout=10)

    @property
    def theme_zone_camera(self):
        return self.driver.find_object(By.NAME, "ThemeZone", By.NAME, "Main Camera")

    @property
    def theme_image(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/Loadout/ThemeZone/Image", timeout=10
        )

    @property
    def theme_selector_right(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/Loadout/ThemeZone/ThemeSelector/ButtonRight"
        )

    @property
    def accessories_selector_down(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/Loadout/AccessoriesSelector/ButtonBottom"
        )

    @property
    def leader_board_button(self):
        return self.driver.wait_for_object(By.NAME, "OpenLeaderboard", timeout=10)

    @property
    def leaderboard_high_score_name(self):
        return self.driver.find_objects_which_contain(
            By.PATH, "/UICamera/Leaderboard/Background/Display/Score/Name"
        )[0]

    @property
    def store_button(self):
        return self.driver.wait_for_object(By.NAME, "StoreButton", timeout=10)

    @property
    def mission_button(self):
        return self.driver.wait_for_object(By.NAME, "MissionButton", timeout=10)

    @property
    def settings_button(self):
        return self.driver.wait_for_object(By.NAME, "SettingButton", timeout=10)

    @property
    def run_button(self):
        return self.driver.wait_for_object(By.NAME, "StartButton", timeout=10)

    @property
    def alt_tester_logo(self):
        return self.driver.wait_for_object(
            By.PATH, "/AltTesterPrefab/AltDialog/Icon", timeout=10
        )

    def is_displayed(self):
        self.log("MainMenu: Checking if displayed")
        return all([
            self.store_button,
            self.leader_board_button,
            self.settings_button,
            self.mission_button,
            self.run_button,
            self.character_name,
            self.theme_name,
            self.theme_image,
        ])

    def select_leader_board(self):
        self.log("MainMenu: Selecting leaderboard")
        self.leader_board_button.tap()

    def set_high_score_name(self):
        self.log("MainMenu: Setting high score name")
        self.driver.wait_for_object_which_contains(
            By.PATH, "/UICamera/Leaderboard/Background/Display/Score/Name", timeout=10
        )
        self.leaderboard_high_score_name.set_text("HighScore")

    def press_store(self):
        self.log("MainMenu: Pressing Store")
        self.store_button.tap()

    def press_settings(self):
        self.log("MainMenu: Pressing Settings")
        self.settings_button.tap()

    def press_run(self):
        self.log("MainMenu: Pressing Run")
        self.run_button.tap()

    def tap_arrow_button(self, section, direction):
        """section = character, power, theme; direction = Right, Left"""
        self.log(f"MainMenu: Tapping {direction} arrow for {section}")
        path = "/UICamera/Loadout"
        if section == "character":
            path += f"/CharZone/CharName/CharSelector/Button{direction}"
        if section == "power":
            path += f"/PowerupZone/Button{direction}"
        if section == "theme":
            path += f"/ThemeZone/ThemeSelector/Button{direction}"
        self.driver.wait_for_object(By.PATH, path, timeout=5).tap()

    def select_raccoon_character(self):
        self.log("MainMenu: Selecting Raccoon character")
        while self.get_character_name() != "Rubbish Raccoon":
            self.tap_arrow_button("character", "Right")

    def change_accessory(self):
        self.log("MainMenu: Changing accessory")
        self.accessories_selector_down.tap()

    def set_resolution(self, x, y, fullscreen):
        self.log(f"MainMenu: Setting resolution to {x}x{y}")
        self.driver.call_static_method(
            "UnityEngine.Screen",
            "SetResolution",
            "UnityEngine.CoreModule",
            [x, y, fullscreen],
            ["System.Int32", "System.Int32", "System.Boolean"],
        )

    def get_character_name(self):
        return self.character_name.get_component_property(
            "UnityEngine.UI.Text", "text", "UnityEngine.UI"
        )

    def move_object(self, obj, x_moving=20, y_moving=20):
        self.log(f"MainMenu: Moving object {obj.name}")
        initial_position = obj.get_screen_position()
        finger_id = self.driver.begin_touch(initial_position)
        new_position = (initial_position[0] - x_moving, initial_position[1] + y_moving)
        self.driver.move_touch(finger_id, new_position)
        self.driver.end_touch(finger_id)

    def get_key_player_pref(self, key, set_value):
        self.driver.set_key_player_pref(key, set_value)
        return self.driver.get_string_key_player_pref(key)

    def get_all_buttons(self):
        all_buttons = self.driver.find_objects_which_contain(By.NAME, "Button")
        all_buttons.append(
            self.driver.find_object_which_contains(By.NAME, "Leaderboard")
        )
        buttons_names = []
        for button in all_buttons:
            buttons_names.append(
                button.get_component_property(
                    "UnityEngine.UI.Button", "name", "UnityEngine.UI"
                )
            )
        return buttons_names

    def buttons_and_text_displayed_correctly(self):
        everything_is_fine = True
        text_from_page = self.driver.find_objects(By.NAME, "Text")
        for text_object in text_from_page:
            title = text_object.get_component_property(
                "UnityEngine.UI.Text", "text", "UnityEngine.UI"
            )
            button_title = text_object.get_parent().name
            if title == "LEADERBOARD" and button_title != "OpenLeaderboard":
                everything_is_fine = False
            elif title == "STORE" and button_title != "StoreButton":
                everything_is_fine = False
            elif title == "MISSIONS" and button_title != "MissionButton":
                everything_is_fine = False
            elif title == "Settings" and button_title != "SettingButton":
                everything_is_fine = False
        return everything_is_fine
