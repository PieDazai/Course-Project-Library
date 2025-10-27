namespace Model
{
    internal class Reader
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public int ReaderCategoryID { get; set; }

        public Reader(int iD, string fullName, string address, string phoneNumber,
                    string email, DateTime? birthDate, int readerCategoryID)
        {
            ID = iD;
            FullName = fullName;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            BirthDate = birthDate;
            ReaderCategoryID = readerCategoryID;
        }
    }
}
