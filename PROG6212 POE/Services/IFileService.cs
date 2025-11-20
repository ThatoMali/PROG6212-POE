using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Services
{
    public interface IFileService
    {
        Task<Document> SaveFileAsync(IFormFile file, int claimId);
        Task<(byte[] fileData, string contentType, string fileName)> GetFileAsync(int claimId);
        bool ValidateFile(IFormFile file);
    }
}