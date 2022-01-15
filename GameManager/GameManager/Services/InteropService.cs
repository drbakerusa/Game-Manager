using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameManager.Services
{
    public class InteropService
    {
        public void StartProcess(string command)
        {
            try
            {
                Process.Start(command);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
                {
                    command = command.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {command}") { CreateNoWindow = true });
                }
                else if ( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) )
                {
                    Process.Start("xdg-open", command);
                }
                else if ( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) )
                {
                    Process.Start("open", command);
                }
                else
                {
                    throw;
                }
            }
        }

        public void OpenUrl(string url) => StartProcess(url);
    }
}
