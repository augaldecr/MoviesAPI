using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Shared.Entities;
using MoviesAPI.Shared.Helpers;
using MoviesAPI.Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record MovieCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(300)]
        public string Title { get; init; }
        public bool OnBillboard { get; init; }
        public DateTime ReleaseDate { get; init; }
        [FileSizeValidation(4)]
        [FileTypeValidation(FileTypesGroups.Image)]
        public IFormFile Poster { get; init; }

        [ModelBinder(BinderType = typeof(TypeBinder<ICollection<int>>))]
        public ICollection<int> GenresIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<ICollection<ActorMovieCreateDTO>>))]
        public ICollection<ActorMovieCreateDTO> Actors { get; set; }

        public static implicit operator Movie(MovieCreateDTO movieDTO)
        {
            return new Movie
            {
                Title = movieDTO.Title,
                OnBillboard = movieDTO.OnBillboard,
                ReleaseDate = movieDTO.ReleaseDate,
                ActorMovies = movieDTO.Actors is null ? 
                               new List<ActorMovie>() :
                               movieDTO.Actors.Select(a => new ActorMovie 
                                                           { 
                                                                ActorId = a.ActorId, 
                                                                Character = a.Character 
                                                            }
                               ).ToList(),
            };
        }
    }
}
