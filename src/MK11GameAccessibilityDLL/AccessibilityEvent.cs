using System.Text.Json;

namespace MK11GameAccessibilityDLL;

public sealed record AccessibilityEvent(
    string Type,
    string? Text = null,
    int? Player1Health = null,
    int? Player2Health = null,
    int? Player1Combo = null,
    int? Player2Combo = null,
    int? Round = null);

internal static class AccessibilityEventParser
{
    public static AccessibilityEvent? Parse(string json)
    {
        try { return JsonSerializer.Deserialize<AccessibilityEvent>(json); }
        catch (JsonException) { return null; }
    }
}
