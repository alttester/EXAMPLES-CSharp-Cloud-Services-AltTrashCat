import time

import pytest

from pages import MainMenuPage, StartPage


@pytest.mark.usefixtures("setup")
class TestStartPage:
    def setup_method(self):
        self.start_page = StartPage(self.alt_driver)
        self.start_page.load()
        self.main_menu_page = MainMenuPage(self.alt_driver)

    def test_start_page_loaded_correctly(self):
        assert self.start_page.is_displayed()

    def test_start_button_load_main_menu(self):
        self.start_page.press_start()
        assert self.main_menu_page.is_displayed()

    def teardown_method(self):
        time.sleep(1)
