namespace GameManager.Data;

public class DataContext : DbContext
{
    public DbSet<GameMetadata> GameMetadata => Set<GameMetadata>();
    public DbSet<LibraryGame> Library => Set<LibraryGame>();
    public DbSet<DeletedGame> DeletedGames => Set<DeletedGame>();
    public DbSet<Settings> Settings => Set<Settings>();

    public string DbPath { get; set; } = string.Empty;

    public DataContext()
    {

        var path = Path.Join((Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), "F95 Game Manager");

        if ( !Directory.Exists(path) )
            Directory.CreateDirectory(path);

        if ( CurrentEnviroment.IsDevelopment )
            DbPath = Path.Join(path, "GameManager.dev.db");
        else
            DbPath = Path.Join(path, "GameManager.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
