namespace Data.Interfaces
{
    public record LoanFilter
    {
        public static LoanFilter Empty => new();
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public int? ReaderId { get; init; }
        public int? BookId { get; init; }
        public string? Status { get; init; }
    }
}
