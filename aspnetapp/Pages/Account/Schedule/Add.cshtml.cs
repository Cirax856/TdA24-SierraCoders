using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetapp.Pages.Account.Schedule
{
    public class AddModel : LoggedInPage
    {
        public Models.Account Account { get; private set; }

        public ActionResult OnGet()
        {
            if (tryGetAcount(out Models.Account account))
            {
                if (!account.HasLecturer)
                    return Redirect("/account/lecturer");

                Account = account;



                return Page();
            }
            else
                return Redirect("/account/login");
        }
    }
}
