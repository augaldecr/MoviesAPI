namespace MoviesAPI.Shared.DTOs
{
    public record HashResult
    {
        public string Hash { get; init; }
        public byte[] Salt { get; init; }
    }
}
