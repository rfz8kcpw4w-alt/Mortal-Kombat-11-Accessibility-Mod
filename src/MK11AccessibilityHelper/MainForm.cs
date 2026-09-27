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
    private readonly Button installTesseractButton = new() { Text = "Tesseract kur" };
    private readonly TextBox tessdataPathBox = new() { Width = 420, Text = "" };
    private readonly Label status = new() { AutoSize = true, Text = "Hazır" };
    private readonly AccessibilityPanel panel;
    private IntPtr gameWindow;
    private bool tesseractReady = false;

    public MainForm()
    {
        Text = "MK11 Accessibility Helper v0.9";
        Width = 760;
        Height = 480;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        panel = new AccessibilityPanel(FindTessdataPath());

        output.Multiline = true;
        output.ReadOnly = true;
        output.ScrollBars = ScrollBars.Vertical;
        output.Dock = DockStyle.Fill;

        var flow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
        flow.Controls.Add(installTesseractButton);
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

        installTesseractButton.Click += async (_, _) => await InstallTesseract();
        findWindowButton.Click += (_, _) => FindGameWindow();
        readMenuButton.Click += (_, _) => ReadMenu();
        speakButton.Click += (_, _) => panel.Speak(output.Text);

        Shown += async (_, _) =>
        {
            tessdataPathBox.Text = FindTessdataPath();
            await CheckTesseractStatus();
            FindGameWindow();
        };
    }

    private async Task CheckTesseractStatus()
    {
        status.Text = "Tesseract kontrol ediliyor...";
        tesseractReady = await TesseractInstaller.EnsureTesseractInstalled();
        if (tesseractReady)
        {
            status.Text = "Tesseract hazır ✓";
            output.Text = "Tesseract OCR hazır. Oyun penceresini bulabilirsiniz.";
        }
        else
        {
            status.Text = "Tesseract kurulum başarısız. El ile kur veya Tesseract kur butonunu kullan.";
            output.Text = "Tesseract kurulumu başarısız. 'Tesseract Kur' düğmesini kullanarak otomatik kurulumu deneyebilirsiniz.";
        }
    }

    private async Task InstallTesseract()
    {
        installTesseractButton.Enabled = false;
        status.Text = "Tesseract kurulumu başlatılıyor...";
        output.Text = "Tesseract OCR ve dil dosyaları indiriliyor. Lütfen bekleyin...\n\n";

        try
        {
            var installed = await TesseractInstaller.EnsureTesseractInstalled();
            if (installed)
            {
                tesseractReady = true;
                status.Text = "Tesseract kurulumu tamamlandı ✓";
                output.Text += "Tesseract başarıyla kuruldu!\n\nArtık Menü Oku düğmesini kullanabilirsiniz.";
                tessdataPathBox.Text = TesseractInstaller.GetLocalTessdataPath();
            }
            else
            {
                status.Text = "Tesseract kurulumu başarısız.";
                output.Text += "Kurulum başarısız. Lütfen manuel olarak Tesseract OCR kurun:\nhttps://github.com/UB-Mannheim/tesseract/releases";
            }
        }
        catch (Exception ex)
        {
            status.Text = $"Hata: {ex.Message}";
            output.Text += $"Kurulum hatası: {ex.Message}";
        }
        finally
        {
            installTesseractButton.Enabled = true;
        }
    }

    private static string FindTessdataPath()
    {
        var localPath = TesseractInstaller.GetLocalTessdataPath();
        if (Directory.Exists(localPath)) return localPath;

        var candidates = new[]
        {
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
        if (!tesseractReady)
        {
            status.Text = "Tesseract henüz hazır değil. Lütfen Tesseract Kur düğmesini kullanın.";
            return;
        }

        var hwnd = WindowFinder.FindMk11Window();
        if (hwnd is null)
        {
            status.Text = "MK11 penceresi bulunamadı.";
            return;
        }

        gameWindow = hwnd.Value;
        var rect = WindowFinder.GetWindowBounds(gameWindow);
        status.Text = $"Pencere bulundu: {gameWindow} | {rect.Width}x{rect.Height}";
        output.Text = "Pencere bulundu. Menü okumak için 'Menü Oku' düğmesini kullanın.";
    }

    private void ReadMenu()
    {
        if (!tesseractReady)
        {
            status.Text = "Tesseract hazır değil.";
            return;
        }

        if (gameWindow == IntPtr.Zero)
        {
            status.Text = "Önce oyun penceresini bulun.";
            return;
        }

        try
        {
            status.Text = "Menü okunuyor...";
            var rect = WindowFinder.GetWindowBounds(gameWindow);
            if (rect.IsEmpty)
            {
                status.Text = "Pencere bulunamadı.";
                return;
            }

            var safeArea = Rectangle.Inflate(rect, -40, -80);
            var text = OcrReader.ReadTextFromRegion(safeArea, tessdataPathBox.Text);
            output.Text = text;
            status.Text = "Menü okuma tamamlandı.";
            panel.Speak(text);
        }
        catch (Exception ex)
        {
            status.Text = $"OCR hatası: {ex.Message}";
            output.Text = $"Hata: {ex.Message}";
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
