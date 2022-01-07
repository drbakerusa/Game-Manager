
using System.Reflection;

using Flurl;
using Flurl.Http;

using GameManager.Data.Github;

namespace GameManager.Services;

public class GitHubService : IDisposable
{
    string _repoUrl = "https://api.github.com/repos/drbakerusa/Game-Manager/";

    public async Task<bool> CheckIfNewerVersionExists()
    {
        var releases = await GetAllReleases();
        var latestRelease = releases.OrderByDescending(r => r.WhenPublished).FirstOrDefault();
        if ( latestRelease != null )
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            using ( var settings = new SettingsService() )
            {
                settings.LatestApplicationVersion = latestRelease.TagName;
                settings.NewerVersionExists = currentVersion < latestRelease.Version;
            }

            return currentVersion < latestRelease.Version;
        }
        return false;
    }

    private async Task<IEnumerable<Release>> GetAllReleases()
        => await _repoUrl.AppendPathSegment("releases")
                               .WithHeaders(new
                               {
                                   Accept = "application/vnd.github.v3+json",
                                   User_Agent = "Game-Manager"
                               })
                               .GetJsonAsync<List<Release>>();

    public void Dispose() { }
}
