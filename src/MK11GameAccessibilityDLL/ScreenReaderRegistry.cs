using System.Diagnostics;

namespace MK11GameAccessibilityDLL;

public sealed class ScreenReaderRegistry : IDisposable
{
    private readonly List<IScreenReaderOutput> outputs = new();

    public IReadOnlyList<ScreenReaderKind> ActiveReaders => outputs.Select(x => x.Kind).ToArray();

    public void Detect()
    {
        DisposeOutputs();
        var nvda = new NvdaOutput();
        if (nvda.IsAvailable) outputs.Add(nvda); else nvda.Dispose();

        if (IsRunning("jfw", "jfwapp")) outputs.Add(new SapiOutput(ScreenReaderKind.JAWS));
        else if (IsRunning("Narrator")) outputs.Add(new SapiOutput(ScreenReaderKind.Narrator));

        // Always keep a fallback so users without a screen reader still hear status.
        if (outputs.Count == 0) outputs.Add(new SapiOutput());
    }

    public void Speak(string text, bool interrupt = false)
    {
        foreach (var output in outputs) output.Speak(text, interrupt);
    }

    private static bool IsRunning(params string[] names)
    {
        try { return Process.GetProcesses().Any(p => names.Contains(p.ProcessName, StringComparer.OrdinalIgnoreCase)); }
        catch { return false; }
    }

    private void DisposeOutputs()
    {
        foreach (var output in outputs) output.Dispose();
        outputs.Clear();
    }

    public void Dispose() => DisposeOutputs();
}
