# Mortal Kombat 11 Erişilebilirlik Modu

Bu depo, görme engelli oyuncular için Mortal Kombat 11'e erişilebilirlik özellikleri eklemek üzere hazırlanmış açık kaynak bir başlangıç projesidir.

## v0.2 güncellemesi

- Steam bulunamazsa oyun klasörünü elle seçme
- `appmanifest_976310.acf` ile oyun doğrulama
- Kurulum öncesi güvenlik onayı ve mod dosyası manifesti
- Yalnızca manifestteki dosyaları kopyalama
- Yol geçişi (`..`) saldırılarına karşı koruma
- NVDA/JAWS için erişilebilir adlar, klavye odağı ve durum bildirimleri
- SAPI seslendirmesini açıp kapatma
- Kurulum günlüğü

> Bu depo, Mortal Kombat 11'in kendisine ait dosyaları içermez. `ModPayload` içine yalnızca dağıtım hakkınız olan, test edilmiş mod dosyalarını ekleyin. Oyun içi sesli açıklamalar henüz prototip aşamasındadır; çevrim içi rekabeti etkileyen hile veya otomasyon özellikleri eklenmeyecektir.

## Derleme

```powershell
dotnet build src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj -c Release
```

## Kullanım

1. Uygulamayı çalıştırın.
2. Steam oyunu otomatik bulamazsa **Klasörü seç** düğmesini kullanın.
3. **Modu kur** düğmesine basın.
4. Orijinal dosyalar `MK11AccessibilityBackup` altında yedeklenir.
5. Gerekirse **Yedekten geri yükle** düğmesini kullanın.

Kurulum uygulaması, `ModPayload/manifest.txt` dosyası varsa yalnızca orada listelenen dosyaları kurar. Manifest yoksa klasördeki dosyalar taranır; `README.txt` ve `manifest.txt` kurulmaz.

## Gereksinimler

- Windows 10 veya 11
- .NET 8 Desktop Runtime
- Steam üzerinden kurulmuş Mortal Kombat 11
- NVDA veya JAWS (isteğe bağlı; arayüz Windows erişilebilirlik API'lerini kullanır)

## Proje yapısı

```text
src/MK11AccessibilityInstaller/
  MainForm.cs
  SteamLocator.cs
  InstallerService.cs
  PayloadManifest.cs
  Program.cs
ModPayload/
  manifest.txt
  README.txt
```

Kod MIT lisansı ile sunulur. Mortal Kombat 11 ve Steam, ilgili hak sahiplerine aittir.
