# MK11 Accessibility Helper v0.9

Mortal Kombat 11 için harici yardımcı uygulama. Menüleri OCR ile okur ve NVDA/JAWS/SAPI ile seslendirir.

## Özellikler

- ✅ Tesseract OCR otomatik kurulum
- ✅ MK11 penceresi otomatik bulma
- ✅ Menü metinlerini OCR ile okuma
- ✅ NVDA/JAWS/SAPI ile sesli anlatım
- ✅ Basit Windows Forms arayüzü
- ✅ Türkçe arayüz ve bildirimler

## Gereksinimler

- Windows 10/11
- .NET 8 SDK
- Internet bağlantısı (ilk kurulum için Tesseract indirmesi)

## Kurulum

### 1. Projeyi klonlayın

```bash
git clone https://github.com/rfz8kcpw4w-alt/Mortal-Kombat-11-Accessibility-Mod.git
cd Mortal-Kombat-11-Accessibility-Mod
```

### 2. Proje dosyalarını geri yükleyin

```powershell
dotnet restore src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj
```

### 3. Derleyin

```powershell
dotnet build src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj -c Release
```

### 4. Çalıştırın

```powershell
dotnet run --project src/MK11AccessibilityHelper/MK11AccessibilityHelper.csproj
```

## Kullanım

1. **Tesseract OCR Kurulumu**
   - Uygulamayı açtığınızda ilk olarak "Tesseract Kur" düğmesine basın
   - Uygulama, Tesseract ve `eng.traineddata` dosyasını otomatik olarak indirecektir
   - Kurulum tamamlandığında "Tesseract hazır ✓" mesajı görünecektir

2. **Oyun Penceresini Bulma**
   - "Oyun penceresini bul" düğmesine basın
   - Uygulama MK11 penceresi bulur ve koordinatlarını gösterir

3. **Menüyü Okuma**
   - Mortal Kombat 11'de menüye gidin
   - "Menü Oku" düğmesine basın
   - Uygulama menü metinlerini OCR ile okur ve SAPI üzerinden seslendirir
   - Metin, pencerenin metin alanında da görünecektir

4. **Metni Tekrar Okutma**
   - "Metni Oku" düğmesine basarak son okunan metni tekrar dinleyebilirsiniz

## Teknik Detaylar

### Dosya Yapısı

```
src/MK11AccessibilityHelper/
├── MK11AccessibilityHelper.csproj
├── MainForm.cs              # Ana uygulama arayüzü
├── WindowFinder.cs          # MK11 penceresini bulma
├── OcrReader.cs             # Tesseract OCR entegrasyonu
├── ScreenReaderBridge.cs    # NVDA/SAPI köprüsü
└── TesseractInstaller.cs    # Otomatik Tesseract kurulumu
```

### Bağımlılıklar

- `System.Speech` - Windows SAPI 5.1 sesli bildirim
- `Tesseract` - OCR kütüphanesi
- `System.Windows.Forms` - Arayüz
- `System.Drawing` - Ekran yakalama

### OCR Süreci

1. MK11 penceresinin koordinatları bulunur
2. Pencere bölgesi bitmap olarak yakalanır
3. Tesseract OCR ile metin çıkarılır
4. Metin temizlenir ve satırlar ayrılır
5. SAPI üzerinden okunur
6. Metin arayüzde gösterilir

## Sınırlamalar

⚠️ **Bu bir harici yardımcı uygulamadır**
- Oyun içine gömülü mod değildir
- Oyunun menü sistemini değiştirmez
- Menülere tıklama otomasyonu sağlamaz
- Çevrim içi oyunlarda kullanılmaz
- Oyun penceresi kapatılırsa çalışmaz

## Gelecek Sürümler

- [ ] Menü öğelerine klavye simülasyonu
- [ ] NVDA Controller Client doğrudan entegrasyonu
- [ ] Karakter seçim ekranı navigasyonu
- [ ] Hikaye modu rehberi
- [ ] Konfigürasyon dosyası desteği
- [ ] Hotkeyler (F9 menüyü oku, F10 sessiz mod, vb.)

## Sorun Giderme

### "Tesseract kütüphanesi bulunamadı" hatası
- "Tesseract Kur" düğmesini kullanarak otomatik kurulum yapın
- Veya manuel olarak Tesseract OCR kurun: https://github.com/UB-Mannheim/tesseract/releases

### "MK11 penceresi bulunamadı" hatası
- Mortal Kombat 11'in çalıştırıldığından emin olun
- Pencere başlığı "Mortal Kombat" veya "MK11" içermelidir

### OCR hatalı sonuç veriyor
- Oyun çözünürlüğünü kontrol edin (1920x1080 minimum önerilir)
- Menü bölgesinin yeterince parlak olduğundan emin olun
- "Tesseract tessdata yolu" ayarının doğru olduğunu kontrol edin

## Lisans

MIT Lisansı - Detaylar için LICENSE dosyasına bakın.

Moreal Kombat 11 ve Warner Bros. Games'e ait tüm haklar saklıdır.

## Katkıda Bulunma

Buglar, feature istekleri ve pull requestler memnuniyetle kabul edilir.
