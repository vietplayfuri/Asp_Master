using GToken.Web.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web;
using BlueChilli.Lib;
using System.Net;

namespace GToken.Models
{
    public class SendEmailModel
    {
        public bool SendMail(string toEmail, object data)
        {
            try
            {
                string path = HttpContext.Current.Server
                        .MapPath(ConfigurationManager.AppSettings["ResetPasswordTemplate"]);
                var parserData = data.ToDynamic();
                string content = StringExtend.TransformWithDynamic(File.ReadAllText(path), parserData);
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                var mailMessage = new MailMessage("hi@gtoken.com", toEmail)
                {
                    Subject = "GToken - Reset Password",
                    Body = content,
                    IsBodyHtml = true
                };

                client.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}