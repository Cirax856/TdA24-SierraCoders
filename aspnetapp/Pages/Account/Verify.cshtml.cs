using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class VerifyModel : PageModel
    {
        public string AccountName { get; private set; }

        public ActionResult OnGet()
        {
            if (!Request.Query.TryGetValue("id", out StringValues _id) || !Database.emailVerifications.TryGetValue(_id.ToString(), out uint accountId))
                return Content("Invalid verification id");
            else
            {
                Database.emailVerifications.Remove(_id.ToString());
                Database.acounts[accountId].Verified = true;
                Database.Save();

                AccountName = Database.acounts[accountId].Username;

                return Page();
            }
        }
    }
}
