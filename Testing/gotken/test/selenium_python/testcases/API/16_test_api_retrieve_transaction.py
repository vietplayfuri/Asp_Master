# nosetests selenium_python/testcases/API/16_test_api_retrieve_transaction.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

order_id = 'GTest_' + glb.uni_val
source_currency = 'USD'
destination_currency = 'SGD'
description = "Testing by FoxyMax"
transaction = json.dumps(   {"username": "gtokentester", "order_id" : order_id, "original_price" : 10,
                            "original_final_amount": 9, "original_currency" : source_currency, "discount_percentage" : 0.1,
                            "payment_method": "cash", "description": description, "revenue_percentage": 0.02})

class list_test_api_create_transaction(TestCase):
    #
    # def test01_api_get_conversion_rate(self):
    #     payload = {"partner_id": glb.partner_id, "hashed_token":  glb.hashed_token, "transaction" : transaction}
    #     result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()
    #
    #     assert (result["success"] == True),"Get conversion rate successfully."

    def test02_api_get_conversion_rate_with_invalid_hashed_token(self):
        error_code = 'INVALID_HASHED_TOKEN'
        message = 'Invalid hashed token'

        payload = {"partner_id": glb.partner_id, "hashed_token":  'as@c', "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test03_api_get_conversion_rate_with_empty_hashed_token(self):
        error_code = 'INVALID_HASHED_TOKEN'
        message = 'Invalid hashed token'

        payload = {"partner_id": glb.partner_id, "hashed_token":  '', "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test04_api_get_conversion_rate_with_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload = {"partner_id": 'asc', "hashed_token":  glb.hashed_token, "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test05_api_get_conversion_rate_with_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload = {"partner_id": '', "hashed_token":  glb.hashed_token, "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test06_api_get_conversion_rate_with_invalid_JSON(self):
        error_code = 'INVALID_JSON_TRANSACTION'
        message = 'Invalid transaction info (JSON)'

        payload = {"partner_id": glb.partner_id, "hashed_token":  glb.hashed_token, "transaction" : 'sa@'}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly. "
        assert (result["message"] == message), "Message showed correctly."

    def test07_api_get_conversion_rate_with_existing_order_id(self):
        transaction = json.dumps(   {"username": "gtokentester", "order_id" : glb.gtoken_order, "original_price" : 10,
                            "original_final_amount": 9, "original_currency" : source_currency, "discount_percentage" : 0.1,
                            "payment_method": "cash", "description": description, "revenue_percentage": 0.02})

        error_code = 'EXISTING_ORDERID'
        message = 'Order ID already exists'

        payload = {"partner_id": glb.partner_id, "hashed_token":  glb.hashed_token, "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test08_api_get_conversion_rate_with_non_existing_username(self):
        transaction = json.dumps(   {"username": "@@GTokenTester@@", "order_id" : order_id, "original_price" : 10,
                            "original_final_amount": 9, "original_currency" : source_currency, "discount_percentage" : 0.1,
                            "payment_method": "cash", "description": description, "revenue_percentage": 0.02})

        error_code = 'NON_EXISTING_USER'
        message = 'User Account does not exist'

        payload = {"partner_id": glb.partner_id, "hashed_token":  glb.hashed_token, "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test09_api_get_conversion_rate_with_missing_key(self):
        transaction = json.dumps(   {"order_id" : order_id, "original_price" : 10,
                            "original_final_amount": 9, "original_currency" : source_currency, "discount_percentage" : 0.1,
                            "payment_method": "cash", "description": description, "revenue_percentage": 0.02})

        error_code = 'MISSING_FIELDS'
        message = 'Required field(s) is blank'

        payload = {"partner_id": glb.partner_id, "hashed_token":  glb.hashed_token, "transaction" : transaction}
        result = requests.post(glb.url_api_create_transaction, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."












