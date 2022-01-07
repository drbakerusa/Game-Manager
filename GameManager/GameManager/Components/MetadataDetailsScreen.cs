namespace GameManager.Components;

public class MetadataDetailsScreen
{
    public static void Show(int metadataId)
    {
        var library = new LibraryService();

        var metadata = library.GetGameMetadata(metadataId);

        if ( metadata == null )
            MainMenu.Show();
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

        if ( library.GameIsInLibrary(metadataId) )
        {
            if ( UIElements.Confirm("View game in library") )
                ViewInLibrary(metadataId);
            else
                AddGameScreen.Show();
        }
        else
        {
            if ( UIElements.Confirm("Add game to library") )
            {
                library.AddGameToLibrary(metadataId);
                ViewInLibrary(metadataId);
            }
            else
                AddGameScreen.Show();
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
