namespace GameManager.Interface.Components
{
    public class TopMenu : MenuBar
    {
        private readonly IInteropService _interop;

        public TopMenu(IInteropService interop)
        {
            _interop = interop;

            Menus = new MenuBarItem[]
            {
                FileMenu
            };
        }

        private MenuBarItem FileMenu => new MenuBarItem("_File", new MenuItem[]
        {
            new MenuItem("Latest _Release","", () => _interop.OpenUrl("https://github.com/drbakerusa/Game-Manager/releases/latest")),
            new MenuItem("_Quit", "", () => Application.RequestStop())
        });
    }
}
