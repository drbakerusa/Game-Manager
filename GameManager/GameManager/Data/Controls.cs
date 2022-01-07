namespace GameManager.Data;

public class Controls
{
    public string MetadataUpdateControlId { get; set; } = "0";

    public string LatestApplicationVersion { get; set; } = string.Empty;
    public bool NewerVersionExists { get; set; }
}
