namespace GameManager.Data.F95;

public class Game
{
    [JsonProperty(PropertyName = "thread_id")]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string? Title { get; set; }

    [JsonProperty(PropertyName = "creator")]
    public string? Developer { get; set; }

    [JsonProperty(PropertyName = "version")]
    public string? Version { get; set; }

    [JsonProperty(PropertyName = "prefixes")]
    public List<int> Prefixes { get; set; } = new List<int>();

    [JsonProperty(PropertyName = "tags")]
    public List<int> Tags { get; set; } = new List<int>();

    [JsonProperty(PropertyName = "rating")]
    public double? Rating { get; set; }

    [JsonProperty(PropertyName = "cover")]
    public string CoverImageUrl { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "screens")]
    public List<string> ScreenshotUrls { get; set; } = new List<string>();

    [JsonProperty(PropertyName = "date")]
    public string? FuzzyTime { get; set; }

    [JsonProperty(PropertyName = "new")]
    public bool IsNew { get; set; }
}
