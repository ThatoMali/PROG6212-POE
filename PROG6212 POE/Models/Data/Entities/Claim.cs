using System;
using System.ComponentModel.DataAnnotations;

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

        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Required]
        public DateTime Date { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign keys
        public int LecturerId { get; set; }

        public int? ApprovedById { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string Notes { get; set; }

        // Navigation properties (for display purposes - will be populated by service)
        public string LecturerName { get; set; }
        public string ApprovedByName { get; set; }
    }
}