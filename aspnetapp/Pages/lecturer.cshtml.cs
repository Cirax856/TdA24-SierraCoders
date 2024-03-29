using System.Reflection;
using System.Text.Json;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages
{
    public class lecturerModel : PageModel
    {
        public Models.Lecturer lecturer { get; private set; }

        public IActionResult OnGet()
        {
            if (Request.Query.TryGetValue("id", out StringValues values) && Guid.TryParse(values.FirstOrDefault(), out Guid id) && Database.ContainsLecturer(id))
            {
                lecturer = Database.GetLecturer(id);
                return Page();
            }
            else
                return StatusCode(404);
        }
    }
}
