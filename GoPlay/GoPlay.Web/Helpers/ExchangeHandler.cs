using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Core;
using System.Dynamic;
using GoPlay.Web.Helpers.Extensions;
using Platform.Models.Models;
using System.Net;
using Platform.Utility;
using GoPlay.Web.Models;
using log4net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace GoPlay.Web.Helpers
{
    public interface BaseExchangeHandlerInterface
    {
        bool validate();
        Task<bool> exchange();
        dynamic getCoinTransaction();
        Task<Tuple<decimal, decimal>> calculate();
        Dictionary<string, List<string>> getErrors();
    }

    public class BaseExchangeHandler
    {
        public customer_account user { get; set; }
        public Game game { get; set; }
        public dynamic exchangeOption { get; set; }
        public int inGameAmount { get; set; }
        public dynamic coinTransaction { get; set; }
        public credit_transaction creditTransaction { get; set; }
        public Dictionary<string, List<string>> errors { get; set; }
        public IPAddress ip { get; set; }

        public BaseExchangeHandler(customer_account user, Game game, dynamic exchangeOption, int amount, IPAddress ip)
        {
            this.user = user;
            this.game = game;
            this.exchangeOption = exchangeOption;
            this.inGameAmount = amount;
            this.coinTransaction = null;
            this.creditTransaction = null;
            this.ip = ip;
        }

        public bool validate()
        {
            this.errors = new Dictionary<string, List<string>>();

            if (exchangeOption == null)
            {
                addError("exchange_option_id",
                         "Exchange Option has been removed");
                return false;
            }

            if (this.exchangeOption.is_archived)
            {
                this.addError("exchange_option_id", "Exchange Option has been removed");
            }

            if (this.exchangeOption.game_id != this.game.id)
            {
                this.addError("exchange_option_id", "Exchange Option does not belong to game");
            }

            //Game is not archived and active
            RBAC rbac = new RBAC(user.id);
            if (!(rbac.HasRole("admin") || rbac.HasRole("game_admin")))
            {
                if (!this.game.is_active || this.game.is_archived)
                    this.addError("game_id", "Game is not active or has been removed");
            }
            try
            {
                //self.inGameAmount = int(self.inGameAmount)
                if (this.exchangeOption is CreditType)
                {
                    if (this.inGameAmount < 0)
                    {
                        this.addError("exchange_amount", "Exchange amount needs to be a positive integer");
                    }
                }
                else if (this.exchangeOption is Package)
                {
                    if (this.inGameAmount != 1)
                    {
                        this.addError("exchange_amount", "Exchange amount needs to be 01");
                    }
                }

            }
            catch (Exception)
            {
                this.addError("exchange_amount", "Exchange amount needs to be a positive integer");
            }

            if (this.inGameAmount == 0)
            {
                this.addError("exchange_amount", "Exchange amount is required");
            }

            if (this.errors.Count == 0)
            {
                Tuple<decimal, decimal> tuple = calculate().Result;
                if (!(tuple.Item1 > 0 || tuple.Item2 > 0))
                    this.addError("exchange_amount", "Insufficient Balance");
            }
            if (this.errors.Count > 0) { return false; }
            return true;
        }

        public void finalizeTransactions(string status)
        {
            var api = GoPlayApi.Instance;
            if (coinTransaction != null && creditTransaction != null)
            {
                if (coinTransaction is coin_transaction)
                {
                    api.UpdateCoinTransactionStatus(coinTransaction.id, status);
                }
                else
                {
                    api.UpdateFreeCoinTransactionStatus(coinTransaction.id, status);
                }
                api.UpdateCreditTransactionStatus(creditTransaction.id, status);
                coinTransaction.status = status;
                creditTransaction.status = status;
            }
            if (status == "success")
            {
                //prepare TransactionJSON to submit to GTOKEN
                var token_transaction = new TokenTransactionJson()
                {
                    username = this.user.username,
                    order_id = coinTransaction.order_id,
                    transaction_type = "consumption",
                    description = coinTransaction.description,
                    amount = coinTransaction.amount
                };
                if (creditTransaction.coin_transaction_id.HasValue)
                {
                    token_transaction.token_type = "Play Token";
                }
                else
                {
                    token_transaction.token_type = "Free Play Token";
                }

                var gTokenTransaction = GoPlayApi.Instance.GTokenAPITransaction(new GtokenModelTransactionAction
                {
                    token_transaction = token_transaction,
                    ip_address = ip.ToString()
                });
            }

        }

        public Task<Tuple<decimal, decimal>> calculate()
        {
            return Task.FromResult(this.exchangeOption.calculatePlayToken(this.user, this.inGameAmount));
        }


        public void generateTransactions()
        {
            Tuple<decimal, decimal> tuple = calculate().Result;

            if (tuple.Item1 > 0)
            {

                var result = initTransaction(tuple.Item1, "free_play_token");
                coinTransaction = result.coinTransaction;
                creditTransaction = result.creditTransaction;
            }
            else if (tuple.Item2 > 0)
            {
                var result = initTransaction(tuple.Item2, "play_token");
                coinTransaction = result.coinTransaction;
                creditTransaction = result.creditTransaction;
            }

        }

        public InitTransactionModel initTransaction(decimal playTokenAmount, string balanceType)
        {
            var api = GoPlayApi.Instance;
            var creditTransaction = new credit_transaction();
            dynamic coinTransaction = new ExpandoObject();
            if (balanceType == "free_play_token")
            {
                coinTransaction = CreateFreeCoinTrans(playTokenAmount);
                creditTransaction.amount = this.inGameAmount;
            }
            else
            {
                coinTransaction = CreateCoinTrans(playTokenAmount);
                creditTransaction.amount = this.inGameAmount;
            }
            if (this.exchangeOption is CreditType)
            {
                creditTransaction.credit_type_id = this.exchangeOption.id;
            }
            else if (this.exchangeOption is Package)
            {
                creditTransaction.package_id = exchangeOption.id;
            }

            creditTransaction.customer_account_id = user.id;
            creditTransaction.game_id = game.id;
            creditTransaction.status = "payment_created";
            creditTransaction.description = String.Format("Exchange for{0} in {1}", exchangeOption.name, game.name);

            if (balanceType == "free_play_token")
                creditTransaction.free_coin_transaction_id = coinTransaction.id;
            else if (balanceType == "play_token")
                creditTransaction.coin_transaction_id = coinTransaction.id;

            creditTransaction = api.CreateCreditTransaction(creditTransaction).Data;

            return new InitTransactionModel()
            {
                coinTransaction = coinTransaction,
                creditTransaction = creditTransaction
            };
        }

        private free_coin_transaction CreateFreeCoinTrans(decimal playTokenAmount)
        {
            var coinTransaction = new free_coin_transaction();
            coinTransaction.order_id = Guid.NewGuid().ToString();
            coinTransaction.customer_account_id = this.user.id;
            coinTransaction.amount = -1 * playTokenAmount;
            coinTransaction.game_id = this.game.id;

            if (this.exchangeOption is CreditType)
            {
                coinTransaction.credit_type_id = this.exchangeOption.id;
            }
            else if (this.exchangeOption is Package)
            {
                coinTransaction.package_id = exchangeOption.id;
            }
            coinTransaction.status = "payment_created";
            coinTransaction.description = String.Format("Exchange for {0} in {1}", exchangeOption.name, game.name);

            var country_name = String.Empty;
            ip.GetCountryCode(c => coinTransaction.country_code = c, n => country_name = n);
            coinTransaction.ip_address = ip.ToString();
            var api = GoPlayApi.Instance;

            var id = api.CreateFreeCoinTransaction(coinTransaction);
            return api.GetFreeCoinTransactionById(id).Data;

        }

        private coin_transaction CreateCoinTrans(decimal playTokenAmount)
        {
            var coinTransaction = new coin_transaction();
            coinTransaction.order_id = Guid.NewGuid().ToString();
            coinTransaction.customer_account_id = this.user.id;
            coinTransaction.amount = -1 * playTokenAmount;
            coinTransaction.game_id = this.game.id;

            if (this.exchangeOption is CreditType)
            {
                coinTransaction.credit_type_id = this.exchangeOption.id;
            }
            else if (this.exchangeOption is Package)
            {
                coinTransaction.package_id = exchangeOption.id;
            }
            coinTransaction.status = "payment_created";
            coinTransaction.description = String.Format("Exchange for {0} in {1}", exchangeOption.name, game.name);

            var country_name = String.Empty;
            ip.GetCountryCode(c => coinTransaction.country_code = c, n => country_name = n);
            coinTransaction.ip_address = ip.ToString();
            var api = GoPlayApi.Instance;

            return api.CreateCoinTransaction(coinTransaction).Data;

        }

        public void addError(string key, string errorMsg)
        {
            if (!this.errors.Keys.Contains(key))
            {
                this.errors[key] = new List<string>();
            }
            this.errors[key].Add(errorMsg);
        }

        public static BaseExchangeHandlerInterface retrieveExchangeHandler(customer_account user, Game game, dynamic exchangeOption, int amount, IPAddress ip)
        {
            var exchangeHandlers = new Dictionary<string, BaseExchangeHandlerInterface>();
            exchangeHandlers.Add("8b1d8776e813536ecfy", new MineManiaExchangeHandler(user, game, exchangeOption, amount, ip));
            exchangeHandlers.Add("ob5d4579e123381ecfy", new SushiZombieExchangeHandler(user, game, exchangeOption, amount, ip));
            exchangeHandlers.Add("853461dsfwdgf85m0op", new SlamdunkExchangeHandler(user, game, exchangeOption, amount, ip));
            exchangeHandlers.Add("c4c8d825a0ee6a78", new FishingHeroExchangeHandler(user, game, exchangeOption, amount, ip));

            if (exchangeHandlers.Keys.Contains(game.guid))
            {
                return exchangeHandlers[game.guid];
            }
            return new StandardExchangeHandler(user, game, exchangeOption, amount, ip);

        }
        public static dynamic retrieveExchangeOption(string exchangeOptionType, int exchangeOptionID, bool? isActive = true)
        {
            dynamic exchangeOption = new ExpandoObject();
            var api = GoPlayApi.Instance;
            if (string.IsNullOrEmpty(exchangeOptionType))
            {
                exchangeOption = api.GetPackage(exchangeOptionID, true, false).Data;
                if (exchangeOption == null)
                {
                    exchangeOption = api.GetCreditType(exchangeOptionID, isActive).Data;
                }
            }
            else
            {
                if (string.Compare(exchangeOptionType, GoPlayConstantValues.S_CREDIT_TYPE, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exchangeOption = api.GetCreditType(exchangeOptionID, isActive).Data;
                }
                else if (string.Compare(exchangeOptionType, GoPlayConstantValues.S_PACKAGE, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exchangeOption = api.GetPackage(exchangeOptionID, isActive, false).Data;
                }
            }
            return exchangeOption;
        }

        public static dynamic retrieveExchangeOptionByStrIdentifier(string stringIdentifier, bool? isActive = true)
        {
            var api = GoPlayApi.Instance;
            var credit = api.GetCreditType(stringIdentifier, isActive).Data;
            if (credit == null)
                return api.GetPackage(stringIdentifier, isActive).Data;
            return credit;
        }

        public dynamic getCoinTransaction()
        {
            return this.coinTransaction;
        }

        public Dictionary<string, List<string>> getErrors()
        {
            return this.errors;
        }
    }



    public class MineManiaExchangeHandler : BaseExchangeHandler, BaseExchangeHandlerInterface
    {
        public MineManiaExchangeHandler(customer_account user, Game game, object exchangeOption, int amount, IPAddress ip)
            : base(user, game, exchangeOption, amount, ip)
        {

        }

        Task<bool> BaseExchangeHandlerInterface.exchange()
        {
            this.generateTransactions();
            var service = new MineManiaService.MMWSSoapClient();

            int amount = (int)this.creditTransaction.amount.Value;
            var response = service.UpdateGoldForUser(this.user.id.ToString(), amount);
            if (response)
            {
                this.finalizeTransactions("success");
                return Task.FromResult(true);
            }

            this.finalizeTransactions("failure");
            return Task.FromResult(false);
        }
    }
    public class FishingHeroExchangeHandler : BaseExchangeHandler, BaseExchangeHandlerInterface
    {
        public FishingHeroExchangeHandler(customer_account user, Game game, object exchangeOption, int amount, IPAddress ip)
            : base(user, game, exchangeOption, amount, ip)
        {

        }

        Task<bool> BaseExchangeHandlerInterface.exchange()
        {
            this.generateTransactions();
            this.finalizeTransactions("success");
            return Task.FromResult(true);
        }
    }

    public class SushiZombieExchangeHandler : BaseExchangeHandler, BaseExchangeHandlerInterface
    {
        public SushiZombieExchangeHandler(customer_account user, Game game, object exchangeOption, int amount, IPAddress ip)
            : base(user, game, exchangeOption, amount, ip)
        {

        }

        Task<bool> BaseExchangeHandlerInterface.exchange()
        {
            // Create transactions
            this.generateTransactions();

            SuzyService.SuzyServiceSoapClient service = new SuzyService.SuzyServiceSoapClient();
            int amount = (int)this.creditTransaction.amount.Value;
            var response = service.UpdateGoldForUser(this.user.id.ToString(), amount);
            if (response)
            {
                this.finalizeTransactions("success");
                return Task.FromResult(true);
            }
            else
            {
                this.finalizeTransactions("failure");
                return Task.FromResult(false);
            }
        }
    }

    public class StandardExchangeHandler : BaseExchangeHandler, BaseExchangeHandlerInterface
    {
        private ILog logger = log4net.LogManager.GetLogger(typeof(StandardExchangeHandler));

        public StandardExchangeHandler(customer_account user, Game game, object exchangeOption, int amount, IPAddress ip)
            : base(user, game, exchangeOption, amount, ip)
        {

        }

        //async Task<bool> BaseExchangeHandlerInterface.exchange()
        Task<bool> BaseExchangeHandlerInterface.exchange()
        {
            generateTransactions();
            var result = true;
            if (!String.IsNullOrEmpty(this.game.endpoint))
            {
                //

                var payload = new PayLoad()
                        {
                            game_id = game.guid,
                            user_id = user.id,
                            exchange_option_id = exchangeOption.id,
                            exchange_option_identifier = exchangeOption.string_identifier,
                            quantity = (int)creditTransaction.amount.Value,
                            client_id = game.gtoken_client_id,
                            client_secret = game.gtoken_client_secret
                        };

                if (creditTransaction.coin_transaction_id.HasValue)
                {
                    payload.is_free = false;
                }
                else if (creditTransaction.free_coin_transaction_id.HasValue)
                {
                    payload.is_free = true;
                }
                payload.transaction_id = coinTransaction.order_id;
                payload.gtoken_value = coinTransaction.amount;
                payload.play_token_value = coinTransaction.amount;
                if (exchangeOption is CreditType)
                {
                    payload.exchange_option_type = "CreditType";
                }
                else if (exchangeOption is Package)
                {
                    payload.exchange_option_type = "Package";
                }


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(game.endpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // HTTP POST
                    //var content = JsonConvert.SerializeObject(payload);

                    Dictionary<string, string> collection = (from x in payload.GetType().GetProperties() select x).ToDictionary(x => x.Name, x => (x.GetGetMethod().Invoke(payload, null) == null ? "" : x.GetGetMethod().Invoke(payload, null).ToString()));

                    var formContent = new FormUrlEncodedContent(collection.ToList<KeyValuePair<string, string>>());

                    HttpResponseMessage response = client.PostAsync("", formContent).Result;
                    try
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var r = JsonConvert.DeserializeObject<PayLoadResult>(response.Content.ReadAsStringAsync().Result);
                            if (r.success)
                            {
                                finalizeTransactions("success");
                            }
                            else
                            {
                                result = false;
                                addError("game_id", r.message);
                                this.finalizeTransactions("failure");
                                this.coinTransaction.description += ". " + r.message;

                                var api = GoPlayApi.Instance;
                                if (coinTransaction is coin_transaction)
                                {
                                    api.UpdateCoinTransaction(this.coinTransaction.id, coinTransaction.status, coinTransaction.description, coinTransaction.telkom_order_id);
                                }
                                else
                                {
                                    api.UpdateFreeCoinTransaction(this.coinTransaction.id, coinTransaction.status, coinTransaction.description);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder errorBuilder = new StringBuilder();
                        errorBuilder.AppendLine("--------------------------: Controller ");
                        errorBuilder.AppendLine("URL:  " + game.endpoint);
                        errorBuilder.AppendLine("Params:");
                        if (collection != null && collection.Any())
                            foreach (var item in collection)
                            {
                                errorBuilder.AppendLine(String.Format("{0} - {1}", item.Key, item.Value));
                            }

                        errorBuilder.AppendLine("Response:");
                        errorBuilder.AppendLine(response.Content.ReadAsStringAsync().Result);
                        errorBuilder.AppendLine(Environment.NewLine);
                        errorBuilder.AppendLine(ex.ToString());
                        //Log file and send email
                        logger.Error(errorBuilder.ToString());
                        return Task.FromResult(false);
                    }
                }
            }
            else
            {
                finalizeTransactions("pending");
            }
            return Task.FromResult(result);
        }
    }

    public class SlamdunkExchangeHandler : BaseExchangeHandler, BaseExchangeHandlerInterface
    {
        private ILog logger = log4net.LogManager.GetLogger(typeof(StandardExchangeHandler));
        public SlamdunkExchangeHandler(customer_account user, Game game, object exchangeOption, int amount, IPAddress ip)
            : base(user, game, exchangeOption, amount, ip)
        {

        }

        public async Task<string> retrieveUserID()
        {
            var payload = new PayLoadSlamdunk()
            {
                @do = "query",
                platform = 1000,
                sid = 1,
                acct = this.user.id
            };
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var content = GetQueryString(payload);

                HttpResponseMessage response = client.PostAsync(new Uri(ConfigurationManager.AppSettings["SLAMDUNK_ENDPOINT"] + "?" + content), new StringContent(content, Encoding.UTF8, "text/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var r = JsonHelper.DeserializeObject<PayLoadResultSlamdunk>(response.Content.ReadAsStringAsync().Result);
                    if (r != null)
                        if (r.code != 0)
                        {
                            //this.addError("user_id", JsonConvert.SerializeObject(r.desc));
                            this.addError("user_id", r.desc);
                            return null;
                        }
                        else
                        {
                            return r.userdata.uid;
                        }
                }
            }
            return null;

        }

        public async Task<Tuple<string, decimal>> generateSlamdunkOrder(string slamdunkUserID)
        {
            var payload = new PayLoadSlamdunk()
            {
                @do = "gettoken",
                platform = 1000,
                sid = 1,
                uid = slamdunkUserID,
                buyid = this.exchangeOption.old_db_id
            };
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var content = GetQueryString(payload);

                HttpResponseMessage response = client.PostAsync(new Uri(ConfigurationManager.AppSettings["SLAMDUNK_ENDPOINT"] + "?" + content), new StringContent(content, Encoding.UTF8, "text/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var r = JsonHelper.DeserializeObject<PayLoadResultSlamdunk1>(response.Content.ReadAsStringAsync().Result);
                    if (r.code != 0)
                    {
                        this.addError("user_id", JsonConvert.SerializeObject(r.desc));
                        return Tuple.Create(String.Empty, 0m);
                    }
                    else
                    {
                        return Tuple.Create(r.desc.orderid, r.desc.money);
                    }
                }
            }
            return Tuple.Create(String.Empty, 0m);
        }

        public async void executeSlamdunkOrder(string orderId, string slamdunkOrderID, decimal slamdunkMoney)
        {
            string rawTokenString = String.Format("orderid={0}&coid={1}&money={2}{3}", orderId, slamdunkOrderID, slamdunkMoney, ConfigurationManager.AppSettings["SLAMDUNK_KEY"]);

            var generatedToken = Helper.CalculateMD5Hash(rawTokenString);
            var payload = new PayLoadSlamdunk()
            {
                @do = "pay",
                platform = 1001,
                token = generatedToken.ToLower(),
                orderid = orderId,
                coid = slamdunkOrderID,
                money = slamdunkMoney
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var content = GetQueryString(payload);

                HttpResponseMessage response = client.PostAsync(new Uri(ConfigurationManager.AppSettings["SLAMDUNK_ENDPOINT"] + "?" + content), new StringContent(content, Encoding.UTF8, "text/json")).Result;
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var r = JsonHelper.DeserializeObject<PayLoadResultSlamdunk>(response.Content.ReadAsStringAsync().Result);
                        if (r != null && r.code != 0)
                        {
                            this.addError("user_id", JsonConvert.SerializeObject(r.desc));
                        }
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder errorBuilder = new StringBuilder();
                    errorBuilder.AppendLine("--------------------------: Controller ");
                    errorBuilder.AppendLine("URL:  " + game.endpoint);
                    errorBuilder.AppendLine("Params:" + content);
                    errorBuilder.AppendLine("Response:");
                    errorBuilder.AppendLine(response.Content.ReadAsStringAsync().Result);
                    errorBuilder.AppendLine(Environment.NewLine);
                    errorBuilder.AppendLine(ex.ToString());
                    //Log file and send email
                    logger.Error(errorBuilder.ToString());
                    throw;
                }
            }

        }

        async Task<bool> BaseExchangeHandlerInterface.exchange()
        {
            this.generateTransactions();

            var slamdunkUserID = retrieveUserID().Result;

            if (!String.IsNullOrEmpty(slamdunkUserID))
            {
                var tuple = generateSlamdunkOrder(slamdunkUserID).Result;
                executeSlamdunkOrder(this.coinTransaction.order_id, tuple.Item1, tuple.Item2);
            }
            if (this.errors.Count > 0)
            {
                this.finalizeTransactions("failure");
                return false;
            }
            else
            {
                this.finalizeTransactions("success");
                return true;
            }
        }
        private string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }

}