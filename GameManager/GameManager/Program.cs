
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

UIElements.Normal("Checking Database");
using ( var db = new DataContext() )
{
    if ( db.Database.GetPendingMigrations().Any() )
    {
        UIElements.Warning("Updating Schema");
        db.Database.GetService<IMigrator>().Migrate();
    }

    UIElements.Success("OK!");
    UIElements.Blank();
}

UIElements.Normal("Verifying Application Settings");
using ( var settingsService = new SettingsService() )
{
    var hasValidCredentials = settingsService.HasValidF95Credentials().Result;
    while ( !hasValidCredentials )
    {
        UIElements.Error($"F95 Credentials are invalid: {settingsService.HasValidF95Credentials().Reason}");
        UIElements.Error("Please update your credentials now");
        var input = UIElements.GetCredentials(settingsService.F95Credentials.Username);
        var result = settingsService.SetF95Credentials(input.Username, input.Password);
        hasValidCredentials = result.Result;
    }

    UIElements.Success("OK!");
    UIElements.Blank();
}

UIElements.Normal("Checking for Application Version");
using ( var gitHub = new GitHubService() )
{
    if ( await gitHub.CheckIfNewerVersionExists() )
        UIElements.Success("A newer version is available!");
    else
        UIElements.Success("On latest version");
}
UIElements.Blank();

UIElements.Normal("Fetching Game Metadata from F95");
using ( var loader = new LoaderService() )
{
    try
    {
        await loader.LoadMetadata();
        UIElements.Success("OK!");
    }
    catch ( Exception e )
    {
        UIElements.Error($"Cannot access F95: {e.Message}");
    }


    UIElements.Blank();
}

UIElements.Normal("Checking library for updates");
new LibraryService().CheckLibraryForUpdates();

UIElements.Success("OK!");
UIElements.Blank();

UIElements.Divider();

Console.Clear();

MainMenu.Show();
