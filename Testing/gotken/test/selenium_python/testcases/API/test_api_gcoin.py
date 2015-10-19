# nosetests selenium_python/testcases/API/test_api_gcoin.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

s_gameID = '8b1d8776e813536e2cb5fa3341079597'
payload_login = {'username': 'gtokentester', 'password':'123456','game_id': s_gameID}

# class list_test_api_gcoin(TestCase):
#     glb.url_homepage = 'http://staging.gtoken.com'
#     glb.url_api_reward = glb.url_homepage + '/api/1/game/reward-gcoin'
#     glb.url_api_login = glb.url_homepage + '/api/1/account/login-password'
#     def setUp(self):
#         result = requests.post(glb.url_api_login, data=payload_login, verify=False).json()
#         assert (result["success"] == True), "Sign In Successfully." + glb.msgLogInSuccessfully
#         self.session = result["session"]
#
#     def test_gcoin_api_success(self):
#         payloadReward = {'session': self.session, 'amount':'5','game_id': s_gameID, 'description':'Testing by FoxyMax'}
#         result = requests.post(glb.url_api_reward, data=payloadReward, verify=False).json()
#         assert (result["success"] == True),"Convert GCoin Successfully."
#
#         file = open(glb.path_gcoin_income, "w")
#         file.write(result["transaction"]["transaction_id"])
#         file.close()
#
#     def test_gcoin_invalid_amount(self):
#         payloadReward = {'session': self.session, 'amount':'1a','game_id': s_gameID}
#         result = requests.post(glb.url_api_reward, data=payloadReward, verify=False).json()
#         assert (result["message"] == 'Invalid amount'),"Cannot convert GCoin with invalid amount."
#
#     def test_gcoin_negative_amount(self):
#         payloadReward = {'session': self.session, 'amount':'-1','game_id': s_gameID}
#         result = requests.post(glb.url_api_reward, data=payloadReward, verify=False).json()
#         assert (result["message"] == 'Invalid amount'),"Cannot convert GCoin with negative amount."