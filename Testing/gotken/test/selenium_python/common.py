from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException

from selenium_python import global_variables as glb

import time
import requests
import json

class common(object):
    def close_alert (self, driver, accepted = True):
        try:
            # WebDriverWait(self.driver, 3).until(EC.alert_is_present(), 'Timed out waiting for PA creation')
            time.sleep(3)
            alert = driver.switch_to_alert()

            if accepted == True:
                alert.accept()
            else:
                alert.dismiss()
            assert True, "alert accepted/dismiss."
        except Exception:
            raise
            # assert False, "No alert or cannot accept!"

    def scroll_to_bottom(self, driver):
        driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")

    def scroll_to_top(self, driver):
        driver.execute_script("window.scrollTo(0, 0);")
        
class waitModule(object):

    def wait_for_text(self, driver, text, timeout = glb.int_waiting_time*2):
        try:
            WebDriverWait(driver, timeout).until(EC.visibility_of_element_located((By.XPATH, "//*[contains(text(),'"+text+"')]")))
        except Exception, e:
            assert False, "Fail to wait for text " + "'" +text + "'"

    def wait_for_element(self, driver, selector, selector_name = 'none'):
        step = 1   # in seconds; sleep for 1s
        current_wait = 0
        boolean_wait = False
        while current_wait < glb.int_waiting_time*2:
            try:
                if selector.is_displayed():
                    boolean_wait = True
                    break
                else:
                    time.sleep(step)
                    current_wait += step
            except:
                time.sleep(step)
                current_wait += step
        if selector_name != 'none':
            assert boolean_wait, "Fail to wait for element " + selector_name
        else:
            assert boolean_wait, "Fail to wait for element "
        
class api_common(object):
    def post_api_request(url, paras):
        try:
            return requests.post(url, data= paras, verify=False).json()
        except NotGetReponse, e:
            assert (False),"Cannot get response from API: " + url
            return None



