import time

import pytest

from pages import (
    GameOverScreen,
    GamePlay,
    GetAnotherChancePage,
    MainMenuPage,
    PauseOverlayPage,
    SettingsPage,
    StartPage,
    StorePage,
)


@pytest.mark.usefixtures("setup")
class TestUserJourney:
    def setup_method(self):
        self.main_menu_page = MainMenuPage(self.alt_driver)
        self.game_play = GamePlay(self.alt_driver)
        self.pause_overlay_page = PauseOverlayPage(self.alt_driver)
        self.get_another_chance_page = GetAnotherChancePage(self.alt_driver)
        self.game_over_screen = GameOverScreen(self.alt_driver)
        self.settings_page = SettingsPage(self.alt_driver)
        self.start_page = StartPage(self.alt_driver)
        self.store_page = StorePage(self.alt_driver)
        self.main_menu_page.load_scene()

    def test_user_journey_play_and_pause(self):
        # User opens the game
        self.main_menu_page.press_run()
        assert self.game_play.is_displayed()
        self.game_play.avoid_obstacles(5)
        assert self.game_play.get_current_life() > 0
        # User pauses the game
        self.game_play.press_pause()
        assert self.pause_overlay_page.is_displayed()
        self.pause_overlay_page.press_resume()
        assert self.game_play.is_displayed()
        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        # Character dies and game over screen is displayed
        self.get_another_chance_page.press_game_over()
        assert self.game_over_screen.is_displayed()

    def test_user_journey_buy_items(self):
        # Delete current game data
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        # Verify buttons are disabled when no money
        assert not self.store_page.buy_buttons_state()
        self.store_page.get_more_money()
        time.sleep(1)
        self.store_page.go_to_tab("Character")
        self.store_page.reload_items()
        time.sleep(1)
        # Get coins and verify buttons get enabled
        assert self.store_page.buy_buttons_state()
        # Buy magnet and night theme
        self.store_page.buy("Items", 0)  # buy magnet
        self.store_page.go_to_tab("Themes")
        self.store_page.buy("Themes", 1)
        self.store_page.close_store()
        self.main_menu_page.tap_arrow_button("power", "Left")
        assert self.main_menu_page.theme_selector_right is not None
        self.main_menu_page.tap_arrow_button("theme", "Right")
        time.sleep(0.1)
        # Verify bought items are available in game
        self.main_menu_page.press_run()
        assert self.game_play.in_game_power_up is not None
        assert self.game_play.night_lights is not None
        self.game_play.activate_in_game_power_up()
        assert self.game_play.power_up_icon is not None

    def test_user_journey_revive_and_get_a_second_chance(self):
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        # Verify buttons are disabled when no money
        assert not self.store_page.buy_buttons_state()
        self.store_page.get_more_money()
        time.sleep(1)
        self.store_page.go_to_tab("Character")
        self.store_page.reload_items()
        time.sleep(1)
        # Get coins and verify buttons get enabled
        assert self.store_page.buy_buttons_state()
        self.store_page.buy("Items", 3)  # buy life
        self.store_page.close_store()
        self.main_menu_page.tap_arrow_button("power", "Left")
        self.main_menu_page.press_run()

        while self.game_play.get_current_life() > 1:
            time.sleep(0.005)

        self.game_play.activate_in_game_power_up()
        time.sleep(0.005)
        assert self.game_play.get_current_life() == 2
        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        assert self.get_another_chance_page.is_displayed()
        self.get_another_chance_page.press_premium_button()

        timeout = 20
        while timeout > 0:
            try:
                self.game_over_screen.is_displayed()
                break
            except Exception:
                timeout -= 1
        assert self.game_over_screen.is_displayed()

    def test_the_number_of_all_enabled_elements_from_different_pages_is_different(self):
        main_menu_elements = self.alt_driver.get_all_elements(enabled=True)
        self.main_menu_page.press_run()
        time.sleep(1)
        game_play_elements = self.alt_driver.get_all_elements(enabled=True)
        assert len(main_menu_elements) != len(game_play_elements)

    def test_the_number_of_all_disabled_elements_from_different_pages_is_different(self):
        self.main_menu_page.load_scene()
        main_menu_elements = self.alt_driver.get_all_elements(enabled=False)
        self.main_menu_page.press_run()
        time.sleep(1)
        game_play_elements = self.alt_driver.get_all_elements(enabled=False)
        assert len(main_menu_elements) != len(game_play_elements)

    def test_methods_that_handle_scenes(self):
        loaded_scene_names = self.alt_driver.get_all_loaded_scenes()
        assert loaded_scene_names[0] == "Main"
        assert self.alt_driver.get_current_scene() == "Main"

        self.main_menu_page.press_store()

        self.alt_driver.unload_scene("Main")
        assert self.alt_driver.get_current_scene() == "Shop"

        self.alt_driver.load_scene("Shop", load_single=True)
        assert self.alt_driver.get_current_scene() == "Shop"

        assert self.alt_driver.get_all_loaded_scenes()[0] == "Shop"
        self.main_menu_page.load_scene()

    def teardown_method(self):
        time.sleep(1)
