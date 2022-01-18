namespace GameManager.Components;

public static class AddGameScreen
{
    public static void Show()
    {
        var library = new LibraryService();

        Console.Clear();
        UIElements.PageTitle("Add Game");

        var options = new List<string>
        {
            "By Thread ID",
            "By Forum URL",
            "By Name",
            "By Developer Name",
            "By Prefix",
            "By Tag",
            "Return to Library"
        };

        switch ( UIElements.Menu(options) )
        {
            case 0: // By Thread ID
                var threadID = UIElements.IntegerInput(null, "Enter F95 Thread ID (leave empty to cancel)");
                if ( threadID is not null )
                {
                    MetadataDetailsScreen.Show((int) threadID);
                }
                Show();
                break;
            case 1: // By Forum URL
                var metadata = library.GetGameMetadata(UIElements.TextInput("Enter Forum URL"));
                if ( metadata is not null )
                    MetadataDetailsScreen.Show(metadata.Id);
                Show();
                break;
            case 2: // By Name
                Show();
                break;
            case 3: // By Developer
                Show();
                break;
            case 4: // By Prefix
                Show();
                break;
            case 5: // By Tag
                Show();
                break;
            case 6: // Return to library
                LibraryMenu.Show();
                break;
            default:
                Show();
                break;
        }

    }
}
