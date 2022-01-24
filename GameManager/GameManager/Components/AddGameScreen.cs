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
            "Recently Updated",
            "Search By Name",
            "Search By Developer Name",
            "Search By Prefix",
            "Search By Tag",
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
            case 2: // Recently Updated
                MetadataSearchResultsScreen.Show(library.RecentlyUpdatedMetadata, "Recently Updated");
                Show();
                break;
            case 3: // By Name
                var nameSearch = UIElements.TextInput("Enter search string");
                if ( !string.IsNullOrEmpty(nameSearch) )
                    MetadataSearchResultsScreen.Show(library.GetMetadataByGameName(nameSearch), $"Search for '{nameSearch}'");
                Show();
                break;
            case 4: // By Developer
                var developerSearch = UIElements.TextInput("Enter search string");
                if ( !string.IsNullOrEmpty(developerSearch) )
                    MetadataSearchResultsScreen.Show(library.GetMetadataByDeveloperName(developerSearch), $"Search for '{developerSearch}'");
                Show();
                break;
            case 5: // By Prefix
                TagPickerScreen.Show(TagType.Prefix, StoreType.Metadata);
                Show();
                break;
            case 6: // By Tag
                TagPickerScreen.Show(TagType.Tag, StoreType.Metadata);
                Show();
                break;
            case 7: // Return to library
                LibraryMenu.Show();
                break;
            default:
                Show();
                break;
        }

    }
}
