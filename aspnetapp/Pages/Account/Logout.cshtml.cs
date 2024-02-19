using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class LogoutModel : LoggedInPage
    {
        public ActionResult OnGet()
        {
            if (logedIn())
                Response.Cookies.Delete("session");

            return Redirect("/");
        }
    }
}
