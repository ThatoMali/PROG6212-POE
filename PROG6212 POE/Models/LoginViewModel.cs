using System.ComponentModel.DataAnnotations;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Role")]
        public UserType Role { get; set; }
    }
}