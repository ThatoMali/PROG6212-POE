using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models.Entities
{
    public class Claim
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(10, 500)]
        public decimal HourlyRate { get; set; }

        // Automated calculation
        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Required]
        public DateTime Date { get; set; }

        // Automated status management
        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        // Automated priority calculation
        public int Priority { get; set; } = 1;

        // Foreign keys
        public int LecturerId { get; set; }

        public int? ApprovedById { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string Notes { get; set; }

        // Automated workflow tracking
        public string WorkflowStage { get; set; } = "Submitted";

        public DateTime? StageUpdated { get; set; }

        // Navigation properties
        public string LecturerName { get; set; }
        public string ApprovedByName { get; set; }

        // Automated audit fields
        public int ViewCount { get; set; } = 0;
        public DateTime? LastViewed { get; set; }

        // Workflow history (not stored in database, populated when needed)
        [NotMapped]
        public List<ClaimWorkflow> WorkflowHistory { get; set; } = new List<ClaimWorkflow>();
    }

    public class ClaimWorkflow
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string Action { get; set; }
        public int PerformedById { get; set; }
        public string PerformedByName { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public string WorkflowStage { get; set; }
    }

    public class ClaimReport
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public int GeneratedById { get; set; }
        public string ReportData { get; set; }
        public byte[] ReportFile { get; set; }
        public string FileName { get; set; }
    }
}