from selenium_python import global_variables as glb

import time
import requests
import json
   
class api(object):
    def post_api_request(url, paras):
        try:
            return requests.post(url, data= paras, verify=False).json()
        except NotGetReponse, e:
            assert (False),"Cannot get response from API: " + url
            return None



