using System.IO.Pipes;
using System.Text;

namespace MK11GameAccessibilityDLL;

/// <summary>
/// Güvenli yerel olay köprüsü. Gerçek oyun eklentisi, izinli bir entegrasyon
/// üzerinden JSON olaylarını bu kanala gönderdiğinde sesli bildirim üretir.
/// DLL enjeksiyonu veya rastgele bellek okuma yapmaz.
/// </summary>
public sealed class LocalEventBridge : IDisposable
{
    private readonly VoiceAnnouncer announcer;
    private readonly CancellationTokenSource cancellation = new();
    private Task? listener;

    public LocalEventBridge(VoiceAnnouncer announcer) => this.announcer = announcer;

    public void Start()
    {
        listener ??= Task.Run(ListenAsync);
    }

    private async Task ListenAsync()
    {
        while (!cancellation.IsCancellationRequested)
        {
            try
            {
                await using var pipe = new NamedPipeServerStream(
                    "MK11AccessibilityEvents", PipeDirection.In, 1,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await pipe.WaitForConnectionAsync(cancellation.Token);
                using var reader = new StreamReader(pipe, Encoding.UTF8);
                while (!cancellation.IsCancellationRequested && !reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync(cancellation.Token);
                    var item = line is null ? null : AccessibilityEventParser.Parse(line);
                    if (item is not null) Announce(item);
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
            catch (IOException) { }
        }
    }

    private void Announce(AccessibilityEvent item)
    {
        var text = item.Type switch
        {
            "round_start" => $"Tur {item.Round ?? 1} başladı.",
            "health" => $"Sağlık: oyuncu 1 {item.Player1Health ?? 0}, oyuncu 2 {item.Player2Health ?? 0}.",
            "combo" => $"Kombo: oyuncu 1 {item.Player1Combo ?? 0}, oyuncu 2 {item.Player2Combo ?? 0}.",
            "round_end" => "Tur bitti.",
            "fight_end" => item.Text ?? "Maç bitti.",
            "menu" => item.Text ?? "Menü.",
            _ => item.Text
        };
        if (!string.IsNullOrWhiteSpace(text)) announcer.Enqueue(text);
    }

    public void Dispose()
    {
        cancellation.Cancel();
        try { listener?.Wait(TimeSpan.FromSeconds(1)); } catch { }
        cancellation.Dispose();
    }
}
