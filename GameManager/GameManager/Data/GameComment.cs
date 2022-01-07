namespace GameManager.Data
{
    public class GameComment
    {
        public int Id { get; set; }
        public LibraryGame Game { get; set; } = new LibraryGame();
        public int GameId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string Comment { get; set; } = string.Empty;
        public string VersionCommentedOn { get; set; } = string.Empty;

        [NotMapped]
        public string CommentDisplay
        {
            get
            {
                var version = string.IsNullOrEmpty(VersionCommentedOn) ? string.Empty : $" [{VersionCommentedOn}]";
                return $"[{TimeStamp.Humanize(utcDate: false)}]{version} {Comment}";
            }
        }
    }
}
