using GoPlay.Core;
using GoPlay.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Areas.Admin.Models
{
    /// <summary>
    /// Used for shown list in index page of report
    /// </summary>
    public class AdminReportModel
    {
        public AdminReportModel()
        {
            games = new List<SimpleGame>();
            source = new List<AdminReportDataModel>();
            
        }
        public string timezone { get; set; }
        public int game_id { get; set; }
        public List<SimpleGame> games { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public string isDaily { get; set; }
        public string query { get; set; }
        public string export { get; set; }
        public List<AdminReportDataModel> source { get; set; }
    }

    public class AdminReportDataModel
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }

    public class AdminReportActiveUserModel
    {
        public AdminReportActiveUserModel()
        {
            source = new List<AdminReportDataModel>();
            empty_games = new List<int>();
        }
        public string timezone { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public string query { get; set; }
        public List<AdminReportDataModel> source { get; set; }

        public List<int> empty_games { get; set; }
    }
}