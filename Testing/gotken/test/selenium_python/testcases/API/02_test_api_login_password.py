# nosetests selenium_python/testcases/API/02_test_api_login_password.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

class list_test_api_login_password(TestCase):

    def test_api_login_successfull(self):
        payloadLogin =   {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == True),"Login successfully."
        assert (result["profile"]["account"] == glb.s_gtoken_username),"username showed correctly."
        assert (result["profile"]["inviter"] == glb.s_gtoken_inviter),"Inviter showed correctly."
        assert (result["profile"]["email"] == glb.s_gtoken_email),"Email showed correctly."

    def test_api_login_with_invalid_username(self):
        error_code = 'INVALID_USN_PWD'
        message = 'Username or Password is incorrect'

        payloadLogin =   {'username': '@abc~', 'password': glb.s_gtoken_password,
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_login_with_invalid_password(self):
        error_code = 'INVALID_USN_PWD'
        message = 'Username or Password is incorrect'

        payloadLogin =   {'username': glb.s_gtoken_username, 'password': '@@asd2',
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with invalid password should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_login_with_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadLogin =   {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                            'partner_id': 'ascas@'}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with empty partner_id should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_login_with_empty_username(self):
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payloadLogin =   {'username': '', 'password': glb.s_gtoken_password,
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_login_with_empty_password(self):
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payloadLogin =   {'username': glb.s_gtoken_username, 'password': '',
                            'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with empty password should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_login_with_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadLogin =   {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                            'partner_id': ''}
        result = requests.post(glb.url_api_login_password, data=payloadLogin, verify=False).json()
        assert (result["success"] == False),"Login with empty partner_id should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
