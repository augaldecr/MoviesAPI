using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Shared.DTOs
{
    public record CinemaDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public static implicit operator Cinema(CinemaDTO cinemaDTO)
        {
            return new Cinema
            {
                Id = cinemaDTO.Id,
                Name = cinemaDTO.Name,
            };
        }
    }
}
