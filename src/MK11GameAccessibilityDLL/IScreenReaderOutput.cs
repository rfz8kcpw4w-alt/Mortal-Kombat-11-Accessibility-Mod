namespace MK11GameAccessibilityDLL;

public enum ScreenReaderKind
{
    None,
    NVDA,
    JAWS,
    Narrator,
    SapiFallback
}

public interface IScreenReaderOutput : IDisposable
{
    ScreenReaderKind Kind { get; }
    bool IsAvailable { get; }
    void Speak(string text, bool interrupt = false);
}
