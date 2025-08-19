using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentManagement.Web.Pages.Student
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        [BindProperty]
        public StudentViewModel Student { get; set; } = new();

        public List<SelectListItem> Courses { get; set; } = new();

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = configuration["APIURL"];
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login/Login");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{apiUrl}Students/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToPage("/Admin/Students");

            Student = await response.Content.ReadFromJsonAsync<StudentViewModel>();

            await LoadCourses();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var token = Request.Cookies["JWToken"];
                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Login/Login");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.PutAsJsonAsync($"{apiUrl}Students/{Student.Id}", Student);

                if (response.IsSuccessStatusCode)
                    return RedirectToPage("/Admin/Students");

                TempData["Error"] = "Failed to update student.";
                await LoadCourses();
                return Page();
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                await LoadCourses();
                return Page();
            }
        }

        private async Task LoadCourses()
        {
            var token = Request.Cookies["JWToken"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{apiUrl}Courses");
            if (response.IsSuccessStatusCode)
            {
                var courses = await response.Content.ReadFromJsonAsync<List<CourseViewModel>>();
                Courses = courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }).ToList();
            }
        }
    }
}
