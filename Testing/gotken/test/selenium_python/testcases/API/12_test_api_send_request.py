# nosetests selenium_python/testcases/API/12_test_api_send_request.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
from selenium_python.common_api import api

import requests
import json

class list_test_api_send_request(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_existing_username_2, 'password': glb.s_existing_password_2,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_send_request_successfully(self):
        error_code = 'REQUEST_ALREADY_SENT'
        message = 'Friend request has already been sent'
        
        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_gtoken_username}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()
        if result["success"]:
            assert (result["success"] == True),"Send request successfully."
        elif (result["success"] == False):
            if (result["error_code"] == error_code):
                assert (result["message"] == message),"Message showed correctly."
            else:
                assert False, "Cannot send friend request."
        else:
            assert (False), "Cannot execute the request."

    def test_api_send_request_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payload =   {'session': 'a@', 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_gtoken_username}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_send_request_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'session': self.session, 'partner_id': '@ac',
                    'friend_username' : glb.s_gtoken_username}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with invalid partner ID should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_send_request_invalid_friend_username(self):
        error_code = 'NON_EXISTING_USER'
        message = 'User Account does not exist'

        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : '2131@@!'}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_send_request_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payload =   {'session': '', 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_gtoken_username}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_send_request_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'session': self.session, 'partner_id': '',
                    'friend_username' : glb.s_gtoken_username}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with empty partner ID should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_send_request_empty_friend_username(self):
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : ''}
        result = requests.post(glb.url_api_send_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Send request with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
