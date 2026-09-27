namespace MK11GameAccessibilityDLL;

/// <summary>
/// MK11Hook entegrasyonlu Ana Erişilebilirlik Modülü v0.8
/// Menü okuma, oyun olayları ve ekran okuyucu desteği.
/// </summary>
public sealed class AccessibilityModule : IDisposable
{
    private UniversalAccessibilityAnnouncer? announcer;
    private GameEventBroadcaster? eventBroadcaster;
    private LocalEventBridge? legacyBridge;
    private bool initialized;

    public bool Initialize()
    {
        if (initialized) return true;
        try
        {
            announcer = new UniversalAccessibilityAnnouncer();
            announcer.Initialize();

            eventBroadcaster = new GameEventBroadcaster(announcer);
            eventBroadcaster.Start();

            legacyBridge = new LocalEventBridge(announcer);
            legacyBridge.Start();

            announcer.Announce("Mortal Kombat 11 Erişilebilirlik Modülü v0.8 Başlatıldı. MK11Hook Menü Desteği Etkin.");
            initialized = true;
            return true;
        }
        catch { return false; }
    }

    public void Shutdown()
    {
        if (!initialized) return;
        initialized = false;
        eventBroadcaster?.Dispose();
        legacyBridge?.Dispose();
        announcer?.Dispose();
    }

    public void Dispose()
    {
        Shutdown();
    }
}
