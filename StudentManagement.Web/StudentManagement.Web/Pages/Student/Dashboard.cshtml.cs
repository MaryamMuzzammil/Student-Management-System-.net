using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StudentManagement.Web.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string CourseTitle { get; set; } = string.Empty;

        public DashboardModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"];
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var token = Request.Cookies["JWToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Login/Login");
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{apiUrl}Students/me");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Step 1 → Normal mapping
                    var student = JsonSerializer.Deserialize<StudentViewModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Step 2 → Extract nested course.title
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("course", out var courseElement) &&
                        courseElement.TryGetProperty("title", out var titleElement))
                    {
                        student.CourseTitle = titleElement.GetString() ?? "Not Assigned";
                    }

                    if (student != null)
                    {
                        Name = student.Name;
                        Email = student.Email;
                        Age = student.Age;
                        CourseTitle = student.CourseTitle;
                    }
                }
                else
                {
                    TempData["Error"] = "Failed to fetch student data.";
                }

                return Page();
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while fetching data.";
                return Page();
            }
        }

        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("JWToken");
            return RedirectToPage("/Login/Login");
        }
    }
}
