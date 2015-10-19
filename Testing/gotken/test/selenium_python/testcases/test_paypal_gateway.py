# nosetests selenium_python/testcases/test_paypal_gateway.py -v --with-holmium --holmium-browser=firefox --logging-level=WARNING
from holmium.core import TestCase, Page, Element, Locators, ElementMap

from selenium import webdriver

from selenium_python.interfaces.page_login import page_login
from selenium_python.interfaces.page_paypal import page_paypal
from selenium_python.interfaces.page_profile import tab_transaction, tab_paypal
# from selenium_python.decorators import screenshot_on_error
from selenium_python import global_variables as glb

import time

def action_choose_package(package_selector):
    array_amount_gtoken = []

class list_test_login(TestCase):
    def setUp(self):
        self.driver.maximize_window()

    # @screenshot_on_error
    def test_login(self):
        # self.assertTrue(False)
        pg_login = page_login(self.driver, glb.url_signIn)
        pg_paypal = page_paypal(self.driver)
        tb_transaction = tab_transaction(self.driver)
        tb_paypal = tab_paypal(self.driver)

        pg_login.login(glb.s_username, glb.s_password)
        
        tb_transaction.lnk_topUp_panel.click()
        tb_transaction.panel_paypal.click()

        tb_paypal.btn_recruit_package.click()

        pg_paypal.tab_paypal_login.click()
        pg_paypal.txt_username.send_keys(glb.s_username_paypal)
        pg_paypal.txt_password.send_keys(glb.s_password_paypal)
        pg_paypal.btn_login.click()
        pg_paypal.btn_continue_paypal.click()
        time.sleep(8)
        


