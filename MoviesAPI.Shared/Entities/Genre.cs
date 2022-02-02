using MoviesAPI.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Movie> Movies { get; set; }

        public static implicit operator GenreDTO(Genre genre) => new GenreDTO { Id = genre.Id, Name = genre.Name };
    }
}
