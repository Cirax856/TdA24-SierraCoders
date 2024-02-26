using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetapp.Pages.Account
{
    public class IndexModel : LoggedInPage
    {
        public Models.Account Account { get; private set; }

        public ActionResult OnGet()
        {
            if (tryGetAcount(out Models.Account account))
            {
                Account = account;
                return Page();
            }
            else
                return Redirect("/account/login");
        }
    }
}
