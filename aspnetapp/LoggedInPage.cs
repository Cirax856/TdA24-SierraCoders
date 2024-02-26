using aspnetapp.Auth;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.Ocsp;

namespace aspnetapp
{
    public class LoggedInPage : PageModel
    {
        public bool logedIn()
            => Request.Cookies.TryGetValue("session", out string session) && AccountManager.TryGetAcount(session, out _);

        public bool tryGetAcount(out Account acount)
        {
            acount = null;
            return Request.Cookies.TryGetValue("session", out string session) && AccountManager.TryGetAcount(session, out acount);
        }
    }
}
