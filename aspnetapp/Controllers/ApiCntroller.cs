using System.Reflection;
using System.Text.Json;
using aspnetapp.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("api/lecturers")]
    public class LecturersController : Controller
    {
        public LecturersController()
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            Log.Request(Request);
            try
            {
                string requestText;
                using (StreamReader reader = new StreamReader(Request.Body))
                    requestText = await reader.ReadToEndAsync();

                Lecturer lecturer;
                try
                {
                    lecturer = JsonSerializer.Deserialize<Lecturer>(requestText);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    Log.Info($"Request text: {requestText}");
                    return StatusCode(400);
                }

                Log.Debug(lecturer.ToString());

                if (!Lecturer.Validate(lecturer))
                    return StatusCode(400);

                Database.AddLectuer(lecturer);

                return Json(lecturer);
            } catch (Exception ex)
            {
                Log.Exception(ex);
                return StatusCode(500); // Internal server error
            }
        }

        [HttpGet]
        public ActionResult Get()
        {
            Log.Request(Request);
            return Json(Database.lectuerers.Select(item => item.Value).ToArray());
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult SpecificGet(Guid guid)
        {
            Log.Request(Request);
            if (Database.TryGetLecturer(guid, out Lecturer lecturer))
                return Json(lecturer);
            else
                return StatusCode(404);
        }

        [HttpPut]
        [Route("{guid}")]
        public async Task<ActionResult> Put(Guid guid)
        {
            Log.Request(Request);
            try
            {
                string requestText;
                using (StreamReader reader = new StreamReader(Request.Body))
                    requestText = await reader.ReadToEndAsync();

                Lecturer lecturer;
                try
                {
                    lecturer = JsonSerializer.Deserialize<Lecturer>(requestText);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    Log.Info($"Request text: {requestText}");
                    return StatusCode(400);
                }

                Log.Debug(lecturer.ToString());

                if (!Lecturer.Validate(lecturer))
                    return StatusCode(400);

                if (Database.ContainsKey(guid)) {
                    Database.Remove(guid);
                    Database.AddLectuer(lecturer);
                    JsonResult res = Json(lecturer);
                    res.StatusCode = 200;
                    return res;
                }

                return StatusCode(404);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return StatusCode(500); // Internal server error
            }
        }

        [HttpDelete]
        [Route("{guid}")]
        public ActionResult Delete(Guid guid)
        {
            Log.Request(Request);
            if (Database.lectuerers.ContainsKey(guid))
            {
                Database.Remove(guid);
                return StatusCode(204);
            }

            return StatusCode(404);
        }
    }
}
