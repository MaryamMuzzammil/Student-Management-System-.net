using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentWeb.ViewModels;
using System.Net.Http.Headers;

namespace StudentWeb.Pages.Student
{
    public class IndexModel : AuthenticatedPageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiUrl;

        public List<StudentViewModel> StudentsVms { get; set; } = new();

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
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
                    return RedirectToPage("/Login/Login");

                // Add JWT token in headers
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // ✅ Correct endpoint to match backend: /api/Students
                var response = await httpClient.GetAsync($"{apiUrl}Students");

                if (response.IsSuccessStatusCode)
                {
                    StudentsVms = await response.Content.ReadFromJsonAsync<List<StudentViewModel>>();
                }
                else
                {
                    TempData["Error"] = "Failed to retrieve student data.";
                }

                return Page();
            }
            catch (Exception)
            {
                StudentsVms = new List<StudentViewModel>(); // fallback in case of error
                TempData["Error"] = "Something went wrong while fetching data.";
                return Page();
            }
        }
    }
}
