using MoviesAPI.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150)]
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Photo { get; set; }

        public List<ActorMovie> ActorMovies { get; set; }

        public static implicit operator ActorDTO(Actor actor)
        {
            return new ActorDTO
            {
                Id = actor.Id,
                Name = actor.Name,
                Birthday = actor.Birthday,
                Photo = actor.Photo,
            };
        }

        public static implicit operator ActorPatchDTO(Actor actor)
        {
            return new ActorPatchDTO
            {
                Name = actor.Name,
                Birthday = actor.Birthday,
            };
        }
    }
}
