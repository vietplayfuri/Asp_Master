using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.Utility;
namespace GoPlay.Models
{
    public class promotion
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public decimal threshold { get; set; }
        public decimal progress { get; set; }
        public bool is_archived { get; set; }

        #region Extend
        public int game_id { get; set; }
        #endregion

        public object ToDict()
        {
            return new {
                id = this.id,
                name = this.name,
                code = this.code,
                description = this.description,
                start_at = TimeHelper.EpochFromDatetime(this.start_at),
                end_at = TimeHelper.EpochFromDatetime(this.end_at),
                threshold = this.threshold,
                progress = this.progress,
                is_archived = this.is_archived
            };
        }
    }
}
