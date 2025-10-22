using PROG6212_POE.Models;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IFileService _fileService;
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "lecturer1", Password = "password", Email = "lecturer1@university.com", FullName = "Dr. John Smith", Role = UserType.Lecturer },
            new User { Id = 2, Username = "coordinator1", Password = "password", Email = "coordinator1@university.com", FullName = "Prof. Sarah Johnson", Role = UserType.ProgramCoordinator },
            new User { Id = 3, Username = "manager1", Password = "password", Email = "manager1@university.com", FullName = "Dr. Michael Brown", Role = UserType.AcademicManager }
        };

        private static List<PROG6212_POE.Models.Entities.Claim> _claims = new List<PROG6212_POE.Models.Entities.Claim>
        {
            new PROG6212_POE.Models.Entities.Claim { Id = 1, Title = "Research Materials", Description = "Books and journals for research", HoursWorked = 10, HourlyRate = 150, Date = DateTime.Now.AddDays(-5), Status = "Approved", LecturerId = 1, ApprovedById = 2, ApprovalDate = DateTime.Now.AddDays(-3), Notes = "Approved for research purposes", CreatedDate = DateTime.Now.AddDays(-5) },
            new PROG6212_POE.Models.Entities.Claim { Id = 2, Title = "Conference Travel", Description = "Travel expenses for academic conference", HoursWorked = 8, HourlyRate = 120, Date = DateTime.Now.AddDays(-12), Status = "Pending", LecturerId = 1, Notes = "International conference presentation", CreatedDate = DateTime.Now.AddDays(-12) },
            new PROG6212_POE.Models.Entities.Claim { Id = 3, Title = "Software License", Description = "Annual license for statistical software", HoursWorked = 5, HourlyRate = 200, Date = DateTime.Now.AddDays(-18), Status = "Approved", LecturerId = 1, ApprovedById = 3, ApprovalDate = DateTime.Now.AddDays(-10), Notes = "Essential research tool", CreatedDate = DateTime.Now.AddDays(-18) }
        };

        private static int _nextClaimId = 4;

        public ClaimService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<List<PROG6212_POE.Models.Entities.Claim>> GetAllClaimsAsync()
        {
            var claims = _claims
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => PopulateNavigationProperties(c));
            return await Task.FromResult(claims);
        }

        public async Task<List<PROG6212_POE.Models.Entities.Claim>> GetClaimsByLecturerAsync(int lecturerId)
        {
            var claims = _claims
                .Where(c => c.LecturerId == lecturerId)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => PopulateNavigationProperties(c));
            return await Task.FromResult(claims);
        }

        public async Task<List<PROG6212_POE.Models.Entities.Claim>> GetPendingClaimsAsync()
        {
            var claims = _claims
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => PopulateNavigationProperties(c));
            return await Task.FromResult(claims);
        }

        public async Task<PROG6212_POE.Models.Entities.Claim> GetClaimByIdAsync(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                PopulateNavigationProperties(claim);
            }
            return await Task.FromResult(claim);
        }

        public async Task<PROG6212_POE.Models.Entities.Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId)
        {
            try
            {
                var claim = new PROG6212_POE.Models.Entities.Claim
                {
                    Id = _nextClaimId++,
                    Title = model.Title?.Trim() ?? "Untitled Claim",
                    Description = model.Description?.Trim(),
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Date = model.Date,
                    Notes = model.Notes?.Trim(),
                    Status = "Pending",
                    LecturerId = lecturerId,
                    CreatedDate = DateTime.Now
                };

                _claims.Add(claim);
                PopulateNavigationProperties(claim);

                return await Task.FromResult(claim);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error creating claim: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim == null) return await Task.FromResult(false);

            claim.Status = status;
            claim.ApprovedById = approvedById; // This should be int? but we're passing int
            claim.ApprovalDate = DateTime.Now;
            PopulateNavigationProperties(claim);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null) return await Task.FromResult(false);

            _claims.Remove(claim);
            return await Task.FromResult(true);
        }

        public async Task<bool> UploadDocumentAsync(int claimId, IFormFile file)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.Id == claimId);
                if (claim == null) return await Task.FromResult(false);

                var document = await _fileService.SaveFileAsync(file, claimId);
                return await Task.FromResult(document != null);
            }
            catch (Exception ex)
            {
                // Log error
                return await Task.FromResult(false);
            }
        }

        public async Task<Document> GetDocumentAsync(int claimId)
        {
            // Use FileService to get the document
            var result = await _fileService.GetFileAsync(claimId);
            if (result.fileData == null)
                return null;

            return new Document
            {
                FileName = result.fileName,
                ContentType = result.contentType,
                FileData = result.fileData,
                FileSize = result.fileData.Length,
                ClaimId = claimId
            };
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        private PROG6212_POE.Models.Entities.Claim PopulateNavigationProperties(PROG6212_POE.Models.Entities.Claim claim)
        {
            var lecturer = _users.FirstOrDefault(u => u.Id == claim.LecturerId);
            claim.LecturerName = lecturer?.FullName ?? "Unknown Lecturer";

            if (claim.ApprovedById.HasValue)
            {
                var approver = _users.FirstOrDefault(u => u.Id == claim.ApprovedById.Value);
                claim.ApprovedByName = approver?.FullName ?? "Unknown Approver";
            }

            return claim;
        }
    }
}