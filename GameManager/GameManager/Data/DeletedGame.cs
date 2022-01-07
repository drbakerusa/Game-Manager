namespace GameManager.Data;

public class DeletedGame
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime WhenDeleted { get; set; } = DateTime.Now;
    public string ReasonForDeleting { get; set; } = string.Empty;

    [NotMapped]
    public string WhenDeletedFuzzy => WhenDeleted.Humanize(utcDate: false);
}
