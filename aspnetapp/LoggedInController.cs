using aspnetapp.Auth;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp
{
    public abstract class LoggedInController : ControllerBase
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
