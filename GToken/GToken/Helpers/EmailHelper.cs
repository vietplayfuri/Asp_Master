using GToken.Web.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web;
using RazorEngine.Templating;
using PreMailer.Net;
using System.Net;
using System.Threading.Tasks;

namespace GToken.Models
{
    public class EmailHelper
    {
        public static async Task<bool> SendMailResetPasword(string toEmail, object data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                var supportEmail = ConfigurationManager.AppSettings["HI_EMAIL_SENDER"];
                var displayName = ConfigurationManager.AppSettings["HI_EMAIL_NAME"];
                var from = new MailAddress(supportEmail, displayName);
                var to = new MailAddress(toEmail);

                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "GToken - Reset Password",
                    Body = GetEmailBody(ConfigurationManager.AppSettings["ResetPasswordTemplate"],data),
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetEmailBody(string template, object data)
        {
            string path = HttpContext.Current.Server
                       .MapPath("~/") + template;
            TemplateService templateService = new TemplateService();
            var emailHtmlBody = templateService.Parse(File.ReadAllText(path), data, null, null);
            return PreMailer.Net.PreMailer.MoveCssInline(emailHtmlBody, true).Html;
        }
    }
}