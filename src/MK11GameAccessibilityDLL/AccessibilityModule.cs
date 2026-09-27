namespace MK11GameAccessibilityDLL;

/// <summary>
/// Oyun durumunu seslendiren güvenli prototip. Bilinmeyen bellek adreslerine
/// erişmez ve oyuna DLL enjeksiyonu yapmaz.
/// </summary>
public sealed class AccessibilityModule : IDisposable
{
    private readonly VoiceAnnouncer announcer = new();
    private readonly LocalEventBridge events;
    private bool initialized;

    public AccessibilityModule() => events = new LocalEventBridge(announcer);

    public bool Initialize()
    {
        if (initialized) return true;
        initialized = true;
        events.Start();
        announcer.Enqueue("Erişilebilirlik ses köprüsü hazır.");
        return true;
    }

    public void Shutdown() => Dispose();

    public string GetGameStatusSummary() => "Durum, izinli oyun entegrasyonundan bekleniyor.";

    public void Dispose()
    {
        if (!initialized) return;
        initialized = false;
        events.Dispose();
        announcer.Dispose();
    }
}
