using PROG6212_POE.Models;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Services
{
    public interface IClaimService
    {
        Task<List<PROG6212_POE.Models.Entities.Claim>> GetAllClaimsAsync();
        Task<List<PROG6212_POE.Models.Entities.Claim>> GetClaimsByLecturerAsync(int lecturerId);
        Task<List<PROG6212_POE.Models.Entities.Claim>> GetPendingClaimsAsync();
        Task<PROG6212_POE.Models.Entities.Claim> GetClaimByIdAsync(int id);
        Task<PROG6212_POE.Models.Entities.Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId);
        Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById);
        Task<bool> DeleteClaimAsync(int id);
        Task<bool> UploadDocumentAsync(int claimId, IFormFile file);
        Task<Document> GetDocumentAsync(int claimId);

        // Add the missing methods
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int id);
    }
}