using System.Net;
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
                    return statusWithJson(400);
                }

                Log.Debug(lecturer.ToString());

                if (!Lecturer.Validate(lecturer))
                {
                    Log.Debug("400 ERROR");
                    return statusWithJson(400);
                }

                Database.AddLectuer(lecturer);

                Log.Debug("200 OK");
                return Json(lecturer);
            } catch (Exception ex)
            {
                Log.Error("ERRORRRORORORROROROROR");
                Log.Exception(ex);
                return statusWithJson(500); // Internal server error
            }
        }

        [HttpGet]
        public ActionResult Get()
        {
            Log.Request(Request);
            return Json(Database.lectuerers.Select(item => (Lecturer)item.Value).ToArray());
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult SpecificGet(Guid guid)
        {
            Log.Request(Request);
            if (Database.TryGetLecturer(guid, out Lecturer lecturer))
            {
                Log.Debug("200 OK");
                return Json(lecturer);
            }
            else
            {
                Log.Debug("404 ERROR");
                return statusWithJson(404);
            }
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
                    return statusWithJson(400);
                }

                Log.Debug(lecturer.ToString());

                if (!Lecturer.Validate(lecturer))
                {
                    Log.Debug("400 ERROR");
                    return statusWithJson(400);
                }

                if (Database.ContainsKey(guid)) {
                    Lecturer oldLecturer = Database.GetLecturer(guid);
                    Database.Remove(guid);
                    lecturer = oldLecturer.Apply(lecturer);
                    Database.AddLectuer(lecturer);
                    JsonResult res = Json(lecturer);
                    res.StatusCode = 200;
                    Log.Debug("200 OK");
                    return res;
                }

                return statusWithJson(404);
            }
            catch (Exception ex)
            {
                Log.Error("ERRORRRORORORROROROROR");
                Log.Exception(ex);
                return statusWithJson(500); // Internal server error
            }
        }

        [HttpDelete]
        [Route("{guid}")]
        public ActionResult Delete(Guid guid)
        {
            try { 
                Log.Request(Request);
                if (Database.lectuerers.ContainsKey(guid))
                {
                    Database.Remove(guid);
                    Log.Debug("204 OK");
                    return statusWithJson(204);
                }

                Log.Debug("404 ERROR");
                return statusWithJson(404);
            } catch (Exception ex)
            {
                Log.Error("ERRORRRORORORROROROROR");
                Log.Exception(ex);
                return statusWithJson(500); // Internal server error
            }
        }

        private ActionResult statusWithJson(int statusCode)
        {
            JsonResult res = Json(new StatusResponse()
            {
                code = statusCode,
                message = ((HttpStatusCode)statusCode).ToString()
            });
            res.StatusCode = statusCode;
            return res;
        }
    }
}
