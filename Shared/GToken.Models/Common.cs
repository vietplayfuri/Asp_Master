using System;
using System.ComponentModel;
using System.Configuration;
namespace Platform.Models
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

    public enum Partners
    {
        None,
        IAH
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
        Reconcile_fail,

        [Description("phone_number_pending")]
        Phone_number_pending,

        [Description("speedy_pending")]
        Speedy_pending,

        [Description("tmoney_pending")]
        Tmoney_pending,

        [Description("telkomsel_voucher_pending")]
        Telkomsel_voucher_pending,

        [Description("telkom_voucher_pending")]
        Telkom_voucher_pending,

        [Description("spin_voucher_pending")]
        Spin_voucher_pending,
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

    /// <summary>
    /// kind of login (facebook, google, ..)
    /// </summary>
    public enum LoginType
    {
        [Description("facebook")]
        Facebook,

        [Description("google")]
        Google,

        [Description("other")]
        Other,
    }
    public enum VenviciAction
    {
        CheckGtoken = 0,
        Addmember = 1,
        CreditInCC = 2,
        PushBV = 3,
        deductGToken = 4,
        CreditInGT = 5,
    }

    public enum ReferralCampaignStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        [Description("Active")]
        Active = 1,
        /// <summary>
        /// Running
        /// </summary>
        [Description("Running")]
        Running = 2,
        /// <summary>
        /// Finished
        /// </summary>
        /// 
        [Description("Finished")]
        Finished = 3,
        /// <summary>
        /// Inactive Admin force stop
        /// </summary>
        [Description("Inactive_Force_Stop")]
        Inactive = 4
    }

    public enum VenviciReult
    {
        [Description("SUCCESS")]
        success,
        [Description("FALSE")]
        fail,
    }

    public enum PaymentMethod
    {
        [Description("UPoint Balance Deduction")]
        UPoint_Deduction,
        [Description("UPoint Speedy")]
        UPoint_Speedy,
        [Description("UPoint T-Money")]
        UPoint_TMoney,
        [Description("UPoint Telkomsel Voucher")]
        UPoint_TelkomselVoucher,
        [Description("UPoint SPIN Voucher")]
        UPoint_SPINVoucher,
        [Description("UPoint Telkom Voucher")]
        UPoint_Telkomoucher,
        [Description("UPoint Telkom Voucher")]
        UPoint_Telkom,
        [Description("UPoint SPIN Voucher")]
        UPoint_SPIN,
        [Description("PayPal")]
        PayPal,
        [Description("Top Up Card")]
        TopUpCard,
        [Description("convert gcoin")]
        Convert_Gcoin
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
        public const string DEFAULT_GAME_THUMBNAIL_URL = "/static/images/game-thumbnail.png";
        public const string DEFAULT_CREDIT_TYPE_GOLD_ICON_URL = "/static/images/exchange-gold.png";
        public const string DEFAULT_CREDIT_TYPE_GEM_ICON_URL = "/static/images/exchange-gem.png";
        public const string DEFAULT_PACKAGE_ICON_URL = "/static/images/exchange-package.png";
        public const string AVATAR_URL = "/static/images/profile_avatars/{0}.png";
        public const string COVER_URL = "/static/images/profile_covers/{0}.png";
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

        public enum GTOKEN_ADMIN_EMAIL_SENDER
        {
            [Description("GToken Admin")]
            Name,
            [Description("admin@gtoken.com")]
            Email
        }
        public enum CUSTOMER_SUPPORT_EMAIL_SENDER
        {
            [Description("GToken Customer Support")]
            Name,
            [Description("hi@gtoken.com")]
            Email
        }
        public enum SUPPORT_EMAIL_SENDER
        {
            [Description("GToken Support")]
            Name,
            [Description("support@gtoken.com")]
            Email
        }
        public const int PAYPAL_MAX_TRANSACTION_AMOUNT = 4000;
        public const string DEFAULT_PAYPAL_CURRENCY = "USD";

        //# Generate timezones list
        //from pytz import timezone, common_timezones
        //from datetime import datetime

        //READABLE_COMMON_TIMEZONES = []
        //for tzStr in common_timezones:
        //    tz = timezone(tzStr)
        //    readableTimezone = "%s GMT%s" % (tzStr, datetime.now(tz).strftime("%z"))
        //    READABLE_COMMON_TIMEZONES.append((tzStr, readableTimezone))

        public const string AVATAR_UPLOAD_DIR = "avatars/";
    }

    public static class Urls
    {
        public const string VENVICI_ACTION_CHECKGTOKEN = @"checkGToken.jsp";
        public const string VENVICI_ACTION_ADDMEMBER = @"addMember.jsp";
        public const string VENVICI_ACTION_CREDITINCC = @"creditInCC.jsp";
        public const string VENVICI_ACTION_PUSHBV = @"pushBv.jsp";
        public const string VENVICI_ACTION_DEDUCTGTOKEN = @"deductGToken.jsp";
        public const string VENVICI_ACTION_CREDITINGT = @"creditInGT.jsp";

        private const string BASE_GT_DEV_SERVER = @"https://dev.gtoken.com/api/1/";  // TODO:CHeck this
        private const string BASE_GT_LIVE_SERVER = @"http://gtoken.com/api/1/";  // TODO:CHeck this

        public const string CHAT_SERVICE_HOST = @"user/login";
        public const string ACTION_PROFILE = @"account/profile";
        public const string ACTION_EDIT_PROFILE = @"account/edit-profile";
        public const string ACTION_REGISTER = @"account/register";
        public const string ACTION_LOGIN = @"account/login-password";
        public const string ACTION_RESET_PASSWORD = @"account/reset-password";
        public const string ACTION_CONFIRMED_PASSWORD = @"account/confirmed-password";
        public const string ACTION_QUERY_USER_ID = @"account/query-user-id";
        public const string ACTION_LOGIN_OAUTH = @"account/login-oauth";
        public const string ACTION_DISCONNECT_OAUTH = @"account/disconnect-oauth";
        public const string ACTION_CONNECT_OAUTH = @"account/connect-oauth";
        public const string ACTION_CHECK_OAUTH_CONNECTION = @"account/check-oauth-connection";
        public const string ACTION_GET_NOTIFICATIONS = @"account/get-notifications";

        public const string ACTION_CREATE_TRANSACTION = @"transaction/create-transaction";
        public const string ACTION_RETRIEVE_TRANSACTION = @"transaction/retrieve-transaction";
        public const string ACTION_EXECUTE_TRANSACTION = @"transaction/execute-transaction";
        public const string ACTION_RECORD_TOKEN_TRANSACTION = @"transaction/record-token-transaction";
        public const string ACTION_CHANGE_PASSWORD = @"account/change-password";
        /// <summary>
        /// Controller partner/0/purchase-play-token
        /// </summary>
        public const string ACTION_PARTNER_PURCHASE_PLAY_TOKEN = @"partner/0/purchase-play-token";
        /// <summary>
        /// Controller partner/0/update-vip-status
        /// </summary>
        public const string ACTION_PARTNER_UPDATE_VIP_STATUS = @"partner/0/update-vip-status";

        /// <summary>
        /// api/1/game/in-app-purchase
        /// </summary>
        public const string ACTION_GAME_IN_APP_PURCHASE = @"api/1/game/in-app-purchase";

        #region Friend
        public const string ACTION_FRIEND_FRIEND_LIST = @"friend/friend-list";
        public const string ACTION_FRIEND_FRIEND_SEARCH = @"friend/search";
        public const string ACTION_FRIEND_SEND_REQUEST = @"friend/send-request";
        public const string ACTION_FRIEND_RESPONSE_REQUEST = @"friend/respond-request";
        #endregion

        #region GoEat

        public const string PARTNER_ID = "fee79113-8619-449a-b042-b4700ad452dd";
        public const string RESTAURANT_ID = "31280650-33f4-4b91-bc11-94d231b6f483";
        public const string HASHED_TOKEN = "6dd061cebeebd1f68fa35ce58959c8a94a672656f9410214c006aa9df0c1a54ea89dcdfa6c26a14aad71f7cdcbb6e79483628d9c66c8a8b6d74f0f7943553f89";

        public const string ACTION_RESET_CONFIRMED_PASSWORD = @"account/confirmed-password";
        public const string ACTION_DIRECT_CHARGE_GTOKEN = @"transaction/direct-charge-gtoken";
        public const string ACTION_CHECK_GTOKEN_BALANCE = @"transaction/check-gtoken-balance";
        #endregion

        public const string RESET_PASSWORD_LINK = @"https://dev.gtoken.com/account/reset-password?return_url=";

        public static Uri GetBaseServerUri(bool useLiveServer = false)
        {
#if (!DEBUG)
            // Make sure we don't accidentally use Debug Server in Release mode //
            System.Diagnostics.Trace.Assert(useLiveServer == true);
#endif

            return new Uri(useLiveServer ? BASE_GT_LIVE_SERVER : BASE_GT_DEV_SERVER);
            //return new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
        }

        public static int GetRestaurantId()
        {
            return 1;
        }

        public static int GetDiscountId()
        {
            return 2;
        }

        public static Uri GetGTokenServerUri(bool useLiveServer = false)
        {
#if (!DEBUG)
            // Make sure we don't accidentally use Debug Server in Release mode //
            System.Diagnostics.Trace.Assert(useLiveServer == true);
#endif

            return new Uri(useLiveServer ? BASE_GT_LIVE_SERVER : BASE_GT_DEV_SERVER);
        }
    }

}
