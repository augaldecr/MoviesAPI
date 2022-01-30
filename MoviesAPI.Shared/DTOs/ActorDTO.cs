using MoviesAPI.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record ActorDTO
    {
        public int Id { get; init; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150)]
        public string Name { get; init; }
        public DateTime Birthday { get; init; }
        public string Photo { get; init; }

        public static implicit operator Actor(ActorDTO actorDTO)
        {
            return new Actor
            {
                Id = actorDTO.Id,
                Name = actorDTO.Name,
                Birthday = actorDTO.Birthday,
                Photo = actorDTO.Photo,
            };
        }
    }
}
