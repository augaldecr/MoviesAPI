using MoviesAPI.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(300)]
        public string Title { get; set; }
        public bool OnBillboard { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }

        public static implicit operator MovieDTO(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                OnBillboard = movie.OnBillboard,
                ReleaseDate = movie.ReleaseDate,
                Poster = movie.Poster,
            };
        }

        public static implicit operator MoviePatchDTO(Movie movie)
        {
            return new MoviePatchDTO
            {
                Title = movie.Title,
                OnBillboard = movie.OnBillboard,
                ReleaseDate = movie.ReleaseDate,
            };
        }
    }
}
