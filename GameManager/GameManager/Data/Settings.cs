namespace GameManager.Data;

public class Settings
{
    public string F95Username { get; set; } = string.Empty;
    public string F95Password { get; set; } = string.Empty;
    public int DefaultPageSize { get; set; }
    public bool AutomaticallyCheckForGameUpdates { get; set; } = false;
}
