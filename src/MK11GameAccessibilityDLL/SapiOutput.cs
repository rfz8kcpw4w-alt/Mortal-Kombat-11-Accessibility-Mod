using System.Speech.Synthesis;

namespace MK11GameAccessibilityDLL;

/// <summary>
/// Universal fallback. JAWS, Narrator and other readers can consume the
/// standard Windows SAPI output when a native controller is unavailable.
/// </summary>
public sealed class SapiOutput : IScreenReaderOutput
{
    private readonly SpeechSynthesizer synth = new();
    public ScreenReaderKind Kind { get; }
    public bool IsAvailable => true;

    public SapiOutput(ScreenReaderKind kind = ScreenReaderKind.SapiFallback) => Kind = kind;

    public void Speak(string text, bool interrupt = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            if (interrupt) synth.SpeakAsyncCancelAll();
            synth.SpeakAsync(text);
        }
        catch { }
    }

    public void Dispose()
    {
        try { synth.SpeakAsyncCancelAll(); synth.Dispose(); } catch { }
    }
}
