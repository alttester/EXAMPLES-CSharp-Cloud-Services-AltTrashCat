import time

import pytest

from alttester import By
from pages import (
    GamePlay,
    GetAnotherChancePage,
    MainMenuPage,
    SettingsPage,
    StorePage,
)


@pytest.mark.usefixtures("setup")
class TestMainMenu:
    def setup_method(self):
        self.main_menu_page = MainMenuPage(self.alt_driver)
        self.game_play_page = GamePlay(self.alt_driver)
        self.settings_page = SettingsPage(self.alt_driver)
        self.store_page = StorePage(self.alt_driver)
        self.get_another_chance_page = GetAnotherChancePage(self.alt_driver)
        self.main_menu_page.load_scene()

    def test_main_menu_page_loaded_correctly(self):
        assert self.main_menu_page.is_displayed()

    def test_names_of_all_buttons_from_page(self):
        buttons_names = self.main_menu_page.get_all_buttons()
        assert "OpenLeaderboard" in buttons_names
        assert "StoreButton" in buttons_names
        assert "MissionButton" in buttons_names
        assert "SettingButton" in buttons_names
        assert "StartButton" in buttons_names

    def test_buttons_are_correctly_displayed(self):
        assert self.main_menu_page.buttons_and_text_displayed_correctly()

    def test_delete_data(self):
        self.main_menu_page.load_scene()
        self.settings_page.delete_data()
        self.main_menu_page.press_store()
        assert self.store_page.counters_reset()

    def test_leader_board_name_high_score_changes(self):
        self.main_menu_page.load_scene()
        self.main_menu_page.select_leader_board()
        self.main_menu_page.set_high_score_name()
        assert self.main_menu_page.leaderboard_high_score_name.get_text() == "HighScore"

    @pytest.mark.parametrize(
        "slider_name",
        ["MasterSlider", "MusicSlider", "MasterSFXSlider"],
    )
    def test_slider_values_change_as_expected(self, slider_name):
        self.main_menu_page.load_scene()
        self.main_menu_page.press_settings()
        self.settings_page.move_slider(slider_name, -1000)

        initial_slider_value = self.settings_page.get_slider_value(slider_name)
        self.settings_page.move_slider(slider_name, 20)

        final_slider_value = self.settings_page.get_slider_value(slider_name)
        assert initial_slider_value != final_slider_value

    def test_get_parent(self):
        self.main_menu_page.load_scene()
        time.sleep(0.1)
        alt_object_parent = self.main_menu_page.theme_zone_camera.get_parent()
        assert alt_object_parent.name == "Loadout"

    def test_get_time_scale_in_game(self):
        time_scale = self.alt_driver.get_time_scale()
        self.alt_driver.set_time_scale(0.1)
        self.main_menu_page.press_run()
        time.sleep(1)
        assert self.alt_driver.get_time_scale() == 0.1
        time.sleep(1)
        self.alt_driver.set_time_scale(1)

    def test_get_current_scene_is_main(self):
        assert self.main_menu_page.driver.get_current_scene() == "Main"

    def test_get_application_screen_size(self):
        initial_screen_size = self.alt_driver.get_application_screen_size()
        initial_x = str(int(initial_screen_size.x))
        initial_y = str(int(initial_screen_size.y))
        resolution_x = "375"
        resolution_y = "667"

        self.main_menu_page.set_resolution(resolution_x, resolution_y, "false")
        screen_size = self.alt_driver.get_application_screen_size()
        assert str(int(screen_size.x)) == resolution_x
        self.main_menu_page.set_resolution(initial_x, initial_y, "false")

    def test_string_key_player_pref(self):
        set_string_pref = "stringPlayerPrefInTrashcat"
        string_player_pref = self.main_menu_page.get_key_player_pref(
            "test", set_string_pref
        )
        assert string_player_pref == set_string_pref

    def test_delete_key(self):
        self.main_menu_page.driver.delete_player_pref()
        self.main_menu_page.driver.set_key_player_pref("test", 1)
        val = self.main_menu_page.driver.get_int_key_player_pref("test")
        assert val == 1
        self.main_menu_page.driver.delete_key_player_pref("test")
        with pytest.raises(Exception):
            self.main_menu_page.driver.get_int_key_player_pref("test")

    def test_get_server_version(self):
        server_version = self.alt_driver.get_server_version()
        print(f"App was instrumented with server version: {server_version}")
        assert server_version == "2.2.5"

    def test_get_active_cameras(self):
        list_active_cameras = self.alt_driver.get_all_active_cameras()
        assert len(list_active_cameras) == 1
        assert list_active_cameras[0].name == "Main Camera"

    def test_get_all_components(self):
        expected_components = [
            "UnityEngine.RectTransform",
            "UnityEngine.CanvasRenderer",
            "UnityEngine.UI.Image",
            "UnityEngine.UI.Button",
            "LevelLoader",
            "UnityEngine.AudioSource",
        ]
        store_btn_components = self.main_menu_page.store_button.get_all_components()
        assert len(store_btn_components) > 0
        for index, component in enumerate(store_btn_components):
            assert component.component_name == expected_components[index]

    def test_get_all_properties(self):
        store_btn_properties = self.main_menu_page.store_button.get_all_properties(
            component_name="UnityEngine.CanvasRenderer",
            assembly_name="UnityEngine.UIModule",
        )
        assert len(store_btn_properties) > 12
        assert store_btn_properties[0].name == "hasPopInstruction"
        assert store_btn_properties[0].value == "False"

    def test_get_all_fields(self):
        store_btn_fields = self.main_menu_page.store_button.get_all_fields(
            component_name="UnityEngine.UI.Button",
            assembly_name="UnityEngine.UI",
        )
        assert len(store_btn_fields) == 2
        assert store_btn_fields[0].name == "m_OnClick"
        assert store_btn_fields[1].name == "m_CurrentIndex"

    def test_get_all_methods(self):
        store_btn_methods = self.main_menu_page.store_button.get_all_methods(
            component_name="UnityEngine.CanvasRenderer",
            assembly_name="UnityEngine.UIModule",
        )
        assert len(store_btn_methods) > 80
        assert store_btn_methods[0] == "Boolean get_hasPopInstruction()"
        assert store_btn_methods[1] == "Void set_hasPopInstruction(Boolean)"

    def test_get_screenshot(self):
        import os

        path = "test-screenshot.png"
        self.alt_driver.get_png_screenshot(path)
        assert os.path.exists(path)
        os.remove(path)

    def teardown_method(self):
        time.sleep(1)
