# nosetests selenium_python/testcases/API/08_test_api_edit_profile.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb

import requests
import json
import random

edited_email = 'edited_email_' + glb.uni_val +'@gmail.com'
edited_nickname = 'nickname_' + glb.uni_val
edited_inviter = 'inviter_' + glb.uni_val
edited_gender = random.choice(['male','female','other'])

class list_test_api_edit_profile(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_existing_username, 'password': glb.s_existing_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_edit_profile_successfully(self):
        payloadEditedProfile =  {'session': self.session, 'partner_id': glb.partner_id,
                                'email': edited_email, 'nickname':edited_nickname, 'gender':edited_gender,
                                'referral_code' : edited_inviter}
        result = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()

        print result
        assert (result["success"] == True),"Edited profile successfully."
        assert (result["profile"]["account"] == glb.s_existing_username),"Account " + glb.s_gtoken_username + " has been added correctly."
        assert (result["profile"]["inviter"] == glb.s_existing_inviter),"Inviter shouldn't be able to edit correctly."
        assert (result["profile"]["email"] == edited_email),"Email edited correctly." + self.session
        assert (result["profile"]["nickname"] == edited_nickname),"Nickname edited correctly."
        assert (result["profile"]["gender"] == edited_gender),"Gender edited correctly."

    def test_api_edit_profile_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadEditedProfile = {'session': '@@acsa', 'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()

        assert (result["success"] == False),"Edited profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadEditedProfile = {'session': '', 'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()

        assert (result["success"] == False),"Edited profile with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadEditedProfile = {'session': self.session, 'partner_id': 'asc@'}
        result = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()

        assert (result["success"] == False),"Edited profile with invalid session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_edit_profile_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadEditedProfile = {'session': self.session, 'partner_id': ''}
        result = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()

        assert (result["success"] == False),"Edited profile with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
