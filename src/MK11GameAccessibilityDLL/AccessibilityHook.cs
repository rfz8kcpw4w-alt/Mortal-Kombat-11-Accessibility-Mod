using System.Runtime.InteropServices;

namespace MK11AccessibilityDLL;

/// <summary>
/// Oyun belleğinden durumu okumaya çalışan kancalama sistemi.
/// Mortal Kombat 11'in yapısına göre özelleştirilmelidir.
/// </summary>
public class AccessibilityHook
{
    private IntPtr gameHandle = IntPtr.Zero;
    private GameState currentState = new();
    private VoiceAnnouncer announcer = new();
    private bool isHooked = false;
    private System.Timers.Timer? pollTimer;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

    private const uint PROCESS_VM_READ = 0x0010;
    private const uint PROCESS_QUERY_INFORMATION = 0x0400;

    public bool Hook()
    {
        try
        {
            var proc = System.Diagnostics.Process.GetProcessesByName("MK11").FirstOrDefault();
            if (proc == null) { proc = System.Diagnostics.Process.GetProcessesByName("MortalKombat11").FirstOrDefault(); }
            if (proc == null) return false;

            gameHandle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, (uint)proc.Id);
            if (gameHandle == IntPtr.Zero) return false;

            isHooked = true;
            StartPolling();
            announcer.Enqueue("Oyun erişilebilirlik modülü bağlandı.");
            return true;
        }
        catch { return false; }
    }

    public void Unhook()
    {
        isHooked = false;
        pollTimer?.Stop();
        announcer.Stop();
        if (gameHandle != IntPtr.Zero) { try { Marshal.FreeHGlobal(gameHandle); } catch { } }
    }

    private void StartPolling()
    {
        pollTimer = new System.Timers.Timer(500); // 500ms aralıkla kontrol et
        pollTimer.Elapsed += (_, _) => Poll();
        pollTimer.Start();
    }

    private void Poll()
    {
        if (!isHooked || gameHandle == IntPtr.Zero) return;
        try
        {
            // Oyun bellek adreslerini çekmek için MK11'in spesifik yapısı gereklidir.
            // Aşağıda örnek değerler; ürün kodda güncellenmelidir.
            var player1HealthAddr = IntPtr.Add(gameHandle, 0x140000000); // Örnek adres
            var player2HealthAddr = IntPtr.Add(gameHandle, 0x140000100);

            byte[] buffer = new byte[4];

            // Can seviyelerini oku
            if (ReadProcessMemory(gameHandle, player1HealthAddr, buffer, 4, out _))
            {
                currentState.Player1Health = BitConverter.ToInt32(buffer, 0);
            }

            if (ReadProcessMemory(gameHandle, player2HealthAddr, buffer, 4, out _))
            {
                currentState.Player2Health = BitConverter.ToInt32(buffer, 0);
            }

            announcer.UpdateAndAnnounce(currentState);
        }
        catch { /* Bellek okuma hatası */ }
    }

    public GameState GetCurrentState() => currentState;
    public VoiceAnnouncer GetAnnouncer() => announcer;
}
