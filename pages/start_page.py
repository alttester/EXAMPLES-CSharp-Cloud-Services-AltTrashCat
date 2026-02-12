from alttester import By

from .base_page import BasePage


class StartPage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    def load(self):
        self.log("StartPage: Loading scene")
        self.driver.load_scene("Start")

    @property
    def start_button(self):
        return self.driver.wait_for_object(By.NAME, "StartButton", timeout=5)

    @property
    def start_text(self):
        return self.driver.wait_for_object(By.NAME, "StartText", timeout=5)

    @property
    def logo_image(self):
        return self.driver.wait_for_object(By.NAME, "LogoImage", timeout=5)

    @property
    def unity_url_button(self):
        return self.driver.wait_for_object(By.NAME, "UnityURLButton", timeout=5)

    def is_displayed(self):
        self.log("StartPage: Checking if displayed")
        return all([
            self.start_button,
            self.start_text,
            self.logo_image,
            self.unity_url_button,
        ])

    def press_start(self):
        self.log("StartPage: Pressing Start")
        self.start_button.tap()
