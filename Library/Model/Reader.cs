using System.Text.RegularExpressions;

namespace Domain
{
    public class Reader
    {
        private static readonly string PHONE_NUMBER_FORMAT = @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})";
        private string _phoneNumber;

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber {
            get => _phoneNumber;
            set => _phoneNumber = FormatPhoneNumber(value);
        }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }

        public Reader() { }

        private static string FormatPhoneNumber(string phoneNumber) {
            string result = Regex.Replace(phoneNumber, @"[^\d]", "");
            return Regex.Replace(result, PHONE_NUMBER_FORMAT, "$1 ($2) $3-$4-$5");
        }
    }
}
