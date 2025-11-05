namespace Data.Interfaces
{
    public record ReaderFilter
    {
        public static ReaderFilter Empty => new();
        public int? TicketNumber { get; init; }
        public string? FullName { get; init; }
        public string? PhoneNumber { get; init; }
    }
}
