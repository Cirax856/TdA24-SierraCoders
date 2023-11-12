using System.Reflection;
using System.Text.Json;
using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("lecturers")]
    public class LecturersController : Controller
    {
        [HttpPost]
        public ActionResult Post()
        {
            try
            {
                string requestText;
                using (StreamReader reader = new StreamReader(Request.Body))
                    requestText = reader.ReadToEnd();

                Lecturer lecturer;
                try
                {
                    lecturer = JsonSerializer.Deserialize<Lecturer>(requestText);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return StatusCode(400);
                }

                if (!Lecturer.IsValid(lecturer))
                    return StatusCode(400);

                // TODO add to database
                throw new NotImplementedException("/lecturers POST isn't implemented");

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
            Lecturer[] lectures;
            // TODO get lecturers from db (SELECT *)

            // testing only
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "aspnetapp.lecturer.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                lectures = new Lecturer[]
                {
                    JsonSerializer.Deserialize<Lecturer>(reader.ReadToEnd())
                };
            }

            return Json(lectures);
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult SpecificGet(Guid guid)
        {
            Lecturer lecturer;
            // TODO get lecturer from db
            throw new NotImplementedException("/lecturers/{guid} GET isn't implemented");

            return Json(lecturer);
        }

        [HttpPut]
        [Route("{guid}")]
        public ActionResult Put(Guid guid)
        {
            try
            {
                string requestText;
                using (StreamReader reader = new StreamReader(Request.Body))
                    requestText = reader.ReadToEnd();

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

                // TODO get lecturer from db, is null return StatusCode(404), else put new data to db
                throw new NotImplementedException("/lecturers/{guid} PUT isn't implemented");

                return Json(lecturer);
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
            // TODO if lecturer with guid exists, delete, StatusCode(204), else StatusCode(404)
            throw new NotImplementedException("/lecturers/{guid} DELETE isn't implemented");
        }
    }

    public class Secret
    {
        public string secret { get; set; } = "The cake is a lie";
    }
}
