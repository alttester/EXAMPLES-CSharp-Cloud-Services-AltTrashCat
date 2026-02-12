import time

from alttester import By

from .base_page import BasePage


class GamePlay(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    @property
    def pause_button(self):
        return self.driver.wait_for_object(By.NAME, "pauseButton", timeout=2)

    @property
    def character(self):
        return self.driver.wait_for_object(By.NAME, "PlayerPivot")

    @property
    def in_game_power_up(self):
        return self.driver.wait_for_object(By.NAME, "Inventory", timeout=20)

    @property
    def power_up_icon(self):
        return self.driver.wait_for_object(By.NAME, "PowerupIcon")

    @property
    def raccon_mesh(self):
        return self.driver.wait_for_object(
            By.PATH, "/PlayerPivot/CharacterSlot/character(Clone)/RacoonMesh"
        )

    @property
    def night_lights(self):
        return self.driver.wait_for_object(By.NAME, "LightGlows", timeout=10)

    @property
    def raccon_construction_gear(self):
        return self.driver.wait_for_object(
            By.PATH,
            "/PlayerPivot/CharacterSlot/character(Clone)/ConstructionGearMesh",
        )

    @property
    def character_slot(self):
        return self.driver.wait_for_object(By.PATH, "/PlayerPivot/CharacterSlot")

    @property
    def character_found_by_which_contains_with_camera(self):
        return self.driver.find_object_which_contains(
            By.NAME, "Character", By.NAME, "Main Camera"
        )

    def is_displayed(self):
        self.log("GamePlay: Checking if displayed")
        return self.pause_button is not None and self.character is not None

    def jump(self, character):
        self.log("GamePlay: Jump")
        character.call_component_method(
            "CharacterInputController", "Jump", "Assembly-CSharp", []
        )

    def slide(self, character):
        self.log("GamePlay: Slide")
        character.call_component_method(
            "CharacterInputController", "Slide", "Assembly-CSharp", []
        )

    def move_right(self, character):
        self.log("GamePlay: Move right")
        character.call_component_method(
            "CharacterInputController",
            "ChangeLane",
            "Assembly-CSharp",
            ["1"],
        )

    def move_left(self, character):
        self.log("GamePlay: Move left")
        character.call_component_method(
            "CharacterInputController",
            "ChangeLane",
            "Assembly-CSharp",
            ["-1"],
        )

    def get_current_life(self):
        self.log("GamePlay: Getting current life")
        return self.character.get_component_property(
            "CharacterInputController", "currentLife", "Assembly-CSharp"
        )

    def activate_in_game_power_up(self):
        self.log("GamePlay: Activating power-up")
        self.in_game_power_up.tap()

    def press_pause(self):
        self.log("GamePlay: Pressing Pause")
        self.pause_button.tap()

    def set_character_invincible(self, state):
        """state can be 'True' or 'False'"""
        self.log(f"GamePlay: Setting invincible to {state}")
        self.character_slot.call_component_method(
            "CharacterCollider",
            "SetInvincibleExplicit",
            "Assembly-CSharp",
            [state],
        )

    def character_is_moving(self):
        self.log("GamePlay: Checking if character is moving")
        character_initial_position = self.character.get_world_position()
        self.avoid_obstacles(3)
        return character_initial_position.z != self.character.update_object().get_world_position().z

    def avoid_obstacles(self, number_of_obstacles):
        self.log(f"GamePlay: Avoiding {number_of_obstacles} obstacles")
        character = self.character
        moved_left = False
        moved_right = False

        self.driver.wait_for_object_which_contains(By.NAME, "Obstacle")
        for _ in range(number_of_obstacles):
            all_obstacles = self.driver.find_objects_which_contain(
                By.NAME, "Obstacle"
            )
            all_obstacles.sort(key=lambda x: x.worldZ)
            all_obstacles = [
                obs for obs in all_obstacles if obs.worldZ >= character.worldZ
            ]
            obstacle = all_obstacles[0]

            while obstacle.worldZ - character.worldZ > 5:
                obstacle = self.driver.find_object(By.ID, str(obstacle.id))
                character = self.driver.find_object(By.NAME, "PlayerPivot")

            if "ObstacleHighBarrier" in obstacle.name:
                self.slide(character)
            elif "ObstacleLowBarrier" in obstacle.name or "Rat" in obstacle.name:
                self.jump(character)
            else:
                if obstacle.worldZ == all_obstacles[1].worldZ:
                    if obstacle.worldX == character.worldX:
                        if all_obstacles[1].worldX == -1.5:
                            self.move_right(character)
                            moved_right = True
                        else:
                            self.move_left(character)
                            moved_left = True
                    else:
                        if all_obstacles[1].worldX == character.worldX:
                            if obstacle.worldX == -1.5:
                                self.move_right(character)
                                moved_right = True
                            else:
                                self.move_left(character)
                                moved_left = True
                else:
                    if obstacle.worldX == character.worldX:
                        self.move_right(character)
                        moved_right = True

            while (
                character.worldZ - 3 < obstacle.worldZ
                and character.worldX < 99
            ):
                obstacle = self.driver.find_object(By.ID, str(obstacle.id))
                character = self.driver.find_object(By.NAME, "PlayerPivot")

            if moved_right:
                self.move_left(character)
                moved_right = False
            if moved_left:
                self.move_right(character)
                moved_left = False
