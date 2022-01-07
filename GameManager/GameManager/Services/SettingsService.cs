namespace GameManager.Services;

public class SettingsService : IDisposable
{
    private readonly string _settingsFilePath;
    private readonly string _controlFilePath;
    private readonly string _dataFolderPath;

    public SettingsService()
    {
        _dataFolderPath = Path.Join((Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), "F95 Game Manager");

        if ( CurrentEnviroment.IsDevelopment )
        {
            _settingsFilePath = Path.Combine(_dataFolderPath, "Settings.dev.json");
            _controlFilePath = Path.Combine(_dataFolderPath, "Controls.dev.json");
        }
        else
        {
            _settingsFilePath = Path.Combine(_dataFolderPath, "Settings.json");
            _controlFilePath = Path.Combine(_dataFolderPath, "Controls.json");
        }
        InitializeSettings();
    }

    public Settings GetSettings() => JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFilePath)) ?? new Settings();
    public Controls GetControls() => JsonConvert.DeserializeObject<Controls>(File.ReadAllText(_controlFilePath)) ?? new Controls();
    public string SettingsFilePath => _settingsFilePath;
    public bool CheckForUpdatesAutomatically => GetSettings().AutomaticallyCheckForGameUpdates;

    public string LatestApplicationVersion
    {
        get => GetControls().LatestApplicationVersion;
        set
        {
            var controls = GetControls();
            controls.LatestApplicationVersion = value;
            SaveControls(controls);
        }
    }

    public bool NewerVersionExists
    {
        get => GetControls().NewerVersionExists;
        set
        {
            var controls = GetControls();
            controls.NewerVersionExists = value;
            SaveControls(controls);
        }
    }

    public string ControlId
    {
        get => GetControls().MetadataUpdateControlId;
        set
        {
            var controls = GetControls();
            controls.MetadataUpdateControlId = value;
            SaveControls(controls);
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

        if ( !File.Exists(_controlFilePath) )
            SaveControls(new Controls());

        // add new settings here
        // this will update users with older settings schemas
        var settings = GetSettings();

        if ( settings.DefaultPageSize == 0 )
            settings.DefaultPageSize = 15;

        SaveSettings(settings);
    }

    private void SaveSettings(Settings settings) => File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(settings));

    private void SaveControls(Controls controls) => File.WriteAllText(_controlFilePath, JsonConvert.SerializeObject(controls));

    public void Dispose()
    {
    }

}
