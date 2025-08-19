namespace StudentManagement.Dtos
{
    public class SaveStudentDto
    {
        public string Name { get; set; }
        public string StudentCode { get; set; }   // NEW
        public string Email { get; set; }
        public int Age { get; set; }
        public int CourseId { get; set; }
    }
}
