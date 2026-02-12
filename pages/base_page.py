class BasePage:
    annotate_callback = None

    def __init__(self, driver):
        self.driver = driver

    def log(self, message, level="info"):
        if BasePage.annotate_callback:
            BasePage.annotate_callback(message, level)
