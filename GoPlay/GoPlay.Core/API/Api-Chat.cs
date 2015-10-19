
using Newtonsoft.Json;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public string LoginChatSystem(int userId, string username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CHAT_SERVER"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(ConstantValues.S_USERNAME, username), 
                    new KeyValuePair<string, string>(ConstantValues.S_USERID, userId.ToString())
                });

                HttpResponseMessage response = client.PostAsync(Urls.CHAT_SERVICE_HOST, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;

                    return data != null ? data : string.Empty;
                }
                //TODO: send email when error
                return string.Empty;
            }
        }
    }
}
