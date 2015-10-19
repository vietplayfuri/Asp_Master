# nosetests selenium_python/testcases/API/01_test_api_register.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

username = 'foxymax_' + glb.uni_val
password = '123'
email = 'phuong.gtoken@gmail.com'
nickname = 'nickname_' + glb.uni_val
gender = 'male'
referral_code = glb.s_gtoken_username

class list_test_api_register(TestCase):

    def test_api_register_with_referral_successfull(self):
        other_username = username + 'other'
        payloadRegister =   {'username': other_username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == True),"Register successfully."
        assert (result["profile"]["account"] == other_username),"Account " + other_username + " has been added correctly."
        assert (result["profile"]["inviter"] == referral_code),"Inviter added correctly."
        assert (result["profile"]["nickname"] == nickname), "Nickname added correctly."
        assert (result["profile"]["email"] == email),"Email added correctly."
        assert (result["profile"]["gtoken"] == 0),"GToken amount should be 0."

    def test_api_register_but_add_referral_late(self):
        payloadRegister =   {'username': username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == True),"Register successfully."
        assert (result["profile"]["account"] == username),"Account " + username + " has been added correctly."
        login_session = result["session"]

        payloadEditedProfile =  {'session': login_session, 'partner_id': glb.partner_id,
                                'referral_code' : glb.s_existing_username}
        result_edit_profile = requests.post(glb.url_api_edit_profile, data=payloadEditedProfile, verify=False).json()
        assert (result_edit_profile["success"] == True),"Edited profile successfully."
        assert (result_edit_profile["profile"]["inviter"] == glb.s_existing_username),"Add referral code successfully."

    # def test_api_register_for_send_request_api(self):
    #     payloadRegister =   {'username': glb.s_existing_username_2, 'password': glb.s_existing_password_2,
    #                         'partner_id': glb.partner_id}
    #     result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
    #     assert (result["success"] == True),"Register successfully." + json.dumps(result)

    def test_api_register_existing_username(self):
        error_code = 'EXISTING_USERNAME_EMAIL'
        message = 'Account with such username/email already exists'

        payloadRegister =   {'username': glb.s_existing_username, 'password': glb.s_existing_password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': glb.s_gtoken_username}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with existing username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_empty_username(self):
        invalid_username = ''
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payloadRegister =   {'username': invalid_username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_empty_password(self):
        invalid_password = ''
        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payloadRegister =   {'username': username, 'password': invalid_password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_empty_partner_id(self):
        invalid_partner_id = ''
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payloadRegister =   {'username': username, 'password': password,'partner_id': invalid_partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_special_char(self):
        invalid_username = 'foxymax@'
        error_code = 'INVALID_USERNAME'
        message = 'Username does not accept special characters'

        payloadRegister =   {'username': invalid_username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_username_less_than_3_chars(self):
        invalid_username = 'av321312312312dascas213'
        error_code = 'USERNAME_LENGTH'
        message = 'Username is between 3-20 characters'

        payloadRegister =   {'username': invalid_username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_username_more_than_20_chars(self):
        invalid_username = 'ab'
        error_code = 'USERNAME_LENGTH'
        message = 'Username is between 3-20 characters'

        payloadRegister =   {'username': invalid_username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_password_less_than_3_chars(self):
        invalid_password = 'ab'
        error_code = 'PASSWORD_LENGTH'
        message = 'Password must be more than 3 characters'

        payloadRegister =   {'username': username, 'password': invalid_password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with invalid username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."

    def test_api_register_with_non_existing_referral_code(self):
        invalid_referral_code = 'abc@@'
        error_code = 'NON_EXISTING_REFERRAL_CODE'
        message = 'Referral Code does not exist'

        payloadRegister =   {'username': username, 'password': password,'partner_id': glb.partner_id,
                            'email':email, 'nickname': nickname, 'gender': gender,
                            'referral_code': invalid_referral_code}
        result = requests.post(glb.url_api_register, data=payloadRegister, verify=False).json()
        assert (result["success"] == False),"Register with empty username should be unsuccessful."
        assert (result["error_code"] == error_code),"Error code showed correctly."
        assert (result["message"] == message),"Message showed correctly."


