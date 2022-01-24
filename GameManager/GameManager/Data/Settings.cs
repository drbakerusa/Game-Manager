namespace GameManager.Data;

public class Settings
{
    public int Id { get; set; } = 0;
    public string F95Username { get; set; } = string.Empty;
    public string F95Password { get; set; } = string.Empty;
    public int DefaultPageSize { get; set; } = 15;
    public bool AutomaticallyCheckForGameUpdates { get; set; } = false;
    public string MetadataUpdateControlId { get; set; } = "0";
    public string LatestApplicationVersion { get; set; } = string.Empty;
    public bool NewerVersionExists { get; set; }
    public int RecentThresholdDays { get; set; } = 7;
}
