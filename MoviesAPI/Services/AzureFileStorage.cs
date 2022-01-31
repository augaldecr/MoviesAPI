using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class AzureFileStorage : IFilesStorage
    {
        private readonly string connectionString;

        public AzureFileStorage(IConfiguration configuration)
        {
            //connectionString = configuration.GetConnectionString("AzureStorage");
            connectionString = configuration["AzureStorage"];
        }
        public async Task DeleteFile(string route, string container)
        {
            if (string.IsNullOrEmpty(route))
                return;

            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            var file = Path.GetFileName(route);
            var blob = client.GetBlobClient(file);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(byte[] data, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container); 
            return await SaveFile(data, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] data, string extension, string container, string contentType)
        {
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(PublicAccessType.Blob);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = contentType;
            blobUploadOptions.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(new BinaryData(data), blobUploadOptions);
            return blob.Uri.ToString();
        }
    }
}
