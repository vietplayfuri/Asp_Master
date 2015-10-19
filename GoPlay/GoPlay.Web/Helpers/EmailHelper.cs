using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Net;
using RazorEngine.Templating;
using PreMailer.Net;
using GoPlay.Web.Models;
using GoPlay.Models;
using System.Threading.Tasks;

namespace GoPlay.Web.Helpers
{
    public class EmailHelper
    {
        public static async Task<bool> SendMailWelcome(string toEmail, object data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                var welcomeMail = ConfigurationManager.AppSettings["GOPLAY_WELCOME_EMAIL_SENDER"];
                var displayName = ConfigurationManager.AppSettings["GOPLAY_WELCOME_EMAIL_NAME"];
                var from = new MailAddress(welcomeMail, displayName);
                var to = new MailAddress(toEmail);

                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "Thank you for signing up!",
                    Body = GetEmailBody(ConfigurationManager.AppSettings["WelcomeMessageTemplate"], data),
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
        public static async Task<bool> SendMailResetPassword(string toEmail, object data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                var supportEmail = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                var displayName = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_NAME"];
                var from = new MailAddress(supportEmail, displayName);
                var to = new MailAddress(toEmail);

                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "PlayToken - Reset Password",
                    Body = GetEmailBody(ConfigurationManager.AppSettings["ResetPasswordTemplate"], data),
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

        public static async Task<bool> SendMailSupport(SupportViewModel data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                var adminEmail = ConfigurationManager.AppSettings["GOPLAY_ADMIN_EMAIL_SENDER"];
                var emailSupport = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];// TODO: have to replace to CUSTOMER_SUPPORT_EMAIL_SENDER when deploy
                var displayName = ConfigurationManager.AppSettings["GOPLAY_ADMIN_EMAIL_NAME"];
                var from = new MailAddress(adminEmail, displayName);
                var to = new MailAddress(emailSupport);

                var mailMessage = new MailMessage(from, to)
                {
                    Subject = string.Format("{0} [From {1}]", data.subject, data.customerEmail),
                    Body = GetEmailBody(ConfigurationManager.AppSettings["SupportTemplate"], data),
                    IsBodyHtml = true
                };
                mailMessage.ReplyToList.Add(data.customerEmail);
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> SendMailInvoice(InvoiceViewModel data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                var supportEmail = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                //   var email_temp = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER_TEST"];// TODO: have to replace to data.payer.email when deploy
                var displayName = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_NAME"];
                var from = new MailAddress(supportEmail, displayName);
                var to = new MailAddress(data.payer.email);//data.payer.email
                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "PlayToken - Electronic Receipt",
                    Body = GetEmailBody(ConfigurationManager.AppSettings["InvoiceTemplate"], data),
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

        public static async Task<bool> SendUpointCallBack(string body, string emailTo)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                var email_temp = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER_TEST"];

                var supportEmail = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                var displayName = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_NAME"];
                var from = new MailAddress(supportEmail, displayName);
                var to = new MailAddress(emailTo);//emailTo

                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "GToken - Something wrong with your UPoint transaction",
                    Body = body,
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

        public static async Task<bool> SendMailFriendRequest(FriendRequestEmail data)
        {
            try
            {
                var client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                var supportEmail = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                var displayName = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_NAME"];
                //var email_temp = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER_TEST"];

                var from = new MailAddress(supportEmail, displayName);
                var to = new MailAddress(data.to_email);
                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "You have a friend request!",
                    Body = GetEmailBody(ConfigurationManager.AppSettings["FriendRequestTemplate"], data),
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

        public static void SendMail(MailMessage mailMessage)
        {
            var client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Send(mailMessage);
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
