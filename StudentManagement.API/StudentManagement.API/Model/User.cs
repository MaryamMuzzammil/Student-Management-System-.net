using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Student";

        // 🔗 Link to Student (nullable)
        public int? StudentId { get; set; }

        // navigation property (optional)
        public StudentApi.Model.Student? Student { get; set; }
    }
}
