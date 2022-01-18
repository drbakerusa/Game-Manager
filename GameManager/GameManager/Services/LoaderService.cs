using Flurl.Http;

using GameManager.Data.F95;

using HtmlAgilityPack;

namespace GameManager.Services;

public class LoaderService : IDisposable
{
    private readonly SettingsService _settingsService;
    private CookieJar _jar = new CookieJar();
    string _baseUrl = "https://f95zone.to/";

    public LoaderService()
    {
        _settingsService = new SettingsService();
        _jar = new CookieJar();
    }

    public async Task RefreshAllMetadata()
    {
        _settingsService.ControlId = "0";
        await LoadMetadata();
    }

    public async Task LoadMetadata()
    {
        await AuthenticateF95Async();

        var pageNumber = 1;
        var totalPages = await GetTotalPagesAsync();
        var controlIdFound = false;
        var newControlId = string.Empty;
        var performingFullDataLoad = _settingsService.ControlId == "0";

        if ( performingFullDataLoad )
            Console.WriteLine("Loading all metadata from F95. This will take a few minutes");
        else
            Console.WriteLine("Getting latest updates . . .");

        while ( pageNumber <= totalPages && !controlIdFound )
        {
            var games = (await GetPageAsync(pageNumber)).Content.Games;

            if ( pageNumber == 1 )
                newControlId = games.First().Id
                    ?? throw new NullReferenceException(nameof(newControlId));

            if ( performingFullDataLoad )
                Console.WriteLine($"Loading page {pageNumber} of {totalPages}");

            foreach ( var game in games )
            {
                if ( game.Id == _settingsService.ControlId )
                {
                    controlIdFound = true;
                    break;
                }
                SaveGameToDatabase(game);
            }
            pageNumber++;
        }
        _settingsService.ControlId = newControlId;
    }

    private void SaveGameToDatabase(Game game)
    {
        var gameMetadata = ConvertGameToMetadata(game);

        using ( var db = new DataContext() )
        {
            var gameRecord = db.GameMetadata.Find(gameMetadata.Id);

            if ( gameRecord != null )
            {
                db.GameMetadata.Remove(gameRecord);
            }
            db.GameMetadata.Add(gameMetadata);
            db.SaveChanges();
        }
    }

    private GameMetadata ConvertGameToMetadata(Game game)
    {
        _ = game.Id ?? throw new ArgumentNullException(nameof(game));

        return new GameMetadata
        {
            Id = int.Parse(game.Id),
            Name = game.Title ?? "unknown",
            Version = game.Version ?? "unknown",
            Developer = game.Developer ?? "unknown",
            Prefixes = BuildTags(Map.GetPrefixFromId(game.Prefixes)),
            Tags = BuildTags(Map.GetTagFromId(game.Tags)),
            WhenRefreshed = Map.ConvertFuzzyTime(game.FuzzyTime)
        };
    }

    private string BuildTags(List<string> tags)
    {
        var updatedTags = new List<string>();
        foreach ( var tag in tags )
        {
            updatedTags.Add($"[{tag}]");
        }
        return string.Join(' ', updatedTags);
    }

    private async Task AuthenticateF95Async()
    {
        var settings = _settingsService.GetSettings();

        if ( string.IsNullOrEmpty(settings.F95Username) )
            throw new ArgumentNullException(nameof(settings.F95Username));

        if ( string.IsNullOrEmpty(settings.F95Password) )
            throw new ArgumentNullException(nameof(settings.F95Password));

        var testUrl = _baseUrl.WithCookies(_jar).AppendPathSegment("login/login");

        var testResult = await testUrl.GetStringAsync();
        var testHtml = new HtmlDocument();
        testHtml.LoadHtml(testResult);

        var dataLoggedIn = testHtml.DocumentNode.ChildNodes
             .Where(n => n.Name == "html")
             .FirstOrDefault()?
             .Attributes
                 .Where(a => a.Name == "data-logged-in")
                 .FirstOrDefault()?
                 .Value ?? string.Empty;

        if ( !string.IsNullOrEmpty(dataLoggedIn) && !bool.Parse(dataLoggedIn) )
        {
            var url = _baseUrl.WithCookies(_jar).AppendPathSegment("login/login");
            var html = new HtmlDocument();

            var result = await url.GetStringAsync();
            html.LoadHtml(result);

            string token = "";

            token = html.DocumentNode.Descendants("input")
                .Where(i => i.Attributes.Where(a => a.Value == "_xfToken").Any())
                .FirstOrDefault()?.Attributes
                .Where(a => a.Name == "value")
                .FirstOrDefault()?.Value ?? string.Empty;

            if ( token != string.Empty
                 && settings is not null )
            {
                var formContents = new[]
                {
                        new KeyValuePair<string, string>("login", settings.F95Username ?? throw new NullReferenceException(nameof(settings.F95Username))),
                        new KeyValuePair<string, string>("url",""),
                        new KeyValuePair<string, string>("password", settings.F95Password ?? throw new NullReferenceException(nameof(settings.F95Password))),
                        new KeyValuePair<string, string>("password_confirm",""),
                        new KeyValuePair<string, string>("additional_security",""),
                        new KeyValuePair<string, string>("_xfRedirect",_baseUrl),
                        new KeyValuePair<string, string>("website_code",""),
                        new KeyValuePair<string, string>("_xfToken",token)
                    };
                await url.PostUrlEncodedAsync(formContents);
            }
            else
                throw new ArgumentNullException("token");
        }
    }

    async Task<int> GetTotalPagesAsync() => (await GetPageAsync(1)).Content.Pages.TotalPages;

    async Task<Result> GetPageAsync(int pageNumber)
    {
        Result result = new();
        bool success = false;

        do
        {
            try
            {
                result = await _baseUrl.WithCookies(_jar)
                    .AppendPathSegment("sam")
                    .AppendPathSegment("latest_alpha")
                    .AppendPathSegment("latest_data.php")
                    .SetQueryParams(
                        new
                        {
                            cmd = "list",
                            cat = "games",
                            page = pageNumber,
                            rows = 90
                        })
                    .GetJsonAsync<Result>();
            }
            catch ( FlurlHttpException e )
            {
                if ( e.StatusCode == 429 )
                {
                    Console.WriteLine("Request failed due to too many request. This is a limit set by F95 and will resolve itself within 10 minutes. Please wait and try again. The application will now quit.");
                    Environment.Exit(1);
                }
                else
                    throw (e);
            }
            success = true;
        }
        while ( !success );
        return result;
    }

    public void Dispose() { }
}
