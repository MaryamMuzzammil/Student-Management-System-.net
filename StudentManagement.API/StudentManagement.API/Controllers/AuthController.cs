using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Services;
using StudentManagement.Data;
using StudentManagement.Model;
using StudentManagement.API.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(ITokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userDto.Username || u.Email == userDto.Username);

            if (user == null)
                return Unauthorized("Invalid username/email or password");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid username/email or password");

            // If this user is linked to a Student, include studentId claim
            Dictionary<string, string>? extraClaims = null;
            if (user.StudentId.HasValue)
            {
                extraClaims = new Dictionary<string, string>
                {
                    { "studentId", user.StudentId.Value.ToString() }
                };
            }

            var token = extraClaims != null
                ? _tokenService.CreateToken(user.Username, user.Role, extraClaims)
                : _tokenService.CreateToken(user.Username, user.Role);

            return Ok(new
            {
                token,
                role = user.Role,
                username = user.Username,
                studentId = user.StudentId
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Username == dto.Username);

            if (existingUser != null)
                return Conflict("User with this email/username already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = hashedPassword,
                Role = "Student" // default
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Try to link existing Student (if any) that has same email
            var sameEmailStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == dto.Email);

            if (sameEmailStudent != null)
            {
                newUser.StudentId = sameEmailStudent.Id;
                _context.Users.Update(newUser);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "User registered successfully",
                studentLinked = sameEmailStudent != null,
                studentId = newUser.StudentId
            });
        }

        [HttpPost("student-login")]
        [AllowAnonymous]
        public async Task<IActionResult> StudentLogin([FromBody] StudentLoginDto dto)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == dto.Email && s.StudentCode == dto.StudentCode);

            if (student == null)
                return Unauthorized("Invalid student credentials");

            var extraClaims = new Dictionary<string, string>
            {
                { "studentId", student.Id.ToString() }
            };

            var token = _tokenService.CreateToken(student.Name, "Student", extraClaims);

            return Ok(new
            {
                token,
                name = student.Name,
                studentId = student.Id
            });
        }
    }
}
