using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetapp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
            Log.Info("reg");
        }
    }
}
