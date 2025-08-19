using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Model;
using StudentManagement.API.Dtos;
using StudentManagement.Repositories;

namespace CourseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepositoy courseRepositoy;

        public CoursesController(ICourseRepositoy courseRepositoy)
        {
            this.courseRepositoy = courseRepositoy;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Course>> GetCourses()
        {
            var courses = courseRepositoy.GetAll();
            return Ok(courses);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Course> GetCourse(int id)
        {
            var course = courseRepositoy.GetById(id);
            if (course == null) return NotFound();
            return Ok(course);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Course> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title
            };

            var created = courseRepositoy.Add(course);
            return CreatedAtAction(nameof(GetCourse), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult UpdateCourse(int id, [FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Id = id,
                Title = dto.Title
            };

            var updated = courseRepositoy.Update(course);
            if (updated == null) return NotFound();
            return Ok(updated);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteCourse(int id)
        {
            if (courseRepositoy.HasStudents(id))
                return BadRequest("Course is assigned to one or more students and cannot be removed.");

            courseRepositoy.Delete(id);
            return NoContent();
        }
        [HttpGet("count")]
        public ActionResult<int> GetCoursesCount()
        {
            var count = courseRepositoy.GetAll().Count();
            return Ok(count);
        }

    }
}
