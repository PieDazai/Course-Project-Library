namespace Model
{
    internal class Loan
    {
        public int ID { get; set; }
        public DateTime IssuanceDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Fine { get; set; }
        public string Status { get; set; }
        public int FinalPrice { get; set; }
        public int ReaderID { get; set; }
        public int BookID { get; set; }

        public Loan(int iD, DateTime issuanceDate, DateTime? returnDate, int fine,
                    string status, int finalPrice, int readerID, int bookID)
        {
            ID = iD;
            IssuanceDate = issuanceDate;
            ReturnDate = returnDate;
            Fine = fine;
            Status = status;
            FinalPrice = finalPrice;
            ReaderID = readerID;
            BookID = bookID;
        }

    }
}
