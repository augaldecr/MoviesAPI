using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record GenreCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50)]
        public string Name { get; init; }
    }
}
