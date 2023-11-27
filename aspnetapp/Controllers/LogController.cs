using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    public class LogController : Controller
    {
        [HttpGet]
        [Route("log")]
        public ActionResult GetLog()
        {
            return Content(Log.ToString());
        }
    }
}
