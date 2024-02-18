using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("plspls")]
    public class DbSaveController : Controller
    {
        [HttpGet]
        [Route("emPass")]
        public ActionResult EmPass()
        {
            if (!string.IsNullOrWhiteSpace(Database.emailPass))
                return Content("Pass already set");
            else if (Request.Query.TryGetValue("ep", out StringValues _ep))
            {
                Database.emailPass = _ep.ToString();
                return Content("Ok");
            }
            else
                return Content("Invalid query");
        }

        [HttpGet]
        [Route("save")]
        public ActionResult Save()
        {
            try
            {
                Database.Save();
                return Ok();
            } catch(Exception ex)
            {
                // exceptions was already loged
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("load")]
        public ActionResult Load()
        {
            try
            {
                Database.Load();
                return Ok();
            }
            catch (Exception ex)
            {
                // exceptions was already loged
                return StatusCode(500);
            }
        }
    }
}
