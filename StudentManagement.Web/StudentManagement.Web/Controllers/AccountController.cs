using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentManagement.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public readonly string apiUrl;

        public LoginModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiUrl = this.configuration["APIURL"];
        }

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginData = new { Username, Password };

            var response = await httpClient.PostAsJsonAsync($"{apiUrl}/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                // Save token in cookie
                Response.Cookies.Append("JWToken", tokenResponse.token, new CookieOptions
                {
                    HttpOnly = true, // helps prevent JavaScript access to cookie
                    Secure = true, // ensures cookie is only sent over HTTPS
                    SameSite = SameSiteMode.Strict, // adjust depending on your needs
                    Expires = DateTimeOffset.UtcNow.AddHours(1) // set cookie expiration
                });
                return RedirectToPage("/Student/Index");
            }

            ErrorMessage = "Invalid login credentials.";
            return Page();
        }

        public class TokenResponse
        {
            public string token { get; set; }
        }
    }


}
