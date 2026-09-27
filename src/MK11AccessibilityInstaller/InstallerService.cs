namespace MK11AccessibilityInstaller;

internal static class InstallerService
{
    private const string BackupFolderName = "MK11AccessibilityBackup";

    public static string BackupDirectory(string gamePath) => Path.Combine(gamePath, BackupFolderName);

    public static int Install(string gamePath, string payloadDirectory, Action<string>? log = null)
    {
        if (!Directory.Exists(gamePath)) throw new DirectoryNotFoundException($"Oyun klasörü bulunamadı: {gamePath}");
        if (!Directory.Exists(payloadDirectory)) return 0;

        var copied = 0;
        foreach (var relative in PayloadManifest.Read(payloadDirectory))
        {
            var source = Path.GetFullPath(Path.Combine(payloadDirectory, relative));
            var destination = Path.GetFullPath(Path.Combine(gamePath, relative));
            var payloadRoot = Path.GetFullPath(payloadDirectory) + Path.DirectorySeparatorChar;
            var gameRoot = Path.GetFullPath(gamePath) + Path.DirectorySeparatorChar;
            if (!source.StartsWith(payloadRoot, StringComparison.OrdinalIgnoreCase) ||
                !destination.StartsWith(gameRoot, StringComparison.OrdinalIgnoreCase)) continue;
            if (!File.Exists(source)) throw new FileNotFoundException($"Manifest dosyası bulunamadı: {relative}");

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            if (File.Exists(destination))
            {
                var backup = Path.Combine(BackupDirectory(gamePath), relative);
                Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                if (!File.Exists(backup)) File.Copy(destination, backup);
            }
            File.Copy(source, destination, true);
            log?.Invoke(relative);
            copied++;
        }
        File.WriteAllText(Path.Combine(gamePath, ".mk11-accessibility-installed"), DateTimeOffset.UtcNow.ToString("O"));
        return copied;
    }

    public static int Restore(string gamePath)
    {
        var backup = BackupDirectory(gamePath);
        if (!Directory.Exists(backup)) return 0;
        var restored = 0;
        foreach (var source in Directory.EnumerateFiles(backup, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(backup, source);
            if (!PayloadManifest.IsSafeRelativePath(relative)) continue;
            var destination = Path.Combine(gamePath, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(source, destination, true);
            restored++;
        }
        return restored;
    }
}
