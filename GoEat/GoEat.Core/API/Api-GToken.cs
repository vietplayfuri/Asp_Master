using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GoEat.Dal.Common;
using GoEat.Dal.Models;
using GoEat.Dal;
using GoEat.Models;
using GoEat.Utility;
using Newtonsoft.Json;
using GoEat.Models;

namespace GoEat.Core
{
    public partial class GoEatApi
    {
        /// <summary>
        /// Call login to Goplay API
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="pwd">password</param>
        /// <param name="game_id">restaurant id</param>
        /// <returns>result</returns>
        public async Task<Result<LoginModel>> Login(string userName, string pwd, string game_id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetBaseServerUri(); // TODO: Need a switch for live and debug server //
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", userName), 
                    new KeyValuePair<string, string>("password", pwd),
                    new KeyValuePair<string, string>("partner_id", Urls.PARTNER_ID)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_LOGIN, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var customerLogin = JsonConvert.DeserializeObject<LoginModel>(await response.Content.ReadAsStringAsync());
                    if (customerLogin.success)
                    {
                        if (UpdateCustomerAccountProfile(customerLogin.profile.uid, JsonConvert.SerializeObject(customerLogin.profile), customerLogin.session))
                        {
                            return Result<LoginModel>.Make(customerLogin);
                        }
                        //create register existing user of Gtoken master to restaurant.
                        var customer = new CustomerAccount_ToRemove
                        {
                            id = customerLogin.profile.uid,
                            username = customerLogin.profile.account,
                            created_at = DateTime.Now,
                            Profile = customerLogin.profile,
                            session = customerLogin.session
                        };
                        if (CreateCustomerAccount(customer))
                        {
                            return Result<LoginModel>.Make(customerLogin);
                        }
                    }
                }
                return Result<LoginModel>.Null(ErrorCodes.InvalidUserNameOrPassword);
            }
        }

        /// <summary>
        /// Register new account by calling the Goplay register API
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="game_id"></param>
        /// <param name="email"></param>
        /// <param name="nickname"></param>
        /// <param name="gender"></param>
        /// <param name="referral_code"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public async Task<Result<LoginModel>> Register(string userName, string pwd, string game_id,
            string email, string nickname, string gender, string referral_code, string ip_address)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetBaseServerUri(); // TODO: Need a switch forlive and debug server //
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", userName), 
                    new KeyValuePair<string, string>("password", pwd),
                    new KeyValuePair<string, string>("partner_id", Urls.PARTNER_ID),
                    new KeyValuePair<string, string>("email", email),
                    new KeyValuePair<string, string>("nickname",nickname),
                    new KeyValuePair<string, string>("gender", gender),
                    new KeyValuePair<string, string>("referral_code", referral_code),
                    new KeyValuePair<string, string>("ip_address", ip_address)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_REGISTER, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var registerRsult = JsonConvert.DeserializeObject<LoginModel>(await response.Content.ReadAsStringAsync());
                    if (registerRsult.success)
                    {
                        var customer = new CustomerAccount_ToRemove
                        {
                            id = registerRsult.profile.uid,
                            username = registerRsult.profile.account,
                            created_at = DateTime.Now,
                            Profile = registerRsult.profile,
                            session = registerRsult.session
                        };
                        if (CreateCustomerAccount(customer))
                        {
                            return Result<LoginModel>.Make(registerRsult);
                        }
                    }
                    else
                    {
                        // The conversion of Text to Enum should use EnumFromDescription helper //
                        var errCode = EnumConverter.EnumFromDescription<ErrorCodes>(registerRsult.error_code);
                        return Result<LoginModel>.Null(errCode);
                    }
                }
                return Result<LoginModel>.Null(ErrorCodes.NotFound);
            }
        }

        public async Task<bool> ResetPassword(string userName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetBaseServerUri(); // TODO: Need a switch forlive and debug server //
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", userName)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_RESET_PASSWORD, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var registerRsult = JsonConvert.DeserializeObject<LoginModel>(await response.Content.ReadAsStringAsync());
                    if (registerRsult.success)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        public async Task<bool> ConfirmPassword(string userName, string newPassword, string confirmedPassword)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetBaseServerUri(); // TODO: Need a switch forlive and debug server //
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", userName),
                     new KeyValuePair<string, string>("password", newPassword),
                      new KeyValuePair<string, string>("confirmed_password", confirmedPassword)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_RESET_CONFIRMED_PASSWORD, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var registerRsult = JsonConvert.DeserializeObject<LoginModel>(await response.Content.ReadAsStringAsync());
                    return registerRsult.success;
                }
                return false;
            }
        }

        /// <summary>
        /// Call Create new transaction of GToken API
        /// </summary>
        /// <param name="modal"></param>
        public Result<GTokenTransactionModel> CreateGTokenTransaction(GTokenTransaction modal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>( ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, Urls.HASHED_TOKEN),
                    new KeyValuePair<string, string>(ConstantValues.S_TRANSACTION, JsonConvert.SerializeObject(modal))
                });

                HttpResponseMessage response = client.PostAsync(Urls.ACTION_CREATE_TRANSACTION, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(response.Content.ReadAsStringAsync().Result);
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        /// <summary>
        /// Call to api "/api/1/transaction/direct-charge-gtoken" of Gtoken
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        public Result<ResultDirectChargeGtoken> DirectChargeGtoken(DirectChargeGtokenModel modal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, Urls.HASHED_TOKEN),
                    new KeyValuePair<string, string>(ConstantValues.S_DIRECT_GTOKEN_TRANSACTION, JsonConvert.SerializeObject(modal)),
                    new KeyValuePair<string, string>(ConstantValues.S_IP_ADDRESS, string.Empty),
                });

                HttpResponseMessage response = client.PostAsync(Urls.ACTION_DIRECT_CHARGE_GTOKEN, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResultDirectChargeGtoken>(response.Content.ReadAsStringAsync().Result);
                    if (result.success)
                    {
                        return Result<ResultDirectChargeGtoken>.Make(result);
                    }
                    return Result<ResultDirectChargeGtoken>.Null((ErrorCodes)Enum.Parse(typeof(ErrorCodes), result.error_code));
                }
                return Result<ResultDirectChargeGtoken>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<GTokenTransactionModel>> ExecuteGTokenTransaction(GTokenTransaction modal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, Urls.HASHED_TOKEN),
                    new KeyValuePair<string, string>(ConstantValues.S_GTOKEN_TRANSACTION_ID, modal.gtoken_transaction_id),
                    new KeyValuePair<string, string>(ConstantValues.S_STATUS, modal.status)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_EXECUTE_TRANSACTION, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<GTokenTransactionModel>> RecordTokenTransaction(GRecordTokenTransaction modal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, Urls.HASHED_TOKEN),
                    new KeyValuePair<string, string>(ConstantValues.S_TOKEN_TRANSACTION, JsonConvert.SerializeObject(modal))
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_RECORD_TOKEN_TRANSACTION, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<GTokenpProfileModel>> GTokenEditProfile(string session, string email, string nickname,
            string gender, string bio, string countryCode, string countryName, string referralCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>( ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_SESSION, session),
                    new KeyValuePair<string, string>( ConstantValues.S_EMAIL, email), 
                    new KeyValuePair<string, string>(ConstantValues.S_NICKNAME, nickname),
                    new KeyValuePair<string, string>( ConstantValues.S_GENDER, gender), 
                    new KeyValuePair<string, string>(ConstantValues.S_BIO, bio),
                    new KeyValuePair<string, string>( ConstantValues.S_COUNTRY_NAME, countryName), 
                    new KeyValuePair<string, string>(ConstantValues.S_COUNTRY_CODE, countryCode),
                    new KeyValuePair<string, string>( ConstantValues.S_REFERRAL_CODE, referralCode)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_EDIT_PROFILE, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenpProfileModel>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        if (UpdateCustomerAccountProfile(result.profile.uid, JsonConvert.SerializeObject(result.profile), session))
                        {
                            return Result<GTokenpProfileModel>.Make(result);
                        }
                    }
                    return Result<GTokenpProfileModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenpProfileModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<GTokenTransactionModel>> ChangePasswordGToken(string session, string oldPassword, string newPassword, string confirmPassword)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_SESSION, session),
                    new KeyValuePair<string, string>(ConstantValues.S_OLD_PASSWORD, oldPassword),
                    new KeyValuePair<string, string>(ConstantValues.S_NEW_PASSWORD, newPassword),
                    new KeyValuePair<string, string>(ConstantValues.S_CONFIRM_PASSWORD, confirmPassword)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_CHANGE_PASSWORD, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<CheckGtokenBalance>> CheckGTokenBalance(string username, decimal amount)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = Urls.GetGTokenServerUri();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, Urls.PARTNER_ID), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, Urls.HASHED_TOKEN),
                    new KeyValuePair<string, string>(ConstantValues.S_USERNAME, username),
                    new KeyValuePair<string, string>(ConstantValues.S_AMOUNT, amount.ToString())
                });

                HttpResponseMessage response =  client.PostAsync(Urls.ACTION_CHECK_GTOKEN_BALANCE, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<CheckGtokenBalance>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        return Result<CheckGtokenBalance>.Make(result);
                    }
                    return Result<CheckGtokenBalance>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<CheckGtokenBalance>.Null(ErrorCodes.HttpRequestError);
            }
        }
    }
}
