namespace Domain
{
    public class Loan
    {
        public int Id { get; set; }
        public DateOnly IssuanceDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public int Fine { get; set; }
        public string Status { get; set; }
        public int FinalPrice { get; set; }
        public Reader Reader { get; set; }
        public Book Book { get; set; }

        public Loan() {}

    }
}
