using System.Reflection;

namespace GameManager.Components;

public static class CurrentEnviroment
{
    public static bool IsDevelopment
    {
        get
        {
            var assemblyPath = Assembly.GetEntryAssembly()?.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            if ( assemblyDirectory != null && assemblyDirectory.Contains(@"\Debug") )
                return true;
            return false;
        }
    }
}
