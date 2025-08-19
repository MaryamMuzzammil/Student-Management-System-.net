using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace StudentManagement.Pages.Login
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Delete JWT cookie
            Response.Cookies.Delete("JWToken");

            // Sign out if using authentication middleware
            await HttpContext.SignOutAsync();

            // Redirect to login page
            return RedirectToPage("/Login/Login");
        }

        // Optional: handle GET requests so page doesn't hang if navigated directly
        public IActionResult OnGet()
        {
            return RedirectToPage("/Login/Login");
        }
    }
}
