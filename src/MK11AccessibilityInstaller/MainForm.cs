using System.Speech.Synthesis;

namespace MK11AccessibilityInstaller;

internal sealed class MainForm : Form
{
    private readonly Label statusLabel = new() { AutoSize = true, AccessibleName = "Durum", AccessibleRole = AccessibleRole.StatusBar };
    private readonly TextBox gamePathBox = new() { ReadOnly = true, Width = 560, AccessibleName = "Mortal Kombat 11 oyun klasörü", AccessibleRole = AccessibleRole.Text }; 
    private readonly Button locateButton = new() { Text = "Steam oyununu bul", AutoSize = true, AccessibleName = "Steam oyununu bul", AccessibleRole = AccessibleRole.PushButton };
    private readonly Button chooseButton = new() { Text = "Klasörü seç", AutoSize = true, AccessibleName = "Oyun klasörünü elle seç", AccessibleRole = AccessibleRole.PushButton };
    private readonly Button installButton = new() { Text = "Modu kur", AutoSize = true, AccessibleName = "Modu kur", AccessibleRole = AccessibleRole.PushButton };
    private readonly Button restoreButton = new() { Text = "Yedekten geri yükle", AutoSize = true, AccessibleName = "Yedekten geri yükle", AccessibleRole = AccessibleRole.PushButton };
    private readonly CheckBox speechCheck = new() { Text = "Durumları seslendir", Checked = true, AutoSize = true, AccessibleName = "Durumları seslendir", AccessibleRole = AccessibleRole.CheckButton };
    private readonly SpeechSynthesizer speech = new();
    private readonly Button[] navigationButtons;
    private string? gamePath;

    public MainForm()
    {
        Text = "Mortal Kombat 11 Erişilebilirlik Modu Kurulumu";
        Width = 720; Height = 390; StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10); KeyPreview = true;
        ScreenReaderSupport.ApplyAccessibilityDefaults(this);

        var title = new Label { Text = "Mortal Kombat 11 Erişilebilirlik Modu", AutoSize = true, Font = new Font(Font, FontStyle.Bold), AccessibleName = "Başlık" };
        var subtitle = new Label { Text = "Steam otomatik aranır; bulunamazsa klasörü elle seçebilirsiniz. F6 düğmeler arasında ilerler.", AutoSize = true, AccessibleName = "Açıklama" };
        navigationButtons = new[] { locateButton, chooseButton, installButton, restoreButton, speechCheck };

        locateButton.Click += (_, _) => Locate(); chooseButton.Click += (_, _) => ChooseFolder();
        installButton.Click += (_, _) => Install(); restoreButton.Click += (_, _) => Restore();
        installButton.Enabled = restoreButton.Enabled = false;

        var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        layout.Controls.Add(title); layout.Controls.Add(subtitle); layout.Controls.Add(locateButton); layout.Controls.Add(chooseButton); layout.Controls.Add(gamePathBox); layout.Controls.Add(installButton); layout.Controls.Add(restoreButton); layout.Controls.Add(speechCheck); layout.Controls.Add(statusLabel);
        Controls.Add(layout);
        KeyDown += HandleKeyboardNavigation;

        Shown += (_, _) =>
        {
            var reader = ScreenReaderSupport.Detect();
            SetStatus(ScreenReaderSupport.GetStatusMessage(reader) + " Steam aranıyor...");
            Locate();
        };
    }

    private void HandleKeyboardNavigation(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) { statusLabel.Focus(); e.SuppressKeyPress = true; return; }
        if (e.KeyCode != Keys.F6) return;
        var current = Array.IndexOf(navigationButtons, ActiveControl);
        var next = (current + 1 + navigationButtons.Length) % navigationButtons.Length;
        navigationButtons[next].Focus(); e.SuppressKeyPress = true;
    }

    private void Locate()
    {
        gamePath = SteamLocator.FindGame(); UpdatePath(gamePath);
        SetStatus(ScreenReaderSupport.GetStatusMessage(ScreenReaderSupport.Detect()) + " " + (gamePath is null ? "Mortal Kombat 11 bulunamadı." : "Mortal Kombat 11 bulundu."));
    }

    private void ChooseFolder()
    {
        using var dialog = new FolderBrowserDialog { Description = "Mortal Kombat 11 oyun klasörünü seçin" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        if (!File.Exists(Path.Combine(dialog.SelectedPath, "MK11.exe")) && !Directory.Exists(Path.Combine(dialog.SelectedPath, "Binaries"))) { SetStatus("Seçilen klasör Mortal Kombat 11 gibi görünmüyor."); return; }
        gamePath = dialog.SelectedPath; UpdatePath(gamePath); SetStatus("Oyun klasörü seçildi.");
    }

    private void UpdatePath(string? path)
    {
        gamePathBox.Text = path ?? "Mortal Kombat 11 bulunamadı";
        installButton.Enabled = restoreButton.Enabled = path is not null;
    }

    private void Install()
    {
        if (gamePath is null) return;
        try { installButton.Enabled = false; SetStatus("Kurulum yapılıyor..."); var count = InstallerService.Install(gamePath, Path.Combine(AppContext.BaseDirectory, "ModPayload"), file => SetStatus($"Kopyalanıyor: {file}")); SetStatus(count == 0 ? "ModPayload boş." : $"Kurulum tamamlandı. {count} dosya kopyalandı."); }
        catch (Exception ex) { SetStatus($"Kurulum hatası: {ex.Message}"); }
        finally { installButton.Enabled = gamePath is not null; }
    }

    private void Restore()
    {
        if (gamePath is null) return;
        try { restoreButton.Enabled = false; SetStatus("Geri yükleme yapılıyor..."); var count = InstallerService.Restore(gamePath); SetStatus($"Geri yükleme tamamlandı. {count} dosya geri yüklendi."); }
        catch (Exception ex) { SetStatus($"Geri yükleme hatası: {ex.Message}"); }
        finally { restoreButton.Enabled = gamePath is not null; }
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message; statusLabel.AccessibleName = message; statusLabel.AccessibleDescription = message; Announce(message);
    }

    private void Announce(string message)
    {
        if (!speechCheck.Checked) return;
        try { speech.SpeakAsyncCancelAll(); speech.SpeakAsync(message); } catch { }
    }
}
