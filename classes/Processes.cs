using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Reflection;
using System.Security.Principal;

namespace PerformanceTool.classes
{
    public class Processes
    {
        public bool isAdministrator;

        public Processes()
        {
            this.isAdministrator = IsAdministrator();
        }

        public void DisableXboxGameBar()
        {
            try
            {
                if (isAdministrator)
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.AddScript("Set-ExecutionPolicy RemoteSigned -Scope Process -Force");
                        powerShell.AddScript("Get-AppxPackage -AllUsers Microsoft.XboxGamingOverlay | Remove-AppxPackage");
                        powerShell.Invoke();
                    }
                }
                else
                {
                    RestartAsAdministrator();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing script to delete Xbox Game Bar", ex);
            }
            finally
            {
                System.Console.WriteLine("GameBar successfully deleted");
            }
        }

        private bool IsAdministrator()
        {

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RestartAsAdministrator()
        {
            var exeName = Assembly.GetEntryAssembly().Location;
            var startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas"
            };
            Process.Start(startInfo);
        }
    }
}
