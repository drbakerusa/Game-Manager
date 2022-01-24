namespace GameManager.Components
{
    public class LibrarySearchResultsScreen
    {
        public static void Show(IEnumerable<LibraryGame> games, string pageTitle, bool launchInsteadOfDetails = false)
        {
            var libraryService = new LibraryService();

            Console.Clear();
            UIElements.PageTitle(pageTitle);

            //if ( !games.Any() )
            //    AddGameScreen.Show();

            var selection = UIElements.PagedMenu(games.Select(g => g.Name).ToList(), pageTitle, "Blank to cancel");

            if ( selection is not null )
            {
                if ( launchInsteadOfDetails )
                {
                    libraryService.LaunchGame(games.ElementAt((int) selection).Id);
                    GameDetailsScreen.Show(games.ElementAt((int) selection).Id);
                }
                else
                    GameDetailsScreen.Show(games.ElementAt((int) selection).Id);
            }
            else
                LibraryMenu.Show();
        }
    }
}
