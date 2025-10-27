namespace Model
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int RentalCost { get; set; }
        public int Deposit { get; set; }
        public DateTime PublishedYear { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public int RackNumber { get; set; }


        public Book(int iD, string title, string author, string genre,
            int rentalCost, int deposit, DateTime publishedYear,
            int availableCopies, int totalCopies, int rackNumber)
        {
            ID = iD;
            Title = title;
            Author = author;
            Genre = genre;
            RentalCost = rentalCost;
            Deposit = deposit;
            PublishedYear = publishedYear;
            AvailableCopies = availableCopies;
            TotalCopies = totalCopies;
            RackNumber = rackNumber;
        }
    }
}
