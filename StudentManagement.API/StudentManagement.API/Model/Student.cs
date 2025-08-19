using System.ComponentModel.DataAnnotations;

namespace StudentApi.Model
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // UNIQUE
        [Required]
        public string StudentCode { get; set; }  // e.g., roll no / registration no

        [EmailAddress]
        [Required]                 // UNIQUE
        public string Email { get; set; }

        [Range(18, 60)]
        public int Age { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        // back-navigation to User (optional)
        public StudentManagement.Model.User? User { get; set; }
    }
}
