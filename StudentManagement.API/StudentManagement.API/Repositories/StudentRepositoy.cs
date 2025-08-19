using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentApi.Model;

namespace StudentApi.Repositories
{
    public interface IStudentRepositoy
    {
        IEnumerable<Student> GetAll();
        Student GetById(int studentId);
        Student Add(Student student);
        Student Update(Student existingStudent);
        void Delete(int id);

        (IEnumerable<Student> data, int total) Search(string? q, int? courseId, int? minAge, int? maxAge, int page, int pageSize);
        Student? GetByEmailAndCode(string email, string studentCode);
    }

    public class StudentRepositoy : IStudentRepositoy
    {
        private readonly AppDbContext context;
        public StudentRepositoy(AppDbContext context) => this.context = context;

        public IEnumerable<Student> GetAll()
            => context.Students.Include(s => s.Course).ToList();

        public Student GetById(int studentId)
            => context.Students.Include(s => s.Course).SingleOrDefault(x => x.Id == studentId);

        public Student Add(Student student)
        {
            context.Students.Add(student);
            context.SaveChanges();
            return student;
        }

        public Student Update(Student existingStudent)
        {
            if (existingStudent == null) return null;
            var student = context.Students.SingleOrDefault(x => x.Id == existingStudent.Id);
            if (student == null) return null;

            student.Name = existingStudent.Name;
            student.StudentCode = existingStudent.StudentCode;
            student.Email = existingStudent.Email;
            student.Age = existingStudent.Age;
            student.CourseId = existingStudent.CourseId;

            context.SaveChanges();
            return student;
        }

        public void Delete(int id)
        {
            var entity = context.Students.FirstOrDefault(s => s.Id == id);
            if (entity == null) return;
            context.Students.Remove(entity);
            context.SaveChanges();
        }

        public (IEnumerable<Student> data, int total) Search(string? q, int? courseId, int? minAge, int? maxAge, int page, int pageSize)
        {
            var query = context.Students.Include(s => s.Course).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(s => s.Name.Contains(q) || s.Email.Contains(q) || s.StudentCode.Contains(q));

            if (courseId.HasValue)
                query = query.Where(s => s.CourseId == courseId.Value);

            if (minAge.HasValue)
                query = query.Where(s => s.Age >= minAge.Value);

            if (maxAge.HasValue)
                query = query.Where(s => s.Age <= maxAge.Value);

            var total = query.Count();
            var data = query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (data, total);
        }

        public Student? GetByEmailAndCode(string email, string studentCode)
            => context.Students.Include(s => s.Course)
                               .FirstOrDefault(s => s.Email == email && s.StudentCode == studentCode);
    }
}
