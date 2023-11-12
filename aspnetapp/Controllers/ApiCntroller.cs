using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("api")]
        public IActionResult Api()
        {
            return Json(new Secret());
        }
    }

    public class Secret
    {
        public string secret { get; set; } = "The cake is a lie";
    }
}
