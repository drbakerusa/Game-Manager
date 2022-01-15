namespace GameManager.Services
{
    public interface ISettingsService
    {
        bool CheckForUpdatesAutomatically { get; }
        string ControlId { get; set; }
        int DefaultPageSize { get; }
        string LatestApplicationVersion { get; set; }
        bool NewerVersionExists { get; set; }
        string SettingsFilePath { get; }

        void Dispose();
        Controls GetControls();
        Settings GetSettings();
        bool HasF95Credentials();
    }
}