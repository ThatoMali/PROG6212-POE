using System;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class ClaimViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Claim Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Supporting Document")]
        public IFormFile Document { get; set; }

        public string FileName { get; set; }
    }
}