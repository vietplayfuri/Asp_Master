using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Api.Filters;
using GoPlay.Web.Helpers;
using GoPlay.Web.Helpers.Extensions;
using GoPlay.Web.Models;
using Newtonsoft.Json;
using Platform.Models;
using Platform.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace GoPlay.WebApi.V1
{
    public class TransactionController : BaseApiController
    {
        [HttpPost]
        [ActionName("update-external-exchange")]
        [ExecutionMeter]
        public object UpdateExternalExchange()
        {
            var request = HttpContext.Current.Request;
            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = request.Params["game_id"];
            Game game = api.GetGame(gameUid).Data;
            //string session = request.Params["session"];
            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];

            customer_account user = api.LoadFromAccessToken(session).Data;
            string identifier = request.Params["exchange_option_identifier"] == null
                ? string.Empty
                : request.Params["exchange_option_identifier"];
            string transactionId = request.Params["transaction_id"];

            ErrorCodes? error = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else
            {
                RBAC rbac = new RBAC(user.id);
                bool? isActive = true;
                //# To allow admin account to test exchange option without showing it to everyone
                if (rbac.HasRole(GoPlayConstantValues.S_ROLE_GAME_ADMIN) || rbac.HasRole(GoPlayConstantValues.S_ROLE_ADMIN))
                    isActive = null;

                Tuple<string, int> tuple = api.GetExchangeOption(identifier, isActive, false);
                if (tuple == null)
                    error = ErrorCodes.INVALID_EXCHANGE_OPTION;
                else
                {
                    var lstExternalExchange = api.GetExternalExchanges(user.id, game.id, null, transactionId);
                    if (lstExternalExchange != null && lstExternalExchange.Any())
                        error = ErrorCodes.EXCHANGE_RECORDED;
                    else
                    {
                        var externalExchange = new external_exchange
                        {
                            customer_account_id = user.id,
                            game_id = game.id,
                            exchange_option_identifier = identifier,
                            transaction_id = transactionId
                        };

                        if (string.Compare(tuple.Item1, GoPlayConstantValues.S_CREDIT_TYPE, StringComparison.OrdinalIgnoreCase) == 0)
                            externalExchange.credit_type_id = tuple.Item2;
                        else if (string.Compare(tuple.Item1, GoPlayConstantValues.S_PACKAGE, StringComparison.OrdinalIgnoreCase) == 0)
                            externalExchange.package_id = tuple.Item2;

                        if (!api.CreateExternalExchange(externalExchange))
                            error = ErrorCodes.ServerError;
                    }
                }
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            if (error == null)
                return Json(new { success = true });

            return Json(new
            {
                success = false,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode()
            });
        }

        [HttpPost]
        [ActionName("reject-exchange")]
        [ExecutionMeter]
        public object RejectExchanges()
        {
            var request = HttpContext.Current.Request;
            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = request.Params["game_id"];
            Game game = api.GetGame(gameUid).Data;
            //string session = request.Params["session"];
            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;
            string transactionID = request.Params["transaction_id"] == null
                ? string.Empty
                : request.Params["transaction_id"];

            var transaction = api.GetCoinTransaction(transactionID).Data;

            ErrorCodes? error = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (transaction == null)
                error = ErrorCodes.INVALID_TRANSACTION_ID;
            else if (string.Compare(transaction.status, ConstantValues.S_PENDING, StringComparison.OrdinalIgnoreCase) != 0)
                error = ErrorCodes.TRANSACTION_ALREADY_PROCESSED;
            else
            {
                bool isUpdate = false;
                if (transaction.coin_id.HasValue)
                    isUpdate = api.UpdateCoinTransactionStatus(transaction.coin_id.Value, ConstantValues.S_FAILURE);
                else if (transaction.free_coin_id.HasValue)
                    isUpdate = api.UpdateFreeCoinTransactionStatus(transaction.free_coin_id.Value, ConstantValues.S_FAILURE);

                if (!isUpdate || !api.UpdateCreditTransactionStatus(transaction.credit_transaction_id, ConstantValues.S_FAILURE))
                    error = ErrorCodes.ServerError;
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                exchange = error == null ? transaction.ToDictionary() : null
            };
        }

        [HttpPost]
        [ActionName("fulfill-exchange")]
        [ExecutionMeter]
        public object FulfillExchanges(FulfilledExchangeModel param)
        {
            var request = HttpContext.Current.Request;

            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = param.game_id;
            Game game = api.GetGame(gameUid).Data;

            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;
            string transactionID = string.IsNullOrEmpty(param.transaction_id)
                ? string.Empty
                : param.transaction_id;

            dynamic transaction;
            transaction = api.GetCoinTransactionByOderId(transactionID).Data;
            bool isPlayToken = true;
            if (transaction == null)
            {
                isPlayToken = false;
                transaction = api.GetFreeCoinTransactionByOderId(transactionID).Data;
            }

            ErrorCodes? error = null;
            credit_transaction credit = new credit_transaction();
            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else if (transaction == null)
                error = ErrorCodes.INVALID_TRANSACTION_ID;
            else if (string.Compare(transaction.status, ConstantValues.S_PENDING, StringComparison.OrdinalIgnoreCase) != 0)
                error = ErrorCodes.TRANSACTION_ALREADY_PROCESSED;
            else
            {
                //Call api record transaction of Gtoken
                var result = api.GTokenAPITransaction(new GtokenModelTransactionAction
                    {
                        token_transaction = new TokenTransactionJson
                        {
                            token_type = isPlayToken
                                ? GoPlayConstantValues.S_PLAY_TOKEN
                                : GoPlayConstantValues.S_FREE_PLAY_TOKEN,
                            username = user.username,
                            description = transaction.description,
                            order_id = transaction.order_id,
                            transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                            amount = transaction.amount
                        }
                    });

                if (!result.Succeeded)
                {
                    error = result.Error.HasValue ? result.Error.Value : ErrorCodes.ServerError;
                }
                else
                {
                    bool isUpdate = false;
                    if (isPlayToken)
                        isUpdate = api.UpdateCoinTransactionStatus(transaction.id, ConstantValues.S_SUCCESS);
                    else
                        isUpdate = api.UpdateFreeCoinTransactionStatus(transaction.id, ConstantValues.S_SUCCESS);

                    int creditPrimaryKey = (int)transaction.id;
                    credit = api.GetCreditTransaction(creditPrimaryKey).Data;
                    if (credit == null)
                        credit = api.GetCreditTransactionByFreeCoinTransactionId(creditPrimaryKey).Data;

                    if (!isUpdate || !api.UpdateCreditTransactionStatus(credit.id, ConstantValues.S_SUCCESS))
                        error = ErrorCodes.ServerError;
                }
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            string creditTypeStringIdentifier = string.Empty;
            string packageStringIdentifier = string.Empty;
            if (error == null)
            {
                if (credit.credit_type_id.HasValue)
                {
                    var creditType = api.GetCreditType(credit.credit_type_id.Value).Data;
                    if (creditType != null)
                        creditTypeStringIdentifier = creditType.string_identifier;
                }
                else if (credit.package_id.HasValue)
                {
                    var package = api.GetPackage(credit.package_id.Value).Data;
                    if (package != null)
                        packageStringIdentifier = package.string_identifier;
                }
            }
            if (error == null)
            {
                return Json(new
                {
                    success = true,
                    exchange = transaction.ToDictionary(credit, creditTypeStringIdentifier, packageStringIdentifier)
                });
            }
            return Json(new
            {
                success = false,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
            });
        }

        [HttpPost]
        [ActionName("get-unfulfilled-exchanges")]
        [ExecutionMeter]
        public object GetUnfulfilledExchanges(UnfulfilledExchangeModel param)
        {
            var request = HttpContext.Current.Request;

            GoPlayApi api = GoPlayApi.Instance;
            string gameUid = param.game_id;
            Game game = api.GetGame(gameUid).Data;
            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Params["session"];
            customer_account user = api.LoadFromAccessToken(session).Data;

            List<object> result = null;
            ErrorCodes? error = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else
            {
                var creditTransaction = api.GetCoinTransactions(user.id, game.id, ConstantValues.S_PENDING).Data;
                result = new List<object>();
                creditTransaction.ForEach(i => result.Add(i.ToDictionary()));
            }

            api.LogApi("1", request.Url.LocalPath, error == null,
                request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                request.UserHostAddress,
                error.ToErrorCode(), request.Params.ToString());

            return Json(new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                exchanges = error == null ? result : new List<object>()
            });
        }
    }
}
