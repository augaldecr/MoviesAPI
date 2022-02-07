using Microsoft.AspNetCore.Identity;
using MoviesAPI.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.Entities
{
    public class Review : IEntity
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Punctuation { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public static implicit operator ReviewDTO(Review review)
        {
            return new ReviewDTO
            {
                Id = review.Id,
                Comment = review.Comment,
                Punctuation = review.Punctuation,
                MovieId = review.MovieId,
                UserId = review.UserId,
                UserName = review.User.UserName,
            };
        }
    }
}
