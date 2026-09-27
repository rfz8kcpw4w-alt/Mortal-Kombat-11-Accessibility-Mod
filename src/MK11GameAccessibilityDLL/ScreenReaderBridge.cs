using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace MK11AccessibilityDLL;

/// <summary>
/// Oyun UI öğelerini Windows erişilebilirlik sağlayıcısı aracılığıyla NVDA/JAWS'a sunma.
/// </summary>
public class ScreenReaderBridge
{
    private readonly Dictionary<string, string> menuCache = new();
    private AccessibilityHook hook;

    public ScreenReaderBridge(AccessibilityHook accessibilityHook)
    {
        hook = accessibilityHook;
    }

    /// <summary>
    /// Oyun menüsü seçeneğini ekran okuyucuya açar.
    /// </summary>
    public void AnnounceMenuOption(string option, string description)
    {
        menuCache[option] = description;
        hook.GetAnnouncer().Enqueue($"Menü: {option}. {description}");
    }

    /// <summary>
    /// Oyun içi durum özeti (sağlık, tur, kombo) döndürür.
    /// </summary>
    public string GetGameSummary()
    {
        var state = hook.GetCurrentState();
        return $"Tur {state.RoundNumber}. " +
               $"P1 sağlık {state.Player1Health}%. " +
               $"P2 sağlık {state.Player2Health}%. " +
               $"Kombo: P1 {state.Player1ComboCount}x, P2 {state.Player2ComboCount}x.";
    }

    /// <summary>
    /// Oyun ekranını açıklayan UIA elemanı döndürür.
    /// </summary>
    public IRawElementProviderSimple? GetGameScreenAccessibility()
    {
        // Oyun ekran öğesini UIA sağlayıcısıyla döndürmek için
        // oyunun kendi UI katmanıyla entegrasyon gereklidir.
        return null; // Henüz uygulanmamış; oyun API'sine göre özelleştirilecek
    }

    public void ClearCache() => menuCache.Clear();
}
