namespace Domain.Statistics
{
    public record class ReaderActivityStatisticItem
    {
        public required int Year { get; set; }
        public required int Month { get; set; }
        public required int ActiveReaders { get; set; }
        public required int BooksRented { get; set; }

        public string GetPeriodName()
        {
            var date = new DateTime(Year, Month, 1);
            return date.ToString("MMM yyyy");
        }

    }
}
