using System;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models.Entities
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public byte[] FileData { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Foreign key
        public int ClaimId { get; set; }
    }
}