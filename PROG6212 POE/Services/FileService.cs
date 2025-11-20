using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Services
{
    public class FileService : IFileService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };

        // Simple in-memory storage
        private static List<Document> _documents = new List<Document>();
        private static int _nextDocumentId = 1;

        public async Task<Document> SaveFileAsync(IFormFile file, int claimId)
        {
            if (file == null || !ValidateFile(file))
                return null;

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var document = new Document
                {
                    Id = _nextDocumentId++,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    FileData = memoryStream.ToArray(),
                    ClaimId = claimId,
                    UploadDate = DateTime.Now
                };

                // Remove existing document for this claim if any
                var existingDoc = _documents.FirstOrDefault(d => d.ClaimId == claimId);
                if (existingDoc != null)
                {
                    _documents.Remove(existingDoc);
                }

                _documents.Add(document);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(byte[] fileData, string contentType, string fileName)> GetFileAsync(int claimId)
        {
            var document = _documents.FirstOrDefault(d => d.ClaimId == claimId);
            if (document == null)
                return (null, null, null);

            return await Task.FromResult((document.FileData, document.ContentType, document.FileName));
        }

        public bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                return false;

            return true;
        }
    }
}