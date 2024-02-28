using aspnetapp.Models;
using aspnetapp.Models.Schedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace aspnetapp.Controllers
{
    [Route("api/subject")]
    [ApiController]
    public class SubjectController : LoggedInController
    {
        [HttpPut]
        [Route("add")]
        public async Task<ActionResult> Add()
        {
            if (tryGetAcount(out Account account))
            {
                if (!account.HasLecturer)
                    return BadRequest("Account lecturer info not set");

                JObject jo;
                try
                {
                    string requestText;
                    using (StreamReader reader = new StreamReader(Request.Body))
                        requestText = await reader.ReadToEndAsync();

                    jo = JsonConvert.DeserializeObject<JObject>(requestText);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return BadRequest();
                }

                if (jo is not null && jo.TryGetValue("name", out JToken name) && name.Type == JTokenType.String && jo.TryGetValue("desc", out JToken desc) && desc.Type == JTokenType.String)
                {
                    Subject subject = new Subject(name.ToObject<string>()!, desc.ToObject<string>()!);
                    var subjects = Database.GetSchedule(account.LecturerGuid).Subjects;
                    if (subjects.Contains(subject))
                        return Conflict("Subject with this name already exists");
                    else
                        subjects.Add(subject);

                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
                return Unauthorized();
        }
    }
}
