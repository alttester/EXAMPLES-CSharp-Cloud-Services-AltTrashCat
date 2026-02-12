import time

from alttester import By

from .base_page import BasePage


class StorePage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    def load_scene(self):
        self.log("Store: Loading scene")
        self.driver.load_scene("Shop")

    @property
    def close_button(self):
        return self.driver.wait_for_object(By.PATH, "/Canvas/Background/Button")

    @property
    def store_title_money_button(self):
        return self.driver.wait_for_object(By.NAME, "StoreTitle", timeout=5)

    @property
    def items_tab(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/TabsSwitch/Item"
        )

    @property
    def characters_tab(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/TabsSwitch/Character"
        )

    @property
    def accessories_tab(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/TabsSwitch/Accesories"
        )

    @property
    def themes_tab(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/TabsSwitch/Themes"
        )

    @property
    def buy_button(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/Canvas/Background/ItemsList/Container/ItemEntry(Clone)"
            "/NamePriceButtonZone/PriceButtonZone/BuyButton",
        )

    @property
    def coins_counter(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/Coin/CoinsCounter"
        )

    @property
    def coin_image(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/Coin/Image"
        )

    @property
    def premium_plus_button(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/Premium/Button"
        )

    @property
    def premium_coin_image(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/Premium/Image"
        )

    @property
    def premium_counter(self):
        return self.driver.wait_for_object(
            By.PATH, "/Canvas/Background/Premium/PremiumCounter"
        )

    @property
    def premium_button_at_coordinates(self):
        return self.driver.find_object_at_coordinates(
            (self.premium_coin_image.x - 46, self.premium_coin_image.y)
        )

    @property
    def item_count(self):
        return self.driver.find_objects_which_contain(
            By.PATH,
            "/Canvas/Background/ItemsList/Container/ItemEntry(Clone)/Icon/Count",
        )

    def store_is_displayed(self):
        self.log("Store: Checking if displayed")
        return all([
            self.store_title_money_button,
            self.close_button,
            self.items_tab,
            self.characters_tab,
            self.accessories_tab,
            self.themes_tab,
            self.buy_button,
            self.premium_plus_button,
            self.coin_image,
            self.premium_coin_image,
        ])

    def buy_buttons_state(self):
        buy_magnet = self.button_object_state(self.get_objects_buy_button("Items", 0))
        buy_multiplier = self.button_object_state(
            self.get_objects_buy_button("Items", 1)
        )
        buy_invincible = self.button_object_state(
            self.get_objects_buy_button("Items", 2)
        )
        buy_life = self.button_object_state(self.get_objects_buy_button("Items", 3))
        return all([buy_magnet, buy_invincible, buy_multiplier, buy_life])

    def buy(self, tab_name, index):
        """tab_name = Items, Accesories, Character, Themes"""
        self.log(f"Store: Buying item {index} from {tab_name}")
        self.get_objects_buy_button(tab_name, index).tap()

    def buy_all_from_tab(self, tab_name):
        self.log(f"Store: Buying all from {tab_name}")
        index = 0
        while True:
            try:
                self.get_objects_buy_button(tab_name, index).tap()
                index += 1
            except Exception:
                break

    def get_objects_buy_button(self, tab_name, index, end_path=""):
        """tab_name = Items, Accesories, Character, Themes"""
        tab_name_path = self.get_path_by_tab_name(tab_name)
        self.driver.wait_for_object_which_contains(
            By.PATH,
            f"/Canvas/Background/{tab_name_path}/Container/ItemEntry(Clone)"
            f"/NamePriceButtonZone/PriceButtonZone/BuyButton{end_path}",
            timeout=50,
        )
        objects = self.driver.find_objects_which_contain(
            By.PATH,
            f"/Canvas/Background/{tab_name_path}/Container/ItemEntry(Clone)"
            f"/NamePriceButtonZone/PriceButtonZone/BuyButton{end_path}",
        )
        return objects[index]

    def go_to_tab(self, tab_name):
        """tab_name = Item, Character, Accesories, Themes"""
        self.log(f"Store: Going to tab {tab_name}")
        self.driver.wait_for_object(By.NAME, tab_name, timeout=5).tap()

    @staticmethod
    def get_path_by_tab_name(tab_name):
        mapping = {
            "Item": "ItemsList",
            "Items": "ItemsList",
            "Character": "CharacterList",
            "Accesories": "CharacterAccessoriesList",
            "Themes": "ThemeList",
        }
        return mapping.get(tab_name, "")

    def counters_reset(self):
        return (
            self.get_total_amount_of_coins() == 0
            and self.get_total_amount_of_premium_coins() == 0
        )

    def reload_items(self):
        self.log("Store: Reloading items")
        self.items_tab.tap()

    def close_store(self):
        self.log("Store: Closing store")
        self.close_button.tap()

    def get_more_money(self):
        self.log("Store: Getting more money")
        self.store_title_money_button.click()

    def get_number_of(self, index_in_page):
        """index_in_page = item's position in the list"""
        return int(self.item_count[index_in_page].get_text())

    def get_name_object_by_index_in_page(self, tab_name, index):
        tab_name_path = self.get_path_by_tab_name(tab_name)
        self.driver.wait_for_object_which_contains(
            By.PATH,
            f"/Canvas/Background/{tab_name_path}/Container/ItemEntry(Clone)"
            "/NamePriceButtonZone/Name",
        )
        objects = self.driver.find_objects_which_contain(
            By.PATH,
            f"/Canvas/Background/{tab_name_path}/Container/ItemEntry(Clone)"
            "/NamePriceButtonZone/Name",
        )
        return objects[index]

    def get_price_of(self, tab_name, index):
        tab_name_path = self.get_path_by_tab_name(tab_name)
        objects = self.driver.find_objects_which_contain(
            By.PATH,
            f"/Canvas/Background/{tab_name_path}/Container/ItemEntry(Clone)"
            "/NamePriceButtonZone/PriceButtonZone/PriceZone/PriceCoin/Amount",
        )
        return int(objects[index].get_text())

    def different_state_when_pressing_btn(self):
        state1 = self.store_title_money_button.call_component_method(
            "UnityEngine.UI.Button",
            "get_currentSelectionState",
            "UnityEngine.UI",
            [],
        )
        state2 = (
            self.store_title_money_button.pointer_down_from_object().call_component_method(
                "UnityEngine.UI.Button",
                "get_currentSelectionState",
                "UnityEngine.UI",
                [],
            )
        )
        return state1 != state2

    def change_item_name(self, tab_name, index, new_name):
        self.log(f"Store: Changing item {index} name to {new_name}")
        new_object = self.get_name_object_by_index_in_page(tab_name, index)
        new_object.set_text(new_name, submit=True)
        return new_object.get_component_property(
            "UnityEngine.UI.Text", "text", "UnityEngine.UI"
        )

    def get_total_amount_of_coins(self):
        return int(self.coins_counter.get_text())

    def get_total_amount_of_premium_coins(self):
        return int(self.premium_counter.get_text())

    def assert_owning(self, tab_name, index):
        time.sleep(0.5)
        buy_btn_text = self.get_objects_buy_button(tab_name, index, "/Text")
        return buy_btn_text.get_text() == "Owned"

    def enable_button_object(self, button):
        button.set_component_property(
            "UnityEngine.UI.Button",
            "interactable",
            "True",
            "UnityEngine.UI",
        )
        return button.get_component_property(
            "UnityEngine.UI.Button", "interactable", "UnityEngine.UI"
        )

    @staticmethod
    def button_object_state(button):
        return button.get_component_property(
            "UnityEngine.UI.Button", "interactable", "UnityEngine.UI"
        )
