namespace GameManager.Components;

public class MetadataDetailsScreen
{
    public static void Show(int metadataId)
    {
        var library = new LibraryService();

        GameMetadata? metadata = library.GetGameMetadata(metadataId);

        if ( metadata == null )
            AddGameScreen.Show();
        _ = metadata ?? throw new ArgumentNullException(nameof(metadata));

        Console.Clear();
        UIElements.PageTitle($"{metadata.Name} [{metadata.Version}] [{metadata.Developer}]");

        if ( library.GameIsInLibrary(metadataId) )
        {
            UIElements.YellowHighlight("Game is in library");
            UIElements.Blank();
        }

        if ( library.GamePreviouslyDeleted(metadataId) )
        {
            var deletedGame = library.GetDeletedGame(metadataId);
            var reason = string.IsNullOrEmpty(deletedGame.ReasonForDeleting) ? "No Reason Given" : $"Reason: {deletedGame.ReasonForDeleting}";
            UIElements.YellowHighlight($"Version {deletedGame.Version} was previously in library. You deleted it {deletedGame.WhenDeletedFuzzy}.");
            UIElements.YellowHighlight(reason);
            UIElements.Blank();
        }

        UIElements.Normal(metadata.Prefixes);
        UIElements.Blank();

        UIElements.Normal("Tags:");
        UIElements.Normal(metadata.Tags);

        var options = new List<string>
        {
            library.GameIsInLibrary(metadataId) ? "View game in library" : "Add game to library",
            "View F95 Page",
            "Return to Search"
        };

        switch ( UIElements.Menu(options, showDivider: true) )
        {
            case 0: // Add/View Game
                if ( library.GameIsInLibrary(metadataId) )
                {
                    ViewInLibrary(metadataId);
                }
                else
                {
                    library.AddGameToLibrary(metadataId);
                    ViewInLibrary(metadataId);
                }
                break;
            case 1: // View on F95
                library.BrowseGameForum(metadataId, isMetadataId: true);
                Show(metadataId);
                break;
            case 2: // Return to search
                AddGameScreen.Show();
                break;
            default:
                Show(metadataId);
                break;
        }
    }

    private static void ViewInLibrary(int metadataId)
    {
        var library = new LibraryService();

        var gameId = library.GetGameId(metadataId);
        _ = gameId ?? throw new ArgumentNullException(nameof(gameId));
        GameDetailsScreen.Show((int) gameId);
    }
}
