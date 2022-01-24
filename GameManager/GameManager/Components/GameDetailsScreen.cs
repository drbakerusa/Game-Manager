namespace GameManager.Components;

public class GameDetailsScreen
{
    public static void Show(int gameId)
    {
        var library = new LibraryService();

        var game = library.GetGame(gameId);

        if ( game == null )
            MainMenu.Show();
        _ = game ?? throw new ArgumentNullException(nameof(game));

        Console.Clear();
        UIElements.PageTitle(game.DisplayName);

        if ( game.HasUpgradeAvailable )
        {
            UIElements.YellowHighlight($"Version {game.AvailableUpgradeVersion} available!");
            UIElements.Blank();
        }

        var comment = library.GetMostRecentGameComment(gameId);
        if ( comment != null )
        {
            UIElements.Blue($"Latest Comment: {comment.CommentDisplay}");
            UIElements.Blank();
        }

        UIElements.Normal(game.Tags);
        UIElements.Blank();

        UIElements.Normal($"Last Played => {game.LastPlayed}");
        UIElements.Normal($"Number of Times Played => {game.LaunchCount}");
        UIElements.Blank();


        UIElements.Normal($"Check for updates Automatically? => {UIElements.ConvertBoolToYesNo(!game.ManualUpdatesOnly)}");

        var options = new List<string>
        {
            string.IsNullOrEmpty(game.ExecutablePath) ? "Add Game Launch" : "Launch Game",
            "View F95 Page",
            "Comments",
            game.HasUpgradeAvailable ? "Apply Update" : "Check for Updates",
            game.ManualUpdatesOnly ? "Allow Automatic Update Checks" : "Prevent Automatic Update Checks",
            string.IsNullOrEmpty(game.ExecutablePath) ? "Set Executable Path" : "Clear Executable Path",
            "Clear Play Statistics",
            "Delete Game",
            "More from Developer",
            "Back to Library"
        };

        switch ( UIElements.Menu(options, showDivider: true) )
        {
            case 0: // Launch Game
                if ( string.IsNullOrEmpty(game.ExecutablePath) )
                    library.LaunchGameManually(gameId);
                else
                {
                    try
                    {
                        library.LaunchGame(gameId);
                    }
                    catch ( Exception e )
                    {
                        UIElements.Error($"Unable to set path: {e.Message}");
                        _ = UIElements.TextInput("Enter to continue");
                    }
                }
                Show(gameId);
                break;
            case 1: // View F95 Page
                try
                {
                    library.BrowseGameForum(gameId);
                }
                catch ( Exception e )
                {
                    UIElements.Error($"Unable to open forum: {e.Message}");
                    _ = UIElements.TextInput("Enter to continue");
                }
                Show(gameId);
                break;
            case 2: // Comments
                GameCommentsScreen.Show(gameId);
                break;
            case 3: // Check/Apply Updates
                if ( game.HasUpgradeAvailable )
                {
                    if ( UIElements.Confirm($"Upgrade to version {game.AvailableUpgradeVersion}") )
                        library.UpdateGame(gameId);
                }
                else
                {
                    library.CheckGameForUpdates(gameId, loadMetadata: true, showSuccessMessage: true);
                }
                Show(gameId);
                break;
            case 4: // Toggle Automatic Update Check
                library.SetGameManualUpdatesOnly(gameId, !game.ManualUpdatesOnly);
                Show(gameId);
                break;
            case 5: // Set/Clear executable path
                if ( string.IsNullOrEmpty(game.ExecutablePath) )
                {
                    var path = UIElements.TextInput("Enter path to executable file for game");
                    try
                    {
                        library.SetGameExecutablePath(gameId, path);
                    }
                    catch ( FileNotFoundException )
                    {
                        UIElements.Error("File not found");
                        _ = UIElements.TextInput("Enter to continue");
                    }
                    catch ( Exception e )
                    {
                        UIElements.Error($"Unable to set path: {e.Message}");
                        _ = UIElements.TextInput("Enter to continue");
                    }
                }
                else
                    library.ClearGameExecutablePath(gameId);
                Show(gameId);
                break;
            case 6: // Clear launch statistics
                if ( UIElements.Confirm("Are you sure you want to clear statistics", defaultResponse: false) )
                    library.ResetGameLaunchStatistics(gameId);
                Show(gameId);
                break;
            case 7: // Delete game
                if ( UIElements.Confirm("Are you sure", defaultResponse: false) )
                {
                    library.DeleteGame(gameId, UIElements.TextInput("Reason for deleting (blank for none)"));
                    LibraryMenu.Show();
                }
                Show(gameId);
                break;
            case 8: // More from developer
                MetadataSearchResultsScreen.Show(library.GetMetadataByDeveloperName(game.Developer), $"Search for '{game.Developer}'", game.Id);
                break;
            case 9: // Return to library
                LibraryMenu.Show();
                break;
            default:
                Show(gameId);
                break;
        }
    }
}
