namespace MoviesAPI.Shared.DTOs
{
    public record UserToken
    {
        public string Token { get; init; }
        public DateTime Expiration { get; init; }
    }
}
