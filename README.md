# MK11AccessibilityHelper

Bu yardımcı uygulama, Mortal Kombat 11 için dışarıdan çalışan bir erişilebilirlik aracı olarak tasarlanmıştır.

Amaç:
- MK11 pencere boyutunu ve konumunu bulur
- ekran görüntüsünü yakalar
- OCR ile menü metinlerini okur
- NVDA/JAWS/SAPI ile okur
- gerektiğinde klavye komutları gönderir

Önemli:
- Bu uygulama, oyun içi bir mod değil; harici bir yardımcı araçtır.
- MK11'nin kendi menü sistemini doğrudan değiştirmez.
- Başarılı çalışması için Tesseract OCR ve sistemde ekran okuyucu gerekir.

## Gerekli araçlar

- .NET 8 SDK
- Windows 10/11
- Tesseract OCR kurulu (veya `tessdata` klasörü içindeki `eng` dili)
- NVDA veya JAWS (isteğe bağlı; fallback olarak SAPI çalışır)

## Derleme

```powershell
dotnet restore src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj
dotnet build src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj -c Release
```

## Çalıştırma

```powershell
dotnet run --project src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj
```

## Kullanım

1. Mortal Kombat 11'i açın.
2. Yardımcı uygulamayı çalıştırın.
3. "Oyun penceresini bul" düğmesine basın.
4. "Menü oku" butonuna basın.
5. Uygulama ekran görüntüsünü OCR ile analiz eder ve metni sesli okur.
6. Gerekirse doğrulama için ekranı yeniden okur veya kısayol tuşlarını gönderir.

## Notlar

- Bu uygulama, doğrudan oyunun menü sistemi üzerinde değişiklik yapmaz.
- Çevrim içi oyunlarda kullanılmamalıdır.
- Oyun ve ekran boyutları değişirse OCR bölgesi ayarlanmalıdır.

MIT lisansı ile sunulur.
