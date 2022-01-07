﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameManager.Services;

public class LibraryService
{
    DataContext _data;
    Settings _settings;
    public LibraryService()
    {
        _data = new DataContext();
        _settings = new SettingsService().GetSettings();
    }

    public bool MetadataExists(int metadataId) => _data.GameMetadata.Find(metadataId) != null;

    public GameMetadata GetGameMetadata(int metadataId)
        => _data.GameMetadata.Find(metadataId) ?? throw new NullReferenceException();

    public bool GameIsInLibrary(int metadataId) => _data.Library.Any(g => g.MetadataId == metadataId);

    public void AddGameToLibrary(int metadataId)
    {
        if ( GameIsInLibrary(metadataId) )
            throw new ArgumentException("Game is already in library");

        var metadata = GetGameMetadata(metadataId);

        var game = new LibraryGame
        {
            MetadataId = metadata.Id,
            Name = metadata.Name,
            Developer = metadata.Developer,
            Version = metadata.Version,
            Tags = metadata.Tags,
            Prefixes = metadata.Prefixes,
            ManualUpdatesOnly = !_settings.AutomaticallyCheckForGameUpdates
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

    public IEnumerable<LibraryGame> GamesAlphabetical => _data.Library.OrderBy(g => g.Name)
                                                                      .ToList();

    public IEnumerable<LibraryGame> GamesRecentlyUpdated => _data.Library.OrderByDescending(g => g.WhenUpdated)
                                                                         .ToList();

    public IEnumerable<LibraryGame> GamesNeverPlayed => _data.Library.Where(g => g.LaunchCount == 0)
                                                                     .OrderByDescending(g => g.WhenUpdated)
                                                                     .ThenBy(g => g.Name)
                                                                     .ToList();

    public IEnumerable<LibraryGame> GamesManualUpdateCheck => _data.Library.Where(g => g.ManualUpdatesOnly)
                                                                           .OrderByDescending(g => g.WhenUpdated)
                                                                           .ThenBy(g => g.Name)
                                                                           .ToList();

    public void CheckGameForUpdates(int gameId) => CheckGameForUpdates(GetGame(gameId));

    public void CheckGameForUpdates(LibraryGame game)
    {
        _ = game ?? throw new ArgumentNullException(nameof(game));

        var metadata = GetGameMetadata(game.MetadataId);

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
    }

    public void CheckLibraryForUpdates()
    {
        foreach ( var game in _data.Library.Where(g => !g.ManualUpdatesOnly) )
        {
            CheckGameForUpdates(game);
        }
    }

    public bool GameUpdatesAreAvailable => _data.Library.Any(g => g.HasUpgradeAvailable);

    public int NumberOfUpdatesAvailable => _data.Library.Count(g => g.HasUpgradeAvailable);

    public void UpdateGame(int gameId)
    {
        var game = GetGame(gameId);
        var metadata = GetGameMetadata(game.MetadataId);

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

    public IEnumerable<LibraryGame> GamesWithUpdates => _data.Library.Where(g => g.HasUpgradeAvailable)
                                                                     .OrderByDescending(g => g.WhenUpgradeDiscovered)
                                                                     .ToList();

    public void BrowseGameForum(int gameId)
    {
        var game = GetGame(gameId);
        var url = $"https://f95zone.to/threads/{game.MetadataId}";
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

    public IEnumerable<LibraryGame> RecentlyPlayedGames => _data.Library.Where(g => g.WhenLastLaunched != null)
                                                                        .OrderByDescending(g => g.WhenLastLaunched)
                                                                        .ToList();

    public IEnumerable<LibraryGame> ReadyToPlayGames => _data.Library.Where(g => !string.IsNullOrEmpty(g.ExecutablePath))
                                                                     .OrderByDescending(g => g.WhenLastLaunched)
                                                                     .ThenByDescending(g => g.Name)
                                                                     .ToList();

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

    public bool GamePreviouslyDeleted(int metadataId) => _data.DeletedGames.Find(metadataId) != null;

    public DeletedGame GetDeletedGame(int metadataId) => _data.DeletedGames.Find(metadataId) ?? throw new ArgumentException();

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
}