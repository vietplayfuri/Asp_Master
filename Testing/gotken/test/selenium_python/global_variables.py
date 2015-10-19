import time

# Partner ID (GoPlay)
partner_id = '7bb4797a-1cb9-43d8-a0e7-0e5ae1a2a295'

# Hashed token (GoPlay) on dev.gtoken.com
hashed_token = '5ab3687c27ca1cdde0f846c150b9907093e1b9139eb48d010aca5d0711c35eabcef4c7d1156b072dd5f4e336c40e387959951ce9d735f03feb27a090cffeb3ce'

# Order_id
gtoken_order = 'gtoken_order'

# Variable of path
path_gcoin_income = "TestData/gcoin_income.txt"
path_gcoin_outcome = "TestData/gcoin_outcome.txt"

# Variable of URL
url_homepage = 'https://dev2.gtoken.com/service'
url_register = url_homepage + '/account/register'
url_signIn = url_homepage + '/account/login'
url_profile = url_homepage + '/account/profile'
url_admin = url_homepage + '/admin'
url_logout = url_homepage + '/account/logout'

# Varialbe of API End Point
url_api_register = url_homepage +'/api/1/account/register'
url_api_login_password = url_homepage +'/api/1/account/login-password'
url_api_profile = url_homepage +'/api/1/account/profile'
url_api_edit_profile = url_homepage +'/api/1/account/edit-profile'
url_api_change_password = url_homepage +'/api/1/account/change-password'
url_api_friend_list = url_homepage +'/api/1/friend/friend-list'
url_api_friend_search = url_homepage +'/api/1/friend/search'
url_api_send_request = url_homepage +'/api/1/friend/send-request'
url_api_response_request = url_homepage +'/api/1/friend/respond-request'
url_api_get_conversion_rate = url_homepage +'/api/1/transaction/get-conversion-rate'
url_api_create_transaction = url_homepage +'/api/1/transaction/create-transaction'

s_username = 'foxymax'
s_password = '123'
s_email = 'phuong.gtoken@gmail.com'

s_gtoken_username = 'han1'
s_gtoken_password = 'b123'
s_gtoken_email = 'quochuy298@gmail.com'
s_gtoken_inviter = 'thaotest1'

s_existing_username_2 = 'jang'
s_existing_password_2 = '123456'

s_existing_username = 'thaotest1'
s_existing_password = '123'
s_existing_inviter = 'test'

s_admin_username = 'foxyadmin'
s_admin_password = '123'

s_chat_username = 'foxychat'
s_chat_password = '123'

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