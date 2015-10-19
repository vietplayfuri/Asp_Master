using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    public enum EGoPlayAction
    {
        #region Account
        /// <summary>
        /// ip_address / username / password / game_id
        /// </summary>
        Login = 0,
        /// <summary>
        ///  username / password / email / nickname / gender / referralCode / game_id / ip_address
        /// </summary>
        Register = 1,
        Profile = 2,
        EditProfile = 3,
        /// <summary>
        ///  game_id / ip_adress / oauth_id
        /// </summary>
        QueryUserId = 4,
        /// <summary>
        ///  session / service / access_token / game_id / ip_address
        /// </summary>
        ConnectOauth = 5,
        /// <summary>
        /// session / service / access_token / access_token / game_id / ip_address
        /// </summary>
        DisconnectOauth = 6,
        ChangePassword = 7,

        /// <summary>
        /// api/1/account/check-oauth-connection -----
        /// params: ip_address / session / game_id / service / token
        /// </summary>
        CheckOauthConnection = 8,
        /// <summary>
        /// ip_address / service / accessToken / game_id
        /// </summary>
        LoginOauth = 9,

        /// <summary>
        /// api/1/account/get-notifications ------
        /// params: session / game_id
        /// </summary>
        GetNotifications = 10,
        /// <summary>
        /// ip_address / username / token / amount
        /// </summary>
        PurchasePlayToken = 11,

        /// <summary>
        /// api/1/game/in-app-purchase -----
        /// params: ip_address / session / game_id / order_id / amount / exchange_option_type / exchange_option_id
        /// </summary>
        InAppPurchase = 12,       

        /// <summary>
        /// partner/0/update-vip-status -----
        /// params: ip_address / username / status / token / promotion_ratio
        /// </summary>
        UpdateVIPStatus = 13,
        
        #endregion
    }

    public enum EThirdPartyService
    {
        [Description("Facebook")]
        Facebook =1,
        [Description("Apple")]
        Apple =2
    }
    class Common
    {
    }
}
