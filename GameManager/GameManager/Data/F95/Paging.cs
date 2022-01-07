namespace GameManager.Data.F95;

public class Paging
{
    [JsonProperty(PropertyName = "page")]
    public int CurrentPage { get; set; }

    [JsonProperty(PropertyName = "total")]
    public int TotalPages { get; set; }
}
