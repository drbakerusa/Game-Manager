namespace GameManager.Data.F95;

public class Result
{
    [JsonProperty(PropertyName = "status")]
    public string? Status { get; set; }

    [JsonProperty(PropertyName = "msg")]
    public Content Content { get; set; } = new Content();
}
