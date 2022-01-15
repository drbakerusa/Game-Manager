namespace GameManager.Services
{
    public interface IGitHubService
    {
        Task<bool> CheckIfNewerVersionExists();
        void Dispose();
    }
}