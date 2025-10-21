using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models.Entities
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

        // Changed from UserRole to UserType with correct enum values
        public UserType Role { get; set; }
    }

    public enum UserType
    {
        Lecturer,  // Fixed from "lecture" to "Lecturer"
        ProgramCoordinator,
        AcademicManager
    }
}