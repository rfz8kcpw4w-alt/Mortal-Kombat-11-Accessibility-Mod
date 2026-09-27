# Mortal Kombat 11 Erişilebilirlik Modu

Görme engelli oyuncular için Mortal Kombat 11 erişilebilirlik projesi.

## v0.4 düzeltmesi

- v0.3'te yanlış dosyalara yazılan `README.md` ve `MainForm.cs` düzeltildi.
- NVDA ve JAWS algılama korunuyor.
- Windows Forms kontrollerine erişilebilir adlar ve açıklamalar eklendi.
- Kurulum, geri yükleme ve durum mesajları ekran okuyucu/SAPI ile duyuruluyor.
- `F6` ile ana düğmeler arasında, `Esc` ile durum alanına odaklanma desteği eklendi.
- SAPI bağımlılığı proje dosyasına açıkça eklendi.

## Derleme

```powershell
dotnet restore src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj
dotnet build src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj -c Release
```

## Kullanım

1. NVDA veya JAWS çalışıyorsa uygulama bunu algılar.
2. Steam'de oyun otomatik bulunamazsa **Klasörü seç** düğmesini kullanın.
3. `ModPayload/manifest.txt` içine kurulacak dosyaların göreli yollarını yazın.
4. **Modu kur** düğmesine basın.
5. Gerekirse **Yedekten geri yükle** düğmesini kullanın.

Bu proje kurulum aracının erişilebilirliğini sağlar. Oyunun gerçek oyun içi görüntüsünü NVDA/JAWS'a aktaran bir bileşen henüz yoktur; oyun içi mod desteği ayrıca oyun sürümüne ve izin verilen modlama yöntemlerine göre geliştirilmelidir.

## Gereksinimler

- Windows 10/11
- .NET 8 Desktop Runtime veya self-contained yayın
- Steam Mortal Kombat 11 kurulumu
- NVDA veya JAWS (isteğe bağlı)

## Güvenlik

Kurulumdan önce mevcut dosyalar `MK11AccessibilityBackup` klasörüne kopyalanır. Proje Mortal Kombat 11 dosyalarını içermez ve çevrim içi rekabeti etkileyen otomasyon/hile özellikleri sağlamaz.

Kod MIT lisansı ile sunulur. Mortal Kombat 11 ve Steam ilgili hak sahiplerine aittir.
