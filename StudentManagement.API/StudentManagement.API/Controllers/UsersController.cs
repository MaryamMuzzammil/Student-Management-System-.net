using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;

namespace StudentManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Student")]
        [HttpPost("link/{userId:int}/{studentId:int}")]
        public async Task<IActionResult> LinkUserToStudent(int userId, int studentId)
        {
            var user = await _context.Users.FindAsync(userId);
            var student = await _context.Students.FindAsync(studentId);
            if (user == null || student == null) return NotFound();

            // Ensure Student only links themselves
            if (User.IsInRole("Student"))
            {
                var idClaim = User.FindFirst("studentId")?.Value;
                if (idClaim == null || int.Parse(idClaim) != studentId)
                    return Forbid(); // Student can only link to their own profile
            }

            user.StudentId = studentId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Linked", userId = user.Id, studentId = studentId });
        }

    }
}
