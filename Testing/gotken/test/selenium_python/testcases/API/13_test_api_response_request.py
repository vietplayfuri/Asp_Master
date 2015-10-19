# nosetests selenium_python/testcases/API/13_test_api_response_request.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
from selenium_python.common_api import api

import requests
import json

accepted_status = 'accepted'
rejected_status = 'rejected'

class list_test_api_response_request(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test01_api_response_invalid_status(self):
        error_code = 'INVALID_FRIEND_REQUEST_STATUS'
        message = u"Friend request status must be either 'accepted' or 'rejected'"

        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status':'@saca@'}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with invalid status should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    # def test02_api_response_empty_status(self):
    #     error_code = 'MISSING_FIELDS'
    #     message = 'Required field(s) is blank'
    #
    #     payload =   {'session': self.session, 'partner_id': glb.partner_id,
    #                 'friend_username' : glb.s_existing_username_2, 'status':''}
    #     result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()
    #
    #     assert (result["success"] == False),"Response request with empty status should be unsuccessful."
    #     assert (result["error_code"] == error_code),"Error code showed correctly."
    #     assert (result["message"] == message),"Message showed correctly."

    def test03_api_response_with_invalid_username(self):
        error_code = 'NON_EXISTING_USER'
        message = 'User Account does not exist'

        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : '@DSD', 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test04_api_response_with_empty_username(self):
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : '', 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test05_api_response_with_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payload =   {'session': '@WSDA', 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test06_api_response_with_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payload =   {'session': '', 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test07_api_response_with_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'session': self.session, 'partner_id': '@Sd',
                    'friend_username' : glb.s_existing_username_2, 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with empty partner ID should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test08_api_response_with_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'session': self.session, 'partner_id': '',
                    'friend_username' : glb.s_existing_username_2, 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Response request with empty partner ID should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test09_api_response_invalid_status(self):
        error_code = 'INVALID_FRIEND_REQUEST_STATUS'
        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status': "ascAAS"}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == False),"Cannot reponse friend request with invalid status."
        assert (result["error_code"] == error_code),"Error code showed correctly."

    def test10_api_response_rejected_status(self):
        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status': rejected_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()
        print result
        assert (result["success"] == True),"Reject friend request successfully."
        #assert (glb.s_existing_username_2 not in result["friends"]), "The friend shouldn't be added."

    def test11_api_response_accepted_status(self):
        payload =   {'session': self.session, 'partner_id': glb.partner_id,
                    'friend_username' : glb.s_existing_username_2, 'status': accepted_status}
        result = requests.post(glb.url_api_response_request, data=payload, verify=False).json()

        assert (result["success"] == True),"Accept friend request successfully."
        assert (glb.s_existing_username_2 in result["friends"]), "The friend should be added."
