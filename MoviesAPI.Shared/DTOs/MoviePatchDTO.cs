using MoviesAPI.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.DTOs
{
    public class MoviePatchDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(300)]
        public string Title { get; init; }
        public bool OnBillboard { get; init; }
        public DateTime ReleaseDate { get; init; }

        public static implicit operator Movie(MoviePatchDTO movieDTO)
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
