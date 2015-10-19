using System;
using System.ComponentModel;
using System.Configuration;
namespace GoEat.Models
{

    public enum Roles
    {
        User,
        Accountant,
        Administrator
    }

    public enum LoginTypes
    {
        Normal,
        FaceBook,
        Partners
    }

    public enum Genders
    {
        [Description("male")]
        Male,

        [Description("female")]
        Female,

        [Description("other")]
        Others
    }


    public enum TransactionStatus
    {
        [Description("pending")]
        Pending,

        [Description("processed")]
        Processed,

        [Description("failure")]
        Failure,

        [Description("cancelled")]
        Cancelled,

        [Description("success")]
        Success,

        [Description("pending_reconcile")]
        Pending_reconcile,

        [Description("reconcile_fail")]
        Reconcile_fail
    }

    public enum FriendStatus
    {
        [Description("accepted")]
        Accepted,
        [Description("pending")]
        Pending,
        [Description("waiting")]
        Waiting,
        [Description("rejected")]
        Rejected
    }

    public static class ConstantCommon
    {
        public const string BABEL_DATE_READABLE_FORMAT = "MMM d, yyyy";
        public const string BABEL_DATETIME_READABLE_FORMAT = "hh:mmtt, dd MMM";
        public const string MONEY_FORMAT = "N";

        public const bool LOGIN_DISABLED = false;

        public const string OAUTH2_PROVIDER_ERROR_URI = "/500";
        public const int OAUTH2_PROVIDER_TOKEN_EXPIRES_IN = 3600000; //#basically forever

        public const string DEFAULT_AVATAR_URL = "/static/images/avatar-pic.png";
        public const string DEFAULT_COVER_URL = "/static/images/cover-batman.png";

        //Default language
        public const string BABEL_DEFAULT_LOCALE = "en";
        //# Available languages
        public enum LANGUAGES
        {
            [Description("English")]
            en,
            [Description("Indonesian")]
            id,
            [Description("Malay")]
            ms,
            [Description("ไทย")]
            th,
            [Description("简体中文")]
            zh
        }

        public const int NICKNAME_MAX_LENGTH = 20; //# set to zero to disable this
        public const int BIO_MAX_LENGTH = 225; //# set to zero to disable this

        public const int PAYPAL_MAX_TRANSACTION_AMOUNT = 4000;
        public const string DEFAULT_PAYPAL_CURRENCY = "USD";

        public const string AVATAR_UPLOAD_DIR = "avatars/";
    }

    public static class Urls
    {

        //private const string BASE_GT_SERVER = @"https://dev.gtoken.com/api/1/";

        public const string CHAT_SERVICE_HOST = @"user/login";
        public const string ACTION_PROFILE = @"account/profile";
        public const string ACTION_EDIT_PROFILE = @"account/edit-profile";
        public const string ACTION_REGISTER = @"account/register";
        public const string ACTION_LOGIN = @"account/login-password";
        public const string ACTION_RESET_PASSWORD = @"account/reset-password";
        public const string ACTION_CONFIRMED_PASSWORD = @"account/confirmed-password";
        public const string ACTION_QUERY_USER_ID = @"account/query-user-id";
        public const string ACTION_LOGIN_OAUTH = @"account/login-oauth";
        public const string ACTION_CONNECT_OAUTH = @"account/connect-oauth";
        public const string ACTION_CHECK_OAUTH_CONNECTION = @"account/check-oauth-connection";
        public const string ACTION_GET_NOTIFICATIONS = @"account/get-notifications";

        public const string ACTION_CREATE_TRANSACTION = @"transaction/create-transaction";
        public const string ACTION_EXECUTE_TRANSACTION = @"transaction/execute-transaction";
        public const string ACTION_RECORD_TOKEN_TRANSACTION = @"transaction/record-token-transaction";
        public const string ACTION_CHANGE_PASSWORD = @"account/change-password";

        public const string PARTNER_ID = "fee79113-8619-449a-b042-b4700ad452dd";
        public const string RESTAURANT_ID = "31280650-33f4-4b91-bc11-94d231b6f483";
        public const string HASHED_TOKEN = "6dd061cebeebd1f68fa35ce58959c8a94a672656f9410214c006aa9df0c1a54ea89dcdfa6c26a14aad71f7cdcbb6e79483628d9c66c8a8b6d74f0f7943553f89";

        public const string ACTION_RESET_CONFIRMED_PASSWORD = @"account/confirmed-password";
        public const string ACTION_DIRECT_CHARGE_GTOKEN = @"transaction/direct-charge-gtoken";
        public const string ACTION_CHECK_GTOKEN_BALANCE = @"transaction/check-gtoken-balance";

        public static Uri GetBaseServerUri()
        {
            return new Uri(ConfigurationManager.AppSettings["BASE_GT_SERVER"]);
        }

        public static int GetRestaurantId()
        {
            return 1;
        }

        public static int GetDiscountId()
        {
            return 2;
        }

        public static Uri GetGTokenServerUri()
        {
            return new Uri(ConfigurationManager.AppSettings["BASE_GT_SERVER"]);
        }
    }

}
