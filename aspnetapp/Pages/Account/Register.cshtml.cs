using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public string Error { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string Password2 { get; private set; } = string.Empty;

        public ActionResult OnGet()
        {
            if (Request.Query.TryGetValue("username", out StringValues _username))
                Username = _username;
            if (Request.Query.TryGetValue("email", out StringValues _email))
                Email = _email;
            if (Request.Query.TryGetValue("password", out StringValues _password))
                Password = _password;
            if (Request.Query.TryGetValue("password2", out StringValues _password2))
                Password2 = _password2;

            if (Request.Query.TryGetValue("error", out StringValues _error))
            {
                Error = _error;
            }
            else if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Password2))
            {
                if (!Utils.Verify(Username, "Username", out string error, 4, 30))
                    return Redirect(getUrl(error));
                else if (!Utils.Verify(Email, "Email", out error, 4, 60, 1))
                    return Redirect(getUrl(error));
                else if (!Utils.Verify(Password, "Password", out error, 8, 60))
                    return Redirect(getUrl(error));
                else if (Password != Password2)
                    return Redirect(getUrl("Passwords must match"));

                return Redirect("success");

                string getUrl(string error)
                {
                    Dictionary<string, string> _query = new Dictionary<string, string>()
                    {
                        { "username", Username },
                        { "email", Email },
                        { "password", Password },
                        { "password2", Password2 },
                        { "error", error }
                    };

                    return $"{Request.Path}{QueryString.Create(_query)}";
                }
            }

            return Page();
        }
    }
}
