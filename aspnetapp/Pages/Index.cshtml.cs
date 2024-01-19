using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public KeyValuePair<Guid, DbLecturer>[] lecturers { get; private set; }
    public List<Searcher.RatedString> result { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public ActionResult OnGet()
    {
        // name
        string searchQuery = string.Empty;
        if (Request.Query.TryGetValue("lecName", out StringValues _searchQuery))
            searchQuery = _searchQuery.ToString().ToLowerInvariant();

        lecturers = Database.lectuerers.ToArray();
        result = Searcher.Search(Database.lectuerers.ToArray(), lecturer => ((Lecturer)lecturer.Value).DisplayName.ToLowerInvariant(), searchQuery).ToList();

        if (result.Count == 0)
            return Page();

        // remove entries with score lower than or equal to 0
        int i;
        for (i = 0; i < result.Count; i++)
            if (result[i].Score <= 0f)
                break;

        result = result.Take(i).ToList();

        // price
        int minPrice = int.MinValue;
        int maxPrice = int.MaxValue;
        if (Request.Query.TryGetValue("minPrice", out StringValues _minPrice))
            if (!int.TryParse(_minPrice.ToString(), out minPrice))
                minPrice = int.MinValue;
        if (Request.Query.TryGetValue("maxPrice", out StringValues _maxPrice))
            if (!int.TryParse(_maxPrice.ToString(), out maxPrice))
                maxPrice = int.MaxValue;

        // location
        string location = string.Empty;
        if (Request.Query.TryGetValue("location", out StringValues _location))
            location = _location.ToString().ToLowerInvariant();

        for (i = 0; i < result.Count; i++)
        {
            Lecturer lec = lecturers[result[i].OgIndex].Value;
            if (lec.price_per_hour < minPrice || lec.price_per_hour > maxPrice || (location != string.Empty && Searcher.Rate(lec.location.ToLowerInvariant(), 0, location).Score < 0f))
            {
                result.RemoveAt(i);
                i--;
            }
        }

        // tags
        string[] tags = new string[0];
        if (Request.Query.TryGetValue("tag", out StringValues _tags))
            tags = _tags.ToArray();

        for (i = 0; i < result.Count; i++)
        {
            bool invalid = false;
            for (int j = 0; j < tags.Length; j++)
                if (!lecturers[result[i].OgIndex].Value.tags.ToLowerInvariant().Contains(tags[j].ToLowerInvariant()))
                {
                    invalid = true;
                    break;
                }

            if (invalid)
            {
                result.RemoveAt(i);
                i--;
            }
        }

        return Page();
    }
}
