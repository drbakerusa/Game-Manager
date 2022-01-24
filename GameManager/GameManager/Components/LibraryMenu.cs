namespace GameManager.Components;

public static class LibraryMenu
{
    public static void Show()
    {
        Console.Clear();
        UIElements.PageTitle("Library");

        var library = new LibraryService();

        if ( library.GameUpdatesAreAvailable )
        {
            string message = "games have updates available";
            if ( library.NumberOfUpdatesAvailable == 1 )
                message = "game has an update available";
            UIElements.GreenHighlight($"{library.NumberOfUpdatesAvailable} {message}");
            UIElements.Blank();
        }

        var options = new List<string>
        {
            $"Launch Game ({library.ReadyToPlayGames.Count()})",
            "Add Game",
            "Check for Updates",
            $"Games with Updates ({library.NumberOfUpdatesAvailable})",
            $"Recently Played ({library.RecentlyPlayedGames.Count()})",
            $"Never Played ({library.GamesNeverPlayed.Count()})",
            $"Automatic Updates Disabled ({library.GamesManualUpdateCheck.Count()})",
            "By Name",
            "By Recently Added or Upgraded",
            "Main Menu"
        };

        var selection = UIElements.Menu(options);

        switch ( selection )
        {
            case 0: // Launch Game
                LibrarySearchResultsScreen.Show(library.ReadyToPlayGames, "Ready to Play", launchInsteadOfDetails: true);
                Show();
                break;
            case 1: // Add Game
                AddGameScreen.Show();
                break;
            case 2: // Check for Updates
                library.CheckLibraryForUpdates(loadMetadata: true);
                Show();
                break;
            case 3: // Games with Updates
                LibrarySearchResultsScreen.Show(library.GamesWithUpdates, "Games with Updates");
                break;
            case 4: // Recently Played
                LibrarySearchResultsScreen.Show(library.RecentlyPlayedGames, "Recently Played");
                break;
            case 5:
                LibrarySearchResultsScreen.Show(library.GamesNeverPlayed, "Games Never Played");
                break;
            case 6:
                LibrarySearchResultsScreen.Show(library.GamesManualUpdateCheck, "Games with Automatic Updating Disabled");
                break;
            case 7:
                LibrarySearchResultsScreen.Show(library.GamesAlphabetical, "Library by Name");
                break;
            case 8:
                LibrarySearchResultsScreen.Show(library.GamesRecentlyUpdated, "Recently Added/Upgraded Games");
                break;
            case 9:
                MainMenu.Show();
                break;
            default:
                Show();
                break;
        }
    }
}
