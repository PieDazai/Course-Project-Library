namespace Domain.Statistics
{
    public record class ReaderCategoryStatisticItem
    {
        public required int AgeStart  { get; set; }
        public required int AgeEnd { get; set; }
        public required int Count { get; set; }
        public required double Percent { get; set; }
        public string AgeRange => $"{AgeStart}-{AgeEnd} лет";

    }
}
