using aspnetapp.Models.Schedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages.Lecturer
{
    public class ReserveModel : PageModel
    {
        public Models.Lecturer Lecturer { get; private set; }
        public ScheduleInfo Schedule { get; private set; }
        public DateOnly Date { get; private set; }

        public ActionResult OnGet()
        {
            if (Request.Query.TryGetValue("id", out StringValues values) && Guid.TryParse(values.FirstOrDefault(), out Guid id) && Database.ContainsLecturer(id))
            {
                Lecturer = Database.GetLecturer(id);
                Schedule = Database.GetSchedule(id);

                if (Request.Query.TryGetValue("date", out StringValues _date) && DateOnly.TryParse(_date, out DateOnly date))
                    Date = date;
                else
                    Date = DateOnly.FromDateTime(DateTime.UtcNow);

                Date = Date.ClampToMonday();

                return Page();
            }
            else
                return StatusCode(404);
        }
    }
}
