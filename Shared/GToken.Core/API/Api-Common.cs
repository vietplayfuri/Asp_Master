using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.Models;
using System.Net;
using Newtonsoft.Json;
using Platform.Utility;
using Platform.Dal;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web.Helpers;
namespace Platform.Core
{
    public partial class Api
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task LogApi(string version, string action, bool status, string user_agent,
            string partner_identifier, string customer_username, string ip_address, string message, string data)
        {

            try
            {
                IPAddress ip;
                string countryCode = string.Empty;
                string countryName = string.Empty;
                if (IPAddress.TryParse(ip_address, out ip))
                {
                    if (!ip.GetCountryCode(c => countryCode = c, n => countryName = n))
                    {
                        countryCode = ip.GetDefaultCountryCode();
                    }
                }

                var log = await Task.Factory.StartNew(() => new ApiLog
                {
                    action = action,
                    version = version,
                    ip_address = ip_address,
                    country_code = countryCode,
                    user_agent = user_agent,
                    partner_identifier = partner_identifier,
                    customer_username = customer_username,
                    data = data,
                    status = status,
                    message = message
                });

                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    repo.CreateLogApi(db, log);
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("Cannot write to log_api:  " + ex.ToString());
            }
        }

        public string HandleFile(string serverPath, Stream stream, string destinationPath, string filename = null)
        {
            filename = string.Format("{0}.jpg",
                string.IsNullOrEmpty(filename) ? Guid.NewGuid().ToString() : filename);

            destinationPath += filename;

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["USE_AWS_S3"]))
            {
                return string.Empty;
            }
            else
            {
                string outPut = SaveFileInHost(stream, 100, 100, destinationPath);
                if (string.IsNullOrEmpty(outPut))
                {
                    return string.Empty;
                }
                filename = outPut.Replace(serverPath, "\\");
            }

            return filename;
        }

        /// <summary>
        /// Save file in our host
        /// </summary>
        /// <param name="image">Image which will be save</param>
        /// <param name="width">min Width</param>
        /// <param name="height">min Height</param>
        /// <param name="quality">quality of new image from 1 to 99</param>
        /// <param name="filePath">destination path</param>
        /// <returns>True: save is success AND ELSE</returns>
        private string SaveFileInHost(Stream image, int width, int height, string filePath)
        {
            try
            {
                if (image == null)
                    return string.Empty;
                var newImage = new WebImage(image);

                var original_width = newImage.Width;
                var original_height = newImage.Height;

                if (original_width > original_height)
                {
                    var leftRightCrop = (original_width - original_height) / 2;
                    newImage.Crop(0, leftRightCrop, 0, leftRightCrop);
                }
                else if (original_height > original_width)
                {
                    var topBottomCrop = (original_height - original_width) / 2;
                    newImage.Crop(topBottomCrop, 0, topBottomCrop, 0);
                }
                newImage.Resize(width, height);
                newImage.Save(filePath, "jpg");
                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }



        public List<string> ImportReferral(string serverPath, Stream stream, string destinationPath, string filename,
            int campaignId,
            int userId, string ip_address, out ImportReferralHistory history)
        {
            try
            {
                history = new ImportReferralHistory();
                List<Tuple<string, string>> listUsers = new List<Tuple<string, string>>();
                string savedFile = destinationPath + filename;
                List<string> builderErrors = new List<string>(); ;
                List<Tuple<int, string>> listValidUsername = new List<Tuple<int, string>>();
                List<string> listInValidUsername = new List<string>();

                using (CsvFileReader reader = new CsvFileReader(stream))
                {
                    CsvRow row = new CsvRow();
                    int rowNumber = 1;
                    List<string> username = new List<string>();

                    using (CsvFileWriter writer = new CsvFileWriter(savedFile))
                    {
                        while (!reader.EndOfStream)
                        {
                            var errorMsg = reader.CheckValidRow(row, rowNumber == 1, ref username);
                            if (!string.IsNullOrEmpty(errorMsg))
                                builderErrors.Add(string.Format("Line {0}: {1}", rowNumber, errorMsg));
                            else if (username != null && username.Any())
                            {
                                foreach (var item in username)
                                {
                                    if (!listValidUsername.Select(t => t.Item2).Contains(item) && !listInValidUsername.Contains(item))
                                    {
                                        var id = GetIdByUsername(item);
                                        if (id > 0)
                                            listValidUsername.Add(new Tuple<int, string>(id, item));
                                        else
                                        {
                                            listInValidUsername.Add(item);
                                            builderErrors.Add(string.Format("Line {0}: {1} is invalid username", rowNumber, item));
                                        }
                                    }
                                }
                                listUsers.Add(new Tuple<string, string>(username[0], username[1]));
                            }
                            else if (rowNumber != 1 && !username.Any())
                            {
                                builderErrors.Add(string.Format("Line {0}: This row is empty", rowNumber));
                            }
                            rowNumber++;

                            reader.ReadRow(row);
                            writer.WriteRow(row);
                        }
                    }
                }

                if (builderErrors != null && builderErrors.Any())
                    return builderErrors;

                string filePath = savedFile.Replace(serverPath, "\\");
                if (string.IsNullOrEmpty(filePath))
                {
                    builderErrors.Add(ErrorCodes.SAVE_FILE_ERROR.ToErrorMessage());
                    return builderErrors;
                }

                #region Save data to DB
                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    ReferralCampaign referralCampaign = repo.GetReferralCampaign(db
                        , new List<int>() { (int)ReferralCampaignStatus.Running, (int)ReferralCampaignStatus.Finished }
                        , campaignId);
                    if (referralCampaign == null)
                    {
                        builderErrors.Add(ErrorCodes.INVALID_REFERRAL_CAMPAIGN.ToErrorMessage());
                        return builderErrors;
                    }

                    //if (repo.CountRecordDownloadHistory(db, referralCampaign.id) + listUsers.Count() > referralCampaign.quantity)
                    //{
                    //    builderErrors.Add(ErrorCodes.REFERRAL_CAMPAIGN_QUANTITY_IS_OVER.ToErrorMessage());
                    //    return builderErrors;
                    //}

                    List<Tuple<string, string, string>> listFail = new List<Tuple<string, string, string>>();

                    foreach (var row in listUsers)
                    {
                        var uid = listValidUsername.FirstOrDefault(u => string.Compare(u.Item2, row.Item1, true) == 0).Item1;
                        string error = ImportUserRerralCampaigns(repo, db, uid, referralCampaign.game_id, row.Item1, row.Item2, ip_address, referralCampaign);
                        if (!string.IsNullOrEmpty(error))
                        {
                            listFail.Add(new Tuple<string, string, string>(row.Item1, row.Item2, error));
                        }
                    }

                    string failData = string.Empty;
                    if (listFail.Any())
                    {
                        failData = SaveFailedImportResult(destinationPath, filename, listFail);
                    }
                    ImportReferralResult importResult = new ImportReferralResult
                    {
                        failed = listFail.Count(),
                        file_path = !string.IsNullOrEmpty(failData)
                            ? failData.Replace(serverPath, "\\")
                            : string.Empty,
                        pass = listUsers.Count() - listFail.Count(),
                        total = listUsers.Count()
                    };

                    //TODO: need to consider about the case when 100% is fail from VV api
                    CustomerAccount creator = repo.GetCustomerById(db, userId).Data;
                    history = new ImportReferralHistory
                    {
                        file_name = filename,
                        file_path = filePath,
                        game_id = referralCampaign.game_id, //TODO: can remove
                        importer_username = creator.username,
                        result = JsonConvert.SerializeObject(importResult),
                        referral_campaign_id = campaignId
                    };
                    history.id = repo.CreateImportReferralHistory(db, history);
                    return null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Fatal("ImportReferral:  " + ex.ToString());
                history = null;
                return new List<string>() { ErrorCodes.SYSTEM_ERROR.ToErrorMessage() };
            }
        }

        private string SaveFailedImportResult(string destinationPath, string filename, List<Tuple<string, string, string>> listFail)
        {
            string failData = destinationPath + "error_" + filename;
            using (CsvFileWriter writer = new CsvFileWriter(failData))
            {
                CsvRow row = new CsvRow();
                row.Add("Downloader username");
                row.Add("Referral username");
                row.Add("Error");

                writer.WriteRow(row);

                foreach (var item in listFail)
                {
                    row = new CsvRow();
                    row.Add(item.Item1);
                    row.Add(item.Item2);
                    row.Add(item.Item3);
                    writer.WriteRow(row);
                }
            }
            return failData;
        }
    }
}
