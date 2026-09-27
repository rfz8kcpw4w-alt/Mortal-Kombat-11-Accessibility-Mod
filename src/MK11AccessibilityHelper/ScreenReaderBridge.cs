using System.Diagnostics;
using System.Drawing;
using System.Speech.Synthesis;

namespace MK11AccessibilityHelper;

public sealed class ScreenReaderBridge : IDisposable
{
    private readonly SpeechSynthesizer synth = new();

    public void Speak(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            synth.SpeakAsyncCancelAll();
            synth.SpeakAsync(text);
        }
        catch { }
    }

    public void Dispose() => synth.Dispose();
}

public sealed class KeyboardHelper
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    private const int KEYEVENTF_KEYDOWN = 0x0000;
    private const int KEYEVENTF_KEYUP = 0x0002;

    public static void SendKey(Keys key)
    {
        var vk = (byte)key.GetHashCode();
        keybd_event(vk, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(vk, 0, KEYEVENTF_KEYUP, 0);
    }
}

public sealed class AccessibilityPanel
{
    private readonly ScreenReaderBridge bridge = new();
    private readonly string tessdataPath;

    public AccessibilityPanel(string tessdataPath) => this.tessdataPath = tessdataPath;

    public string ReadCurrentWindowText(IntPtr hwnd)
    {
        var rect = WindowFinder.GetWindowBounds(hwnd);
        if (rect.IsEmpty) return "Pencere bulunamadı.";

        var safeArea = Rectangle.Inflate(rect, -40, -80);
        var text = OcrReader.ReadTextFromRegion(safeArea, tessdataPath);
        if (!string.IsNullOrWhiteSpace(text))
        {
            bridge.Speak(CleanText(text));
        }
        return CleanText(text);
    }

    private static string CleanText(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var lines = input
            .Replace("\r", "")
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Take(12);
        return string.Join(". ", lines);
    }

    public void Speak(string message) => bridge.Speak(message);

    public void Dispose() => bridge.Dispose();
}
