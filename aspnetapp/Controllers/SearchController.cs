using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                Lecturer[] lecturers = Database.lectuerers.Select(item => (Lecturer)item.Value).ToArray();

                if (Request.Query.ContainsKey("name"))
                {
                    // sort
                    Searcher.RatedString[] ratedLecturers =
                        Searcher.Search(lecturers, lecturer => lecturer.DisplayName, Request.Query["name"]);

                    // remove entries with score lower than or equal to 0
                    int i;
                    for (i = 0; i < ratedLecturers.Length; i++)
                        if (ratedLecturers[i].Score <= 0f)
                            break;

                    ratedLecturers = ratedLecturers.Take(i).ToArray();

                    Lecturer[] _lecturers = new Lecturer[lecturers.Length];
                    Array.Copy(lecturers, _lecturers, lecturers.Length);
                    lecturers = new Lecturer[ratedLecturers.Length];
                    for (i = 0; i < ratedLecturers.Length; i++)
                        lecturers[i] = _lecturers[ratedLecturers[i].OgIndex].Clone();
                }
                // TODO add filtering by price (from - to, ui - double slider?), by tags, by location

                return Json(lecturers);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500); // Internal server error
            }
        }
    }
}
