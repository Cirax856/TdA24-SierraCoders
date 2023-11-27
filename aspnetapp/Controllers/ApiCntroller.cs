using System.Reflection;
using System.Text.Json;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("api/lecturers")]
    public class LecturersController : Controller
    {
        private LecturerContext context => Program.dbContext;
        public LecturersController()
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
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
                    Console.WriteLine(ex);
                    return StatusCode(400);
                }

                if (!Lecturer.IsValid(lecturer))
                    return StatusCode(400);

                context.lecturers.Add(lecturer);
                context.SaveChanges();

                return Json(lecturer);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500); // Internal server error
            }
        }

        [HttpGet]
        public ActionResult Get()
        {
            DbLecturer[] _lectures = context.lecturers.ToArray();

            return Json(_lectures.Select(lecturer => (Lecturer)lecturer).ToArray());
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult SpecificGet(Guid guid)
        {
            DbLecturer[] lecturers = context.lecturers.ToArray();
            for (int i = 0; i < lecturers.Length; i++)
                if (lecturers[i].UUID == guid)
                    return Json(lecturers[i]);

            return StatusCode(404);
        }

        [HttpPut]
        [Route("{guid}")]
        public async Task<ActionResult> Put(Guid guid)
        {
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
                    Console.WriteLine(ex);
                    return StatusCode(400);
                }

                if (!Lecturer.IsValid(lecturer))
                    return StatusCode(400);

                DbLecturer[] lecturers = context.lecturers.ToArray();
                for (int i = 0; i < lecturers.Length; i++)
                    if (lecturers[i].UUID == guid)
                    {
                        context.lecturers.Remove(lecturers[i]);
                        context.lecturers.Add(lecturer);
                        context.SaveChanges();
                        JsonResult res = Json(lecturers[i]);
                        res.StatusCode = 204;
                        return res;
                    }

                return StatusCode(404);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500); // Internal server error
            }
        }

        [HttpDelete]
        [Route("{guid}")]
        public ActionResult Delete(Guid guid)
        {
            DbLecturer[] lecturers = context.lecturers.ToArray();
            for (int i = 0; i < lecturers.Length; i++)
                if (lecturers[i].UUID == guid)
                {
                    context.lecturers.Remove(lecturers[i]);
                    context.SaveChanges();
                    return StatusCode(204);
                }

            return StatusCode(404);
        }
    }
}
