namespace GameManager.Interface
{
    public class UI
    {
        private readonly IInteropService _interop;

        public UI(IInteropService interop)
        {
            _interop = interop;
            Application.Init();
        }

        public void Start()
        {

            var top = Application.Top;

            var window = new Window(UIElements.ApplicationTitle)
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };

            var menu = new TopMenu(_interop);

            top.Add(menu, window);
            Application.Run();
        }
    }
}
