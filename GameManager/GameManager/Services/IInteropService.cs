namespace GameManager.Services
{
    public interface IInteropService
    {
        void OpenUrl(string url);
        void StartProcess(string command);
    }
}