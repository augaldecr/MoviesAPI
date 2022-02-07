namespace MoviesAPI.Shared.DTOs
{
    public record EditRoleDTO
    {
        public string UserId { get; init; }
        public string RoleName { get; init; }
    }
}
