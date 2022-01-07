using System.Reflection;

namespace GameManager.Components;

public static class UIElements
{
    public static void Error(string message) => PrintMessageWithColor(ConsoleColor.Red, message);

    public static void Warning(string message) => YellowHighlight(message);
    public static void YellowHighlight(string message) => PrintMessageWithColor(ConsoleColor.Yellow, message);

    public static void Success(string message) => GreenHighlight(message);
    public static void GreenHighlight(string message) => PrintMessageWithColor(ConsoleColor.Green, message);

    public static void Blue(string message) => PrintMessageWithColor(ConsoleColor.Blue, message);

    public static void Normal(string message) => Console.WriteLine(message);

    public static void Blank() => Console.WriteLine();

    public static void Divider(bool fullwidth = true, bool includeSpace = false)
    {
        int width = 0;
        if ( fullwidth )
            width = Console.WindowWidth - 2;
        else
            width = 7;

        if ( includeSpace )
            Blank();

        Normal(("".PadLeft(width, '-')));

        if ( includeSpace )
            Blank();
    }

    public static void PageTitle(string title)
    {
        PageHeader();
        Blank();
        Console.WriteLine(title);
        Underline(title);
        Blank();
    }

    public static int Menu(List<string> options, bool showDivider = false)
    {
        if ( showDivider )
            Divider(fullwidth: false, includeSpace: true);

        foreach ( var option in options )
        {
            Normal(MenuOption(index: (options.IndexOf(option) + 1), optionText: option));
        }

        var selection = SelectionPrompt("Make a selection", numberOfOptions: options.Count);

        return selection is null ? -1 : (int) selection - 1;
    }

    public static int? PagedMenu(List<string> options, string listTitle, string promptCancelMessage, bool showDivider = false)
    {
        if ( showDivider )
            Divider(fullwidth: false, includeSpace: true);

        var pageSize = new SettingsService().DefaultPageSize;
        var selectionMade = false;
        var selectedItem = string.Empty;
        var currentPage = 1;
        var totalPages = CalculateTotalPages(options.Count);

        while ( !selectionMade )
        {
            Console.Clear();
            PageHeader();
            Blank();

            var optionsPage = new List<string>();
            var message = $"Page {currentPage} of {totalPages}";
            Normal(listTitle);
            Normal(message);
            Underline(message);
            Blank();

            optionsPage = options.Skip((currentPage - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();
            if ( currentPage != 1 )
                optionsPage.Add("Previous Page");
            if ( currentPage != totalPages )
                optionsPage.Add("Next Page");

            foreach ( var option in optionsPage )
            {
                Normal(MenuOption(index: (optionsPage.IndexOf(option) + 1), optionText: option));
            }

            var selection = SelectionPrompt($"Make a selection ({promptCancelMessage})", numberOfOptions: options.Count);

            if ( selection == null )
                return null;

            selectedItem = optionsPage[(int) selection - 1];

            if ( selectedItem == "Previous Page" )
                currentPage--;
            else if ( selectedItem == "Next Page" )
                currentPage++;
            else
                selectionMade = true;
        }

        return options.IndexOf(selectedItem);
    }

    public static string TextInput(string prompt)
    {
        Normal(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static bool Confirm(string message, bool defaultResponse = true)
    {
        Divider(fullwidth: false, includeSpace: true);

        var choices = defaultResponse ? "(Y/n)" : "(y/N)";
        Normal($"{message}? {choices}");
        var response = Console.ReadLine()?.ToLower();

        if ( defaultResponse == true )
            return string.IsNullOrEmpty(response) || response == "y";
        else
            return !string.IsNullOrEmpty(response) || response == "y";
    }

    public static string ConvertBoolToYesNo(bool value) => value ? "yes" : "no";

    public static string MenuOption(int index, string optionText) => $"{index,4}.     {optionText}";

    private static int? SelectionPrompt(string message, int numberOfOptions)
    {
        var selectionMade = false;
        int selection = int.MinValue;

        while ( !selectionMade )
        {
            Divider(fullwidth: false, includeSpace: true);

            Console.WriteLine(message);
            var input = Console.ReadLine();

            if ( string.IsNullOrEmpty(input) )
                return null;

            if ( int.TryParse(input, out selection) )
            {
                if ( selection > 0 && selection <= numberOfOptions )
                    selectionMade = true;
            }
        }

        return selection;
    }

    private static void PrintMessageWithColor(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void PageHeader()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = string.Empty;

        if ( version != null )
        {
            versionString = $"v{version.Major}.{version.Minor}.{version.Build}";

            using ( var settings = new SettingsService() )
            {
                if ( settings.NewerVersionExists )
                    versionString += $" [{settings.LatestApplicationVersion} is available!]";
            }

        }

        Blue($"F95 Game Manager {versionString}");
        Divider();
    }

    private static void Underline(string message) => Console.WriteLine("".PadRight(message.Length, '-'));

    private static int CalculateTotalPages(int numberOfItems)
        => (int) Math.Ceiling((decimal) numberOfItems / (decimal) new SettingsService().DefaultPageSize);


}
