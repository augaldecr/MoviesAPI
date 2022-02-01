using Microsoft.AspNetCore.Http;
using MoviesAPI.Shared.Entities;
using MoviesAPI.Shared.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static implicit operator Movie(MovieCreateDTO movieDTO)
        {
            return new Movie
            {
                Title = movieDTO.Title,
                OnBillboard = movieDTO.OnBillboard,
                ReleaseDate = movieDTO.ReleaseDate,
            };
        }
    }
}
