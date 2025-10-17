using System.ComponentModel.DataAnnotations;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE.Models.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public UserRole Role { get; set; }

        // Navigation properties
        public List<Claim> SubmittedClaims { get; set; } = new List<Claim>();
        public List<Claim> ApprovedClaims { get; set; } = new List<Claim>();
    }
}
