using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentManagement.Web.Pages.Admin
{
    public class CreateCourseModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        [BindProperty]
        public CourseViewModel Course { get; set; } = new();

        public CreateCourseModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"];
        }

        public IActionResult OnGet()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync($"{apiUrl}Courses", Course);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Courses");
            }

            TempData["Error"] = "Failed to create course.";
            return Page();
        }
    }
}
