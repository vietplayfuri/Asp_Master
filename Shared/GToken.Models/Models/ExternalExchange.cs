using System;
using System.Collections.Generic;

namespace Platform.Models
{

    public class ExchangeRate : ModelBase
    {
        public decimal exchange_rate { get; set; }
    }

    public class ExchangeRateData : ExchangeRate
    {
        public string month { get; set; }
        public string year { get; set; }
        public string source_currency { get; set; }
        public string destination_currency { get; set; }
    }

    public class YahooExchangeField
    {
        public decimal change { get; set; }
        public decimal chg_percent { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }

        public string symbol { get; set; }

        public string ts { get; set; }
        public string type { get; set; }
        public DateTime utctime { get; set; }

        public string volume { get; set; }
    }
    public class YahooExchangeSource
    {
        public string classname { get; set; }
        public YahooExchangeField fields { get; set; }
    }
    public class YahooExchange
    {
        public List<YahooExchangeResource> resources { get; set; }
    }
    public class YahooExchangeResource
    {
        public YahooExchangeSource resource { get; set; }
    }
  
}
