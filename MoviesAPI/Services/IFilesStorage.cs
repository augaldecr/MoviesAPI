using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public interface IFilesStorage
    {
        Task<string> SaveFile(byte[] data, string extension, string container, string contentType);
        Task<string> EditFile(byte[] data, string extension, string container, string route, string contentType);
        Task DeleteFile(string route, string container);
    }
}
