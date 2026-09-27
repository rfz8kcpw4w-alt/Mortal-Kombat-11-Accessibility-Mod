using System.Runtime.InteropServices;

namespace MK11GameAccessibilityDLL;

/// <summary>
/// Optional NVDA Controller Client integration. The official NVDA controller
/// client DLL must be installed separately; otherwise this adapter is inactive.
/// </summary>
public sealed class NvdaOutput : IScreenReaderOutput
{
    [DllImport("nvdaControllerClient64.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern int nvdaController_testIfRunning();

    [DllImport("nvdaControllerClient64.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern int nvdaController_speakText([MarshalAs(UnmanagedType.LPWStr)] string text);

    [DllImport("nvdaControllerClient64.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nvdaController_cancelSpeech();

    public ScreenReaderKind Kind => ScreenReaderKind.NVDA;
    public bool IsAvailable { get; }

    public NvdaOutput()
    {
        try { IsAvailable = nvdaController_testIfRunning() == 0; }
        catch (DllNotFoundException) { IsAvailable = false; }
        catch (EntryPointNotFoundException) { IsAvailable = false; }
    }

    public void Speak(string text, bool interrupt = false)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(text)) return;
        try
        {
            if (interrupt) nvdaController_cancelSpeech();
            nvdaController_speakText(text);
        }
        catch { }
    }

    public void Dispose() { }
}
