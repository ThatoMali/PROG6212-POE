using System;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Required]
        public int ClaimId { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string Status { get; set; } = "Generated";

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }

        // Navigation properties
        public string ClaimTitle { get; set; }
        public string LecturerName { get; set; }
    }
}