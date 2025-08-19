using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentManagement.Web.Pages.Student
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        [BindProperty]
        public StudentViewModel Student { get; set; } = new();

        public List<SelectListItem> Courses { get; set; } = new();

        public CreateModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = configuration["APIURL"]; // e.g. "https://localhost:5001/api/"
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadCourses();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (!await SetAuthorizationHeader())
                    return RedirectToPage("/Login/Login");

                var response = await httpClient.PostAsJsonAsync($"{apiUrl}Students", Student);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Student created successfully ✅";
                    return RedirectToPage("/Admin/Students");
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to create student. Details: {error}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Something went wrong: {ex.Message}";
            }

            await LoadCourses();
            return Page();
        }

        private async Task LoadCourses()
        {
            if (!await SetAuthorizationHeader())
                return;

            var response = await httpClient.GetAsync($"{apiUrl}Courses");
            if (response.IsSuccessStatusCode)
            {
                var courses = await response.Content.ReadFromJsonAsync<List<CourseViewModel>>();
                Courses = courses?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title
                }).ToList() ?? new List<SelectListItem>();
            }
        }

        private async Task<bool> SetAuthorizationHeader()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return false;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }
    }
}
