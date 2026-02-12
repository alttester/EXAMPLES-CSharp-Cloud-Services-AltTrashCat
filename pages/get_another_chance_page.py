import json

from alttester import By

from .base_page import BasePage


class GetAnotherChancePage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    @property
    def game_over_button(self):
        return self.driver.wait_for_object(By.NAME, "GameOver")

    @property
    def premium_button(self):
        return self.driver.wait_for_object(By.NAME, "Premium Button")

    @property
    def available_currency(self):
        return self.driver.wait_for_object(By.NAME, "PremiumOwnCount")

    @property
    def get_another_change_object_state(self):
        return self.premium_button.get_component_property(
            "UnityEngine.UI.Button", "interactable", "UnityEngine.UI"
        )

    def is_displayed(self):
        self.log("GetAnotherChance: Checking if displayed")
        return all([
            self.game_over_button,
            self.premium_button,
            self.available_currency,
        ])

    def press_game_over(self):
        self.log("GetAnotherChance: Pressing Game Over")
        self.game_over_button.tap()

    def press_premium_button(self):
        self.log("GetAnotherChance: Pressing Premium button")
        self.premium_button.tap()

    def get_another_chance_button(self):
        return self.premium_button

    def display_all_enabled_elements(self):
        self.driver.get_all_elements(enabled=True)

    def get_current_state_number(self, button):
        return button.call_component_method(
            "UnityEngine.UI.Button",
            "get_currentSelectionState",
            "UnityEngine.UI",
            [],
        )

    @staticmethod
    def get_state_reference(index):
        mapping = {
            0: "normalColor",
            1: "highlightedColor",
            2: "pressedColor",
            3: "selectedColor",
            4: "disabledColor",
        }
        return mapping.get(index, "")

    def get_premium_button_current_color_rgb(self, color_channel):
        premium_current_color = self.premium_button.call_component_method(
            "UnityEngine.CanvasRenderer",
            "GetColor",
            "UnityEngine.UIModule",
            [],
        )
        if isinstance(premium_current_color, str):
            color_data = json.loads(premium_current_color)
        else:
            color_data = premium_current_color
        return color_data[color_channel]

    def get_premium_button_state_color_rgb(self, state, color_state_channel):
        return self.premium_button.get_component_property(
            "UnityEngine.UI.Button",
            f"colors.{state}.{color_state_channel}",
            "UnityEngine.UI",
        )

    def compare_object_color_by_state(self, button):
        state_number = self.get_current_state_number(button)
        state_name = self.get_state_reference(state_number)

        initial_r = self.get_premium_button_current_color_rgb("r")
        initial_g = self.get_premium_button_current_color_rgb("g")
        initial_b = self.get_premium_button_current_color_rgb("b")

        normal_r = self.get_premium_button_state_color_rgb(state_name, "r")
        normal_g = self.get_premium_button_state_color_rgb(state_name, "g")
        normal_b = self.get_premium_button_state_color_rgb(state_name, "b")

        return initial_r == normal_r and initial_g == normal_g and initial_b == normal_b
