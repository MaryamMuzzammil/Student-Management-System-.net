using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentManagement.Web.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }

        public DashboardModel(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiUrl = _config["APIURL"] ?? "";
        }

        public async Task OnGet()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token)) return;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            try
            {
                // run both in parallel
                var studentsTask = GetCountAsync("Students");
                var coursesTask = GetCountAsync("Courses");

                await Task.WhenAll(studentsTask, coursesTask);

                TotalStudents = studentsTask.Result;
                TotalCourses = coursesTask.Result;
            }
            catch
            {
                TotalStudents = 0;
                TotalCourses = 0;
            }
        }

        private async Task<int> GetCountAsync(string resource)
        {
            // 1) Try /{resource}/count if API supports it
            var countUrl = Combine(_apiUrl, $"{resource}/count");
            try
            {
                var r1 = await _httpClient.GetAsync(countUrl);
                if (r1.IsSuccessStatusCode)
                {
                    // server should return a plain number or JSON number
                    var n = await r1.Content.ReadFromJsonAsync<int>();
                    return n;
                }
            }
            catch { /* ignore and fallback */ }

            // 2) Fallback: GET list and count array length
            var listUrl = Combine(_apiUrl, resource);
            try
            {
                var r2 = await _httpClient.GetAsync(listUrl);
                if (r2.IsSuccessStatusCode)
                {
                    using var stream = await r2.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(stream);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        return doc.RootElement.GetArrayLength();
                }
            }
            catch { /* ignore */ }

            return 0;
        }

        private static string Combine(string baseUrl, string path)
        {
            baseUrl = (baseUrl ?? "").TrimEnd('/');
            path = (path ?? "").TrimStart('/');
            return $"{baseUrl}/{path}";
        }
    }
}