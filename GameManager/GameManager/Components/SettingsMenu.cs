namespace GameManager.Components
{
    public static class SettingsMenu
    {
        public static void Show()
        {
            Console.Clear();
            UIElements.PageTitle("Settings");

            var settingsService = new SettingsService();
            var settings = settingsService.GetSettings();

            var options = new List<string>
            {
                $"F95 Credentials => {settingsService.F95CredentialsString}",
                $"Default Page Size => {settings.DefaultPageSize}",
                $"Allow Automatic Game Update Checks by Default => {UIElements.ConvertBoolToYesNo(settings.AutomaticallyCheckForGameUpdates)}",
                $"Recent Threshold => {settings.RecentThresholdDays} days",
                "Main Menu"
            };

            var selection = UIElements.Menu(options);

            switch ( selection )
            {
                case 0: // F95 Credentials
                    var input = UIElements.GetCredentials(settingsService.F95Credentials.Username);
                    _ = settingsService.SetF95Credentials(input.Username, input.Password);
                    Show();
                    break;
                case 1: // Default Page Size
                    settingsService.DefaultPageSize = UIElements.IntegerInput(settings.DefaultPageSize) ?? new Settings().DefaultPageSize;
                    Show();
                    break;
                case 2: // Automatically Check for Game Updates
                    settingsService.CheckForUpdatesAutomatically = UIElements.Confirm("Allow Automatic Game Update Checks by Default", settingsService.CheckForUpdatesAutomatically);
                    Show();
                    break;
                case 3: // Recent Threshold
                    settingsService.RecentThresholdDays = UIElements.IntegerInput(settings.RecentThresholdDays) ?? new Settings().RecentThresholdDays;
                    Show();
                    break;
                case 4: // Main Menu
                    MainMenu.Show();
                    break;
                default:
                    Show();
                    break;
            }
        }
    }
}
