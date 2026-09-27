using System.Speech.Synthesis;

namespace MK11AccessibilityInstaller;

internal sealed class MainForm : Form
{
    private readonly Label statusLabel = new()
    {
        AutoSize = true,
        AccessibleName = "Durum",
        AccessibleRole = AccessibleRole.StaticText
    };

    private readonly TextBox gamePathBox = new()
    {
        ReadOnly = true,
        Width = 520,
        Height = 60,
        Multiline = true,
        AccessibleName = "Mortal Kombat 11 oyun klasörü",
        AccessibleRole = AccessibleRole.StaticText
    };

    private readonly Button locateButton = new()
    {
        Text = "Steam oyununu bul",
        AutoSize = true,
        AccessibleName = "Steam oyununu bul"
    };

    private readonly Button installButton = new()
    {
        Text = "Modu kur",
        AutoSize = true,
        AccessibleName = "Modu kur"
    };

    private readonly Button restoreButton = new()
    {
        Text = "Yedekten geri yükle",
        AutoSize = true,
        AccessibleName = "Yedekten geri yükle"
    };

    private readonly SpeechSynthesizer speech = new();
    private string? gamePath;

    public MainForm()
    {
        Text = "Mortal Kombat 11 Erişilebilirlik Modu Kurulumu";
        Width = 700;
        Height = 350;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);
        
        // Başlık
        var titleLabel = new Label
        {
            Text = "Mortal Kombat 11 Erişilebilirlik Modu",
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            AccessibleName = "Başlık",
            AccessibleRole = AccessibleRole.StaticText
        };

        var descriptionLabel = new Label
        {
            Text = "Bu araç Steam'den Mortal Kombat 11'i bulur ve erişilebilirlik modunu kurar.\nGörme engelli oyuncuların oyunu sesli rehberlikle oynamasını sağlar.",
            AutoSize = true,
            MaximumSize = new Size(650, 0),
            AccessibleName = "Açıklama",
            AccessibleRole = AccessibleRole.StaticText
        };

        // Düğme event'leri
        locateButton.Click += (_, _) => Locate();
        installButton.Enabled = false;
        restoreButton.Enabled = false;
        installButton.Click += (_, _) => Install();
        restoreButton.Click += (_, _) => Restore();

        // Layout
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        layout.Controls.Add(titleLabel);
        layout.Controls.Add(descriptionLabel);
        layout.Controls.Add(new Label { Height = 10 }); // Boşluk
        layout.Controls.Add(locateButton);
        layout.Controls.Add(gamePathBox);
        layout.Controls.Add(installButton);
        layout.Controls.Add(restoreButton);
        layout.Controls.Add(new Label { Height = 10 }); // Boşluk
        layout.Controls.Add(statusLabel);

        Controls.Add(layout);

        // Form yüklendiğinde oyunu bul
        Shown += (_, _) => Locate();
    }

    private void Locate()
    {
        try
        {
            gamePath = SteamLocator.FindGame();
            
            if (gamePath is not null)
            {
                gamePathBox.Text = gamePath;
                installButton.Enabled = true;
                restoreButton.Enabled = true;
                SetStatus("✓ Mortal Kombat 11 bulundu.");
            }
            else
            {
                gamePathBox.Text = "Mortal Kombat 11 bulunamadı. Steam'de oyunu kurun ve tekrar deneyin.";
                installButton.Enabled = false;
                restoreButton.Enabled = false;
                SetStatus("✗ Mortal Kombat 11 bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            SetStatus($"✗ Hata: {ex.Message}");
        }
    }

    private void Install()
    {
        if (gamePath is null) return;

        try
        {
            installButton.Enabled = false;
            SetStatus("⏳ Kurulum yapılıyor...");
            Application.DoEvents();

            var payloadDir = Path.Combine(AppContext.BaseDirectory, "ModPayload");
            var count = InstallerService.Install(gamePath, payloadDir);

            if (count > 0)
            {
                SetStatus($"✓ Kurulum tamamlandı! {count} dosya kopyalandı.");
            }
            else
            {
                SetStatus("⚠ ModPayload klasöründe dosya bulunamadı. Lütfen mod dosyalarını klasöre ekleyin.");
            }
        }
        catch (Exception ex)
        {
            SetStatus($"✗ Kurulum hatası: {ex.Message}");
        }
        finally
        {
            installButton.Enabled = gamePath is not null;
        }
    }

    private void Restore()
    {
        if (gamePath is null) return;

        try
        {
            restoreButton.Enabled = false;
            SetStatus("⏳ Geri yükleme yapılıyor...");
            Application.DoEvents();

            var count = InstallerService.Restore(gamePath);

            if (count > 0)
            {
                SetStatus($"✓ Geri yükleme tamamlandı! {count} dosya geri yüklendi.");
            }
            else
            {
                SetStatus("⚠ Geri yükleme için yedek dosya bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            SetStatus($"✗ Geri yükleme hatası: {ex.Message}");
        }
        finally
        {
            restoreButton.Enabled = gamePath is not null;
        }
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
        Announce(message);
    }

    private void Announce(string message)
    {
        try
        {
            speech.SpeakAsyncCancelAll();
            speech.SpeakAsync(message);
        }
        catch { /* Konuşma başarısız olursa sessiz kal */ }
    }
}
