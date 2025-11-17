using System.Collections.Generic;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Models
{
    public class DashboardViewModel
    {
        public UserType UserRole { get; set; }
        public int TotalClaims { get; set; }
        public int PendingApproval { get; set; }
        public int Approved { get; set; }
        public decimal MonthlyTotal { get; set; }
        public double AverageProcessingTime { get; set; }
        public List<Claim> RecentClaims { get; set; }
        public List<Claim> HighPriorityClaims { get; set; }
    }

    // New ViewModel for validation results
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    // New ViewModel for statistics
    public class DashboardStatistics
    {
        public int TotalClaims { get; set; }
        public int PendingClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public decimal MonthlyTotal { get; set; }
        public double AverageProcessingTime { get; set; }
        public List<Claim> RecentClaims { get; set; }
        public List<Claim> HighPriorityClaims { get; set; }
    }

    // New ViewModel for bulk operations
    public class BulkOperationResult
    {
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}