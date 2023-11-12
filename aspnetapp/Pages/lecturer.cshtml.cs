using System.Reflection;
using System.Text.Json;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetapp.Pages
{
    public class lecturerModel : PageModel
    {
        public Lecturer lecturer { get; private set; }

        public IActionResult OnGet()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "aspnetapp.lecturer.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                lecturer = JsonSerializer.Deserialize<Lecturer>(reader.ReadToEnd());
            }
            
            return Page();
        }
    }
}
