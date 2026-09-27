using System.Text;

namespace MK11AccessibilityInstaller;

internal static class PayloadManifest
{
    public static IReadOnlyList<string> Read(string payloadDirectory)
    {
        var manifest = Path.Combine(payloadDirectory, "manifest.txt");
        if (!File.Exists(manifest))
        {
            return Directory.EnumerateFiles(payloadDirectory, "*", SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(payloadDirectory, x))
                .Where(IsInstallable)
                .ToArray();
        }

        return File.ReadLines(manifest, Encoding.UTF8)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0 && !x.StartsWith('#'))
            .Where(IsSafeRelativePath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool IsSafeRelativePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path)) return false;
        var normalized = path.Replace('/', Path.DirectorySeparatorChar);
        return !normalized.Split(Path.DirectorySeparatorChar).Contains("..", StringComparer.Ordinal);
    }

    private static bool IsInstallable(string path) =>
        IsSafeRelativePath(path) &&
        !string.Equals(Path.GetFileName(path), "README.txt", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(Path.GetFileName(path), "manifest.txt", StringComparison.OrdinalIgnoreCase);
}
