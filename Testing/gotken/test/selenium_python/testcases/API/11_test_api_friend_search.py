# nosetests selenium_python/testcases/API/11_test_api_friend_search.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
from selenium_python.common_api import api

import requests
import json

keyword = 'gtoken'

class list_test_api_friend_search(TestCase):

    def setUp(self):
        payloadLogin =  {'username': glb.s_gtoken_username, 'password': glb.s_gtoken_password,
                        'partner_id': glb.partner_id}
        result = requests.post(glb.url_api_login_password, data= payloadLogin, verify=False).json()
        assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
        self.session = result["session"]

    def test_api_friend_search_find_gtoken_friend(self):
        
        payloadFriendSearch = {'session': self.session, 'partner_id': glb.partner_id,
                            'keyword' : keyword}
        result = requests.post(glb.url_api_friend_search, data=payloadFriendSearch, verify=False).json()
        print result
        assert (result["success"] == True),"Get Friend List successfully."

    def test_api_friend_search_invalid_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadFriendSearch =   {'session': 'a@', 'partner_id': glb.partner_id,
                                'keyword' : keyword}
        result = requests.post(glb.url_api_friend_search, data=payloadFriendSearch, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_search_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadFriendSearch =   {'session': self.session, 'partner_id': '@ac',
                                'keyword' : keyword}
        result = requests.post(glb.url_api_friend_search, data=payloadFriendSearch, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_search_empty_session(self):
        error_code = 'INVALID_SESSION'
        message = 'Invalid Session'

        payloadFriendSearch =   {'session': '', 'partner_id': glb.partner_id,
                                'keyword' : keyword}
        result = requests.post(glb.url_api_friend_search, data=payloadFriendSearch, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_friend_search_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadFriendSearch =   {'session': self.session, 'partner_id': '',
                                'keyword' : keyword}
        result = requests.post(glb.url_api_friend_search, data=payloadFriendSearch, verify=False).json()

        assert (result["success"] == False),"Change password with empty session should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."
