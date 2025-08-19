using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentManagement.Web.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        public List<CourseViewModel> Courses { get; set; } = new();

        public CoursesModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"]; // ✅ API base URL from appsettings.json
        }

        public async Task<IActionResult> OnGet()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{apiUrl}Courses");

            if (response.IsSuccessStatusCode)
            {
                Courses = await response.Content.ReadFromJsonAsync<List<CourseViewModel>>();
            }
            else
            {
                TempData["Error"] = "Failed to fetch courses.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.DeleteAsync($"{apiUrl}Courses/{id}");


            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete course. It may be assigned to students.";
            }

            return RedirectToPage();
        }

    }
}
