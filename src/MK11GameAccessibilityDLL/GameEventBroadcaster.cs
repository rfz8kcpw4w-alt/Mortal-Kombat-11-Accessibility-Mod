using System.IO.Pipes;
using System.Text;

namespace MK11GameAccessibilityDLL;

/// <summary>
/// MK11Hook Lua scriptinden Named Pipe üzerinden menü ve oyun olaylarını dinler.
/// Bunları ekran okuyuculara iletir.
/// </summary>
public sealed class GameEventBroadcaster : IDisposable
{
    private readonly UniversalAccessibilityAnnouncer announcer;
    private readonly MenuAccessibilityModule menuModule;
    private readonly CancellationTokenSource cancellation = new();
    private Task? listener;

    public GameEventBroadcaster(UniversalAccessibilityAnnouncer announcer)
    {
        this.announcer = announcer;
        this.menuModule = new MenuAccessibilityModule(announcer);
    }

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
                    if (line is not null) ProcessEvent(AccessibilityEventParser.Parse(line));
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
            catch (IOException) { }
        }
    }

    private void ProcessEvent(AccessibilityEvent? item)
    {
        if (item is null) return;

        switch (item.Type)
        {
            case "menu":
                var itemCount = item.Player1Combo ?? 0;
                menuModule.OnMenuOpen(item.Text ?? "Menü", itemCount);
                break;
            case "menu_close":
                menuModule.OnMenuClose();
                break;
            case "menu_select":
                menuModule.OnMenuItemSelected(item.Text ?? "", item.Player1Combo ?? 0, item.Player2Combo ?? 0);
                break;
            case "round_start":
                announcer.Announce(new AccessibilityEvent("round_start", Round: item.Round));
                break;
            case "health":
                announcer.Announce(new AccessibilityEvent("health", Player1Health: item.Player1Health, Player2Health: item.Player2Health));
                break;
            case "combo":
                announcer.Announce(new AccessibilityEvent("combo", Player1Combo: item.Player1Combo, Player2Combo: item.Player2Combo));
                break;
            case "round_end":
                announcer.Announce(item.Text ?? "Tur bitti.");
                break;
            case "fight_end":
                announcer.Announce(item.Text ?? "Maç bitti.");
                break;
            default:
                if (!string.IsNullOrWhiteSpace(item.Text)) announcer.Announce(item.Text);
                break;
        }
    }

    public void Dispose()
    {
        cancellation.Cancel();
        try { listener?.Wait(TimeSpan.FromSeconds(1)); } catch { }
        cancellation.Dispose();
    }
}
