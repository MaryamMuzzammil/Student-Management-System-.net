using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace StudentManagement.Web.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public LoginModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiUrl = _configuration["APIURL"];
        }

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var loginData = new
            {
                Username, // Username ya Email dono bhej sakte ho
                Password
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                    if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.token))
                    {
                        ErrorMessage = "Invalid server response.";
                        return Page();
                    }

                    // ✅ Save JWT token in cookie
                    Response.Cookies.Append("JWToken", tokenResponse.token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // Local dev = false, Production = true
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    // ✅ Redirect based on role
                    if (tokenResponse.role == "Admin")
                        return RedirectToPage("/Admin/Dashboard");
                    else
                        return RedirectToPage("/Student/Dashboard");
                }

                // ❌ Agar login fail ho
                var errorMsg = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Login failed: {errorMsg}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Something went wrong: {ex.Message}";
                return Page();
            }
        }

        public class TokenResponse
        {
            public string token { get; set; }
            public string role { get; set; }
            public string username { get; set; }
        }
    }
}
