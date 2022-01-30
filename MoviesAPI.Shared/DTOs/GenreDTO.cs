using MoviesAPI.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record GenreDTO
    {
        public int Id { get; init; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50)]
        public string Name { get; init; }

        public static implicit operator Genre(GenreDTO genreDTO) => new Genre { Id = genreDTO.Id, Name = genreDTO.Name };
    }
}
