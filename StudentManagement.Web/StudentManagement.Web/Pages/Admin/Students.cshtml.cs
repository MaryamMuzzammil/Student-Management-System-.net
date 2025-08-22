using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StudentManagement.Web.Pages.Admin
{
    public class StudentsModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        public List<StudentViewModel> StudentsVms { get; set; } = new();

        public StudentsModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"]; // ✅ appsettings.json se API base URL
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var token = Request.Cookies["JWToken"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Login/Login");

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // API call
                var response = await httpClient.GetAsync($"{apiUrl}Students");

                if (response.IsSuccessStatusCode)
                {
                    // Step 1: API ka raw JSON read kar lo
                    var jsonData = await response.Content.ReadFromJsonAsync<JsonElement>();

                    // Step 2: JSON loop karke StudentViewModel me manually map karna
                    StudentsVms = jsonData.EnumerateArray()
                        .Select(s => new StudentViewModel
                        {
                            Id = s.GetProperty("id").GetInt32(),
                            Name = s.GetProperty("name").GetString(),
                            StudentCode = s.GetProperty("studentCode").GetString(),
                            Email = s.GetProperty("email").GetString(),
                            Age = s.GetProperty("age").GetInt32(),
                            CourseId = s.GetProperty("courseId").GetInt32(),
                            CourseTitle = s.TryGetProperty("course", out var course)
                                          ? course.GetProperty("title").GetString()
                                          : string.Empty
                        }).ToList();
                }
                else
                {
                    TempData["Error"] = "Failed to retrieve student data.";
                }

                return Page();
            }
            catch (Exception)
            {
                StudentsVms = new List<StudentViewModel>();
                TempData["Error"] = "Something went wrong while fetching data.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            try
            {
                var token = Request.Cookies["JWToken"];
                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Login/Login");

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync($"{apiUrl}Students/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to delete student.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while deleting student.";
            }

            return RedirectToPage();
        }

    }
}
