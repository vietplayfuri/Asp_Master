﻿using System;

namespace Platform.Models
{
    public class Studio : ModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_archived { get; set; }
    }
}
