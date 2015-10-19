from holmium.core import Element, Elements, ElementMap, Locators, Page, Sections
from selenium.webdriver.common.keys import Keys

from selenium_python import global_variables as glb
from selenium_python.common import common, waitModule
import time


class page_profile(Page):
    lnk_friends_panel = Element( Locators.CSS_SELECTOR, '.acc-nav-links>li>a[href*="friend"]', timeout = glb.int_waiting_time)
    lnk_transaction_panel = Element( Locators.CSS_SELECTOR, '.acc-nav-links>li>a[href*="transaction"]', timeout = glb.int_waiting_time)
    
    btn_login = Element( Locators.CSS_SELECTOR, 'a[href="/account/login"]', timeout = glb.int_waiting_time)

class tab_transaction(Page):
    lnk_topUp_panel = Element( Locators.CSS_SELECTOR, 'a.topup-td', timeout = glb.int_waiting_time)
    lnk_gcoin_panel = Element( Locators.CSS_SELECTOR, '#show-gcoin-panel-link', timeout = glb.int_waiting_time)
    lnk_invoice = Elements( Locators.CSS_SELECTOR, 'td[data-title="Invoice"]>a', timeout = glb.int_waiting_time)

    panel_paypal = Element( Locators.CSS_SELECTOR, 'a[ng-click*="paypal"]', timeout = glb.int_waiting_time)


class tab_gcoin(tab_transaction):
    btn_convert_gcoin = Element( Locators.CSS_SELECTOR, '#submit-convert-gcoin', timeout = glb.int_waiting_time)
    btn_submit = Element( Locators.CSS_SELECTOR, '#submit', timeout = glb.int_waiting_time)

    txt_paypal_email = Element( Locators.CSS_SELECTOR, '#paypalEmail', timeout = glb.int_waiting_time)
    txt_password = Element( Locators.CSS_SELECTOR, '#password', timeout = glb.int_waiting_time)

    def ac_convert_gcoin(self, send_email, password):
        wait = waitModule()
        invoice_id = ''
        self.txt_paypal_email.send_keys(send_email)
        self.btn_convert_gcoin.click()
        self.txt_password.send_keys(password)
        self.btn_submit.click()
        wait.wait_for_text(self.driver, glb.msg_convert_gcoin)
        invoice_id = self.lnk_invoice[0].get_attribute('href').split('=')[1]
        return invoice_id

class tab_paypal(Page):
    btn_recruit_package = Element( Locators.CSS_SELECTOR, '#new-recruit-package', timeout = glb.int_waiting_time)

class tab_chat_messages(Sections):
    user = Element(Locators.CSS_SELECTOR , 'span[ng-bind="msg.user.account"]')
    message = Element(Locators.CSS_SELECTOR , 'span[ng-bind="msg.chat.message"]')
    request = Element(Locators.CSS_SELECTOR , 'a[href*="invite"]')
    icon = Element(Locators.CSS_SELECTOR , 'div[ng-if*="msg"]>img')
        
class panel_friend(Page):
    chat_messages = tab_chat_messages( Locators.CSS_SELECTOR, 'div[ng-repeat="msg in window.messages"]', timeout = glb.int_waiting_time)
    lst_games = ElementMap ( Locators.CSS_SELECTOR, ".chat-games>div>a" , timeout = glb.int_waiting_time )

    def txt_input_message(self, username):
        wait = waitModule()
        selector = self.driver.find_element_by_css_selector('[data-username="'+username +'"]>.chat-input>input')
        wait.wait_for_element(self.driver, selector, 'txt_input_message')
        return selector
    
    def btn_friend_chat(self, username):
    	wait = waitModule()
    	selector = self.driver.find_element_by_css_selector('div[data-username="'+ username +'"]>a')
    	wait.wait_for_element(self.driver, selector, 'btn_friend_chat')
    	return selector

    def lbl_online_status(self, username):
        wait = waitModule()
        selector = self.driver.find_element_by_css_selector('div[data-username="'+ username +'"]>span')
        wait.wait_for_element(self.driver, selector, 'lbl_online_status')
        return selector

    def tab_chat_window(self, username):
        try:
            wait = waitModule()
            selector = self.driver.find_element_by_css_selector('div[class*="chat-window"][data-username="'+ username +'"]')
            wait.wait_for_element(self.driver, selector, 'tab_chat_window')
            return selector
        except Exception, e:
            return False

    def spn_chat_typing(self, username):
        try:
            wait = waitModule()
            selector = self.driver.find_element_by_css_selector('div[class*="chat-window"][data-username="'+ username +'"]>.chat-content>.chat-typing>span')
            wait.wait_for_element(self.driver, selector, 'spn_chat_typing')
            return selector
        except Exception, e:
            return False

    def btn_invite_game_request(self, username):
        try:
            wait = waitModule()
            selector = self.driver.find_element_by_css_selector('div[class*="chat-window"][data-username="'+ username +'"]>.chat-input>a[ng-click*="Invite"]')
            wait.wait_for_element(self.driver, selector, 'btn_invite_game_request')
            return selector
        except Exception, e:
            return False

    def ico_close_tab_chat(self, username):
        try:
            wait = waitModule()
            selector = self.driver.find_element_by_css_selector('div[data-username="'+ username +'"]>.name-bar>a')
            wait.wait_for_element(self.driver, selector, 'ico_close_tab_chat')
            return selector
        except Exception, e:
            return False

    def ac_check_last_message(self, username, message):
        array_user = []
        array_message = []

        time.sleep(2)
        for chat in self.chat_messages:
            array_user.append(chat.user.text)
            array_message.append(chat.message.text)

        assert array_user[-1] == username
        assert array_message[-1] == message

    def ac_check_last_request(self, game):
        array_request = []

        time.sleep(2)
        for chat in self.chat_messages:
            array_request.append(chat.request.text)

        assert ('Click to play game ' + game) in array_request[-1] 

    def ac_open_chat_tab(self, username):
        cm = common()
        cm.scroll_to_bottom(self.driver)
        assert self.lbl_online_status(username).is_displayed(), 'The online status has been appeared.'
        self.btn_friend_chat(username).click()
        assert self.tab_chat_window(username).is_displayed(), 'Open chat tab completely.'
        time.sleep(3)

    def ac_send_message(self, username, message):
        self.txt_input_message(username).send_keys(message)
        self.txt_input_message(username).send_keys(Keys.ENTER)
