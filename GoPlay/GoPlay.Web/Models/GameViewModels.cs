using GoPlay.Models;
using Platform.Utility.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Models
{
    public class GamesViewModel
    {
        public List<Game> games { get; set; }

        public List<Game> featuredGames { get; set; }

        public List<Game> popularGames { get; set; }

        public List<string> genres { get; set; }

        public Pagination pagination { get; set; }
    }

    /// <summary>
    /// Used for API InAppPurchase
    /// </summary>
    public class APIInAppPurchaseModel
    {
        public string game_id { get; set; }
        public string session { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
        public string order_id { get; set; }
        public int exchange_option_id { get; set; }
        public string exchange_option_identifier { get; set; }
        public string exchange_option_type { get; set; }
    }

    /// <summary>
    /// Used for get game list api
    /// </summary>
    public class APIGetGameListModel
    {
        public string game_id { get; set; }
        public string session { get; set; }
    }


    /// <summary>
    /// Used for /1/game/update-game-stats api
    /// </summary>
    public class APIUpdateGameStatusModel
    {
        public string game_id { get; set; }
        public string session { get; set; }
        public string stats { get; set; }
    }

    /// <summary>
    /// Used for 3 apis: save/get/read progress
    /// </summary>
    public class APIProgressModel
    {
        public string game_id { get; set; }
        public string session { get; set; }
        public string data { get; set; }
        public string meta { get; set; }
        public HttpFileCollection file { get; set; }
        public bool? send_data { get; set; }
    }
    public class SearchGamesViewModel
    {
        public List<Game> games { get; set; }
        public List<string> genres { get; set; }
        public Pagination pagination { get; set; }
        public int count { get; set; }
        public string keywork { get; set; }
        public string genre { get; set; }
        public string platform { get; set; }
        public string release { get; set; }
    }


    public class DetailGameViewModel
    {
        public List<Game> games { get; set; }

        public Game game { get; set; }

        public int slider_len { get; set; }
    }

    public class GameImage
    {
        public string index { get; set; }
        public string filename { get; set; }
    }


    public class YoutubeLink
    {
        public string id { get; set; }
        public string link { get; set; }
        public string embedded_player { get; set; }
        public string thumbnail { get; set; }
    }

    public class GameImages
    {
        public List<GameImage> images { get; set; }
    }

    /// <summary>
    /// Result of GetExchangeItems action
    /// </summary>
    public class GetExchangeItemsModel
    {
        public GetExchangeItemsModel()
        {
            Credit = new List<CreditTypeDictationary>();
            Package = new List<PackageDictationary>();
        }
        public List<CreditTypeDictationary> Credit { get; set; }
        public List<PackageDictationary> Package { get; set; }
    }

    public class PayLoad
    {
        public string game_id { get; set; }
        public int user_id { get; set; }
        public int exchange_option_id { get; set; }
        public string exchange_option_identifier { get; set; }
        public int quantity { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public bool is_free { get; set; }
        public string transaction_id { get; set; }
        public decimal gtoken_value { get; set; }
        public decimal play_token_value { get; set; }
        public string exchange_option_type { get; set; }

    }

    public class PayLoadResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
        public string session { get; set; }
        public string action { get; set; }
    }

    public class UserData
    {
        public string uid { get; set; }
    }
    public class Desc
    {
        public decimal money { get; set; }

        public string orderid { get; set; }
    }

    //public class PayLoadResultSlamdunk
    //{
    //    public int code { get; set; }
    //    public Desc desc { get; set; }
    //    public UserData userdata { get; set; }
    //}

    public class PayLoadResultSlamdunk
    {
        public int code { get; set; }
        public dynamic desc { get; set; }
        public UserData userdata { get; set; }
    }


    public class PayLoadResultSlamdunk1
    {
        public int code { get; set; }
        public Desc desc { get; set; }
    }

    public class PayLoadSlamdunk
    {
        public string @do { get; set; }
        public int platform { get; set; }
        public string token { get; set; }
        public int sid { get; set; }
        public string uid { get; set; }
        public int acct { get; set; }
        public int buyid { get; set; }
        public string coid { get; set; }
        public decimal money { get; set; }
        public string orderid { get; set; }
    }

    public class InitTransactionModel
    {
        public dynamic coinTransaction { get; set; }
        public credit_transaction creditTransaction { get; set; }
    }

    public class GameListingViewModel
    {
        public string term { get; set; }

    }

    public class TransactionDict
    {
        public string transaction_id { get; set; }
        public decimal gtoken_value { get; set; }
        public decimal goplay_token_value { get; set; }
        public decimal quantity { get; set; }
        public bool is_free { get; set; }
        public int exchange_option_id { get; set; }
        public string exchange_option_identifier { get; set; }
        public string exchange_option_type { get; set; }
    }

    public class Slider
    {
        public List<SliderImages> images { get; set; }
    }
    public class SliderImages
    {
        public string index { get; set; }
        public string filename { get; set; }
    }

    public class GameSearch
    {
        public GameSearch()
        {
            promotion = new { };
        }
        public int id { get; set; }
        public string name { get; set; }
        public string web_link { get; set; }
        public string game_invite_protocol { get; set; }
        public string icon_filename { get; set; }
        public string @params { get; set; }
        public string studio { get; set; }
        public dynamic promotion { get; set; }
        public bool isComingSoon { get; set; }
        public string banner_filename { get; set; }
        public dynamic platforms { get; set; }
        public string genre { get; set; }
        public string detail_name { get; set; }
    }
}