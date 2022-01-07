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
            "Add Game",
            "Check for Updates",
            $"Games with Updates ({library.NumberOfUpdatesAvailable})",
            $"Recently Played ({library.RecentlyPlayedGames.Count()})",
            $"Ready to Play ({library.ReadyToPlayGames.Count()})",
            $"Never Played ({library.GamesNeverPlayed.Count()})",
            $"Automatic Updates Disabled ({library.GamesManualUpdateCheck.Count()})",
            "By Name",
            "By Recently Added or Upgraded",
            "Main Menu"
        };

        var selection = UIElements.Menu(options);

        switch ( selection )
        {
            case 0:
                AddGameScreen.Show();
                break;
            case 1:
                UIElements.YellowHighlight("Checking for updates");
                library.CheckLibraryForUpdates();
                Show();
                break;
            case 2:
                LibraryDisplayScreen.Show(library.GamesWithUpdates, "Games with Updates");
                break;
            case 3:
                LibraryDisplayScreen.Show(library.RecentlyPlayedGames, "Recently Played");
                break;
            case 4:
                LibraryDisplayScreen.Show(library.ReadyToPlayGames, "Ready to Play");
                break;
            case 5:
                LibraryDisplayScreen.Show(library.GamesNeverPlayed, "Games Never Played");
                break;
            case 6:
                LibraryDisplayScreen.Show(library.GamesManualUpdateCheck, "Games with Automatic Updating Disabled");
                break;
            case 7:
                LibraryDisplayScreen.Show(library.GamesAlphabetical, "Library by Name");
                break;
            case 8:
                LibraryDisplayScreen.Show(library.GamesRecentlyUpdated, "Recently Added/Upgraded Games");
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
