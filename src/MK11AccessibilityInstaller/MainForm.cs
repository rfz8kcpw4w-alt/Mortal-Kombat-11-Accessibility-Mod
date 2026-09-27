# Mortal Kombat 11 Erişilebilirlik Modu

Bu depo, görme engelli oyuncular için Mortal Kombat 11'e erişilebilirlik özellikleri eklemek üzere hazırlanmış açık kaynak bir başlangıç projesidir.

## v0.3 güncellemesi

- NVDA ve JAWS tespiti eklendi
- Uygulama açılırken ekran okuyucu algılama
- Windows erişilebilirlik adları ve açıklamaları geliştirilmiş
- Her durum değişiminde sesli geri bildirim
- Klasör seçimi ve Steam taraması erişilebilir hale geldi
- Durum metinleri ekran okuyucuya daha iyi okunur hale getirildi

> Bu sürümde ekran okuyucu desteği eklendi. Oyunun gerçek oyun içi sesli açıklama ve kombo rehberi henüz sonraki sürümde eklenir.

## Derleme

```powershell
dotnet build src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj -c Release
```

## Kullanım

1. Uygulamayı çalıştırın.
2. Steam oyunu otomatik bulunamazsa **Klasörü seç** düğmesini kullanın.
3. **Modu kur** düğmesine basın.
4. Orijinal dosyalar `MK11AccessibilityBackup` altında yedeklenir.
5. Gerekirse **Yedekten geri yükle** düğmesini kullanın.

## Gereksinimler

- Windows 10 veya 11
- .NET 8 Desktop Runtime
- Steam üzerinden kurulmuş Mortal Kombat 11
- NVDA veya JAWS (isteğe bağlı)

## Ekran Okuyucu Desteği

Uygulama açıldığında şu işlemleri yapar:
- `nvda.exe` ve `jfw.exe` / `jfwapp.exe` süreçlerini tarar
- Ekran okuyucu algılanırsa metin bildirimleri sesli okunur
- Tüm ana butonlar ve durum alanları erişilebilir adlar taşır
- Windows erişilebilirlik API'leri ve standart UI kontrolleri kullanılır

Bu, görme engelli kullanıcıların kurulum ekranını daha rahat kullanmasını sağlar.

## Proje yapısı

```text
src/MK11AccessibilityInstaller/
  MainForm.cs
  ScreenReaderSupport.cs
  SteamLocator.cs
  InstallerService.cs
  PayloadManifest.cs
  Program.cs
ModPayload/
  manifest.txt
  README.txt
```

Kod MIT lisansı ile sunulur. Mortal Kombat 11 ve Steam, ilgili hak sahiplerine aittir.

## Katkı

Erişilebilirlik iyileştirmeleri ve bug raporları için issue açabilirsiniz.

