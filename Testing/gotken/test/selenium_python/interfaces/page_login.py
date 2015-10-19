from holmium.core import Element, Elements, ElementMap, Locators, Page
import time
from selenium_python import global_variables as glb
from selenium_python.common import waitModule

class page_login(Page):
    btn_submit_login = Element( Locators.CSS_SELECTOR, '#accounts_login>#submit', timeout = glb.int_waiting_time)
    txt_username = Element( Locators.CSS_SELECTOR, '#username', timeout = glb.int_waiting_time)
    txt_password = Element( Locators.CSS_SELECTOR, '#password', timeout = glb.int_waiting_time)

    def ac_login(self, username, password):
        wait = waitModule()
        self.txt_username.send_keys(username)
        self.txt_password.send_keys(password)
        self.btn_submit_login.click()
        wait.wait_for_text(self.driver, 'Account Settings')

