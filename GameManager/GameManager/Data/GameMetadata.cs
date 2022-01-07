namespace GameManager.Data;

public class GameMetadata
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Prefixes { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public DateTime WhenRefreshed { get; set; }
}
