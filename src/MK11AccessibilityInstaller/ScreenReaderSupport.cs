using System.Diagnostics;

namespace MK11AccessibilityInstaller;

internal static class ScreenReaderSupport
{
    public enum ScreenReaderType
    {
        None,
        NVDA,
        JAWS
    }

    public static ScreenReaderType Detect()
    {
        foreach (var process in Process.GetProcesses())
        {
            var name = process.ProcessName.Trim();
            if (name.Equals("nvda", StringComparison.OrdinalIgnoreCase)) return ScreenReaderType.NVDA;
            if (name.Equals("jfw", StringComparison.OrdinalIgnoreCase) || name.Equals("jfwapp", StringComparison.OrdinalIgnoreCase)) return ScreenReaderType.JAWS;
        }
        return ScreenReaderType.None;
    }

    public static string GetStatusMessage(ScreenReaderType type)
    {
        return type switch
        {
            ScreenReaderType.NVDA => "NVDA algılandı. Erişilebilirlik arayüzü etkin.",
            ScreenReaderType.JAWS => "JAWS algılandı. Erişilebilirlik arayüzü etkin.",
            _ => "Ekran okuyucu algılanmadı; Windows erişilebilirlik desteği devre dışı olabilir."
        };
    }

    public static void ApplyAccessibilityDefaults(Form form)
    {
        form.AccessibleName = "Mortal Kombat 11 erişilebilirlik kurulumu";
        form.AccessibleDescription = "Steam üzerinden oyunu bulur, erişilebilirlik modunu kurar ve yedekten geri yükler.";
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.KeyPreview = true;
        form.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.F1)
            {
                form.ActiveControl?.Focus();
            }
        };
    }
}
