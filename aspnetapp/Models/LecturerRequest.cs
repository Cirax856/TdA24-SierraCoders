using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace aspnetapp.Models
{
    public class LecturerRequest
    {
        [StringLength(36, MinimumLength = 36)]
        public Guid uuid { get; set; }
        public string name { get; set; }
    }
}
