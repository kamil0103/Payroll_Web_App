namespace Payroll_Web_App.Server.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_initial { get; set; }
        public string email_address { get; set; }
        public string phone_number { get; set; }
    }
}
