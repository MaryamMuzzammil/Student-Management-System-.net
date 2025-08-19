using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentWeb.ViewModels;

namespace StudentWeb.Pages.Student
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public StudentViewModel StudentVM { get; set; }
        public SelectList Courses { get; set; }

        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public readonly string apiUrl;

        public CreateModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"];
        }
        public void OnGet()
        {
            LoadCourses();
        }

        public async Task<IActionResult> OnPost()
        {
            var response = await httpClient.PostAsJsonAsync(apiUrl + "/students", StudentVM);

            if (response.IsSuccessStatusCode)
            {
                var createdStudent = await response.Content.ReadFromJsonAsync<StudentViewModel>();
                Console.WriteLine($"Student created: {createdStudent.Name} (ID: {createdStudent.Id})");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
            }


            // Save student...
            return RedirectToPage("Index");
        }
        private void LoadCourses()
        {
            var courseList = new List<CourseViewModel>();
            var response = httpClient.GetFromJsonAsync<List<CourseViewModel>>(apiUrl + "/courses");
            if (response.Result != null)
                courseList = response.Result;

            Courses = new SelectList(courseList, "Id", "Title");

        }

    }
}
