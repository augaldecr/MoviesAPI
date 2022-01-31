using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class LocalFileStorage : IFilesStorage
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                var fileName = Path.GetFileName(route);
                string fileLocation = Path.Combine(_env.WebRootPath, container, fileName);

                if (File.Exists(fileLocation))
                    File.Delete(fileLocation);

            }
               
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] data, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container);
            return await SaveFile(data, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] data, string extension, string container, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string directory = Path.Combine(_env.WebRootPath, container);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string route = Path.Combine(directory, fileName);
            await File.WriteAllBytesAsync(route, data);

            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            var filesUrl = Path.Combine(url, container, fileName).Replace("\\", "/");
            return filesUrl;
        }
    }
}
