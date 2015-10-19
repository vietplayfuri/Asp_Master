# nosetests selenium_python/testcases/API/test_api_update_game_stats.py -v --logging-level=WARNING
# import requests
# import json
# import time
#
# from storm.locals import Store
# from storm.locals import create_database
#
# from unittest import TestCase
# from selenium_python import global_variables as glb
#
# s_gameID = '8b1d8776e813536e2cb5fa3341079597'
# payload_login = {'username': 'gtokentester',
#                  'password':'123456',
#                  'game_id': s_gameID}
# stats_status = 'true'
# stats_value = time.strftime("%S")
# stats_title ='level'
# game_stats = '[{"public": "'+stats_status+'", "value": "'+stats_value+'", "title": "'+stats_title+'"}]'
# invalid_game_stats_status = '[{"public": "2", "value": "2", "title": "level"}]'
# invalid_game_stats_keys = '[{"public": "true", "value2": "2", "title": "level"}]'

# class list_test_api_update_game_stats(TestCase):
#     def setUp(self):
#         result = requests.post(glb.url_api_login, data=payload_login, verify=False).json()
#         assert (result["success"] == True), "Sign In Successfully."
#         self.session = result["session"]
#
#     def test_update_game_stats(self):
#         payload_update_game_stats = {'session': self.session,
#                                      'game_id': s_gameID,
#                                      'stats': game_stats}
#         result = requests.post(glb.url_api_update_game_stats, data=payload_update_game_stats, verify=False)
#         # assert ('https' in result.url),"Redirect to HTTPS."
#         result = result.json()
#         assert (result["success"] == True),"Update game stats successfully."
#
#         # Establish db connection
#         conn_string = "postgres://gtokendb:gtokendb@localhost/gtokendb"
#         store = Store.__new__(Store)
#         database = create_database(conn_string)
#         store.__init__(database)
#         pg_conn = store._connection
#
#         pg_cursor = pg_conn.build_raw_cursor()
#         pg_cursor.execute("SELECT * FROM game_access_token WHERE token='%s';" % self.session)
#
#         # retrieve the records from the database
#         result = pg_conn.result_factory(pg_conn, pg_cursor)
#         for row in result:
#            db_game_stats = json.loads(row[7])
#            assert db_game_stats[0]['public'] == stats_status
#            assert db_game_stats[0]['value'] == stats_value
#            assert db_game_stats[0]['title'] ==stats_title
#
#     def test_update_game_stats_with_invalid_json(self):
#         payload_update_game_stats = {'session': self.session, 'game_id': s_gameID, 'stats': invalid_game_stats_status}
#         result = requests.post(glb.url_api_update_game_stats, data=payload_update_game_stats, verify=False)
#         result = result.json()
#         assert (result["success"] == False),"Cannot update game stats with invalid json"
#         assert ('Invalid stat format' in result["message"]),"Readable message was sent to client."
#
#         payload_update_game_stats = {'session': self.session, 'game_id': s_gameID, 'stats': invalid_game_stats_keys}
#         result = requests.post(glb.url_api_update_game_stats, data=payload_update_game_stats, verify=False)
#         result = result.json()
#         assert (result["success"] == False),"Cannot update game stats with invalid json"
#         assert ('Invalid stat format' in result["message"]),"Readable message was sent to client."

    # def test_gcoin_negative_amount(self):
    #   payloadReward = {'session': self.session, 'amount':'-1','game_id': s_gameID}
    #   result = requests.post(glb.url_api_reward, data=payloadReward, verify=False).json()
    #   assert (result["message"] == 'Invalid amount'),"Cannot convert GCoin with negative amount."
        