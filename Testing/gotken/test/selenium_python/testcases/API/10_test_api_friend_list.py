# nosetests selenium_python/testcases/API/10_test_api_friend_list.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

class list_test_api_friend_list(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_friend_list_get_profile_friend(self):
        payloadFriendList = {'session': self.session, 'partner_id': glb.partner_id,
                            'include_profile' : True}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == True),"Get Friend List successfully."
        assert (len(result["friends"]) > 0),"List friends showed correctly."
        assert (glb.s_existing_username in result["friends"]),"The inviter should be a friend in the list."
        assert (len(result["friends"][glb.s_existing_username])>1), "Should show full profile of friend."

    def test_api_friend_list_get_friend_list_only(self):
        payloadFriendList = {'session': self.session, 'partner_id': glb.partner_id,
                            'include_profile' : False}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == True),"Get Friend List successfully."
        assert (len(result["friends"]) > 0),"List friends showed correctly."
        assert (glb.s_existing_username in result["friends"]),"The inviter should be a friend in the list." 
    
    def test_api_friend_list_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadFriendList = {'session': 'a@', 'partner_id': glb.partner_id,
                            'include_profile' : False}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_list_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadFriendList = {'session': self.session, 'partner_id': '@ac',
                            'include_profile' : False}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_list_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadFriendList = {'session': '', 'partner_id': glb.partner_id,
                            'include_profile' : False}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_list_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadFriendList = {'session': self.session, 'partner_id': '',
                            'include_profile' : False}
        result = requests.post(glb.url_api_friend_list, data=payloadFriendList, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
