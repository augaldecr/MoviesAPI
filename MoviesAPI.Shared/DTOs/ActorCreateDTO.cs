using Microsoft.AspNetCore.Http;
using MoviesAPI.Shared.Entities;
using MoviesAPI.Shared.Validations;

namespace MoviesAPI.Shared.DTOs
{
    public record ActorCreateDTO : ActorPatchDTO
    {
        [FileSizeValidation(4)]
        [FileTypeValidation(FileTypesGroups.Image)]
        public IFormFile Photo { get; set; }

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
