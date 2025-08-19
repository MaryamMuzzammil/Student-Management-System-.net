namespace StudentWeb.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalTeachers { get; set; }   // optional
        public List<StudentViewModel> RecentStudents { get; set; }
    }
}
