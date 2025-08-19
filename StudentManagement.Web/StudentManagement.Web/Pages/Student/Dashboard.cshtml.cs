using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace StudentManagement.Web.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DashboardModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int CoursesCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Request.Cookies.TryGetValue("JWToken", out var token) || string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync("https://localhost:5001/api/Students/me");
                if (!response.IsSuccessStatusCode) return RedirectToPage("/Login/Login");

                var student = await response.Content.ReadFromJsonAsync<StudentDto>();
                if (student == null) return RedirectToPage("/Login/Login");

                Id = student.Id;
                Name = student.Name;
                Email = student.Email;
                Age = student.Age;
                CoursesCount = student.CoursesCount;

                return Page();
            }
            catch
            {
                return RedirectToPage("/Login/Login");
            }
        }

        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("JWToken");
            return RedirectToPage("/Login/Login");
        }

        public class StudentDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
            public int CoursesCount { get; set; }
        }
    }
}
