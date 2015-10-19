# nosetests selenium_python/testcases/test_gcoin_convert.py -v --with-holmium --holmium-browser=firefox --logging-level=WARNING
from holmium.core import TestCase, Page, Element, Locators, ElementMap
from selenium import webdriver
from selenium.webdriver.common.keys import Keys

from selenium_python.interfaces.page_header_footer import page_header
from selenium_python.interfaces.page_login import page_login
from selenium_python.interfaces.page_paypal import page_paypal
from selenium_python.interfaces.page_profile import page_profile, tab_transaction, tab_gcoin
from selenium_python.interfaces.page_admin import page_admin, panel_preapproval
from selenium_python.interfaces.page_invoice import page_invoice
from selenium_python import global_variables as glb
from selenium_python.common import waitModule, common

import time


class list_test_gcoin(TestCase):
    glb.url_homepage = 'http://staging.gtoken.com'
    glb.url_signIn = glb.url_homepage + '/account/login'
    glb.url_admin = glb.url_homepage + '/admin'
    glb.url_logout = glb.url_homepage + '/account/logout'

    def setUp(self):
        self.driver.maximize_window()
        self.wait = waitModule()
        self.cm = common()

        self.pg_login = page_login(self.driver, glb.url_signIn)
        self.pg_header = page_header(self.driver)
        self.pg_paypal = page_paypal(self.driver)
        self.pg_profile = page_profile(self.driver)
        self.tb_transaction = tab_transaction(self.driver)
        self.tab_gcoin = tab_gcoin(self.driver)
        self.pg_admin = page_admin(self.driver)
        self.pnl_preapproval = panel_preapproval(self.driver)
        self.page_invoice = page_invoice(self.driver)


    # # @screenshot_on_error
    def test01_delete_preapproval(self):
        self.pg_login.ac_login(glb.s_admin_username, glb.s_admin_password)

        self.driver.get(glb.url_admin)

        self.pg_admin.lnk_paypal_preapproval.click()

        # Delete the current preapproval 
        if self.pnl_preapproval.btn_cancel_preapproval is not None:
            self.pnl_preapproval.cancel_preapproval()

    def test02_convert_gcoin_without_preapproval(self):
        # Login to gtokentester account
        self.pg_login.ac_login(glb.s_gtoken_username, glb.s_gtoken_password)

        # Convert Gcoin without preapproval
        self.pg_profile.lnk_transaction_panel.click()
        self.tb_transaction.lnk_gcoin_panel.click()
        self.cm.scroll_to_bottom(self.driver)
        self.tab_gcoin.ac_convert_gcoin(glb.s_username_paypal, glb.s_gtoken_password)

        # Logout 
        self.pg_header.ac_logout()

        # Login and go to Pending GCoin Transactions
        self.driver.get(glb.url_signIn)
        self.pg_login.ac_login(glb.s_admin_username, glb.s_admin_password)
        self.driver.get(glb.url_admin)
        self.pg_admin.lnk_paypal_preapproval.click()
        self.pnl_preapproval.btn_pending_gcoin.click()

        number_elements = len(self.pnl_preapproval.btn_pay_pending)
        print number_elements
        for btn in range(0, number_elements):
            self.pnl_preapproval.btn_pay_pending[0].click()
            self.wait.wait_for_text(self.driver, glb.msg_transaction_successfully)

    def test03_create_preapproval(self):
        self.pg_login.ac_login(glb.s_admin_username, glb.s_admin_password)

        self.driver.get(glb.url_admin)

        self.pg_admin.lnk_paypal_preapproval.click()

        if self.pnl_preapproval.btn_cancel_preapproval is not None:
            self.pnl_preapproval.cancel_preapproval()
        
        self.pnl_preapproval.btn_get_preapproval.click()

        self.pnl_preapproval.fill_preapproval_form()
        self.pnl_preapproval.btn_submit.click()

        self.pg_paypal.txt_password.send_keys(glb.s_password_paypal_sender)
        self.pg_paypal.btn_login.click()
        self.pg_paypal.btn_approval.click()
        self.pg_paypal.btn_return.click()

        self.assertElementDisplayed(self.pnl_preapproval.btn_cancel_preapproval, 'Get Preapproval successfully.')

    def test04_convert_gcoin_with_preapproval(self):
        
        self.pg_login.ac_login(glb.s_gtoken_username, glb.s_gtoken_password)
        self.pg_profile.lnk_transaction_panel.click()
        self.tb_transaction.lnk_gcoin_panel.click()

        self.cm.scroll_to_bottom(self.driver)
        self.tab_gcoin.ac_convert_gcoin(glb.s_username_paypal, glb.s_gtoken_password)

        self.wait.wait_for_text(self.driver, glb.msg_convert_gcoin)

    def test05_invoice_info(self):
        day_convert = ""
        self.pg_login.ac_login(glb.s_gtoken_username, glb.s_gtoken_password)
        self.pg_profile.lnk_transaction_panel.click()

        self.cm.scroll_to_bottom(self.driver)
        invoice_id = self.tb_transaction.lnk_invoice[0].get_attribute("href").split("=")[1]

        file = open(glb.path_gcoin_outcome, "w")
        file.write(invoice_id)
        file.close()

        self.tb_transaction.lnk_invoice[0].click()
        self.driver.switch_to_window(self.driver.window_handles[-1])

        assert self.page_invoice.table_payer.infos["Name"] == glb.s_gtoken_username
        assert self.page_invoice.table_payer.infos["Email"] == glb.s_gtoken_email
        
        if int(time.strftime("%d")) < 10:
            day_convert = str(time.strftime("%d")).replace("0", "")
        assert self.page_invoice.table_transaction.infos["Issue Date"] == time.strftime("%b") + ' ' + day_convert + ', ' + time.strftime("%Y")
        assert self.page_invoice.table_transaction.infos["Payment Mode"] == glb.s_paypal_method
        assert glb.s_description_gcoin in self.page_invoice.table_transaction.infos["Description"] 









