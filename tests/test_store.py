import time

import pytest

from pages import MainMenuPage, SettingsPage, StorePage


@pytest.mark.usefixtures("setup")
class TestStore:
    def setup_method(self):
        self.store_page = StorePage(self.alt_driver)
        self.store_page.load_scene()
        self.main_menu_page = MainMenuPage(self.alt_driver)
        self.settings_page = SettingsPage(self.alt_driver)

    def test_store_is_displayed(self):
        assert self.store_page.store_is_displayed()

    def test_get_more_money_adds_specific_sum(self):
        current_fishbones = self.store_page.get_total_amount_of_coins()
        current_premium = self.store_page.get_total_amount_of_premium_coins()
        self.store_page.get_more_money()
        assert self.store_page.get_total_amount_of_coins() - current_fishbones == 1000000
        assert self.store_page.get_total_amount_of_premium_coins() - current_premium == 1000

    def test_buy_buttons_become_active_only_when_enough_coins(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        assert not self.store_page.buy_buttons_state()
        self.store_page.get_more_money()
        time.sleep(1)
        self.store_page.go_to_tab("Character")
        self.store_page.reload_items()
        time.sleep(1)
        assert self.store_page.buy_buttons_state()

    def test_buy_magnet_can_be_set_interactable_without_enough_coins(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        assert not self.store_page.buy_buttons_state()
        time.sleep(1)
        self.store_page.enable_button_object(
            self.store_page.get_objects_buy_button("Items", 0)
        )
        time.sleep(1)
        assert self.store_page.enable_button_object(
            self.store_page.get_objects_buy_button("Items", 0)
        )

    def test_buy_magnet(self):
        self._assert_buy_item(0)

    def test_buy_x2(self):
        self._assert_buy_item(1)

    def test_buy_invincible(self):
        self._assert_buy_item(2)

    def test_buy_life(self):
        self._assert_buy_item(3)

    def test_buy_all_items(self):
        self.store_page.go_to_tab("Character")
        self.store_page.get_more_money()
        for i in range(3):
            self._assert_buy_item(i)

    def test_assert_owning_trash_cat_character(self):
        tab_name = "Character"
        self.store_page.go_to_tab(tab_name)
        assert self.store_page.assert_owning(tab_name, 0)

    def test_buy_raccoon(self):
        tab_name = "Character"
        self.store_page.go_to_tab("Item")
        self.store_page.get_more_money()
        self.store_page.go_to_tab(tab_name)
        self.store_page.buy(tab_name, 1)
        assert self.store_page.assert_owning(tab_name, 1)

    def test_buy_safety_hat(self):
        self._assert_buy_accessory(0)

    def test_buy_party_hat(self):
        self._assert_buy_accessory(1)

    def test_buy_smart(self):
        self._assert_buy_accessory(2)

    def test_buy_all_hats(self):
        self._assert_buy_accessory(0)
        self._assert_buy_accessory(1)
        self._assert_buy_accessory(2)

    def test_buy_raccoon_and_hats(self):
        self.test_buy_raccoon()
        self._assert_buy_accessory(3)
        self._assert_buy_accessory(4)

    def test_that_premium_button_at_coordinates_is_found(self):
        self._load_main_scene_and_go_to_store()
        assert self.store_page.premium_button_at_coordinates.get_text() == "+"

    def test_key_preferences_in_store_menu(self):
        self.alt_driver.delete_player_pref()
        self.alt_driver.set_key_player_pref("test", "TestString")
        string_var = self.alt_driver.get_string_key_player_pref("test")
        assert string_var == "TestString"

        self.alt_driver.set_key_player_pref("test", 1)
        int_var = self.alt_driver.get_int_key_player_pref("test")
        assert int_var == 1

        self.alt_driver.set_key_player_pref("test", 1.0)
        float_var = self.alt_driver.get_float_key_player_pref("test")
        assert float_var == 1.0

        self.alt_driver.delete_key_player_pref("test")

    def test_player_prefs_with_static_method(self):
        self.alt_driver.call_static_method(
            "UnityEngine.PlayerPrefs",
            "SetInt",
            "UnityEngine.CoreModule",
            ["Test", "1"],
        )
        a = self.alt_driver.get_int_key_player_pref("Test")
        assert a == 1

    def test_get_static_property_brightness(self):
        brightness = self.alt_driver.get_static_property(
            "UnityEngine.Screen",
            "brightness",
            "UnityEngine.CoreModule",
        )
        assert brightness == 1

    def test_new_magnet_name(self):
        value = "magneeeeeeet"
        tab_name = "Items"
        new_name = self.store_page.change_item_name(tab_name, 0, value)
        assert new_name == value

    def test_different_colors_on_pressing(self):
        assert self.store_page.different_state_when_pressing_btn()

    def teardown_method(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        time.sleep(1)

    # Helper methods
    def _assert_buy_item(self, index):
        tab_name = "Item"
        self.store_page.get_more_money()
        self.store_page.go_to_tab(tab_name)

        money_amount = self.store_page.get_total_amount_of_coins()
        initial_number = self.store_page.get_number_of(index)
        self.store_page.buy(tab_name, index)
        assert self.store_page.get_number_of(index) - initial_number == 1
        assert money_amount - self.store_page.get_total_amount_of_coins() == self.store_page.get_price_of(tab_name, index)

    def _assert_buy_accessory(self, index):
        tab_name = "Accesories"
        self.store_page.get_more_money()
        self.store_page.go_to_tab(tab_name)

        initial_money = self.store_page.get_total_amount_of_coins()
        is_owned = self.store_page.assert_owning(tab_name, index)
        self.store_page.buy(tab_name, index)
        if is_owned:
            assert initial_money == self.store_page.get_total_amount_of_coins()
        else:
            assert (
                initial_money - self.store_page.get_price_of(tab_name, index)
                == self.store_page.get_total_amount_of_coins()
            )
            assert self.store_page.assert_owning(tab_name, index)

    def _load_main_scene_and_go_to_store(self):
        self.main_menu_page.load_scene()
        self.main_menu_page.press_store()
