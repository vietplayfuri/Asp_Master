using GoPlay.Core;
using Platform.Models;
using Platform.Utility.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GoPlay.Models
{
    public class FriendViewModel
    {
        public FriendViewModel()
        {
            pending_friends = new List<SimpleCustomerAccount>();
            accepted_friends = new List<SimpleCustomerAccount>();
        }
        public List<SimpleCustomerAccount> pending_friends { get; set; }
        public List<SimpleCustomerAccount> accepted_friends { get; set; }
        public int count { get; set; }
    }

    public class SearchFriendViewModel
    {
        public SearchFriendViewModel()
        {
            friends = new List<SimpleCustomerAccount>();
        }
        public List<SimpleCustomerAccount> friends { get; set; }
        public int total_count { get; set; }
        public Pagination pagination { get; set; }
        public string term { get; set; }
    }

    public class RequestFriendListAPI
    {
        public string session { get; set; }

        public string partner_id { get; set; }

        /// <summary>
        /// If true, return multiple profile objects. 
        /// If false, return an array of username. Default to false
        /// </summary>
        public bool include_profile { get; set; }
        /// <summary>
        /// Can be either accepted, pending, waiting or rejected. Default to accepted
        /// </summary>
        public string status { get; set; }
    }


    public class TransferModel
    {
        public int receiverId { get; set; }

        public bool forValidate { get; set; }

        public decimal playTokenAmount { get; set; }

        public int senderId { get; set; }
        public decimal maxAmount { get; set; }

        public List<Tuple<string, string>> IsValid(out customer_account receiver)
        {
            var api = GoPlayApi.Instance;
            List<Tuple<string, string>> errors = new List<Tuple<string,string>>();

            receiver = api.GetUserById(this.receiverId).Data;
            if (receiver == null)
            {
                Tuple<string, string> error = new Tuple<string, string>(GoPlayConstantValues.S_RECEIVER_ID, ErrorCodes.RECEIVER_ACCOUNT_NOT_EXIST.ToErrorMessage());
                errors.Add(error);
            }
            else if (this.receiverId == this.senderId)
            {
                Tuple<string, string> error = new Tuple<string, string>(GoPlayConstantValues.S_RECEIVER_ID, ErrorCodes.SENDING_TO_SELF.ToErrorMessage());
                errors.Add(error);
            }

            if (playTokenAmount < GoPlayConstantValues.D_MIN_AMOUNT
                || this.maxAmount < playTokenAmount)
            {
                Tuple<string, string> error = new Tuple<string, string>(GoPlayConstantValues.S_PLAY_TOKEN_AMOUNT, ErrorCodes.INSUFFICIENT_PLAY_TOKEN_AMOUNT.ToErrorMessage());
                errors.Add(error);
            }
            else
            {
                Regex re = new Regex(GoPlayConstantValues.S_DECIMAL_REGEX);
                if (!re.IsMatch(playTokenAmount.ToString()))
                {
                    Tuple<string, string> error = new Tuple<string, string>(GoPlayConstantValues.S_PLAY_TOKEN_AMOUNT, ErrorCodes.INVALID_DECIMAL_PRECISION.ToErrorMessage());
                    errors.Add(error);
                }
            }

            return errors;
        }
    }

    public class FriendRequestEmail
    {
        public FriendRequestEmail()
        {
            var Request = HttpContext.Current.Request;
            mainUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
        }

        public string mainUrl { get; set; }
        public string receiver_display_name { get; set; }
        public string sender_display_name { get; set; }
        public string sender_username { get; set; }
        public string sender_avatar { get; set; }
        public string to_email { get; set; }
    }
}