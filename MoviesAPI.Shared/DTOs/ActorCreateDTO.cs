using MoviesAPI.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record ActorCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150)]
        public string Name { get; init; }
        public DateTime Birthday { get; init; }

        public static implicit operator Actor(ActorCreateDTO actorDTO)
        {
            return new Actor
            {
                Name = actorDTO.Name,
                Birthday = actorDTO.Birthday,
            };
        }
    }
}
