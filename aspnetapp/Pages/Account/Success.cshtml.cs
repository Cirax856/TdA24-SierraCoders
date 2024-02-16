using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class SuccessModel : PageModel
    {
        public string Email { get; private set; } = string.Empty;

        public ActionResult OnGet()
        {
            if (Request.Query.TryGetValue("Email", out StringValues _email))
                Email = _email;

            return Page();
        }
    }
}
