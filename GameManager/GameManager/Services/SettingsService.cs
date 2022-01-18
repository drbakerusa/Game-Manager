namespace GameManager.Services;

public class SettingsService : IDisposable
{
    private DataContext _dataContext;

    public SettingsService()
    {
        _dataContext = new DataContext();
    }

    public Settings GetSettings() => _dataContext.Settings.FirstOrDefault() ?? new Settings();
    public bool CheckForUpdatesAutomatically => GetSettings().AutomaticallyCheckForGameUpdates;

    public string LatestApplicationVersion
    {
        get => GetSettings().LatestApplicationVersion;
        set
        {
            var controls = GetSettings();
            controls.LatestApplicationVersion = value;
            SaveSettings(controls);
        }
    }

    public bool NewerVersionExists
    {
        get => GetSettings().NewerVersionExists;
        set
        {
            var controls = GetSettings();
            controls.NewerVersionExists = value;
            SaveSettings(controls);
        }
    }

    public string ControlId
    {
        get => GetSettings().MetadataUpdateControlId;
        set
        {
            var controls = GetSettings();
            controls.MetadataUpdateControlId = value;
            SaveSettings(controls);
        }
    }

    public (string Username, string Password) F95Credentials
    {
        get
        {
            var settings = GetSettings();
            return (settings.F95Username, settings.F95Password);
        }
    }

    public int DefaultPageSize => GetSettings().DefaultPageSize;

    public (bool Result, string Reason) HasValidF95Credentials()
    {
        var settings = GetSettings();

        if ( string.IsNullOrEmpty(settings.F95Username) || string.IsNullOrEmpty(settings.F95Password) )
            return (Result: false, Reason: "Username/Password is empty");

        return (Result: true, Reason: "Success");

    }

    public (bool Result, string Reason) SetF95Credentials(string username, string password)
    {
        if ( string.IsNullOrEmpty(username) )
            return (Result: false, Reason: "Username is required");

        if ( string.IsNullOrEmpty(password) )
            return (Result: false, Reason: "Password is required");

        var settings = GetSettings();
        settings.F95Username = username;
        settings.F95Password = password;
        SaveSettings(settings);

        return (Result: true, Reason: "Success");
    }

    private void SaveSettings(Settings settings)
    {
        if ( settings.Id == 0 )
            _dataContext.Add(settings);

        _dataContext.SaveChanges();
    }

    public void Dispose()
    {
    }
}
