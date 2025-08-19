using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentWeb.Pages
{
    public class AuthenticatedPageModel : PageModel
    {
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["JWToken"];

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToPageResult("/Login/Login");
            }

            base.OnPageHandlerExecuting(context);
        }
    }
}
