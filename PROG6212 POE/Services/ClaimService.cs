using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;
using PROG6212_POE.Models.Data;
using PROG6212_POE.Models.Data.Entities;

namespace PROG6212_POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ClaimService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Claim>> GetClaimsByLecturerAsync(int lecturerId)
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .Where(c => c.LecturerId == lecturerId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Claim>> GetPendingClaimsAsync()
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<Claim> GetClaimByIdAsync(int id)
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId)
        {
            var claim = new Claim
            {
                Title = model.Title,
                Description = model.Description,
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                Date = model.Date,
                Notes = model.Notes,
                Status = "Pending",
                LecturerId = lecturerId,
                CreatedDate = DateTime.Now
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null) return false;

            claim.Status = status;
            claim.ApprovedById = approvedById;
            claim.ApprovalDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return false;

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadDocumentAsync(int claimId, IFormFile file)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null) return false;

                var document = await _fileService.SaveFileAsync(file, claimId);
                if (document == null) return false;

                // Remove existing document if any
                var existingDoc = await _context.Documents.FirstOrDefaultAsync(d => d.ClaimId == claimId);
                if (existingDoc != null)
                {
                    _context.Documents.Remove(existingDoc);
                }

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }

        public async Task<Document> GetDocumentAsync(int claimId)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.ClaimId == claimId);
        }
    }
}