from alttester import By

from .base_page import BasePage


class PauseOverlayPage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    @property
    def resume_button(self):
        return self.driver.wait_for_object(By.NAME, "Resume", timeout=2)

    @property
    def main_menu_button(self):
        return self.driver.wait_for_object(By.NAME, "Exit", timeout=2)

    @property
    def title(self):
        return self.driver.wait_for_object(By.NAME, "Text", timeout=2)

    def is_displayed(self):
        self.log("PauseOverlay: Checking if displayed")
        return all([self.resume_button, self.main_menu_button, self.title])

    def press_resume(self):
        self.log("PauseOverlay: Pressing Resume")
        self.resume_button.tap()

    def press_main_menu(self):
        self.log("PauseOverlay: Pressing Main Menu")
        self.main_menu_button.tap()
