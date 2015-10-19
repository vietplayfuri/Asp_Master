# nosetests selenium_python/testcases/chat_system/test_sync_btw_tabs.py -v --with-holmium --holmium-browser=firefox --logging-level=WARNING
from holmium.core import TestCase, Page, Element, Locators, ElementMap
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains
import time

from selenium_python.interfaces.page_header_footer import page_header
from selenium_python.interfaces.page_login import page_login
from selenium_python.interfaces.page_profile import page_profile, panel_friend
from selenium_python.common import common, waitModule
from selenium_python import global_variables as glb

class list_test_sync_btw_tabs(TestCase):
    
    def setUp(self):
        self.cm = common()

        self.driver.set_window_size(600, 750)
        self.driver.set_window_position(0, 0)
        self.driver.get(glb.url_signIn)
        
        self.pg_login = page_login(self.driver)
        self.pnl_friend = panel_friend(self.driver)
        self.pg_profile = page_profile(self.driver)
        self.pg_header = page_header(self.driver)

        self.driver1 = webdriver.Firefox()
        self.driver1.set_window_position(650, 0)
        self.driver1.set_window_size(600, 750)
        self.driver1.get(glb.url_signIn)

        self.pg_login1 = page_login(self.driver1)
        self.pg_header1 = page_header(self.driver1)
        self.pnl_friend1 = panel_friend(self.driver1)
        self.pg_profile1 = page_profile(self.driver1)

    def tearDown(self):
        self.driver1.close()

    def test01_sync_btw_tabs(self):
        lnk_current_url = ''
        wait = waitModule()

        self.pg_login.ac_login(glb.s_username, glb.s_password)
        self.pg_profile.lnk_friends_panel.click()
        self.pnl_friend.ac_open_chat_tab(glb.s_referral_username)
        self.pnl_friend.ac_open_chat_tab(glb.s_chat_username)
        lnk_current_url = self.driver.current_url
        ActionChains(self.driver).send_keys(Keys.CONTROL, "t").perform()
        self.driver.get(lnk_current_url)

        time.sleep(8)
        assert self.pnl_friend.tab_chat_window(glb.s_referral_username).is_displayed(), 'Open %s chat tab completely.' % glb.s_referral_username
        assert self.pnl_friend.tab_chat_window(glb.s_chat_username).is_displayed(), 'Open %s chat tab completely.' % glb.s_chat_username