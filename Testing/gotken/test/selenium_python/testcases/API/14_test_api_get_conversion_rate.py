# nosetests selenium_python/testcases/API/14_test_api_get_conversion_rate.py -v --logging-level=WARNING
from unittest import TestCase
from selenium_python import global_variables as glb
import requests
import json

source_currency = 'USD'
destination_currency = 'SGD'

class list_test_api_get_conversion_rate(TestCase):

    def test01_api_get_conversion_rate(self):
        payload =   {'partner_id': glb.partner_id, 'hashed_token': glb.hashed_token,
                    'source_currency' : source_currency,'destination_currency':destination_currency}
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == True),"Get conversion rate successfully."
        assert (result["source_currency"] == source_currency),"Correct source currency"
        assert (result["destination_currency"] == destination_currency),"Correct destination currency"
        assert ("exchange_rate" in result), "Get exchange rate successfully"

    def test02_api_get_conversion_rate_with_invalid_hashed_token(self):
        error_code = 'INVALID_HASHED_TOKEN'
        message = 'Invalid hashed token'

        payload =   {'hashed_token': "ascsa", 'partner_id': glb.partner_id,
                    'source_currency' : source_currency,'destination_currency':destination_currency }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test03_api_get_conversion_rate_with_empty_hashed_token(self):
        error_code = 'INVALID_HASHED_TOKEN'
        message = 'Invalid hashed token'

        payload =   {'hashed_token': "", 'partner_id': glb.partner_id,
                    'source_currency' : source_currency,'destination_currency':destination_currency }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test04_api_get_conversion_rate_with_invalid_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'hashed_token': glb.hashed_token, 'partner_id': 'ascasca',
                    'source_currency' : source_currency,'destination_currency':destination_currency }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test05_api_get_conversion_rate_with_empty_partner_id(self):
        error_code = 'INVALID_PARTNER_ID'
        message = 'Invalid Partner ID'

        payload =   {'hashed_token': glb.hashed_token, 'partner_id': "",
                    'source_currency' : source_currency,'destination_currency':destination_currency }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test06_api_get_conversion_rate_with_invalid_source_currency(self):
        error_code = 'INVALID_CURRENCY_CODE'
        message = 'Currency code not found (ISO 4217)'

        payload =   {'hashed_token': glb.hashed_token, 'partner_id': glb.partner_id,
                    'source_currency' : "FXY",'destination_currency':destination_currency }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."

    def test07_api_get_conversion_rate_with_invalid_destination_currency(self):
        error_code = 'INVALID_CURRENCY_CODE'
        message = 'Currency code not found (ISO 4217)'

        payload =   {'hashed_token': glb.hashed_token, 'partner_id': glb.partner_id,
                    'source_currency' : source_currency,'destination_currency': "FXY" }
        result = requests.post(glb.url_api_get_conversion_rate, data=payload, verify=False).json()

        assert (result["success"] == False)
        assert (result["error_code"] == error_code), "Error code showed correctly."
        assert (result["message"] == message), "Message showed correctly."
