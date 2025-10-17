using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE.Services
{
    public interface IFileService
    {
        Task<Document> SaveFileAsync(IFormFile file, int claimId);
        Task<(byte[] fileData, string contentType, string fileName)> GetFileAsync(int claimId);
        bool ValidateFile(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };

        public async Task<Document> SaveFileAsync(IFormFile file, int claimId)
        {
            if (!ValidateFile(file))
                return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            return new Document
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                FileData = memoryStream.ToArray(),
                ClaimId = claimId,
                UploadDate = DateTime.Now
            };
        }

        public async Task<(byte[] fileData, string contentType, string fileName)> GetFileAsync(int claimId)
        {
            // This would typically query the database
            // For now, return empty result
            return (null, null, null);
        }

        public bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                return false;

            return true;
        }
    }
}
