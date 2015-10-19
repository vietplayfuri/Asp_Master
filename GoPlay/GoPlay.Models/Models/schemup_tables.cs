namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class schemup_tables
    {
        public string table_name { get; set; }
        
        public string version { get; set; }
        public bool is_current { get; set; }

        public string schema { get; set; }
    }
}
