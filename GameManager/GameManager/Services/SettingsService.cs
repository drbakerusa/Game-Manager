namespace GameManager.Services;

public class SettingsService : IDisposable
{
    private readonly string _settingsFilePath;
    private readonly string _dataFolderPath;

    public SettingsService()
    {
        _dataFolderPath = Path.Join((Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), "F95 Game Manager");

        if ( CurrentEnviroment.IsDevelopment )
            _settingsFilePath = Path.Combine(_dataFolderPath, "Settings.dev.json");
        else
            _settingsFilePath = Path.Combine(_dataFolderPath, "Settings.json");
        InitializeSettings();
    }

    public Settings GetSettings() => JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFilePath)) ?? new Settings();
    public string SettingsFilePath => _settingsFilePath;
    public bool CheckForUpdatesAutomatically => GetSettings().AutomaticallyCheckForGameUpdates;

    public string ControlId
    {
        get => GetSettings().MetadataUpdateControlId;
        set
        {
            var settings = GetSettings();
            settings.MetadataUpdateControlId = value;
            SaveSettings(settings);
        }
    }

    public int DefaultPageSize => GetSettings().DefaultPageSize;

    public bool HasF95Credentials()
    {
        var settings = GetSettings();
        return !string.IsNullOrEmpty(settings.F95Username) && !string.IsNullOrEmpty(settings.F95Password);
    }

    private void InitializeSettings()
    {
        if ( !Directory.Exists(_dataFolderPath) )
            Directory.CreateDirectory(_dataFolderPath);

        if ( !File.Exists(_settingsFilePath) )
            SaveSettings(new Settings());

        // add new settings here
        // this will update users with older settings schemas
        var settings = GetSettings();

        if ( settings.DefaultPageSize == 0 )
            settings.DefaultPageSize = 15;

        SaveSettings(settings);
    }

    private void SaveSettings(Settings settings) => File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(settings));

    public void Dispose()
    {
    }

}
