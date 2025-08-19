using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentWeb.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        // API me "name" -> Model me "Name"
        public string Name { get; set; }

        // API me "studentCode" -> Model me "StudentCode"
        public string StudentCode { get; set; }

        // API me "email" -> Model me "Email"
        public string Email { get; set; }

        // API me "age" -> Model me "Age"
        public int Age { get; set; }

        // API me "courseId" -> Model me "CourseId"
        public int CourseId { get; set; }

        // Extra field (sirf UI me dikhane ke liye, API me bhejni nahi)
        [BindNever]
        public string CourseTitle { get; set; }
    }
}
