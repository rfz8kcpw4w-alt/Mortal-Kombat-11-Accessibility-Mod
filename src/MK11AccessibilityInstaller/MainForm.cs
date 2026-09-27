using System.Speech.Synthesis;

namespace MK11AccessibilityInstaller;

internal sealed class MainForm : Form
{
    private readonly Label statusLabel = new() { AutoSize = true, AccessibleName = "Durum" };
    private readonly TextBox gamePathBox = new() { ReadOnly = true, Width = 560, AccessibleName = "Mortal Kombat 11 oyun klasörü" };
    private readonly Button locateButton = new() { Text = "Steam oyununu bul", AutoSize = true, AccessibleName = "Steam oyununu bul" };
    private readonly Button chooseButton = new() { Text = "Klasörü seç", AutoSize = true, AccessibleName = "Oyun klasörünü elle seç" };
    private readonly Button installButton = new() { Text = "Modu kur", AutoSize = true, AccessibleName = "Modu kur" };
    private readonly Button restoreButton = new() { Text = "Yedekten geri yükle", AutoSize = true, AccessibleName = "Yedekten geri yükle" };
    private readonly CheckBox speechCheck = new() { Text = "Durumları seslendir", Checked = true, AutoSize = true, AccessibleName = "Durumları seslendir" };
    private readonly SpeechSynthesizer speech = new();
    private string? gamePath;

    public MainForm()
    {
        Text = "Mortal Kombat 11 Erişilebilirlik Modu Kurulumu"; Width = 720; Height = 360;
        StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10);
        var title = new Label { Text = "Mortal Kombat 11 Erişilebilirlik Modu", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
        locateButton.Click += (_, _) => Locate(); chooseButton.Click += (_, _) => ChooseFolder();
        installButton.Click += (_, _) => Install(); restoreButton.Click += (_, _) => Restore();
        installButton.Enabled = restoreButton.Enabled = false;
        var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        layout.Controls.Add(title); layout.Controls.Add(new Label { Text = "Steam otomatik aranır; bulunamazsa klasörü elle seçebilirsiniz.", AutoSize = true });
        layout.Controls.Add(locateButton); layout.Controls.Add(chooseButton); layout.Controls.Add(gamePathBox); layout.Controls.Add(installButton); layout.Controls.Add(restoreButton); layout.Controls.Add(speechCheck); layout.Controls.Add(statusLabel);
        Controls.Add(layout); Shown += (_, _) => Locate();
    }

    private void Locate()
    {
        gamePath = SteamLocator.FindGame();
        UpdatePath(gamePath);
        Announce(gamePath is null ? "Mortal Kombat 11 bulunamadı." : "Mortal Kombat 11 bulundu.");
    }

    private void ChooseFolder()
    {
        using var dialog = new FolderBrowserDialog { Description = "Mortal Kombat 11 oyun klasörünü seçin" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        if (!File.Exists(Path.Combine(dialog.SelectedPath, "MK11.exe")) && !Directory.Exists(Path.Combine(dialog.SelectedPath, "Binaries")))
        { SetStatus("Seçilen klasör Mortal Kombat 11 gibi görünmüyor."); return; }
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
        try { var count = InstallerService.Install(gamePath, Path.Combine(AppContext.BaseDirectory, "ModPayload"), file => statusLabel.Text = $"Kopyalanıyor: {file}"); SetStatus(count == 0 ? "ModPayload boş." : $"Kurulum tamamlandı. {count} dosya kopyalandı."); }
        catch (Exception ex) { SetStatus($"Kurulum hatası: {ex.Message}"); }
    }

    private void Restore()
    {
        if (gamePath is null) return;
        try { var count = InstallerService.Restore(gamePath); SetStatus($"Geri yükleme tamamlandı. {count} dosya geri yüklendi."); }
        catch (Exception ex) { SetStatus($"Geri yükleme hatası: {ex.Message}"); }
    }

    private void SetStatus(string message) { statusLabel.Text = message; Announce(message); }
    private void Announce(string message) { if (!speechCheck.Checked) return; try { speech.SpeakAsyncCancelAll(); speech.SpeakAsync(message); } catch { } }
}
