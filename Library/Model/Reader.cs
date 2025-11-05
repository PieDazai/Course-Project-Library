namespace Domain
{
    public class Reader
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }

        public Reader() { }
    }
}
