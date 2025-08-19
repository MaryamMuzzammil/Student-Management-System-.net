using StudentManagement.Data;
using StudentApi.Model;

namespace StudentManagement.Repositories
{
    public interface ICourseRepositoy
    {
        IEnumerable<Course> GetAll();
        Course GetById(int courseId);
        Course Add(Course course);
        Course Update(Course existingCourse);
        void Delete(int id);
        bool HasStudents(int courseId);
    }

    public class CourseRepositoy : ICourseRepositoy
    {
        private readonly AppDbContext context;

        public CourseRepositoy(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Course> GetAll() => context.Courses.ToList();

        public Course GetById(int courseId)
            => context.Courses.SingleOrDefault(x => x.Id == courseId);

        public Course Add(Course course)
        {
            context.Courses.Add(course);
            context.SaveChanges();
            return course;
        }

        public Course Update(Course existingCourse)
        {
            if (existingCourse == null) return null;
            var course = context.Courses.SingleOrDefault(x => x.Id == existingCourse.Id);
            if (course == null) return null;

            course.Title = existingCourse.Title;
            context.SaveChanges();
            return course;
        }

        public void Delete(int id)
        {
            var toDelete = context.Courses.FirstOrDefault(c => c.Id == id);
            if (toDelete == null) return;
            context.Courses.Remove(toDelete);
            context.SaveChanges();
        }

        public bool HasStudents(int courseId)
        {
            return context.Students.Any(s => s.CourseId == courseId);
        }
    }
}
