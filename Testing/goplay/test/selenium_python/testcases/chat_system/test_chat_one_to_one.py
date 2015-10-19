# nosetests selenium_python/testcases/chat_system/test_chat_one_to_one.py -v --with-holmium --holmium-browser=firefox --logging-level=WARNING
from holmium.core import TestCase, Page, Element, Locators, ElementMap

from selenium import webdriver
from selenium.webdriver.common.keys import Keys

from selenium_python.interfaces.page_header_footer import page_header
from selenium_python.interfaces.page_login import page_login
from selenium_python.interfaces.page_profile import page_profile, panel_friend
from selenium_python.common import common
from selenium_python import global_variables as glb

import time


class list_test_chat_one_to_one(TestCase):
    def setUp(self):
        self.cm = common()

        self.driver.set_window_size(650, 750)
        self.driver.set_window_position(0,0)
        self.driver.get(glb.url_signIn)
        
        self.pg_login = page_login(self.driver)
        self.pnl_friend = panel_friend(self.driver)
        self.pg_profile = page_profile(self.driver)
        self.pg_header = page_header(self.driver)

        self.driver1 = webdriver.Firefox()
        self.driver1.set_window_size(650, 750)
        self.driver1.set_window_position(655, 4)
        self.driver1.get(glb.url_signIn)

        self.pg_login1 = page_login(self.driver1)
        self.pg_header1 = page_header(self.driver1)
        self.pnl_friend1 = panel_friend(self.driver1)
        self.pg_profile1 = page_profile(self.driver1)

    def tearDown(self):
        self.driver1.close()

    def test01_online_status(self):

        self.pg_login.ac_login(glb.s_username, glb.s_password)

        self.pg_profile.lnk_friends_panel.click()
        self.cm.scroll_to_bottom(self.driver)
        self.assertElementTextEqual(self.pnl_friend.lbl_online_status(glb.s_referral_username), 'offline', 'The current status is offline.')

        self.pg_login1.ac_login(glb.s_referral_username, glb.s_referral_password)

        time.sleep(3)
        self.assertElementTextEqual(self.pnl_friend.lbl_online_status(glb.s_referral_username), 'online', 'The current status is online.')

        self.pg_header1.ac_logout()
        time.sleep(2)
        self.assertElementTextEqual(self.pnl_friend.lbl_online_status(glb.s_referral_username), 'offline', 'The current status is offline after friend logged out.')
    
    # def test02_send_offline_message(self):
        
    #     message1 = 'I\'m so handsome! ' + glb.uni_val
    #     message2 =  'f*ck yeah! ' + glb.uni_val

    #     self.pg_login.ac_login(glb.s_username, glb.s_password)
    #     self.pg_profile.lnk_friends_panel.click()
    #     self.pnl_friend.ac_open_chat_tab(glb.s_referral_username)
    #     self.pnl_friend.ac_send_message(glb.s_referral_username, message1)
    #     self.pnl_friend.ac_check_last_message(glb.s_username, message1)

    #     self.pg_login1.ac_login(glb.s_referral_username, glb.s_referral_password) 
    #     self.pg_profile1.lnk_friends_panel.click()
    #     self.pnl_friend1.ac_open_chat_tab(glb.s_username)
    #     self.pnl_friend1.ac_check_last_message(glb.s_username, message1)

    #     self.pnl_friend1.ac_send_message(glb.s_username, message2)
    #     # print self.driver.title
    #     for x in xrange(1,10):
    #         if 'New messages' in self.driver.title :
    #             assert True, 'There is a title notification for new message.'
    #             break
    #         else:
    #             time.sleep(1)
    #     else:
    #         assert False, 'No tile notification!'
    #     self.pnl_friend.ac_check_last_message(glb.s_referral_username, message2)

    # def test03_open_mutil_chat_tab(self):

    #     self.pg_login.ac_login(glb.s_username, glb.s_password)
    #     self.pg_profile.lnk_friends_panel.click()
    #     self.pnl_friend.ac_open_chat_tab(glb.s_referral_username)
    #     self.pnl_friend.btn_friend_chat(glb.s_chat_username).click()
    #     assert self.pnl_friend.tab_chat_window(glb.s_chat_username).is_displayed(), 'Open the 2nd chat tab completely.'
    #     time.sleep(1)
    #     self.pnl_friend.ico_close_tab_chat(glb.s_chat_username).click()
    #     assert self.pnl_friend.tab_chat_window(glb.s_chat_username) == False, 'Close the 2nd chat tab successfully.'

    # def test04_chat_typing(self):
    #     timeout_typing = 1.5
    #     self.pg_login.ac_login(glb.s_username, glb.s_password)
    #     self.pg_profile.lnk_friends_panel.click()
    #     self.pnl_friend.ac_open_chat_tab(glb.s_referral_username)

    #     self.pg_login1.ac_login(glb.s_referral_username, glb.s_referral_password)
    #     self.pg_profile1.lnk_friends_panel.click()
    #     self.pnl_friend1.ac_open_chat_tab(glb.s_username)

    #     self.pnl_friend.txt_input_message(glb.s_referral_username).send_keys('typing')
    #     assert self.pnl_friend1.spn_chat_typing(glb.s_username).is_displayed(), 'The typing span appears while friend is typing.'
    #     self.pnl_friend.txt_input_message(glb.s_referral_username).clear()
    #     time.sleep(timeout_typing)
    #     assert self.pnl_friend1.spn_chat_typing(glb.s_username) == False, 'The typing span disappears after 1.5s.'

