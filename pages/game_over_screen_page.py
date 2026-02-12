from alttester import By

from .base_page import BasePage


class GameOverScreen(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    @property
    def main_menu_button(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/GameOver/Loadout/Text"
        )

    @property
    def run_button(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/GameOver/RunButton/RunText"
        )

    @property
    def game_over_text(self):
        return self.driver.wait_for_object(By.PATH, "/UICamera/GameOver/Text")

    @property
    def highscore_name(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/UICamera/GameOver/Highscore/PlayerEntry/InputField/Text",
            timeout=20,
        )

    def is_displayed(self):
        self.log("GameOverScreen: Checking if displayed")
        return all([
            self.main_menu_button,
            self.run_button,
            self.game_over_text,
            self.highscore_name,
        ])
