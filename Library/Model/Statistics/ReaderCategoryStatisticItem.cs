namespace Domain.Statistics
{
    public record class ReaderCategoryStatisticItem
    {
        public required string CategoryName { get; set; }
        public required int Count { get; set; }
        public required double Percent { get; set; }
    }
}
