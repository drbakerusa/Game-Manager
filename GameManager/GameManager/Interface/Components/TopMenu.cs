namespace GameManager.Interface.Components
{
    public class TopMenu : MenuBar
    {
        public TopMenu()
        {
            Menus = new MenuBarItem[]
            {
                FileMenu
            };
        }

        private MenuBarItem FileMenu => new MenuBarItem("_File", new MenuItem[]
        {
            new MenuItem("_Quit", "Exit 95Games", () => Application.RequestStop())
        });
    }
}
