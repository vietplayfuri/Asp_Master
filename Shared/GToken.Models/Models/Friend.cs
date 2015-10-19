using System;

namespace Platform.Models
{
    public class Friend : ModelBase
    {
        public string friend1_username { get; set; }
        public string friend2_username { get; set; }
        public DateTime sent_at { get; set; }
        public string status { get; set; }
    }
    
    public class FindFriendDto : ModelBase
    {
        public int id { get; set; }
        public string status { get; set; }
        public int full_count { get; set; }
    }
}
