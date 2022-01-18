namespace GameManager.Components
{
    public class MetadataSearchResultsScreen
    {
        public static void Show(IEnumerable<GameMetadata> games, string pageTitle, int returnGameId = 0)
        {
            Console.Clear();
            UIElements.PageTitle(pageTitle);

            if ( !games.Any() )
                AddGameScreen.Show();

            var selection = UIElements.PagedMenu(games.Select(g => g.Name).ToList(), pageTitle, "Blank to cancel");

            if ( selection == null )
            {
                if ( returnGameId == 0 )
                    AddGameScreen.Show();
                else
                    GameDetailsScreen.Show(returnGameId);
            }
            else
                MetadataDetailsScreen.Show(games.ElementAt((int) selection).Id);
        }
    }
}
