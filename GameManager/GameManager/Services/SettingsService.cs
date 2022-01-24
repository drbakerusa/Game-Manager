using System.Security.Cryptography;
using System.Text;

namespace GameManager.Services;

public class SettingsService : IDisposable
{
    private DataContext _dataContext;

    public SettingsService()
    {
        _dataContext = new DataContext();
    }

    public Settings GetSettings() => _dataContext.Settings.FirstOrDefault() ?? new Settings();
    public bool CheckForUpdatesAutomatically
    {
        get => GetSettings().AutomaticallyCheckForGameUpdates;
        set
        {
            var settings = GetSettings();
            settings.AutomaticallyCheckForGameUpdates = value;
            SaveSettings(settings);
        }
    }

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
        get => string.IsNullOrEmpty(GetSettings().MetadataUpdateControlId) ? "0" : GetSettings().MetadataUpdateControlId;
        set
        {
            var settings = GetSettings();
            settings.MetadataUpdateControlId = value;
            SaveSettings(settings);
        }
    }

    public (string Username, string Password) F95Credentials
    {
        get
        {
            var settings = GetSettings();
            return (settings.F95Username, Decrypt(settings.F95Password));
        }
    }

    public string F95CredentialsString
    {
        get
        {
            var credentials = F95Credentials;
            credentials.Password = $"{credentials.Password.Substring(0, 2)}{"".PadLeft(credentials.Password.Length - 2, '*')}";
            return $"{credentials.Username} / {credentials.Password}";
        }
    }

    public int DefaultPageSize
    {
        get => GetSettings().DefaultPageSize;
        set
        {
            var settings = GetSettings();
            settings.DefaultPageSize = value;
            SaveSettings(settings);
        }
    }

    public int RecentThresholdDays
    {
        get => GetSettings().RecentThresholdDays;
        set
        {
            var settings = GetSettings();
            settings.RecentThresholdDays = value;
            SaveSettings(settings);
        }
    }

    public DateTime RecentThreshold => DateTime.Today.AddDays(-1 * RecentThresholdDays);

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
        settings.F95Password = Encrypt(password);
        SaveSettings(settings);

        return (Result: true, Reason: "Success");
    }

    private void SaveSettings(Settings settings)
    {
        if ( settings.Id == 0 )
            _dataContext.Add(settings);

        _dataContext.SaveChanges();
    }

    private string Encrypt(string clearText)
    {
        var result = string.Empty;
        string EncryptionKey = "e44244c6-1c7f-4984-9675-169e7321555f";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using ( Aes encryptor = Aes.Create() )
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using ( MemoryStream ms = new MemoryStream() )
            {
                using ( CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write) )
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                result = Convert.ToBase64String(ms.ToArray());
            }
        }
        return result;
    }

    private string Decrypt(string cipherText)
    {
        var result = string.Empty;
        string EncryptionKey = "e44244c6-1c7f-4984-9675-169e7321555f";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using ( Aes encryptor = Aes.Create() )
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using ( MemoryStream ms = new MemoryStream() )
            {
                using ( CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write) )
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                result = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return result;
    }

    public void Dispose()
    {
    }
}
