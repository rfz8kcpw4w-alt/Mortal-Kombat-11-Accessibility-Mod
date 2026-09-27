using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MK11AccessibilityHelper;

internal sealed class MainForm : Form
{
    private readonly TextBox output = new();
    private readonly Button findWindowButton = new() { Text = "Oyun penceresini bul" };
    private readonly Button readMenuButton = new() { Text = "Menü oku" };
    private readonly Button speakButton = new() { Text = "Metni oku" };
    private readonly TextBox tessdataPathBox = new() { Width = 420, Text = "" };
    private readonly Label status = new() { AutoSize = true, Text = "Hazır" };
    private readonly AccessibilityPanel panel;
    private IntPtr gameWindow;

    public MainForm()
    {
        Text = "MK11 Accessibility Helper";
        Width = 760;
        Height = 420;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        panel = new AccessibilityPanel(FindTessdataPath());

        output.Multiline = true;
        output.ReadOnly = true;
        output.ScrollBars = ScrollBars.Vertical;
        output.Dock = DockStyle.Fill;

        var flow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
        flow.Controls.Add(findWindowButton);
        flow.Controls.Add(readMenuButton);
        flow.Controls.Add(speakButton);

        var tessWrap = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
        tessWrap.Controls.Add(new Label { Text = "Tesseract tessdata yolu:", AutoSize = true });
        tessWrap.Controls.Add(tessdataPathBox);

        var container = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        container.Controls.Add(flow, 0, 0);
        container.Controls.Add(tessWrap, 0, 1);
        container.Controls.Add(output, 0, 2);
        container.Controls.Add(status, 0, 3);
        Controls.Add(container);

        findWindowButton.Click += (_, _) => FindGameWindow();
        readMenuButton.Click += (_, _) => ReadMenu();
        speakButton.Click += (_, _) => panel.Speak(output.Text);

        Shown += (_, _) =>
        {
            tessdataPathBox.Text = FindTessdataPath();
            FindGameWindow();
        };
    }

    private static string FindTessdataPath()
    {
        var candidates = new[]
        {
            Path.Combine(Environment.CurrentDirectory, "tessdata"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Tesseract-OCR", "tessdata"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Tesseract-OCR", "tessdata"),
            "C:\\Program Files\\Tesseract-OCR\\tessdata",
            "C:\\Program Files (x86)\\Tesseract-OCR\\tessdata"
        };
        foreach (var p in candidates)
            if (Directory.Exists(p)) return p;
        return Environment.CurrentDirectory;
    }

    private void FindGameWindow()
    {
        var hwnd = WindowFinder.FindMk11Window();
        if (hwnd is null)
        {
            status.Text = "MK11 penceresi bulunamadı.";
            return;
        }

        gameWindow = hwnd.Value;
        var rect = WindowFinder.GetWindowBounds(gameWindow);
        status.Text = $"Pencere bulundu: {gameWindow} | {rect.Width}x{rect.Height}";
        output.Text = "Pencere bulundu. Menü okumak için 'Menü oku' düğmesini kullanın.";
    }

    private void ReadMenu()
    {
        if (gameWindow == IntPtr.Zero)
        {
            status.Text = "Önce oyun penceresini bulun.";
            return;
        }

        try
        {
            var text = OcrReader.ReadTextFromRegion(
                Rectangle.Inflate(WindowFinder.GetWindowBounds(gameWindow), -40, -80),
                tessdataPathBox.Text);
            output.Text = text;
            status.Text = "Menü okuma tamamlandı.";
            panel.Speak(text);
        }
        catch (Exception ex)
        {
            status.Text = $"OCR hatası: {ex.Message}";
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        panel.Dispose();
        base.OnFormClosed(e);
    }
}

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
