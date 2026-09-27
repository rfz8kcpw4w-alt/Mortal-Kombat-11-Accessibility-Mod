using Microsoft.Win32;

namespace MK11AccessibilityInstaller;

internal static class SteamLocator
{
    private const string GameFolderName = "Mortal Kombat 11";
    private const string GameAppId = "976310"; // MK11 Steam App ID

    /// <summary>
    /// Steam kütüphanelerinde Mortal Kombat 11'i arar ve klasör yolunu döndürür.
    /// </summary>
    public static string? FindGame()
    {
        foreach (var root in GetSteamRoots())
        {
            // Ana Steam dizininde ara
            var direct = Path.Combine(root, "steamapps", "common", GameFolderName);
            if (Directory.Exists(direct)) return direct;

            // Ek kütüphanelerde ara
            var libraries = Path.Combine(root, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraries)) continue;

            foreach (var library in ReadLibraryPaths(libraries))
            {
                var candidate = Path.Combine(library, "steamapps", "common", GameFolderName);
                if (Directory.Exists(candidate)) return candidate;
            }
        }
        return null;
    }

    /// <summary>
    /// Tüm Steam kök dizinlerini kayıt defterinden bulur.
    /// </summary>
    private static IEnumerable<string> GetSteamRoots()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        {
            foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                    using var key = baseKey.OpenSubKey(@"SOFTWARE\Valve\Steam");
                    var path = key?.GetValue("InstallPath") as string;
                    
                    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                    {
                        roots.Add(path);
                    }
                }
                catch (SecurityException) { /* Erişim yok, devam et */ }
            }
        }
        
        return roots;
    }

    /// <summary>
    /// libraryfolders.vdf dosyasından ek kütüphane yollarını okur.
    /// </summary>
    private static IEnumerable<string> ReadLibraryPaths(string file)
    {
        try
        {
            foreach (var line in File.ReadLines(file))
            {
                var trimmed = line.Trim();
                
                if (!trimmed.StartsWith("\"path\"", StringComparison.OrdinalIgnoreCase))
                    continue;

                var parts = trimmed.Split('"', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var path = parts[^1].Replace("\\\\", "\\");
                    if (Directory.Exists(path))
                    {
                        yield return path;
                    }
                }
            }
        }
        catch (IOException) { /* Dosya okunamadı */ }
    }
}
