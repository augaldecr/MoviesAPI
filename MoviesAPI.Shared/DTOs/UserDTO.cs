namespace MoviesAPI.Shared.DTOs
{
    public record UserDTO
    {
        public string Id { get; init; }
        public string Email { get; init; }
    }
}
