using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record UserInfo
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
