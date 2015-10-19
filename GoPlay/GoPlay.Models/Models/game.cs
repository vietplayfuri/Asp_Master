namespace GoPlay.Models
{
    using Platform.Utility;
    using System;
    using System.Collections.Generic;
    public class Game
    {
        public Game()
        {
        }

        public int id { get; set; }

        public string guid { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public bool is_archived { get; set; }

        public int? studio_id { get; set; }

        public bool is_active { get; set; }

        public string banner_filename { get; set; }

        public string icon_filename { get; set; }

        public string download_links { get; set; }

        public string thumb_filename { get; set; }

        public string genre { get; set; }

        public string short_description { get; set; }

        public string endpoint { get; set; }

        public string gtoken_client_id { get; set; }

        public string gtoken_client_secret { get; set; }

        public string slider_images { get; set; }

        public string current_version { get; set; }

        public string file_size { get; set; }

        public string content_rating { get; set; }

        public string current_changelog { get; set; }

        public DateTime? released_at { get; set; }

        public string warning { get; set; }

        public string game_invite_protocol { get; set; }

        public bool is_featured { get; set; }
        public bool is_popular { get; set; }
        /// <summary>
        /// Can put many links seperated by ","
        /// </summary>
        public string youtube_links { get; set; }

        #region extend properties
        public string studio_name { get; set; }
        public bool isComingSoon { get; set; }
        public dynamic platforms { get; set; }

        public string detail_name {
            get
            {
                return (this.name + "-" + this.id).GenerateSlug();
            }
        }

        #endregion

        

        public object ToDict(string _params, string rootUrl, promotion promotion)
        {
            return new
            {
                id = this.id,
                name = this.name,
                web_link = string.Format("{0}{1}", rootUrl, _params),
                game_invite_protocol = this.game_invite_protocol,
                icon_filename = this.icon_filename,
                @params = _params,
                studio = this.studio_name,
                @promotion = promotion == null ? new promotion() : promotion.ToDict(),
            };
        }
    }
    
    public class InvitableGameModel
    {
        public int game_id { get; set; }
        public DateTime max { get; set; }
    }

    /// <summary>
    /// Used in api game/update-game-stats
    /// </summary>
    public class UpdateGameStatusModel
    {
        public string title { get; set; }
        public string value { get; set; }
        public string @public { get; set; }
    }
    public class SearchCondition
    {
        public string keywork { get; set; }
        public string genre { get; set; }
        public string platform { get; set; }
        public string release { get; set; }
    }

    /// <summary>
    /// Used to show in all dropdownlist about games
    /// </summary>
    public class SimpleGame
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
