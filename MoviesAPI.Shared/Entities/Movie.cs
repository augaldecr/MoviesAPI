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

        public List<ActorMovie> ActorMovies { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Cinema> Cinemas { get; set; }

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

        public static implicit operator MovieDetailsDTO(Movie movie)
        {
            return new MovieDetailsDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                OnBillboard = movie.OnBillboard,
                ReleaseDate = movie.ReleaseDate,
                Poster = movie.Poster,
                Actors = movie.ActorMovies is null ? 
                    new List<ActorMovieDetailsDTO>().ToArray() :
                    movie.ActorMovies.OrderBy(a => a.Order).Select( a => new ActorMovieDetailsDTO
                    {
                        ActorId = a.ActorId,
                        Character = a.Character,
                        PersonName = a.Actor.Name
                    }).ToArray(),
                Genres = movie.Genres.Select<Genre, GenreDTO>(x => x).ToArray(),
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
