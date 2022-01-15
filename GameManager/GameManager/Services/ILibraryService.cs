namespace GameManager.Services
{
    public interface ILibraryService
    {
        IEnumerable<LibraryGame> GamesAlphabetical { get; }
        IEnumerable<LibraryGame> GamesManualUpdateCheck { get; }
        IEnumerable<LibraryGame> GamesNeverPlayed { get; }
        IEnumerable<LibraryGame> GamesRecentlyUpdated { get; }
        IEnumerable<LibraryGame> GamesWithUpdates { get; }
        bool GameUpdatesAreAvailable { get; }
        int NumberOfUpdatesAvailable { get; }
        IEnumerable<LibraryGame> ReadyToPlayGames { get; }
        IEnumerable<LibraryGame> RecentlyPlayedGames { get; }

        void AddGameComment(int gameId, string comment);
        void AddGameToLibrary(int metadataId);
        void BrowseGameForum(int gameId);
        void CheckGameForUpdates(int gameId);
        void CheckGameForUpdates(LibraryGame game);
        void CheckLibraryForUpdates();
        void ClearGameExecutablePath(int gameId);
        void DeleteGame(int gameId, string reason);
        bool GameIsInLibrary(int metadataId);
        bool GamePreviouslyDeleted(int metadataId);
        DeletedGame GetDeletedGame(int metadataId);
        LibraryGame GetGame(int gameId);
        IEnumerable<GameComment> GetGameComments(int gameId);
        int? GetGameId(int metadataId);
        GameMetadata GetGameMetadata(int metadataId);
        GameComment? GetMostRecentGameComment(int gameId);
        void LaunchGame(int gameId);
        void LaunchGameManually(int gameId);
        bool MetadataExists(int metadataId);
        int NumberOfGameComments(int gameId);
        void RemoveGameComment(int gameId, GameComment comment);
        void ResetGameLaunchStatistics(int gameId);
        void SetGameExecutablePath(int gameId, string path);
        void SetGameManualUpdatesOnly(int gameId, bool status);
        void StartProcess(string command);
        void UpdateGame(int gameId);
    }
}