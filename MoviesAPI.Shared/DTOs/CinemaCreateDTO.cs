using MoviesAPI.Shared.Entities;
using MoviesAPI.Shared.Herlpers;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record CinemaCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(120)]
        public string Name { get; init; }
        [Range(-90, 90)]
        public double Lat { get; set; }
        [Range(-180, 180)]
        public double Long { get; set; }

        public static implicit operator Cinema(CinemaCreateDTO cinemaDTO)
        {
            return new Cinema
            {
                Name = cinemaDTO.Name,
                Location = new GeometryHelper().createPoint(cinemaDTO.Long, cinemaDTO.Lat),
            };
        }
    }
}
