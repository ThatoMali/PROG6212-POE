using System;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models
{
    public class ClaimViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Claim title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Claim Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.1, 1000, ErrorMessage = "Hours worked must be between 0.1 and 1000")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(10, 500, ErrorMessage = "Hourly rate must be between R10 and R500")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Claim Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        [Display(Name = "Supporting Document")]
        public IFormFile Document { get; set; }

        // Remove these properties from the ViewModel as they are not user inputs
        // public string Status { get; set; }
        // public string FileName { get; set; }
        // public DateTime CreatedDate { get; set; }
        // public string LecturerName { get; set; }
        // public string ApprovedByName { get; set; }
        // public DateTime? ApprovalDate { get; set; }
    }
}