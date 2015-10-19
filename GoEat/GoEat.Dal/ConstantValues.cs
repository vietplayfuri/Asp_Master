using System.Collections.Generic;
namespace GoEat.Models
{
    static public class ConstantValues
    {
        //Regular Expressions //
        public const string REG_EX_USERNAME = @"^[a-zA-Z0-9]+[\w\-\.]*$";

        // Fields //
        public const string S_ALERT = @"alert";
        public const string S_SUCCESS = @"success";
        public const string S_CANCELLED = @"cancelled";
        public const string S_FAILURE = @"failure";
        public const string S_PENDING_RECONCILE = @"pending_reconcile";
        public const string S_WAITING = @"waiting";
        public const string S_REJECTED = @"rejected";
        public const string S_ACCEPTED = @"accepted";
        public const string S_PENDING = @"pending";
        public const string S_PARTNER_ID = @"partner_id";
        public const string S_HASHED_TOKEN = @"hashed_token";
        public const string S_TRANSACTION = @"transaction";
        public const string S_DIRECT_GTOKEN_TRANSACTION = @"direct_gtoken_transaction";
        public const string S_TOKEN_TRANSACTION = @"token_transaction";
        public const string S_STATUS = @"status";
        public const string S_GTOKEN_TRANSACTION_ID = @"gtoken_transaction_id";
        public const string S_SESSION = @"session";
        public const string S_CURRENCY_USD = @"USD";
        public const string S_CURRENCY_SGD = @"SGD";
        public const string S_PAYMENT_METHOD = @"cash";
        public const string S_SUGAR_TOKEN = "Sugar Token";
        public const string S_OAUTH_ID = "oauth_id";

        #region Used for param of Gtoken API
        public const string S_INCLUDE_PROFILE = "include_profile";
        public const string S_SERVICE = "service";
        public const string S_TOKEN = "token";
        public const string S_KEYWORD = "keyword";
        public const string S_OFFSET = "offset";
        public const string S_COUNT = "count";
        public const string S_FRIEND_USERNAME = "friend_username";
        #endregion

        public const string S_USERID = @"userId";
        public const string S_USERNAME = @"username";
        public const string S_PASSWORD = @"password";
        public const string S_AMOUNT = @"amount";

        public const string S_SGD = "SGD";
        public const string S_MAKE_PAYMENT = @"Make Payment";
        public const string S_USE_TOKEN = @"Make Payment";
        public const string S_DESCRIPTION = "Use token from GDine";
        public const string S_USE_PAYPAL_BUY_TOKEN = @"User received {0} token";
        public const string S_PAYPAL_BUY_TOKEN = @"Buy Sugar Token";

        public const string S_TRANSACTION_TYPE_CONSUMPTION = "consumption";
        public const string S_TRANSACTION_TYPE_TRANSFER = "transfer";

        public const decimal D_GST = 0.07m;
        public const decimal D_SERVICE_CHARGE = 0.1m;
        public const decimal D_RATE_TOKEN_SGD = 1m;
        public const decimal D_REVENUE_PERCENTAGE = 0.05m;
        public const decimal D_TRANSACTION_DRINK_PRICE = 3m;
         #region venviciAPI
        public const string S_INTRODUCERID = @"introducerId";
        public const string S_MD5PASSWORD = @"md5Password";
        public const string S_COUNTRY = @"country";
        public const string S_REFNO = @"refNo";
        public const string S_REMARK = @"remark";
        public const string S_BV = @"bv";
        #endregion
        /// <summary>
        /// Used in getConversion rate of Yahoo
        /// </summary>
        public const string S_YAHOO_DATE_FORMAT = "yyyyddMM";
        public const string S_YAHOO_GET_EXCHANGE_RATE_BY_DATE = @"http://finance.yahoo.com/connection/currency-converter-cache?date={0}";
        public const string S_YAHOO_GET_EXCHANGE_RATE = @"http://finance.yahoo.com/d/quotes.csv?e=.csv&f=sl1d1t1&s={0}{1}=X";

        public const string S_DATE_FORMAT = "yyyy-MM-dd";
        public const string S_SHORT_DATETIME_FORMAT = "MM-dd-yyyy HH:mm tt";
        public const string S_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string S_NICKNAME = @"nickname";
        public const string S_GENDER = @"gender";
        public const string S_BIO = @"bio";
        public const string S_COUNTRY_NAME = @"country_name";
        public const string S_COUNTRY_CODE = @"country_code";
        public const string S_REFERRAL_CODE = @"referral_code";
        public const string S_EMAIL = @"email";
        public const string S_OLD_PASSWORD = @"old_password";
        public const string S_NEW_PASSWORD = @"new_password";
        public const string S_CONFIRM_PASSWORD = @"confirm_password";

        public const string S_IP_ADDRESS = @"ip_address";

        public const string S_DESCRIPTION_AMOUNT_WITH_TOKEN = @"Redeem at GHouse";
        public const string S_DESCRIPTION_AMOUNT_WITH_CASH = "Payment to GHouse";
        public const string S_DESCRIPTION_TOPUP_TOKEN = @"Top-up Sugar Token";

        static public readonly List<string> ListOfFriendRequestType = new List<string> { S_ACCEPTED, S_PENDING, S_WAITING, S_REJECTED  };
        static public readonly List<string> KindsOfTransactionType = new List<string> { S_TRANSACTION_TYPE_DRINK, S_TRANSACTION_TYPE_FOOD };
        public const string S_TRANSACTION_TYPE_DRINK = @"drink";
        public const string S_TRANSACTION_TYPE_FOOD = @"food";
        public const string S_TRANSACTION_ORDER_ID_FORMAT = @"GH_{0}_{1}";
        #region Messages that shown for user
        /// <summary>
        /// need support link
        /// </summary>
        public const string S_ACCOUNT_NOT_FOUNT = @"Account not found. Please contact <a href='{0}'>GToken Customer Support</a> for assistance.";

        /// <summary>
        /// need support link
        /// </summary>
        public const string S_EMAIL_NOT_FOUNT = @"You don't have any email associated with your account. Please contact <a href='{0}'>GToken Customer Support</a> for assistance.";

        /// <summary>
        /// need email address
        /// </summary>
        public const string S_RESET_EMAIL_SUCCESS = @"A reset password email was sent to your mail box at {0}. If you don't see it in your inbox, please check your spam folder";

        /// <summary>
        /// need server error code + support link
        /// </summary>
        public const string S_SERVER_ERROR = @"{0}. Please contact <a href='{1}'>GToken Customer Support</a> for assistance.";
        #endregion

        #region Images in Email
        public const string S_IMAGE_IN_EMAIL_HEADER_LOGO = @"/static/images/header_logo.png";
        public const string S_IMAGE_IN_EMAIL_GREY_TEXTURE = @"/static/images/grey-texture.png";
        public const string S_IMAGE_IN_EMAIL_DIAMOND_TEXTURE = @"/static/images/diamon-texture.png";
        public const string S_AVATAR_UPLOAD_DIR = @"..\\avatars\\";
        public const string S_IMAGE_IN_EMAIL_LOGO_GOPLAY = @"/static/images/logo_img.png";
        public const string S_IMAGE_IN_EMAIL_KENNETH = @"/static/images/Kenneth.jpg";
        #endregion

        #region Role name
        public const string S_VENVICI = "venvici";
        #endregion

        /// <summary>
        /// Description when create free coin transaction
        /// </summary>
        public const string S_DESCRIPTION_CREATE_FREE_COIN_TRANSACTION = @"Newbie Reward";


        public const string S_ERROR_FROM_GTOKEN_API = @"GTokenAPI: ";
        public const string S_ERROR_FROM_GDINE = @"GDine: ";
    }
}
