using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentManagement.Web.Pages.Admin
{
    public class EditCourseModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        [BindProperty]
        public CourseViewModel Course { get; set; } = new();

        public EditCourseModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"];
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{apiUrl}Courses/{id}");

            if (response.IsSuccessStatusCode)
            {
                Course = await response.Content.ReadFromJsonAsync<CourseViewModel>();
                return Page();
            }

            return RedirectToPage("/Admin/Courses");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PutAsJsonAsync($"{apiUrl}Courses/{Course.Id}", Course);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Courses");
            }

            TempData["Error"] = "Failed to update course.";
            return Page();
        }
    }
}
