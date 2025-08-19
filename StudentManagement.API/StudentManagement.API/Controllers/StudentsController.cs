using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Model;
using StudentApi.Repositories;
using StudentManagement.Dtos;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepositoy studentRepositoy;

        public StudentsController(IStudentRepositoy studentRepositoy)
        {
            this.studentRepositoy = studentRepositoy;
        }

        // Admin: search + pagination
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetStudents([FromQuery] string? q, [FromQuery] int? courseId,
                                         [FromQuery] int? minAge, [FromQuery] int? maxAge,
                                         [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

            var (data, total) = studentRepositoy.Search(q, courseId, minAge, maxAge, page, pageSize);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(data);
        }

        // Admin or Student: get by id
        [Authorize(Roles = "Admin,Student")]
        [HttpGet("{id:int}")]
        public ActionResult<Student> GetStudent(int id)
        {
            if (User.IsInRole("Student"))
            {
                var idClaim = User.FindFirst("studentId")?.Value;
                if (idClaim == null || int.Parse(idClaim) != id)
                    return Forbid();
            }

            var student = studentRepositoy.GetById(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        // Student: own profile
        [Authorize(Roles = "Student")]
        [HttpGet("me")]
        public ActionResult<Student> GetMe()
        {
            var idClaim = User.FindFirst("studentId")?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int studentId))
                return Unauthorized();

            var student = studentRepositoy.GetById(studentId);
            if (student == null) return NotFound();

            return Ok(student);
        }

        // Admin: create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Student> PostStudent([FromBody] SaveStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentModel = new Student
            {
                Name = dto.Name,
                StudentCode = dto.StudentCode,
                Age = dto.Age,
                CourseId = dto.CourseId,
                Email = dto.Email
            };

            var created = studentRepositoy.Add(studentModel);
            return CreatedAtAction(nameof(GetStudent), new { id = created.Id }, created);
        }

        // Admin: update
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult UpdateStudent(int id, [FromBody] SaveStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = studentRepositoy.GetById(id);
            if (existing == null)
                return NotFound();

            // Update only allowed fields
            existing.Name = dto.Name;
            existing.StudentCode = dto.StudentCode;
            existing.Email = dto.Email;
            existing.Age = dto.Age;
            existing.CourseId = dto.CourseId;

            var updated = studentRepositoy.Update(existing);
            return Ok(updated);
        }

        // Admin: delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = studentRepositoy.GetById(id);
            if (student == null) return NotFound();

            studentRepositoy.Delete(id);
            return NoContent();
        }
        [HttpGet("count")]
        public ActionResult<int> GetStudentsCount([FromServices] IStudentRepositoy repo)
        {
            var count = repo.GetAll().Count();
            return Ok(count);
        }

    }
}
