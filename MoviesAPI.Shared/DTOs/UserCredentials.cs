using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record UserCredentials
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress]
        public string Email { get; init; }
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}
