namespace GameManager.Data;

public class LibraryGame
{
    public int Id { get; set; }
    public int MetadataId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Prefixes { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public DateTime WhenUpdated { get; set; } = DateTime.Now;
    public bool HasUpgradeAvailable { get; set; } = false;
    public string AvailableUpgradeVersion { get; set; } = string.Empty;
    public DateTime? WhenUpgradeDiscovered { get; set; }
    public string ExecutablePath { get; set; } = string.Empty;
    public bool ManualUpdatesOnly { get; set; }
    public DateTime? WhenLastLaunched { get; set; }
    public int LaunchCount { get; set; }
    public IList<GameComment> Comments { get; set; } = new List<GameComment>();

    [NotMapped]
    public string DisplayName => $"{Name} [{Version}] [{Developer}] {Prefixes}";

    [NotMapped]
    public string LastPlayed => WhenLastLaunched is null ? "Never" : WhenLastLaunched.Humanize(utcDate: false);
}
