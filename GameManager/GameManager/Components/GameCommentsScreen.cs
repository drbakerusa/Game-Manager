namespace GameManager.Components;

public static class GameCommentsScreen
{
    private static IEnumerable<GameComment> _comments = new List<GameComment>();

    public static void Show(int gameId)
    {
        var library = new LibraryService();

        var game = library.GetGame(gameId);

        if ( game == null )
            MainMenu.Show();
        _ = game ?? throw new ArgumentNullException(nameof(game));

        Console.Clear();
        UIElements.PageTitle($"{game.DisplayName} Comments");

        _comments = library.GetGameComments(gameId);

        if ( _comments.Any() )
        {
            var count = 1;
            foreach ( var comment in _comments )
            {
                UIElements.Normal(UIElements.MenuOption((count++), comment.CommentDisplay));
            }
        }

        var options = new List<string>
        {
            "Add Comment",
            "Remove Comment",
            "Back to Game"
        };

        switch ( UIElements.Menu(options, showDivider: true) )
        {
            case 0:
                var comment = UIElements.TextInput("Enter comment");
                if ( !string.IsNullOrEmpty(comment) )
                    library.AddGameComment(gameId, comment);
                GameCommentsScreen.Show(gameId);
                break;
            case 1:
                var input = UIElements.TextInput("Enter comment number");
                if ( int.TryParse(input, out int index) )
                {
                    index--;
                    if ( index >= 0 && index < _comments.Count() )
                        library.RemoveGameComment(gameId, _comments.ElementAt(index));
                }
                GameCommentsScreen.Show(gameId);
                break;
            case 2:
                GameDetailsScreen.Show(gameId);
                break;
            default:
                GameCommentsScreen.Show(gameId);
                break;
        }
    }
}
