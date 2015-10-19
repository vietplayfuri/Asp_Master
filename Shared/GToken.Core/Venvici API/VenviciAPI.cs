using System.Net;
using System.Web.Configuration;
using Platform.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Platform.Utility;
using Platform.Dal;
using System.Web;
using Platform.Models.Models;

namespace Platform.Core
{
    public class VenviciAPI
    {
        #region Singleton
        private VenviciAPI() { }

        public static readonly VenviciAPI Instance;

        static VenviciAPI()
        {
            Instance = new VenviciAPI();
        }
        #endregion

        public bool CheckValidVenviciUser(string username)
        {
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.CheckGtoken,
                username = username
            };
            var result = SendAPIRequest(venvici);
            var suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/checkGToken.jsp", suscess, HttpContext.Current.Request.UserAgent,
                "venvici", username, HttpContext.Current.Request.UserHostAddress, string.Empty, JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            return suscess;
        }


        /// <summary>
        /// Check to know if customer's amount is valid or not
        /// </summary>
        /// <param name="username"></param>
        /// <param name="amount"></param>
        /// <returns>TRUE: valid - FALSE: not valid</returns>
        public bool CheckGToken(string username, decimal amount)
        {
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.CheckGtoken,
                username = username
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/checkGToken.jsp", suscess, HttpContext.Current.Request.UserAgent,
                "venvici", username, HttpContext.Current.Request.UserHostAddress, string.Empty, JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
            {
                var arr = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                decimal balance = 0;
                decimal.TryParse(arr[2], out balance);
                return amount <= balance;
            }
            return false;
        }
        public bool AddMember(CustomerAccount user)
        {
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.Addmember,
                username = user.username,
                email = user.email,
                password = user.unhashed_password,
                md5Password = user.password,
                introducerId = user.inviter_username,
                country = user.country_code
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/addMember.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", user.username, HttpContext.Current.Request.UserHostAddress, string.Empty, JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            return suscess;
        }
        public bool PushBv(string username, Transaction transaction)
        {
            decimal pushbv = Math.Round(transaction.revenue * 0.1m, 2);
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.PushBV,
                username = username,
                bv = pushbv
            };
            var result = SendAPIRequest(venvici);
            bool success = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/pushBv.jsp", success, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty, JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (success)
            {
                string remark = string.Format("BV for transaction: {0}, originator: {1}", transaction.gtoken_transaction_id, transaction.customer_username);
                recordTransaction(username, transaction.gtoken_transaction_id, remark, pushBv: pushbv);
            }
            return success;
        }

        public bool PushBv(string username, TokenTransaction transaction)
        {
            decimal pushbv = transaction.amount;
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.PushBV,
                username = username,
                bv = pushbv
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/pushBv.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty, JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
            {
                string remark = string.Format("BV for transaction: {0}, originator: {1}", transaction.gtoken_transaction_id, transaction.customer_username);
                recordTransaction(username, transaction.gtoken_transaction_id, remark, pushBv: pushbv);
            }
            return suscess;
        }

        public bool commissionCredit(string username, Transaction transaction)
        {
            string remark = string.Format("Commission Credit for transaction: {0}, originator: {1}", transaction.gtoken_transaction_id, transaction.customer_username);
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.CreditInCC,
                username = username,
                amount = transaction.revenue,
                refNo = transaction.gtoken_transaction_id,
                remark = remark
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/creditInCC.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty,
               JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
                recordTransaction(username, transaction.gtoken_transaction_id, remark, commissionCredit: transaction.revenue);
            return suscess;
        }

        public bool commissionCredit(string username, TokenTransaction transaction, string campTitle)
        {
            string remark = string.Format("Commission Credit for transaction: {0} {1}, originator: {2}", transaction.gtoken_transaction_id, campTitle, transaction.customer_username);
            decimal amount = transaction.amount;
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.CreditInCC,
                username = username,
                amount = amount,
                refNo = transaction.gtoken_transaction_id,
                remark = remark
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/creditInCC.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty,
               JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
                recordTransaction(username, transaction.gtoken_transaction_id, remark, commissionCredit: amount);
            return suscess;
        }

        public bool deductGToken(string username, Transaction transaction)
        {
            string remark = string.Format("Deduct GToken for transaction: {0}, originator: {1}", transaction.gtoken_transaction_id, transaction.customer_username);
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.deductGToken,
                username = username,
                amount = transaction.revenue,
                refNo = transaction.gtoken_transaction_id,
                remark = remark
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/deductGToken.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty,
               JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
                recordTransaction(username, transaction.gtoken_transaction_id, remark, gtokenAmount: transaction.revenue);
            return suscess;
        }

        public bool deductGToken(string username, TokenTransaction transaction, string campTitle)
        {
            string remark = string.Format("Deduct GToken for transaction: {0} {1}, originator: {2}", transaction.gtoken_transaction_id, campTitle, transaction.customer_username);
            decimal amount = transaction.amount;
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.deductGToken,
                username = username,
                amount = amount,
                refNo = transaction.gtoken_transaction_id,
                remark = remark
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            Api.Instance.LogApi("1", "web/deductGToken.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", transaction.customer_username, HttpContext.Current.Request.UserHostAddress, string.Empty,
               JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);
            if (suscess)
                recordTransaction(username, transaction.gtoken_transaction_id, remark, gtokenAmount: amount);
            return suscess;
        }

        public string SendAPIRequest(VenviciModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["VENVICI_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                List<KeyValuePair<string, string>> ValueCollection = new List<KeyValuePair<string, string>>();
                string action = string.Empty;
                switch (model.enumAction)
                {
                    case VenviciAction.Addmember:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_EMAIL, model.email));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PASSWORD, model.password));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_MD5PASSWORD, model.md5Password));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_INTRODUCERID, model.introducerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNTRY, model.country));
                        action = Urls.VENVICI_ACTION_ADDMEMBER;
                        break;

                    case VenviciAction.CheckGtoken:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        action = Urls.VENVICI_ACTION_CHECKGTOKEN;
                        break;
                    case VenviciAction.CreditInCC:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_AMOUNT, model.amount.ToString()));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REFNO, model.refNo));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REMARK, model.remark));
                        action = Urls.VENVICI_ACTION_CREDITINCC;
                        break;
                    case VenviciAction.deductGToken:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_AMOUNT, model.amount.ToString()));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REFNO, model.refNo));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REMARK, model.remark));
                        action = Urls.VENVICI_ACTION_DEDUCTGTOKEN;
                        break;
                    case VenviciAction.PushBV:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_BV, model.bv.ToString()));
                        action = Urls.VENVICI_ACTION_PUSHBV;
                        break;
                    case VenviciAction.CreditInGT:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_AMOUNT, model.amount.ToString()));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REFNO, model.refNo));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REMARK, model.remark));
                        action = Urls.VENVICI_ACTION_CREDITINGT;
                        break;
                    default:
                        break;
                }
                var formContent = new FormUrlEncodedContent(ValueCollection);
                HttpResponseMessage response = client.PostAsync(action, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                return string.Empty;
            }
        }
        public int recordTransaction(string username, string gtoken_transaction_id, string remark, decimal gtokenAmount = 0, decimal commissionCredit = 0, decimal pushBv = 0, decimal gtoken_add_amount = 0)
        {
            var venviciTrans = new VenviciTransaction()
            {
                transaction_id = gtoken_transaction_id,
                customer_username = username,
                gtoken_deduct_amount = gtokenAmount,
                commission_credit_amount = commissionCredit,
                pushbv_amount = pushBv,
                gtoken_add_amount = gtoken_add_amount,
                remark = remark
            };
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateVenviciTransaction(db, venviciTrans);
            }
        }
        public void UpdateVenviciBalance(Partner partner, Transaction transaction)
        {
            if (!CheckValidVenviciUser(transaction.customer_username))
                return;
            //decimal gtoken = 0;
            //var inviter = Api.Instance.GetInviterByCustomerUserName(transaction.customer_username).Data;
            //if (inviter != null && !string.IsNullOrEmpty(inviter.username))
            //    gtoken = transaction.revenue;
            //if (CheckGToken(transaction.customer_username, gtoken))
            //{
            //    deductGToken(transaction.customer_username, transaction);
            //    commissionCredit(transaction.customer_username, transaction);
            //}
            //else if (CheckGToken(inviter.username, gtoken))
            //{
            //    deductGToken(inviter.username, transaction);
            //    commissionCredit(inviter.username, transaction);
            //}
            //else
            //    PushBv(transaction.customer_username, transaction);

            string remark = string.Format("Add GToken for transaction: {0}, originator: {1}", transaction.gtoken_transaction_id, transaction.customer_username);
            AddGToken(transaction.customer_username, remark, transaction.revenue, transaction.gtoken_transaction_id);
            PushBv(transaction.customer_username, transaction);
        }

        public bool UpdateCashBack(string username, TokenTransaction transaction, string campTitle)
        {
            if (!deductGToken(username, transaction, campTitle))
                return false;

            return commissionCredit(username, transaction, campTitle);
        }

        public bool AddGToken(string username, string remark, decimal gtoken, string gtoken_transaction_id)
        {
            var venvici = new VenviciModel()
            {
                enumAction = VenviciAction.CreditInGT,
                username = username,
                amount = gtoken,
                remark = remark,
                refNo = gtoken_transaction_id
            };
            var result = SendAPIRequest(venvici);
            bool suscess = result.Contains(Helper.GetDescription(VenviciReult.success));
            if (suscess)
                recordTransaction(username, gtoken_transaction_id, remark, gtoken_add_amount: gtoken);

            Api.Instance.LogApi("1", "web/creditInGT.jsp", suscess, HttpContext.Current.Request.UserAgent,
               "venvici", username, HttpContext.Current.Request.UserHostAddress, string.Empty,
               JsonConvert.SerializeObject(venvici) + " ---- Result: " + result);

            return suscess;
        }
    }
}
