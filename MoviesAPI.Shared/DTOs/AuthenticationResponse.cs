namespace MoviesAPI.Shared.DTOs
{
    public record AuthenticationResponse
    {
        public string Token { get; init; }
        public DateTime Expiration { get; init; }
    }
}
