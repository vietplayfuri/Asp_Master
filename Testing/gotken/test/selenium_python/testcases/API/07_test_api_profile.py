# nosetests selenium_python/testcases/API/07_test_api_profile.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

class list_test_api_profile(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_profile_get_full_info(self):
        payloadProfile = {'session': self.session, 'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()

        assert (result["success"] == True),"Get profile successfully."
        assert (result["profile"]["account"] == glb.s_gtoken_username),"Account " + glb.s_gtoken_username + " has been added correctly."
        assert (result["profile"]["inviter"] == glb.s_gtoken_inviter),"Inviter added correctly."
        assert (result["profile"]["email"] == glb.s_gtoken_email),"Email added correctly."

    # def test_api_profile_get_few_info(self):
    #     payloadProfile = {'username': glb.s_gtoken_username, 'partner_id': glb.partner_id}
    #     result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()
    #
    #     assert (result["success"] == True),"Get profile successfully."
    #     assert (result["profile"]["account"] == glb.s_gtoken_username),"Account " + glb.s_gtoken_username + " has been added correctly."
    #     assert ("inviter" not in result["profile"]),"Inviter not showed."
    #     assert ("email" not in result["profile"]),"Email now showed."

    def test_api_profile_invalid_username(self):
        error_code = 'NON_EXISTING_USER'
        message = 'User Account does not exist'

        payloadProfile = {'username': '@@acsa', 'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()

        assert (result["success"] == False),"Get profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_profile_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadProfile = {'session': self.session, 'partner_id': 'mybadass'}
        result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()

        assert (result["success"] == False),"Get profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_profile_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadProfile = {'session': self.session, 'partner_id': ''}
        result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()

        assert (result["success"] == False),"Get profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_profile_empty_username(self):
        error_code = 'NON_EXISTING_USER'
        message = 'User Account does not exist'

        payloadProfile = {'username': '', 'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_profile, data=payloadProfile, verify=False).json()

        assert (result["success"] == False),"Get profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
