# nosetests selenium_python/testcases/API/09_test_api_change_password.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb

import requests
import json
import random

username = 'chgpwd_' + glb.uni_val
old_password = '123'
new_password = '1234'

class list_test_api_change_password(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_existing_username, 'password': glb.s_existing_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_change_password_successfully(self):
        payloadRegister =   {'username': username, 'password': old_password,
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == True),"Register successfully."
        self.session = result["session"]

        payloadChangePassword =  {'session': self.session, 'partner_id': glb.partner_id,
                                'old_password': old_password, 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()
        assert (result["success"] == True),"Change password successfully."

        payloadLogin =   {'username': username, 'password': new_password,
                            'partner_id': glb.partner_id}
        login_result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (login_result["success"] == True),"Login with new password successfully."

    def test_api_change_password_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadChangePassword = {'session': '@@acsa', 'partner_id': glb.partner_id,
                                'old_password': old_password, 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadChangePassword = {'session': '', 'partner_id': glb.partner_id,
                                'old_password': old_password, 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_change_password_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadChangePassword = {'session': self.session, 'partner_id': 's@@',
                                'old_password': old_password, 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadChangePassword = {'session': self.session, 'partner_id': '',
                                'old_password': old_password, 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_wrong_password(self):
        error_code = 'INVALID_USN_PWD'
        message = 'Username or Password is incorrect'

        payloadChangePassword = {'session': self.session, 'partner_id': glb.partner_id,
                                'old_password': '', 'new_password':new_password,
                                'confirm_password' : new_password}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_not_match_password(self):
        error_code = 'UNIDENTICAL_PASSWORDS'
        message = 'Password and Confirm Pass are not identical'

        payloadChangePassword = {'session': self.session, 'partner_id': glb.partner_id,
                                'old_password': old_password, 'new_password': new_password,
                                'confirm_password' : ''}
        result = requests.post(glb.url_api_change_password, data=payloadChangePassword, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
