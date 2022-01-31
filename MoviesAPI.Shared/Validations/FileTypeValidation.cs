using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(FileTypesGroups fileType)
        {
            if (fileType == FileTypesGroups.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/png", "image/gif", };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile is null)
                return ValidationResult.Success;

            if (!validTypes.Contains(formFile.ContentType))
                return new ValidationResult($"The file type must be one of the following: {string.Join(", ", validTypes)}");
            else
                return ValidationResult.Success;
        }
    }
}
