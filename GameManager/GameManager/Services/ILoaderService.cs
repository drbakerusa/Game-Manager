namespace GameManager.Services
{
    public interface ILoaderService
    {
        void Dispose();
        Task LoadMetadata();
        Task RefreshAllMetadata();
    }
}