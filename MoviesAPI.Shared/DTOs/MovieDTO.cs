using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Shared.DTOs
{
    public record MovieDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public bool OnBillboard { get; init; }
        public DateTime ReleaseDate { get; init; }
        public string Poster { get; init; }

        public static implicit operator Movie(MovieDTO movieDTO)
        {
            return new MovieDTO
            {
                Id = movieDTO.Id,
                Title = movieDTO.Title,
                OnBillboard = movieDTO.OnBillboard,
                ReleaseDate = movieDTO.ReleaseDate,
                Poster = movieDTO.Poster,
            };
        }
    }
}
