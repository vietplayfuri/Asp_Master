using GoPlay.Models;
using System.Net;
using Platform.Utility;
using System;
using GoPlay.Dal;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Data;
using Platform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public async void LogApi(string version, string action, bool status, string user_agent,
            int gameId, int customerId, string ip_address, string message, string data)
        {
            IPAddress ip;
            string countryCode = await Task.Factory.StartNew(() => string.Empty);
            string countryName = string.Empty;
            if (IPAddress.TryParse(ip_address, out ip))
            {
                if (!ip.GetCountryCode(c => countryCode = c, n => countryName = n))
                {
                    countryCode = ip.GetDefaultCountryCode();
                }
            }

            int? game_id = null;
            int? user_id = null;

            if (gameId > 0)
            {
                game_id = gameId;
            }
            if (customerId > 0)
            {
                user_id = customerId;
            }

            var log = new api_log
            {
                action = action,
                version = version,
                ip_address = ip_address,
                country_code = countryCode,
                user_agent = user_agent,
                game_id = game_id,
                customer_account_id = user_id,
                data = data,
                status = status,
                message = message
            };

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                repo.CreateLogApi(db, log);
            }
        }

        public string HandleFile(string serverPath, Stream stream,
            string destinationPath,
            string filename)
        {
            try
            {
                filename = string.Format("{0}-{1}", Guid.NewGuid().ToString(), filename);

                destinationPath += filename;

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["USE_AWS_S3"]))
                {
                    return string.Empty;
                }
                else
                {
                    string outPut = SaveFileInHost(stream, destinationPath);
                    if (string.IsNullOrEmpty(outPut))
                    {
                        return string.Empty;
                    }
                    filename = outPut.Replace(serverPath, "\\");
                }

                return filename;
            }
            catch
            {
                return string.Empty;
            }
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
        private string SaveFileInHost(Bitmap image, int width, int height, int quality, string filePath)
        {
            try
            {
                if (image == null)
                    return string.Empty;

                int original_width = image.Width;
                int original_height = image.Height;

                if (width > original_width || height > original_height)
                {
                    return string.Empty;
                }

                int resize_width = 0;
                int resize_height = 0;
                if (original_width > original_height)
                {
                    resize_width = Convert.ToInt32(height * (float)(original_width / original_height));
                    resize_height = height;
                }
                else
                {
                    resize_width = width;
                    resize_height = Convert.ToInt32(width * (float)(original_height / original_width));
                }

                if (width > resize_width || height > resize_height)
                {
                    return string.Empty;
                }

                Bitmap newImage = new Bitmap(resize_width, resize_height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, resize_width, resize_height);
                }

                var point_left = (resize_width / 2) - (width / 2);
                var point_upper = (resize_height / 2) - (height / 2);

                Bitmap crop = newImage.Clone(new Rectangle(point_left, point_upper, 100, 100), PixelFormat.Undefined);

                crop.Save(filePath);
                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Save without crop
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string SaveFileInHost(Bitmap image, string filePath)
        {
            try
            {
                if (image == null)
                    return string.Empty;

                image.Save(filePath);
                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string SaveFileInHost(Stream data, string filePath)
        {
            try
            {
                if (data == null)
                    return string.Empty;

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    data.CopyTo(fileStream);
                }

                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }


        public bool DeleteFile(string directory)
        {
            try
            {
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["USE_AWS_S3"]))
                {
                    File.Delete(directory);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public int CountCustomerAccountId(string condition)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CountCustomerAccountId(db, condition);
            }
        }

        public Result<List<SimpleApiLog>> GetLogs(int userId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetSimpleApiLogs(db, userId);
            }
        }
    }
}
