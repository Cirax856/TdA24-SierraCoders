using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("api")]
        public IActionResult Api()
        {
            Secret s = new Secret();
            return Json(s);
        }
    }

    public class Secret
    {
        public string secret { get; set; } = "The cake is a lie";
    }
}
