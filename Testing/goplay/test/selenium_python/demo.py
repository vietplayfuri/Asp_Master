# nosetests demo.py -v --with-holmium --holmium-browser=firefox --logging-level=WARNING
from holmium.core import TestCase, Page, Element, Locators, ElementMap

from selenium import webdriver

from interfaces.page_login import page_login
from interfaces.page_profile import tab_transaction
from selenium_python.decorators import screenshot_on_error
from selenium_python import global_variables as glb


class list_test_login(TestCase):
    def setUp(self):
        self.driver.maximize_window()

    # @screenshot_on_error
    def test_login(self):
        # self.assertTrue(False)
        pgLogin = page_login(self.driver, glb.urlSignIn)
        tbTransaction = tab_transaction(self.driver)

        pgLogin.login(glb.sUsername, glb.sPassword)
        
        tbTransaction.lnkTopUpPanel.click()
        tbTransaction.tabPaypal.click()

        


