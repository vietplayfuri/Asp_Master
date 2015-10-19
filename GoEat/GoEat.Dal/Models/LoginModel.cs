namespace GoEat.Dal.Models
{
    public class LoginModel
    {
        public CustomerAccountProfile profile { get; set; }
        public bool success { get; set; }
        // public string message { get; set; } // not used //
        public string error_code { get; set;}
        public string session { get; set; }
        
    }
}
