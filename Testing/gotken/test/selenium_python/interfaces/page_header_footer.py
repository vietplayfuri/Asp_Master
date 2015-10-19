from holmium.core import Element, Elements, ElementMap, Locators, Page, Sections
from selenium.webdriver.common.keys import Keys

from selenium_python import global_variables as glb
from selenium_python.common import common, waitModule
import time


class page_header(Page):
	btn_topbar_menu = Element( Locators.CSS_SELECTOR, '.toggle-topbar.menu-icon>a', timeout = glb.int_waiting_time)
	btn_topbar_user = Element( Locators.CSS_SELECTOR, '.topbar-user', timeout = glb.int_waiting_time)
	btn_logout = Element( Locators.CSS_SELECTOR, '.user-log-out', timeout = glb.int_waiting_time)
	btn_signup = Element( Locators.CSS_SELECTOR, '.register-btn', timeout = glb.int_waiting_time)

	def ac_logout(self):
		time.sleep(1)
		self.driver.get(glb.url_logout)
		# assert self.btn_logout is None, 'You has signed out successfully'
		assert self.btn_signup.is_displayed(), 'You has signed out successfully'
