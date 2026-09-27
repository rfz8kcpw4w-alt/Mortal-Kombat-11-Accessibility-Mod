using System.Speech.Synthesis;

namespace MK11AccessibilityDLL;

/// <summary>
/// SAPI 5.1 aracılığıyla oyun durumunu sesli açıklar.
/// </summary>
public class VoiceAnnouncer
{
    private readonly SpeechSynthesizer synth = new();
    private readonly Queue<string> announceQueue = new();
    private bool isAnnouncing = false;
    private GameState? lastState;

    public VoiceAnnouncer()
    {
        synth.SpeakCompleted += (_, _) => ProcessQueue();
    }

    /// <summary>
    /// Oyun durumundaki değişiklikleri tespit ederek sesli bildirim yapar.
    /// </summary>
    public void UpdateAndAnnounce(GameState current)
    {
        if (lastState == null) { lastState = new GameState { Phase = current.Phase }; return; }

        var messages = new List<string>();

        // Tur başladı
        if (lastState.Phase != GameState.TurnPhase.Fighting && current.Phase == GameState.TurnPhase.Fighting)
            messages.Add($"Tur {current.RoundNumber} başladı.");

        // Kombo bildirimleri
        if (current.Player1ComboCount > lastState.Player1ComboCount && current.Player1ComboCount > 0)
            messages.Add($"P1 kombo: {current.Player1ComboCount}. Hasar: {current.Player1ComboDamage}.");

        if (current.Player2ComboCount > lastState.Player2ComboCount && current.Player2ComboCount > 0)
            messages.Add($"P2 kombo: {current.Player2ComboCount}. Hasar: {current.Player2ComboDamage}.");

        // Kombo sıfırlandı
        if (lastState.Player1ComboCount > 0 && current.Player1ComboCount == 0 && lastState.Phase == GameState.TurnPhase.Fighting)
            messages.Add($"P1 kombo bitti. Toplam: {lastState.Player1ComboCount} hit.");

        if (lastState.Player2ComboCount > 0 && current.Player2ComboCount == 0 && lastState.Phase == GameState.TurnPhase.Fighting)
            messages.Add($"P2 kombo bitti. Toplam: {lastState.Player2ComboCount} hit.");

        // Sağlık değişimi
        if (current.Player1Health < lastState.Player1Health)
            messages.Add($"P1 sağlık: {current.Player1Health}%.");

        if (current.Player2Health < lastState.Player2Health)
            messages.Add($"P2 sağlık: {current.Player2Health}%.");

        // Tur bitişi
        if (lastState.Phase == GameState.TurnPhase.Fighting && current.Phase == GameState.TurnPhase.RoundEnd)
        {
            if (current.Player1Health > current.Player2Health)
                messages.Add("Tur P1'e gitti.");
            else if (current.Player2Health > current.Player1Health)
                messages.Add("Tur P2'ye gitti.");
            else
                messages.Add("Tur berabere.");
        }

        // Oyun bitişi
        if (lastState.Phase != GameState.TurnPhase.FightEnd && current.Phase == GameState.TurnPhase.FightEnd)
        {
            if (current.Player1Health > 0 && current.Player2Health <= 0)
                messages.Add("P1 kazandı!");
            else if (current.Player2Health > 0 && current.Player1Health <= 0)
                messages.Add("P2 kazandı!");
            else
                messages.Add("Oyun bitti.");
        }

        foreach (var msg in messages)
            Enqueue(msg);

        lastState = new GameState
        {
            Player1Health = current.Player1Health,
            Player2Health = current.Player2Health,
            Player1ComboCount = current.Player1ComboCount,
            Player1ComboDamage = current.Player1ComboDamage,
            Player2ComboCount = current.Player2ComboCount,
            Player2ComboDamage = current.Player2ComboDamage,
            Phase = current.Phase,
            RoundNumber = current.RoundNumber
        };
    }

    public void Enqueue(string message)
    {
        announceQueue.Enqueue(message);
        if (!isAnnouncing) ProcessQueue();
    }

    private void ProcessQueue()
    {
        if (announceQueue.Count == 0) { isAnnouncing = false; return; }
        isAnnouncing = true;
        var msg = announceQueue.Dequeue();
        try { synth.SpeakAsync(msg); }
        catch { ProcessQueue(); }
    }

    public void Stop()
    {
        synth.SpeakAsyncCancelAll();
        announceQueue.Clear();
        isAnnouncing = false;
    }

    public void Dispose() => synth?.Dispose();
}
