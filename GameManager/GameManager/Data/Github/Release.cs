namespace GameManager.Data.Github;

public class Release
{
    [JsonProperty(PropertyName = "tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "published_at")]
    public DateTimeOffset WhenPublished { get; set; }

    [JsonIgnore]
    public Version? Version
    {
        get
        {
            if ( !string.IsNullOrEmpty(TagName) )
            {
                var versionString = TagName.Replace("v", string.Empty);
                return new Version(versionString);
            }
            return null;
        }
    }
}
