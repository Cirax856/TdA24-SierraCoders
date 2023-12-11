using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("plspls")]
    public class DbSaveController : Controller
    {
        [HttpGet]
        [Route("save")]
        public ActionResult Save()
        {
            try
            {
                Database.Save();
            } catch(Exception ex)
            {
                Log.Exception(ex);
                return StatusCode(500);
            }
        }
    }
}
