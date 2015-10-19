using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Platform.Dal;
using Platform.Models;
using Platform.Utility;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Web.Configuration;
using DevOne.Security.Cryptography.BCrypt;
using Facebook;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Platform.Core
{
    public partial class Api
    {
        public int CreateReferralCampaign(ReferralCampaign referralCampaign)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var id = repo.CreateReferralCampaign(db, referralCampaign);
                referralCampaign.order_number = id;
                referralCampaign.id = id;
                repo.UpdateReferralCampaign(db, referralCampaign);
                return id;
            }
        }

        /// <summary>
        /// Get all referral campaigns history for report in admin
        /// </summary>
        /// <param name="timezone">Destination timezone</param>
        /// <param name="referrerUsername">inviter username</param>
        /// <param name="from">Time has still not converted to UTC</param>
        /// <param name="to">Time has still not converted to UTC</param>  
        /// <param name="status">null / 1: Live/ 2: Inactive / 3: Scheduled</param>
        public Result<List<RecordDownloadHistory>> GetRecordDownloadHistory(string timezone = null, int? campaign_id=null,
            int? gameId = null, string username = null, string referrerUsername = null,
            string from = null, string to = null, int? skip = null, int? take = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (from != null)
                    fromDate = Helper.timeFromString(from, timezone);
                if (to != null)
                    toDate = Helper.timeFromString(to, timezone);

                return repo.GetRecordDownloadHistory(db, campaign_id, gameId, username, referrerUsername, fromDate, toDate, skip, take);
            }
        }

        public decimal GetTotalReferralMoney(string referrerUsername)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTotalReferralMoney(db, referrerUsername);
            }
        }

        /// <summary>
        /// Get all record download history for user
        /// </summary>
        public Result<List<RecordDownloadHistory>> GetRecordDownloadHistory(string timezone, string earned_username, int? gameId = null, string username = null,
            string from = null, string to = null, int? skip = null, int? take = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (from != null)
                    fromDate = Helper.timeFromString(from, timezone);
                if (to != null)
                    toDate = Helper.timeFromString(to, timezone);

                return repo.GetRecordDownloadHistory(db, earned_username, gameId, username, fromDate, toDate, skip, take);
            }
        }


        /// <summary>
        /// Get all record download history for user
        /// </summary>
        public int CountRecordDownloadHistory(string timezone, string referrerUsername, int? gameId = null, string username = null,
            string from = null, string to = null, int? skip = null, int? take = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (from != null)
                    fromDate = Helper.timeFromString(from, timezone);
                if (to != null)
                    toDate = Helper.timeFromString(to, timezone);

                return repo.CountRecordDownloadHistory(db, referrerUsername, gameId, username, fromDate, toDate);
            }
        }


        /// <summary>
        /// Get all referral campaigns
        /// </summary>
        public Result<List<ReferralCampaign>> GetReferralCampaigns(int? status = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetReferralCampaigns(db, status);
            }
        }
        
        /// <summary>
        /// Get all referral campaigns that have time is match to start to finish
        /// </summary>
        /// <paparam name="status">Status that will be gotten</paparam>
        public Result<List<ReferralCampaign>> GetReferralCampaigns(List<int> status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetReferralCampaigns(db, status);
            }
        }

        public Result<List<ReferralCampaign>> GetCurrentReferralCampaigns(List<int> status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCurrentReferralCampaigns(db, status);
            }
        }

        public Result<List<ReferralCampaign>> GetInCommingReferralCampaigns(List<int> status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetInCommingReferralCampaigns(db, status);
            }
        }

        public bool UpdateReferralCampaigns(ReferralCampaign referralCampaign)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateReferralCampaign(db, referralCampaign);
            }
        }

        public bool UpdateReferralCampaigns(List<int> campaignId, int status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateReferralCampaigns(db, campaignId, status);
            }
        }

        public int CountRecordDownloadHistory(int referralCampaignId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CountRecordDownloadHistory(db, referralCampaignId);
            }
        }

        public bool DeleteReferralCampaign(int referralCampaignId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.DeleteReferralCampaign(db, referralCampaignId);
            }
        }

        public Result<ReferralCampaign> GetReferralCampaignById(int referralCampaignId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetReferralCampaignById(db, referralCampaignId);
            }
        }

        public Result<List<ReferralCampaign>> GetListValidCompaignOfGame(int game_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetListValidCompaignOfGame(db, game_id);
            }
        }


        public bool OrderReferralCampaign(int sourceReferralId, int destinationReferralId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                ReferralCampaign sourceReferral = repo.GetReferralCampaignById(db, sourceReferralId).Data;
                if (sourceReferral == null)
                    return false;
                ReferralCampaign destinationReferral = repo.GetReferralCampaignById(db, destinationReferralId).Data;
                if (destinationReferral == null)
                    return false;

                int sourceOrderNumber = sourceReferral.order_number;
                int destinationOrderNumber = destinationReferral.order_number;

                if (sourceOrderNumber == destinationOrderNumber)
                {
                    return false;
                }
                else
                {
                    sourceReferral.order_number = destinationOrderNumber;
                    destinationReferral.order_number = sourceOrderNumber;
                }

                repo.UpdateReferralCampaign(db, sourceReferral);
                repo.UpdateReferralCampaign(db, destinationReferral);

                return true;
            }
        }


        public int CreateImportReferralHistory(ImportReferralHistory importReferral)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateImportReferralHistory(db, importReferral);
            }
        }

        /// <summary>
        /// Get all import history in this tables
        /// </summary>
        /// <param name="timezone">user timezone</param>
        /// <param name="gameId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="skip">for paging</param>
        /// <param name="take">for paging</param>
        /// <returns></returns>
        public Result<List<ImportReferralHistory>> GetImportHistory(string timezone, int? gameId = null, string from = null, string to = null, int? skip = null, int? take = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (from != null)
                    fromDate = Helper.timeFromString(from, timezone);
                if (to != null)
                    toDate = Helper.timeFromString(to, timezone);

                return repo.GetImportHistory(db, gameId, fromDate, toDate, skip, take);
            }
        }

        /// <summary>
        /// get history which supports for downloading
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<ImportReferralHistory> GetImportHistoryById(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {

                return repo.GetImportHistoryById(db, id);
            }
        }


        public bool CheckImportHistoryByFilename(string filename)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {

                return repo.CheckImportHistoryByFilename(db, filename);
            }
        }
    }
}
