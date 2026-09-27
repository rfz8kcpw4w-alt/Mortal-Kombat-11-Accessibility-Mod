namespace MK11AccessibilityDLL;

/// <summary>
/// Erişilebilirlik modülünün ana giriş noktası.
/// Oyun yükleme sırasında veya mod yükleyici tarafından çağrılır.
/// </summary>
public class AccessibilityModule
{
    private static AccessibilityModule? instance;
    private AccessibilityHook? hook;
    private ScreenReaderBridge? bridge;

    public static AccessibilityModule Instance => instance ??= new();

    private AccessibilityModule() { }

    /// <summary>
    /// Modülü başlat ve oyun prosesine bağlan.
    /// </summary>
    public bool Initialize()
    {
        try
        {
            hook = new AccessibilityHook();
            if (!hook.Hook()) return false;
            bridge = new ScreenReaderBridge(hook);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Modülü kapat ve kaynakları serbest bırak.
    /// </summary>
    public void Shutdown()
    {
        hook?.Unhook();
        hook?.GetAnnouncer().Dispose();
    }

    public AccessibilityHook? GetHook() => hook;
    public ScreenReaderBridge? GetBridge() => bridge;

    /// <summary>
    /// Oyun durum özetini al.
    /// </summary>
    public string GetGameStatusSummary() => bridge?.GetGameSummary() ?? "Erişilemiyor";
}
