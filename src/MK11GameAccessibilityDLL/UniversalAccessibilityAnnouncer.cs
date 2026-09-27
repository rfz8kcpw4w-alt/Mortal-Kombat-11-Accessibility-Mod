namespace MK11GameAccessibilityDLL;

/// <summary>
/// Tek API ile NVDA, JAWS, Narrator ve SAPI çıktısını yönetir.
/// Bu sınıf oyunun verisini kendisi okumaz; izinli oyun eklentisinden gelen
/// AccessibilityEvent nesnelerini ekran okuyuculara yönlendirir.
/// </summary>
public sealed class UniversalAccessibilityAnnouncer : IDisposable
{
    private readonly ScreenReaderRegistry registry = new();

    public void Initialize()
    {
        registry.Detect();
        registry.Speak("Erişilebilirlik ekran okuyucu köprüsü hazır.");
    }

    public void Announce(string text, bool interrupt = false) => registry.Speak(text, interrupt);

    public void Announce(AccessibilityEvent item)
    {
        var text = item.Type switch
        {
            "round_start" => $"Tur {item.Round ?? 1} başladı.",
            "health" => $"Sağlık: oyuncu 1 {item.Player1Health ?? 0}, oyuncu 2 {item.Player2Health ?? 0}.",
            "combo" => $"Kombo: oyuncu 1 {item.Player1Combo ?? 0}, oyuncu 2 {item.Player2Combo ?? 0}.",
            "round_end" => "Tur bitti.",
            "fight_end" => item.Text ?? "Maç bitti.",
            "menu" => item.Text ?? "Menü.",
            _ => item.Text ?? string.Empty
        };
        if (!string.IsNullOrWhiteSpace(text)) Announce(text);
    }

    public void Dispose() => registry.Dispose();
}
