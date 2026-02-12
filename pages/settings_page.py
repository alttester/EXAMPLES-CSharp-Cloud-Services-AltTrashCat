from alttester import By

from .base_page import BasePage


class SettingsPage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    @property
    def settings_button(self):
        return self.driver.wait_for_object(
            By.PATH, "/UICamera/Loadout/SettingButton", timeout=10
        )

    @property
    def delete_data_button(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/UICamera/Loadout/SettingPopup/Background/DeleteData",
            timeout=10,
        )

    @property
    def confirm_yes_button(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/UICamera/Loadout/SettingPopup/ConfirmPopup/Image/YESButton",
            timeout=10,
        )

    @property
    def close_popup_button(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/UICamera/Loadout/SettingPopup/Background/CloseButton",
        )

    def get_slider_value(self, slider_name):
        slider = self.driver.wait_for_object(By.NAME, slider_name)
        return slider.get_component_property(
            "UnityEngine.UI.Slider", "value", "UnityEngine.UI"
        )

    def move_slider(self, slider_name, move_by_number):
        self.log(f"Settings: Moving slider {slider_name} by {move_by_number}")
        slider_handle = self.driver.wait_for_object(
            By.PATH,
            f"/UICamera/Loadout/SettingPopup/Background/{slider_name}"
            "/Handle Slide Area/Handle",
        )
        self.driver.swipe(
            (slider_handle.x, slider_handle.y),
            (slider_handle.x + move_by_number, slider_handle.y),
        )

    def delete_data(self):
        self.log("Settings: Deleting data")
        self.settings_button.tap()
        self.delete_data_button.tap()
        self.confirm_yes_button.tap()
        self.close_popup_button.tap()
