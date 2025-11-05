namespace Data.Interfaces
{
    public record BookFilter
    {
        public static BookFilter Empty => new();
        public string? Author { get; init; }
        public string? Title { get; init; }
        public string? Genre { get; init; }
        public bool? AvailableOnly { get; init; }
    }
}
