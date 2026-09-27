using System.IO.Pipes;
using System.Text;

namespace MK11GameAccessibilityDLL;

public sealed class LocalEventBridge : IDisposable
{
    private readonly UniversalAccessibilityAnnouncer announcer;
    private readonly CancellationTokenSource cancellation = new();
    private Task? listener;

    public LocalEventBridge(UniversalAccessibilityAnnouncer announcer) => this.announcer = announcer;
    public void Start() => listener ??= Task.Run(ListenAsync);

    private async Task ListenAsync()
    {
        while (!cancellation.IsCancellationRequested)
        {
            try
            {
                await using var pipe = new NamedPipeServerStream("MK11AccessibilityEvents", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await pipe.WaitForConnectionAsync(cancellation.Token);
                using var reader = new StreamReader(pipe, Encoding.UTF8);
                while (!cancellation.IsCancellationRequested && !reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync(cancellation.Token);
                    var item = line is null ? null : AccessibilityEventParser.Parse(line);
                    if (item is not null) announcer.Announce(item);
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
            catch (IOException) { }
        }
    }

    public void Dispose()
    {
        cancellation.Cancel();
        try { listener?.Wait(TimeSpan.FromSeconds(1)); } catch { }
        cancellation.Dispose();
    }
}
