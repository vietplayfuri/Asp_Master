from holmium.core import Element, Elements, ElementMap, Locators, Page, conditions
from selenium_python import global_variables as glb
from selenium_python.common import waitModule, common

import time

class page_admin(Page):
    lnk_paypal_preapproval = Element( Locators.CSS_SELECTOR, 'a[href="/admin/paypal/"]', timeout = glb.int_waiting_time)

class panel_preapproval(Page):
	btn_cancel_preapproval = Element( Locators.CSS_SELECTOR, '.button[href*="cancelkey"]', timeout = glb.int_waiting_time)
	btn_get_preapproval = Element(Locators.CSS_SELECTOR, '.button[href*="PreapprovalSubmit"]', timeout = glb.int_waiting_time)
	btn_pending_gcoin = Element(Locators.CSS_SELECTOR, '.button[href*="/admin/gcoin-transaction/"]', timeout = glb.int_waiting_time)
	btn_submit = Element(Locators.CSS_SELECTOR, '#submit', timeout = glb.int_waiting_time)
	btn_process_gcoin = Element(Locators.CSS_SELECTOR, '.button[href*="/admin/gcoin-transaction/"]', timeout = glb.int_waiting_time)
	btn_pay_pending = Elements(Locators.CSS_SELECTOR, 'a[href*="payment-execute"]', timeout = glb.int_waiting_time)

	txt_starting_date = Element( Locators.CSS_SELECTOR, '#starting_date', timeout = glb.int_waiting_time)
	txt_ending_date = Element( Locators.CSS_SELECTOR, '#ending_date', timeout = glb.int_waiting_time)
	txt_payment_amount = Element( Locators.CSS_SELECTOR, '#max_amount_per_payment', timeout = glb.int_waiting_time)
	txt_payment_number = Element( Locators.CSS_SELECTOR, '#max_number_of_payments', timeout = glb.int_waiting_time)
	txt_payments_total_amount = Element( Locators.CSS_SELECTOR, '#max_total_amount_of_all_payments', timeout = glb.int_waiting_time)
	txt_sender_email = Element( Locators.CSS_SELECTOR, '#sender_email', timeout = glb.int_waiting_time)

	def cancel_preapproval(self):
		cm = common()
		wait = waitModule()
		# elements.is_displayed()
		self.btn_cancel_preapproval.click()
		cm.close_alert(self.driver)
		wait.wait_for_text(self.driver, glb.msg_transaction_successfully)

	def fill_preapproval_form(self):
		day = time.strftime("%d")
		next_day = str(int(day) + 1)
		month = time.strftime("%m")
		year = time.strftime("%Y")

		self.txt_starting_date.send_keys(year+'-'+month+'-'+day+' 00:00')
		self.txt_ending_date.send_keys(year+'-'+month+'-'+ next_day +' 00:00')
		self.txt_payment_amount.send_keys('100')
		self.txt_payment_number.send_keys('10')
		self.txt_payments_total_amount.send_keys('1100')
		self.txt_sender_email.send_keys(glb.s_username_paypal_sender)
		self.txt_payment_amount.click()

		

