using MoviesAPI.Shared.DTOs;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.Entities
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(120)]
        public string Name { get; set; }

        public Point Location { get; set; }
        public ICollection<Movie> Movies { get; set; }

        public static implicit operator CinemaDTO(Cinema cinema)
        {
            return new CinemaDTO
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Lat = cinema.Location is null ? 0 : cinema.Location.Y,
                Long = cinema.Location is null ? 0 : cinema.Location.X,
            };
        }
    }
}
