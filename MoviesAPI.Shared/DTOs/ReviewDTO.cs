using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Shared.DTOs
{
    public record ReviewDTO
    {
        public int Id { get; init; }
        public string Comment { get; init; }
        public int Punctuation { get; init; }
        public int MovieId { get; init; }
        public string UserId { get; init; }
        public string UserName { get; init; }

        public static implicit operator Review(ReviewDTO reviewDTO)
        {
            return new Review
            {
                Id = reviewDTO.Id,
                Comment = reviewDTO.Comment,
                UserId = reviewDTO.UserId,
                Punctuation = reviewDTO.Punctuation,
                MovieId = reviewDTO.MovieId,
            };
        }
    }
}
