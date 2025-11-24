using System.Text.RegularExpressions;

namespace Domain
{
    public class Reader
    {
        private static readonly string PHONE_NUMBER_FORMAT = @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})";
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }

        public Reader() { }
    }
}
