namespace MK11AccessibilityInstaller;

internal static class InstallerService
{
    private const string BackupFolderName = "MK11AccessibilityBackup";

    /// <summary>
    /// Yedek klasörünün yolunu döndürür.
    /// </summary>
    public static string BackupDirectory(string gamePath) => 
        Path.Combine(gamePath, BackupFolderName);

    /// <summary>
    /// ModPayload klasöründen mod dosyalarını oyun klasörüne kopyalar.
    /// Mevcut dosyaları yedekler.
    /// </summary>
    public static int Install(string gamePath, string payloadDirectory)
    {
        if (!Directory.Exists(gamePath))
            throw new DirectoryNotFoundException($"Oyun klasörü bulunamadı: {gamePath}");

        if (!Directory.Exists(payloadDirectory))
            return 0; // Payload yoksa kurulum yapma, sadece 0 döndür

        var copied = 0;
        var backupDir = BackupDirectory(gamePath);

        foreach (var source in Directory.EnumerateFiles(payloadDirectory, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(payloadDirectory, source);
            var destination = Path.Combine(gamePath, relative);
            
            // Hedef dizini oluştur
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            // Eğer dosya zaten varsa yedekle
            if (File.Exists(destination))
            {
                var backup = Path.Combine(backupDir, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                
                // Yedek yoksa oluştur (üzerine yazma)
                if (!File.Exists(backup))
                {
                    File.Copy(destination, backup, overwrite: false);
                }
            }

            // Mod dosyasını kopyala
            File.Copy(source, destination, overwrite: true);
            copied++;
        }

        return copied;
    }

    /// <summary>
    /// Yedeklerden oyun dosyalarını geri yükler.
    /// </summary>
    public static int Restore(string gamePath)
    {
        var backup = BackupDirectory(gamePath);
        
        if (!Directory.Exists(backup))
            return 0; // Yedek yoksa hiçbir şey yapma

        var restored = 0;

        foreach (var source in Directory.EnumerateFiles(backup, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(backup, source);
            var destination = Path.Combine(gamePath, relative);
            
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(source, destination, overwrite: true);
            restored++;
        }

        return restored;
    }
}
