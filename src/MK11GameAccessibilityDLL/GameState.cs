namespace MK11AccessibilityDLL;

/// <summary>
/// Oyun durumunun anlık görüntüsü (sağlık, kombo, tur durumu, vb.)
/// </summary>
public class GameState
{
    public enum PlayerPosition { Unknown, Left, Center, Right }
    public enum TurnPhase { Menu, FightStart, Fighting, RoundEnd, FightEnd }

    public int Player1Health { get; set; } = 100;
    public int Player2Health { get; set; } = 100;
    public int Player1MaxHealth { get; set; } = 100;
    public int Player2MaxHealth { get; set; } = 100;

    public int Player1ComboCount { get; set; } = 0;
    public int Player1ComboDamage { get; set; } = 0;
    public int Player2ComboCount { get; set; } = 0;
    public int Player2ComboDamage { get; set; } = 0;

    public PlayerPosition Player1Pos { get; set; } = PlayerPosition.Left;
    public PlayerPosition Player2Pos { get; set; } = PlayerPosition.Right;

    public int RoundNumber { get; set; } = 1;
    public int MaxRounds { get; set; } = 3;

    public TurnPhase Phase { get; set; } = TurnPhase.Menu;

    public string? Player1Name { get; set; }
    public string? Player2Name { get; set; }

    public long Timestamp { get; set; } = DateTime.UtcNow.Ticks;

    public override string ToString() =>
        $"R{RoundNumber} | P1:{Player1Health}% Combo:{Player1ComboCount}x{Player1ComboDamage}dmg | P2:{Player2Health}% Combo:{Player2ComboCount}x{Player2ComboDamage}dmg | {Phase}";
}
