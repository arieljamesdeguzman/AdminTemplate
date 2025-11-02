using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string filePath);
    }
}