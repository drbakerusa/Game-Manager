namespace GameManager.Interface
{
    public class UI
    {
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

            var menu = new TopMenu();

            top.Add(menu, window);
            Application.Run();
        }
    }
}
