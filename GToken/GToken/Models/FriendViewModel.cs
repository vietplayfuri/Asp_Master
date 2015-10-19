using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Platform.Models;

namespace GToken.Models
{
    public class FriendViewModel
    {

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

    public class ResponseFriend
    {
        public string username { get; set; }
        public Profile profile { get; set; }
    }

    public class RequestAddFriendAPI
    {
        public string session { get; set; }
        public string partner_id { get; set; }
        public string friend_username { get; set; }
        public string status { get; set; }
    }

    public class SearchFriendAPI
    {
        public string partner_id { get; set; }
        public string session { get; set; }
        public string keyword { get; set; }
        public int? offset { get; set; } 
        public int? count { get; set; }
    }
}