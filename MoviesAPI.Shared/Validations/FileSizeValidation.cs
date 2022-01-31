using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int _maxFileSizeMB;

        public FileSizeValidation(int maxFileSizeMB)
        {
            _maxFileSizeMB = maxFileSizeMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile is null)
                return ValidationResult.Success;

            if (formFile.Length > _maxFileSizeMB * 1024 * 1024)
                return new ValidationResult($"The file size can't superior than {_maxFileSizeMB} MB.");
            else
                return ValidationResult.Success;
        }
    }
}
