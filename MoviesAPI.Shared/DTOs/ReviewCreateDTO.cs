using MoviesAPI.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record ReviewCreateDTO
    {
        public string Comment { get; init; }
        [Range(1, 5)]
        public int Punctuation { get; init; }
        public string UserId { get; init; }

        public static implicit operator Review(ReviewCreateDTO reviewCreateDTO)
        {
            return new Review
            {
                Comment = reviewCreateDTO.Comment,
                UserId = reviewCreateDTO.UserId,
                Punctuation = reviewCreateDTO.Punctuation,
            };
        }
    }
}
