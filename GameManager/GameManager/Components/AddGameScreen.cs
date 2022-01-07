namespace GameManager.Components;

public static class AddGameScreen
{
    public static void Show()
    {
        var library = new LibraryService();

        Console.Clear();
        UIElements.PageTitle("Add Game");

        var entry = UIElements.TextInput(prompt: "Enter F95 Thread ID (leave empty to cancel)");

        if ( string.IsNullOrEmpty(entry) )
        {
            LibraryMenu.Show();
        }
        else
        {
            int metadataId = 0;

            if ( int.TryParse(entry, out metadataId) && library.MetadataExists(metadataId) )
            {
                MetadataDetailsScreen.Show(metadataId);
            }
            else
            {
                Retry();
            }
        }
    }

    private static void Retry()
    {
        if ( UIElements.Confirm("Not found. Try again") )
            Show();
        else
            LibraryMenu.Show();
    }
}
