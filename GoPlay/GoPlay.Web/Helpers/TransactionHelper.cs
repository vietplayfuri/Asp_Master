using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoPlay.Core;
using Platform.Models;
using GoPlay.Models.Models;
using Platform.Utility;
using GoPlay.Models;
using GoPlay.Web.Models;

namespace GoPlay.Web.Helpers
{
    public class TransactionHelper
    {
        public static string getDescription(GeneralTransaction trans)
        {
            switch (trans.table_name)
            {
                case GoPlayConstantValues.COIN_TRANSACTION:
                    return getCoinTransDescription(trans);
                case GoPlayConstantValues.FREE_COIN_TRANSACTION:
                    return getFreeTransDescription(trans);
            }
            return getGCoinTransDescription(trans);
        }


        public static string getGCoinTransDescription(GeneralTransaction trans)
        {
            var api = GoPlayApi.Instance;
            Game game = null;
            if (trans == null || trans.game_id.HasValue)
                return string.Empty;
            else
            {
                game = api.GetGame(trans.game_id.Value).Data;
                if (game == null)
                    return string.Empty;
            }
            if (!trans.amount.HasValue || trans.amount.Value < 0)
                return trans.description;
            return string.Format(Resources.Resources.Gcoin_description, Helper.displayDecimal(trans.amount),
                                                              game.name, trans.description);
        }

        public static string getCoinTransDescription(GeneralTransaction trans)
        {
            var api = GoPlayApi.Instance;
            Game game = null;
            if (trans == null)
                return string.Empty;
            if (trans.game_id.HasValue)
            {
                var creditTransaction = api.GetCreditTransaction(trans.id).Data;
                game = api.GetGame(trans.game_id.Value).Data;
                string typeName = string.Empty;
                CreditType creditType = null;
                Package package = null;
                if (trans.credit_type_id.HasValue)
                    creditType = api.GetCreditType(trans.credit_type_id.Value).Data;
                else if (trans.package_id.HasValue)
                    package = api.GetPackage(trans.package_id.Value).Data;

                if (creditTransaction != null && game != null)
                {
                    return string.Format(Resources.Resources.Coin_des_exchange,
                                         creditType != null ? creditType.name : package != null ? package.name : Resources.Resources.Unknown,
                                         game.name);
                }
            }
            if (trans.receiver_account_id.HasValue)
            {
                var receiver = api.GetUserById(trans.receiver_account_id.Value).Data;
                return string.Format(Resources.Resources.Coin_des_Transfer,
                    receiver != null ? receiver.GetDisplayName() : string.Empty);
            }

            if (trans.sender_account_id.HasValue)
            {
                var sender = api.GetUserById(trans.sender_account_id.Value).Data;
                return string.Format(Resources.Resources.Coin_des_Receive,
                    sender != null ? sender.GetDisplayName() : string.Empty);
            }
            if (!string.IsNullOrEmpty(trans.payment_method))
                return Resources.Resources.Coin_des_topup;
            if (trans.partner_account_id.HasValue)
            {
                if (trans.amount.HasValue)
                {
                    var partner = api.GetPartner(trans.partner_account_id.Value).Data;
                    if (trans.amount.Value < 0)
                        return string.Format(Resources.Resources.Coin_des_Payment,
                                             partner != null ? partner.name : string.Empty);
                    else
                        return string.Format(Resources.Resources.Coin_des_Received,
                                             partner != null ? partner.name : string.Empty);

                }
            }
            return trans.description;
        }

        public static string getFreeTransDescription(GeneralTransaction trans)
        {
            var api = GoPlayApi.Instance;
            Game game = null;
            if (trans == null)
                return string.Empty;
            if (trans.game_id.HasValue)
            {
                var creditTransaction = api.GetCreditTransactionByFreeCoinTransactionId(trans.id).Data;
                game = api.GetGame(trans.game_id.Value).Data;
                string typeName = string.Empty;
                CreditType creditType = null;
                Package package = null;
                if (trans.credit_type_id.HasValue)
                    creditType = api.GetCreditType(trans.credit_type_id.Value).Data;
                else if (trans.package_id.HasValue)
                    package = api.GetPackage(trans.package_id.Value).Data;

                if (creditTransaction != null && game != null)
                {
                    return string.Format(Resources.Resources.Coin_des_exchange,
                                         creditType != null ? creditType.name : package != null ? package.name : "Unknown",
                                         game.name);
                }
            }
            if (!string.IsNullOrEmpty(trans.payment_method))
                return Resources.Resources.Coin_des_topup;
            return Resources.Resources.Receive_Free_Token;
        }


        public static object toDictionary(dynamic transaction)
        {
            if (transaction.game_id != null)
            {
                var api = GoPlayApi.Instance;

                credit_transaction creditTransaction = null;
                bool isFree = false;
                if (transaction is coin_transaction)
                {
                    creditTransaction = api.GetCreditTransaction(transaction.id).Data;
                }
                else if (transaction is free_coin_transaction)
                {
                    isFree = true;
                    creditTransaction = api.GetCreditTransactionByFreeCoinTransactionId(transaction.id).Data;
                }
                var dict = new TransactionDict()
                {
                    transaction_id = transaction.order_id,
                    gtoken_value = transaction.amount,
                    goplay_token_value = transaction.amount,
                    quantity = creditTransaction.amount.Value,
                    is_free = isFree
                };
                CreditType creditType = null;
                Package package = null;
                if (creditTransaction.credit_type_id.HasValue)
                {
                    dict.exchange_option_type = "CreditType";
                    creditType = api.GetCreditType(creditTransaction.credit_type_id.Value).Data;
                    dict.exchange_option_identifier = creditType.string_identifier;
                }
                else
                {
                    dict.exchange_option_type = "Package";
                    package = api.GetPackage(creditTransaction.package_id.Value).Data;
                    dict.exchange_option_identifier = package.string_identifier;
                }
                return dict;
            }
            return new { };
        }
    }
}