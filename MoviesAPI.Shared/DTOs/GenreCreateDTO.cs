using MoviesAPI.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record GenreCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50)]
        public string Name { get; init; }

        public static implicit operator Genre(GenreCreateDTO genreDTO) => new Genre { Name = genreDTO.Name };
    }
}
