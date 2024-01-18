using aspnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace aspnetapp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public KeyValuePair<Guid, DbLecturer>[] lecturers { get; private set; }
    public Searcher.RatedString[] result { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        string searchQuery = string.Empty;
        if (Request.Query.TryGetValue("lecName", out StringValues _searchQuery))
            searchQuery = _searchQuery.ToString().ToLowerInvariant();

        Console.WriteLine("Query: " + searchQuery);

        lecturers = Database.lectuerers.ToArray();
        result = Searcher.Search(Database.lectuerers.ToArray(), lecturer => ((Lecturer)lecturer.Value).DisplayName.ToLowerInvariant(), searchQuery).ToArray();

        if (result.Length == 0)
            return;

        // remove entries with score lower than or equal to 0
        int i;
        for (i = 0; i < result.Length; i++)
            if (result[i].Score <= 0f)
                break;

        result = result.Take(i).ToArray();
    }
}
