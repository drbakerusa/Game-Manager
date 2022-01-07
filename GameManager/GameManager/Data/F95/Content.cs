

namespace GameManager.Data.F95;

public class Content
{
    [JsonProperty(PropertyName = "data")]
    public List<Game> Games { get; set; } = new List<Game>();
    [JsonProperty(PropertyName = "pagination")]
    public Paging Pages { get; set; } = new Paging();

    [JsonProperty(PropertyName = "count")]
    public int RecordCount { get; set; }
}
