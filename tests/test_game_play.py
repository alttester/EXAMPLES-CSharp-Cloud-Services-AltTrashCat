import time

import pytest

from alttester import By
from pages import (
    GameOverScreen,
    GamePlay,
    GetAnotherChancePage,
    MainMenuPage,
    PauseOverlayPage,
    SettingsPage,
    StorePage,
)


@pytest.mark.usefixtures("setup")
class TestGamePlay:
    def setup_method(self):
        self.main_menu_page = MainMenuPage(self.alt_driver)
        self.game_play_page = GamePlay(self.alt_driver)
        self.pause_overlay_page = PauseOverlayPage(self.alt_driver)
        self.get_another_chance_page = GetAnotherChancePage(self.alt_driver)
        self.game_over_screen = GameOverScreen(self.alt_driver)
        self.settings_page = SettingsPage(self.alt_driver)
        self.store_page = StorePage(self.alt_driver)

        self.main_menu_page.load_scene()
        self.main_menu_page.press_run()

    def test_game_play_displayed_correctly(self):
        assert self.game_play_page.is_displayed()

    def test_game_can_be_paused_and_resumed(self):
        self.game_play_page.press_pause()
        assert self.pause_overlay_page.is_displayed()
        self.pause_overlay_page.press_resume()
        assert self.game_play_page.is_displayed()
        self.main_menu_page.load_scene()

    def test_game_can_be_paused_and_stopped(self):
        self.game_play_page.press_pause()
        self.pause_overlay_page.press_main_menu()
        assert self.main_menu_page.is_displayed()

    def test_avoiding_obstacles(self):
        self.game_play_page.avoid_obstacles(5)
        assert self.game_play_page.get_current_life() > 0
        self.main_menu_page.load_scene()

    def test_player_dies_when_obstacle_not_avoided(self):
        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        assert self.get_another_chance_page.is_displayed()

    def test_game_over_screen_is_accessible(self):
        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        self.get_another_chance_page.press_game_over()
        assert self.game_over_screen.is_displayed()

    def test_get_another_chance_disabled_when_not_enough_coins(self):
        self.game_play_page.press_pause()
        self.pause_overlay_page.press_main_menu()
        self.settings_page.delete_data()
        self.main_menu_page.press_run()
        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        assert not self.get_another_chance_page.get_another_change_object_state

    def test_that_trash_cat_becomes_invincible(self):
        self.game_play_page.set_character_invincible("True")
        time.sleep(20)
        self.alt_driver.wait_for_object_not_be_present(By.NAME, "GameOver")
        self.game_play_page.set_character_invincible("False")
        time.sleep(10)
        assert self.get_another_chance_page.is_displayed()

    def test_assert_character_is_moving(self):
        assert self.game_play_page.character_is_moving()

    def test_magnet_is_used_in_gameplay(self):
        self._load_main_scene_and_go_to_store()
        button_state = self.store_page.button_object_state(
            self.store_page.get_objects_buy_button("Items", 0)
        )
        if button_state:
            self.store_page.buy("Items", 0)  # buy magnet
        else:
            self._get_money_and_go_to_tab("Character")
            self.store_page.reload_items()
            self.store_page.buy("Items", 0)  # buy magnet

        num_of_magnets = self.store_page.get_number_of(0)
        self.store_page.close_store()

        self.main_menu_page.tap_arrow_button("power", "Left")
        self.main_menu_page.press_run()
        self.game_play_page.activate_in_game_power_up()
        assert self.game_play_page.power_up_icon is not None

        self.store_page.load_scene()
        self.store_page.go_to_tab("Item")
        assert num_of_magnets - self.store_page.get_number_of(0) == 1

    def test_that_life_power_up_adds_a_life(self):
        self._load_main_scene_and_go_to_store()
        button_state = self.store_page.buy_buttons_state()
        if button_state:
            self.store_page.buy("Items", 3)  # buy life
        else:
            self._get_money_and_go_to_tab("Character")
            self.store_page.reload_items()
            self.store_page.buy("Items", 3)  # buy life

        self.store_page.close_store()
        self.main_menu_page.tap_arrow_button("power", "Left")
        self.main_menu_page.press_run()

        while self.game_play_page.get_current_life() > 1:
            time.sleep(0.005)
        self.game_play_page.activate_in_game_power_up()
        assert self.game_play_page.get_current_life() == 2

    def test_the_user_can_play_with_raccoon(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        self._get_money_and_go_to_tab("Character")
        self.store_page.buy("Character", 1)  # buy Raccoon
        self.store_page.close_store()
        self.main_menu_page.select_raccoon_character()
        self.main_menu_page.press_run()
        time.sleep(0.02)
        assert self.game_play_page.raccon_mesh is not None

    def test_that_the_character_can_wear_accessories(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        self._get_money_and_go_to_tab("Accesories")
        self.store_page.buy_all_from_tab("Accesories")
        self.store_page.close_store()
        self.main_menu_page.change_accessory()
        self.main_menu_page.press_run()
        time.sleep(0.01)
        assert self.game_play_page.raccon_construction_gear is not None

    def test_night_time_theme_is_applied(self):
        self._load_main_scene_and_go_to_store()
        button_state = self.store_page.buy_buttons_state()
        if button_state:
            self.store_page.go_to_tab("Themes")
            self.store_page.buy("Themes", 1)
        else:
            self._get_money_and_go_to_tab("Themes")
            self.store_page.buy("Themes", 1)

        self.store_page.close_store()
        time.sleep(0.1)
        assert self.main_menu_page.theme_selector_right is not None
        self.main_menu_page.tap_arrow_button("theme", "Right")
        time.sleep(0.1)
        self.main_menu_page.press_run()
        assert self.game_play_page.night_lights is not None

    def test_premium_button_color_changes_as_expected_per_state(self):
        self._load_main_scene_and_go_to_store()
        self.store_page.get_more_money()
        self.store_page.close_store()
        self.main_menu_page.press_run()

        timeout = 20
        while timeout > 0:
            try:
                self.get_another_chance_page.is_displayed()
                break
            except Exception:
                timeout -= 1
        time.sleep(1)

        premium_button = self.get_another_chance_page.get_another_chance_button()
        self.get_another_chance_page.compare_object_color_by_state(premium_button)

        self.get_another_chance_page.premium_button.pointer_down_from_object()
        time.sleep(1)

        self.get_another_chance_page.compare_object_color_by_state(premium_button)
        self.get_another_chance_page.premium_button.pointer_up_from_object()
        time.sleep(1)

        self.get_another_chance_page.compare_object_color_by_state(premium_button)

    def test_get_world_position_trash_cat(self):
        character = self.game_play_page.character
        world_position = character.get_world_position()
        time.sleep(20)
        world_position_updated = character.update_object().get_world_position()
        assert world_position.z != world_position_updated.z

    def test_get_screen_position_trash_cat(self):
        character = self.alt_driver.find_object(By.NAME, "CharacterSlot")
        screen_position = character.get_screen_position()
        time.sleep(5)
        self.game_play_page.move_left(self.game_play_page.character)
        time.sleep(1)

        screen_position_after = character.update_object().get_screen_position()
        assert screen_position.x != screen_position_after.x

    def test_find_object_which_contains_with_camera(self):
        character_name = (
            self.game_play_page.character_found_by_which_contains_with_camera.name
        )
        assert character_name == "CharacterSlot"

    def test_time_scale(self):
        self.alt_driver.set_time_scale(0.1)
        time.sleep(1)
        time_scale_from_game = self.alt_driver.get_time_scale()
        assert time_scale_from_game == 0.1
        self.alt_driver.set_time_scale(1)

    def test_display_all_enabled_elements_from_another_chance_page(self):
        self.game_play_page.avoid_obstacles(3)
        self.get_another_chance_page.display_all_enabled_elements()

    def teardown_method(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        time.sleep(1)

    # Helper methods
    def _get_money_and_go_to_tab(self, tab_name):
        self.store_page.get_more_money()
        self.store_page.go_to_tab(tab_name)

    def _load_main_scene_and_go_to_store(self):
        self.main_menu_page.load_scene()
        self.main_menu_page.move_object(self.main_menu_page.alt_tester_logo)
        self.main_menu_page.press_store()
        time.sleep(0.5)
