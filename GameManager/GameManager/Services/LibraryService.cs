using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameManager.Services;

public class LibraryService
{
    DataContext _data;
    SettingsService _settings;
    LoaderService _loader;

    public LibraryService()
    {
        _data = new DataContext();
        _settings = new SettingsService();
        _loader = new LoaderService();
    }

    public bool GameUpdatesAreAvailable => _data.Library.Any(g => g.HasUpgradeAvailable);

    public int NumberOfUpdatesAvailable => _data.Library.Count(g => g.HasUpgradeAvailable);

    public IEnumerable<LibraryGame> GamesAlphabetical => _data.Library.OrderBy(g => g.Name)
                                                                      .ToList();

    public IEnumerable<LibraryGame> GamesRecentlyUpdated => _data.Library.Where(g => g.WhenUpdated >= _settings.RecentThreshold)
                                                                         .OrderByDescending(g => g.WhenUpdated)
                                                                         .ToList();

    public IEnumerable<LibraryGame> GamesNeverPlayed => _data.Library.Where(g => g.LaunchCount == 0)
                                                                     .OrderByDescending(g => g.WhenUpdated)
                                                                     .ThenBy(g => g.Name)
                                                                     .ToList();

    public IEnumerable<LibraryGame> GamesManualUpdateCheck => _data.Library.Where(g => g.ManualUpdatesOnly)
                                                                           .OrderByDescending(g => g.WhenUpdated)
                                                                           .ThenBy(g => g.Name)
                                                                           .ToList();

    public IEnumerable<LibraryGame> GamesWithUpdates => _data.Library.Where(g => g.HasUpgradeAvailable)
                                                                     .OrderByDescending(g => g.WhenUpgradeDiscovered)
                                                                     .ToList();

    public IEnumerable<LibraryGame> RecentlyPlayedGames => _data.Library.Where(g => g.WhenLastLaunched >= _settings.RecentThreshold)
                                                                        .OrderByDescending(g => g.WhenLastLaunched)
                                                                        .ToList();

    public IEnumerable<LibraryGame> ReadyToPlayGames => _data.Library.Where(g => !string.IsNullOrEmpty(g.ExecutablePath))
                                                                     .OrderByDescending(g => g.WhenLastLaunched)
                                                                     .ThenByDescending(g => g.Name)
                                                                     .ToList();

    public IEnumerable<GameMetadata> RecentlyUpdatedMetadata => _data.GameMetadata.Where(m => m.WhenRefreshed >= _settings.RecentThreshold)
                                                                                  .OrderByDescending(m => m.WhenRefreshed)
                                                                                  .ToList();

    public bool GamePreviouslyDeleted(int metadataId) => _data.DeletedGames.Find(metadataId) != null;

    public DeletedGame GetDeletedGame(int metadataId) => _data.DeletedGames.Find(metadataId) ?? throw new ArgumentException();

    public bool MetadataExists(int metadataId) => _data.GameMetadata.Find(metadataId) != null;

    public GameMetadata? GetGameMetadata(int metadataId)
        => _data.GameMetadata.Find(metadataId);

    public GameMetadata? GetGameMetadata(string forumUrl)
    {
        if ( string.IsNullOrEmpty(forumUrl) || !Uri.IsWellFormedUriString(forumUrl, UriKind.Absolute) )
            return null;

        if ( int.TryParse(forumUrl.Substring(forumUrl.LastIndexOf('.') + 1).Replace("/", string.Empty), out int metadataId) )
            return GetGameMetadata(metadataId);

        return null;
    }

    public bool GameIsInLibrary(int metadataId) => _data.Library.Any(g => g.MetadataId == metadataId);

    public void AddGameToLibrary(int metadataId)
    {
        if ( GameIsInLibrary(metadataId) )
            throw new ArgumentException("Game is already in library");

        var metadata = GetGameMetadata(metadataId);

        _ = metadata ?? throw new Exception($"Metadata is null for ID {metadataId}");

        var game = new LibraryGame
        {
            MetadataId = metadata.Id,
            Name = metadata.Name,
            Developer = metadata.Developer,
            Version = metadata.Version,
            Tags = metadata.Tags,
            Prefixes = metadata.Prefixes,
            ManualUpdatesOnly = !_settings.CheckForUpdatesAutomatically
        };

        var deletedGame = _data.DeletedGames.Find(metadataId);
        if ( deletedGame != null )
        {
            _data.Remove(deletedGame);
        }

        _data.Library.Add(game);
        _data.SaveChanges();
    }

    public int? GetGameId(int metadataId) => _data.Library.FirstOrDefault(g => g.MetadataId == metadataId)?.Id;

    public LibraryGame GetGame(int gameId) => _data.Library.Find(gameId) ?? throw new NullReferenceException();

    public void CheckGameForUpdates(int gameId, bool loadMetadata = false, bool showSuccessMessage = false) => CheckGameForUpdates(GetGame(gameId), loadMetadata, showSuccessMessage);

    public void CheckGameForUpdates(LibraryGame game, bool loadMetadata = false, bool showSuccessMessage = false)
    {
        _ = game ?? throw new ArgumentNullException(nameof(game));

        if ( loadMetadata )
            _loader.LoadMetadata().Wait();

        var metadata = GetGameMetadata(game.MetadataId);

        _ = metadata ?? throw new Exception($"Metadata is null for ID {game.MetadataId}");

        if ( metadata.WhenRefreshed > game.WhenUpdated && metadata.Version != game.Version )
        {
            if ( game.AvailableUpgradeVersion != metadata.Version )
            {
                game.WhenUpgradeDiscovered = DateTime.Now;
                game.AvailableUpgradeVersion = metadata.Version;
                game.HasUpgradeAvailable = true;
                _data.SaveChanges();
            }
        }
        else
        {
            if ( showSuccessMessage )
                UIElements.ShowSuccessMessage("Game is Current");
        }
    }

    public void CheckLibraryForUpdates()
    {
        foreach ( var game in _data.Library.Where(g => !g.ManualUpdatesOnly) )
        {
            CheckGameForUpdates(game);
        }
    }

    public void UpdateGame(int gameId)
    {
        var game = GetGame(gameId);
        var metadata = GetGameMetadata(game.MetadataId);

        _ = metadata ?? throw new Exception($"Metadata is null for ID {gameId}");

        game.Name = metadata.Name;
        game.Version = metadata.Version;
        game.Prefixes = metadata.Prefixes;
        game.Tags = metadata.Tags;
        game.ExecutablePath = string.Empty;
        game.WhenUpdated = DateTime.Now;
        game.HasUpgradeAvailable = false;
        game.WhenUpgradeDiscovered = null;
        game.AvailableUpgradeVersion = string.Empty;

        _data.SaveChanges();
    }

    public void BrowseGameForum(int gameId, bool isMetadataId = false)
    {
        var url = string.Empty;
        if ( isMetadataId )
            url = $"https://f95zone.to/threads/{gameId}";
        else
        {
            var game = GetGame(gameId);
            url = $"https://f95zone.to/threads/{game.MetadataId}";
        }

        StartProcess(url);
    }

    public void LaunchGame(int gameId)
    {
        var game = GetGame(gameId);

        var executable = game.ExecutablePath;

        if ( !File.Exists(executable) )
            throw new FileNotFoundException();

        StartProcess(executable);

        game.WhenLastLaunched = DateTime.Now;
        game.LaunchCount++;
        _data.SaveChanges();
    }

    public void LaunchGameManually(int gameId)
    {
        var game = GetGame(gameId);
        game.WhenLastLaunched = DateTime.Now;
        game.LaunchCount++;
        _data.SaveChanges();
    }

    public void SetGameManualUpdatesOnly(int gameId, bool status)
    {
        var game = GetGame(gameId);
        game.ManualUpdatesOnly = status;
        _data.SaveChanges();
    }

    public void SetGameExecutablePath(int gameId, string path)
    {
        path = path.Trim().Replace("\"", "");

        if ( !File.Exists(path) )
            throw new FileNotFoundException();

        var game = GetGame(gameId);
        game.ExecutablePath = path;
        _data.SaveChanges();
    }

    public void ClearGameExecutablePath(int gameId)
    {
        var game = GetGame(gameId);
        game.ExecutablePath = string.Empty;
        _data.SaveChanges();
    }

    public void ResetGameLaunchStatistics(int gameId)
    {
        var game = GetGame(gameId);
        game.WhenLastLaunched = null;
        game.LaunchCount = 0;
        _data.SaveChanges();
    }

    public void DeleteGame(int gameId, string reason)
    {
        var game = GetGame(gameId);

        var deletedGame = new DeletedGame
        {
            Id = game.MetadataId,
            Name = game.Name,
            Version = game.Version,
            ReasonForDeleting = reason
        };

        _data.DeletedGames.Add(deletedGame);
        _data.Remove(game);
        _data.SaveChanges();
    }

    public void StartProcess(string command)
    {
        try
        {
            Process.Start(command);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
            {
                command = command.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {command}") { CreateNoWindow = true });
            }
            else if ( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) )
            {
                Process.Start("xdg-open", command);
            }
            else if ( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) )
            {
                Process.Start("open", command);
            }
            else
            {
                throw;
            }
        }
    }

    public IEnumerable<GameComment> GetGameComments(int gameId) => _data.Library.Include(g => g.Comments)
                                                                                .FirstOrDefault(g => g.Id == gameId)?.Comments
                                                                                .OrderByDescending(c => c.TimeStamp)
                                                                                .ToList() ?? throw new NullReferenceException();

    public GameComment? GetMostRecentGameComment(int gameId) => _data.Library.Include(g => g.Comments)
                                                                             .FirstOrDefault(g => g.Id == gameId)?.Comments
                                                                             .OrderByDescending(c => c.TimeStamp)
                                                                             .FirstOrDefault();

    public int NumberOfGameComments(int gameId)
    {
        var comments = _data.Library.Include(g => g.Comments)
                                   .FirstOrDefault(g => g.Id == gameId)?.Comments;

        if ( comments != null && comments.Any() )
            return comments.Count();
        return 0;
    }

    public void AddGameComment(int gameId, string comment)
    {
        var game = GetGame(gameId);
        game.Comments.Add(new GameComment
        {
            Comment = comment,
            VersionCommentedOn = game.Version
        });
        _data.SaveChanges();
    }

    public void RemoveGameComment(int gameId, GameComment comment)
    {
        var game = GetGame(gameId);
        game.Comments.Remove(comment);
        _data.SaveChanges();
    }

    public IList<GameMetadata> GetMetadataByGameName(string name) => _data.GameMetadata.Where(g => g.Name.ToLower().Contains(name.Trim().ToLower()))
                                                                                       .OrderBy(g => g.Name)
                                                                                       .ToList();

    public IList<GameMetadata> GetMetadataByDeveloperName(string name) => _data.GameMetadata.Where(g => g.Developer.ToLower().Contains(name.Trim().ToLower()))
                                                                                            .OrderBy(g => g.Name)
                                                                                            .ToList();

    public IList<GameMetadata> GetMetadataByTag(string tag, TagType tagType)
    {
        switch ( tagType )
        {
            case TagType.Tag:
                return _data.GameMetadata.Where(m => m.Tags.Contains(tag))
                                         .OrderBy(g => g.Name)
                                         .ToList();
            case TagType.Prefix:
                return _data.GameMetadata.Where(m => m.Prefixes.Contains(tag))
                                         .OrderBy(g => g.Name)
                                         .ToList();
        }
        return new List<GameMetadata>();
    }
}