﻿
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

Console.Title = UIElements.ApplicationTitle;

UIElements.PageTitle("Loading . . .");

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
    if ( !settingsService.HasF95Credentials() )
    {
        UIElements.Error($"F95 credentials are not set in the settings file. Update the settings file ({settingsService.SettingsFilePath}) and restart the application.");
        _ = UIElements.TextInput("Press ENTER to close application");
        Environment.Exit(1);
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
