using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Models
{
    static public class GoPlayConstantValues
    {
        public const string PLATFORM_IOS = "ios";
        public const string PLATFORM_ANDROID = "android";
        public const string PLATFORM_PC = "pc";
        public const string PLATFORM_APK = "apk";

        /// <summary>
        /// coming
        /// </summary>
        public const string RELEASE_COMING = "coming";
        public const string RELEASE_1WEEK = "1week";
        public const string RELEASE_1MONTH = "1month";
        public const string RELEASE_6MONTH = "6month";

        public const string S_CREDIT_TYPE = "CreditType";
        public const string S_PACKAGE = "Package";

        public const string S_PLAY_TOKEN = @"Play Token";
        public const string S_FREE_PLAY_TOKEN = @"Free Play Token";

        public const string S_ALL_GAMES = @"All games";
        public const string S_ALL = @"All";
        public const string S_WEBSITE = @"Website";
        
        /// <summary>
        /// Transfer how many play token to someone - we need two params
        /// </summary>
        public const string S_TRANSFER_TOKEN_DESCRIPTION = @"You transferred {0} Play Token to {1}";

        /// <summary>
        /// Receive how many play token from someone - we need two params
        /// </summary>
        public const string S_RECEIVE_TOKEN_DESCRIPTION = @"You received {0} Play Token from {1}";

        public const string COIN_TRANSACTION = "coin_transaction";
        public const string GCOIN_TRANSACTION = "gcoin_transaction";
        public const string FREE_COIN_TRANSACTION = "free_coin_transaction";
        public const decimal D_MIN_AMOUNT = 0.01m;
        public const string S_DECIMAL_REGEX = @"^[0-9]+((\.[0-9]?[0-9]?)*)$";

        /// <summary>
        /// Used in transfer function, name of element + after validation, we will return result to this name
        /// </summary>
        public const string S_RECEIVER_ID = @"receiverId";
        /// <summary>
        /// Used in transfer function, name of element + after validation, we will return result to this name
        /// </summary>
        public const string S_PLAY_TOKEN_AMOUNT = @"playTokenAmount";

        public const string S_RESETBALANCE_DESCRIPTION = @"Authenticated Tester Reset Balance";

        /// <summary>
        /// 2 PARAMS: free play token and play token
        /// </summary>
        public const string S_RESETBALANCE_SUCCESS = @"Account balance was reset to {0} Free Play Token and {1} Play Token";

        public const string S_PERMISSION_ACCESS_ADMIN_ACCOUNTANT_PAGE = @"access_admin_accountant_page";
        public const string S_ROLE_CUSTOMER_SUPPORT = @"customer_support";
        public const string S_ROLE_GAME_ADMIN = @"game_admin";
        public const string S_ROLE_ADMIN = @"admin";
        public const string S_ROLE_GAME_ACCOUNTANT = @"game_accountant";
        public const string S_ROLE_ACCESS_ADMIN_GAME_PAGE = @"access_admin_game_page";


        public static readonly List<string> LIST_OF_DELETED_ROLES = new List<string> { S_ROLE_GAME_ADMIN, S_ROLE_GAME_ACCOUNTANT };

        public const string S_GCOIN_CONVERT_DESCRIPTION = @"You converted {0} GCoin to USD and sent to account {1}.";

        public const double I_MAXIMUM_EXCHANGE_RATE = 100000;
        public const double I_MINIMUM_EXCHANGE_RATE = 0;
    }
}
