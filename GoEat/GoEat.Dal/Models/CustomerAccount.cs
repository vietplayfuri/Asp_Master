using System;
using GoEat.Models;
using Newtonsoft.Json;

namespace GoEat.Dal.Models
{
    public class CustomerAccount_ToRemove : ModelBase
    {
        public int id { get; set; }
        public string username { get; set; }
        public string profile { get; set; }
        public CustomerAccountProfile Profile
        {
            get
            {
                if (String.IsNullOrEmpty(profile))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<CustomerAccountProfile>(profile);
            }
            set
            {
                profile = JsonConvert.SerializeObject(value);
            }
        }
        public DateTime created_at { get; set; }

        public string session { get; set; }
    }
}
