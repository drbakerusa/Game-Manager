namespace GameManager.Components;

public static class LibraryDisplayScreen
{
    public static void Show(IEnumerable<LibraryGame> games, string pageTitle)
    {
        Console.Clear();
        UIElements.PageTitle(pageTitle);

        if ( !games.Any() )
            LibraryMenu.Show();

        var selection = UIElements.PagedMenu(games.Select(g => g.DisplayName).ToList(), pageTitle, "Blank to cancel");

        if ( selection == null )
            LibraryMenu.Show();
        else
            GameDetailsScreen.Show(games.ElementAt((int) selection).Id);
    }
}
