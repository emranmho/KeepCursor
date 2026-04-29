using Microsoft.Win32;

namespace CursorKeep.Services
{
    public class StartupService
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "CursorKeep";

        public bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            return key?.GetValue(AppName) != null;
        }

        public void Enable(string exePath)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: true);
            key?.SetValue(AppName, $"\"{exePath}\"");
        }

        public void Disable()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: true);
            key?.DeleteValue(AppName, throwOnMissingValue: false);
        }

        // If user moved the exe after enabling startup, fix the registry path automatically
        public void SyncPath(string currentExePath)
        {
            if (IsEnabled())
                Enable(currentExePath);
        }
    }
}
