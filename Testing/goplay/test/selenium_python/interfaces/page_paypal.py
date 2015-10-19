from holmium.core import Element, Elements, ElementMap, Locators, Page
import time
from selenium_python import global_variables as glb
from selenium_python.common import waitModule

class page_paypal(Page):
    tab_paypal_login = Element( Locators.CSS_SELECTOR, '#loadLogin', timeout = glb.int_waiting_time * 2)
    txt_username = Element( Locators.CSS_SELECTOR, '#login_email', timeout = glb.int_waiting_time * 2)
    txt_password = Element( Locators.CSS_SELECTOR, '#login_password', timeout = glb.int_waiting_time)
    btn_login = Element( Locators.CSS_SELECTOR, '#submitLogin', timeout = glb.int_waiting_time)
    btn_continue_paypal = Element( Locators.CSS_SELECTOR, '#continue_abovefold', timeout = glb.int_waiting_time)
    btn_approval = Element( Locators.CSS_SELECTOR, 'input[id="submit.x"]', timeout = glb.int_waiting_time*2)
    btn_return = Element( Locators.CSS_SELECTOR, '#returnToMerchant', timeout = glb.int_waiting_time*2)