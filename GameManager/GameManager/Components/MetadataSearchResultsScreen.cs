namespace GameManager.Components
{
    public class MetadataSearchResultsScreen
    {
        public static void Show(IEnumerable<GameMetadata> games, string pageTitle)
        {
            Console.Clear();
            UIElements.PageTitle(pageTitle);

            if ( !games.Any() )
                AddGameScreen.Show();

            var selection = UIElements.PagedMenu(games.Select(g => g.Name).ToList(), pageTitle, "Blank to cancel");

            if ( selection == null )
                AddGameScreen.Show();
            else
                MetadataDetailsScreen.Show(games.ElementAt((int) selection).Id);
        }
    }
}
