namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class studio
    {
        public studio()
        {
        }

        public int id { get; set; }
        
        public string name { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public bool is_archived { get; set; }

    }
}
