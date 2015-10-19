import time

# Variable of path
path_gcoin_income = "TestData/gcoin_income.txt"
path_gcoin_outcome = "TestData/gcoin_outcome.txt"

# Variable of URL
# url_homepage = 'http://localhost:9000'
url_homepage = 'http://dev2.gtoken.com'
url_register = url_homepage + '/account/register'
url_signIn = url_homepage + '/account/login'
url_profile = url_homepage + '/account/profile'
url_admin = url_homepage + '/admin'
url_logout = url_homepage + '/account/logout'


# Varialbe of API URl
url_api_profile_edit = url_homepage + '/api/1/account/profile-edit'
url_api_reward = url_homepage + '/api/1/game/reward-gcoin'
url_api_login = url_homepage + '/api/1/account/login-password'
url_api_update_game_stats = url_homepage + '/api/1/game/update-game-stats'

s_username = 'foxymax'
s_password = '123'
s_email = 'phuong.gtoken@gmail.com'

s_admin_username = 'foxyadmin'
s_admin_password = '123'

s_chat_username = 'foxychat'
s_chat_password = '123'

s_gtoken_username = 'gtokentester'
s_gtoken_password = '123456'
s_gtoken_email = 'phuong.gtoken+exchange@gmail.com'

s_referral_username = 'foxyreferral'
s_referral_password = '123'

s_username_paypal_sender = 'khang-facilitator@gtoken.com'
s_password_paypal_sender = 'secretsecret'

hour = time.strftime("%H")
minute = time.strftime("%M")
second = time.strftime("%S")
uni_val = hour + minute + second

s_paypal_method = 'Paypal'
s_description_gcoin = 'You converted 1 GCoin to USD and sent to account khang+paypalbuyer@gtoken.com'

# Paypal Account 
s_username_paypal = 'khang+paypalbuyer@gtoken.com'
s_password_paypal = 'secretsecret'

int_waiting_time = 6

msg_login_successfully = 'Logged in successfully'
msg_transaction_successfully = 'Transaction is successful'
msg_convert_gcoin = 'GCoin transaction is successful'
msg_logout_successfully = 'You have been logged out'