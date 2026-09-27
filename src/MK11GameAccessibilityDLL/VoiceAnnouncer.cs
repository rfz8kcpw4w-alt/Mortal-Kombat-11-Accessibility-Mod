using System.Speech.Synthesis;

namespace MK11GameAccessibilityDLL;

public sealed class VoiceAnnouncer : IDisposable
{
    private readonly SpeechSynthesizer synth = new();
    private readonly Queue<string> queue = new();
    private readonly object gate = new();
    private bool speaking;

    public void Enqueue(string message)
    {
        lock (gate) queue.Enqueue(message);
        SpeakNext();
    }

    private void SpeakNext()
    {
        string? message = null;
        lock (gate)
        {
            if (speaking || queue.Count == 0) return;
            speaking = true;
            message = queue.Dequeue();
        }
        try
        {
            synth.SpeakCompleted -= Completed;
            synth.SpeakCompleted += Completed;
            synth.SpeakAsync(message);
        }
        catch { Completed(this, EventArgs.Empty); }
    }

    private void Completed(object? sender, EventArgs e)
    {
        lock (gate) speaking = false;
        SpeakNext();
    }

    public void Stop()
    {
        synth.SpeakAsyncCancelAll();
        lock (gate) { queue.Clear(); speaking = false; }
    }

    public void Dispose()
    {
        Stop();
        synth.Dispose();
    }
}
