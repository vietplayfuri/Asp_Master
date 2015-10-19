using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Platform.Models.Models;
using Platform.Models;
using GToken.Helpers.Extensions;
using GToken.Models;

namespace GToken.Web.Models
{
    public class TransactionViewModel
    { }

    public class ConversionModel
    {
        public string partner_id { get; set; }
        /// <summary>
        /// Hashed value of secret key
        /// </summary>
        public string hashed_token { get; set; }
        /// <summary>
        /// Currency code ISO 4217
        /// </summary>
        public string source_currency { get; set; }
        /// <summary>
        /// Currency code ISO 4217
        /// </summary>
        public string destination_currency { get; set; }
    }

    public class TransactionModel
    {
        public string partner_id { get; set; }
        public string hashed_token { get; set; }
        /// <summary>
        /// Json object
        /// </summary>
        public string transaction { get; set; }
        public string ip_address { get; set; }
    }

    public class APIRecordTokenTransactionModel
    {
        public string partner_id { get; set; }
        public string hashed_token { get; set; }
        public string token_transaction { get; set; }
        public string ip_address { get; set; }
    }


    /// <summary>
    /// Used for api/1/transaction/retrieve-transaction
    /// </summary>
    public class APIRetrievTransactioneModel
    {
        public string partner_id { get; set; }
        public string hashed_token { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string order_id { get; set; }
    }

    /// <summary>
    /// Used for api/1/transaction/execute-transaction
    /// </summary>
    public class APIExecuteTransactioneModel
    {
        public string partner_id { get; set; }
        public string hashed_token { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string status { get; set; }
    }

    public class TransactionsPaging
    {
        public int count { get; set; }
        public List<MainTransaction> transactions { get; set; }

        public Pagination pagination { get; set; }
    }

    public class ReferalsPaging
    {
        public int count { get; set; }
        public List<RecordDownloadHistory> transactions { get; set; }

        public Pagination pagination { get; set; }
        public decimal totalMoney { get; set; }

        public SearchReferalViewModel model { get; set; }
    }

    public class InvoiceModel
    {
        public Transaction transaction { get; set; }
        public TokenTransaction token_transaction { get; set; }

        public string partnerName
        {
            get
            {
                if (transaction == null && token_transaction == null)
                {
                    return string.Empty;
                }
                else
                {
                    string partner_identifier;
                    partner_identifier = transaction != null ? transaction.partner_identifier : token_transaction.partner_identifier;
                    var partner = Platform.Core.Api.Instance.GetPartnerByIdentifier(partner_identifier).Data;
                    return partner != null ? partner.name : string.Empty;
                }
            }
        }

    }


    /// <summary>
    /// Used such as params of API direct charege gtoken
    /// </summary>
    public class APIDirectChargeGtokenModel
    {
        public string ip_address { get; set; }
        public string partner_id { get; set; }
        public string hashed_token { get; set; }
        public string direct_gtoken_transaction { get; set; }
    }
}
