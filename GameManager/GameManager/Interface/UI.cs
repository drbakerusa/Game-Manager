namespace GameManager.Interface
{
    public class UI
    {
        private readonly InteropService _interop;

        public UI(InteropService interop)
        {
            _interop = interop;
        }

        public void Start()
        {
            Application.Init();

            var top = Application.Top;
            var topFrame = top.Frame;

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
