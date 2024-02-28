using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Account
{
    public class ScheduleModel : LoggedInPage
    {
        public Models.Account Account { get; private set; }
        public DateOnly ScheduleDate { get; private set; }

        public ActionResult OnGet()
        {
            if (tryGetAcount(out Models.Account account))
            {
                if (!account.HasLecturer)
                    return Redirect("/account/lecturer");

                Account = account;

                if (Request.Query.TryGetValue("date", out StringValues _date) && DateOnly.TryParse(_date, out DateOnly date))
                    ScheduleDate = date;
                else
                    ScheduleDate = DateOnly.FromDateTime(DateTime.UtcNow);

                // "clamp" to monday, need to use ToInt, because 0 - sunday, 1 - monday WTF???
                ScheduleDate = ScheduleDate.AddDays(-(ScheduleDate.DayOfWeek.ToInt() % 7));

                return Page();
            }
            else
                return Redirect("/account/login");
        }
    }
}
