using Microsoft.Win32;

namespace MK11AccessibilityInstaller;

internal static class SteamLocator
{
    private const string GameFolderName = "Mortal Kombat 11";
    private const string GameAppId = "976310";

    public static string? FindGame()
    {
        foreach (var root in GetSteamRoots())
        {
            foreach (var library in GetLibraries(root))
            {
                var candidate = Path.Combine(library, "steamapps", "common", GameFolderName);
                var manifest = Path.Combine(library, "steamapps", $"appmanifest_{GameAppId}.acf");
                if (Directory.Exists(candidate) && File.Exists(manifest)) return candidate;
            }
        }
        return null;
    }

    private static IEnumerable<string> GetLibraries(string root)
    {
        yield return root;
        var file = Path.Combine(root, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(file)) yield break;
        foreach (var line in File.ReadLines(file))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith("\"path\"", StringComparison.OrdinalIgnoreCase)) continue;
            var parts = trimmed.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3)
            {
                var path = parts[^1].Replace("\\\\", "\\");
                if (Directory.Exists(path)) yield return path;
            }
        }
    }

    private static IEnumerable<string> GetSteamRoots()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.OpenSubKey(@"SOFTWARE\Valve\Steam");
                if (key?.GetValue("InstallPath") is string path && Directory.Exists(path)) roots.Add(path);
            }
            catch (SecurityException) { }
        }
        return roots;
    }
}
