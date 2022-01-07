namespace GameManager.Components;

public static class MainMenu
{
    public static async void Show()
    {
        Console.Clear();
        UIElements.PageTitle("Main Menu");

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
            "Library",
            "Refresh Metadata from F95",
            "Quit Application"
        };

        var loader = new LoaderService();

        switch ( UIElements.Menu(options) )
        {
            case 0:
                LibraryMenu.Show();
                break;
            case 1:
                try
                {
                    if ( UIElements.Confirm("Get latest updates only", defaultResponse: false) )
                        await loader.LoadMetadata();
                    else if ( UIElements.Confirm("Refresh all metadata", defaultResponse: false) )
                        await loader.RefreshAllMetadata();
                }
                catch ( Exception e )
                {
                    UIElements.Error($"Cannot update metadata: {e.Message}");
                }
                Show();
                break;
            case 2:
                Environment.Exit(0);
                break;
            default:
                MainMenu.Show();
                break;
        }
    }
}
