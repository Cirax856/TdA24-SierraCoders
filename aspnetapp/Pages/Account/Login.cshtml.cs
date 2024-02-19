using aspnetapp.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class LoginModel : PageModel
    {
        public string ErrorMesage { get; private set; } = null;

        public ActionResult OnGet()
        {
            if (Request.Query.TryGetValue("username", out StringValues _username) && Request.Query.TryGetValue("password", out StringValues _password))
            {
                if (AccountManager.TryLogin(_username, _password, out string sessionOrError))
                {
                    if (Request.Cookies.ContainsKey("session"))
                        Response.Cookies.Delete("session");

                    Response.Cookies.Append("session", sessionOrError, new CookieOptions()
                    {
                        Expires = new DateTimeOffset(DateTime.UtcNow.AddHours(2d)),
                    });

                    return Redirect("/");
                }
                else
                    ErrorMesage = sessionOrError;
            }
            return Page();
        }
    }
}
