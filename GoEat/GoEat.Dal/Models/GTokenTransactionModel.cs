namespace GoEat.Dal.Models
{
    public class GTokenTransactionModel
    {
        public GTokenTransaction transaction { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
        public string session { get; set; }
    }

    public class GTokenpProfileModel
    {
        public CustomerAccountProfile profile { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
        public string session { get; set; }
    }

    public class ResultDirectChargeGtoken
    {
        public GRecordTokenTransaction transaction { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
    }

    public class CheckGtokenBalance
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
    }
}
