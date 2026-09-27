# Mortal Kombat 11 Erişilebilirlik Modu

Görme engelli oyuncular için Mortal Kombat 11 erişilebilirlik projesi.

## v0.5 - Oyun İçi Erişilebilirlik Modülü

Bu sürümde oyunun içindeki ekran okuyucu desteği başlatılmıştır:

### Yeni Bileşenler

- **GameState.cs**: Oyun durumunun anlık görüntüsü (can, kombo, tur, faz)
- **VoiceAnnouncer.cs**: SAPI 5.1 aracılığıyla değişiklikleri sesli açıklayan sistem
  - Kombo başlama/bitme
  - Tur sonuçları
  - Sağlık değişimleri
  - Oyun kazanma/kaybetme
- **AccessibilityHook.cs**: Oyun belleğinden durumu okumaya çalışan sistem
  - Proses ve bellek erişimi
  - 500ms aralıkla durum yoklaması
- **ScreenReaderBridge.cs**: NVDA/JAWS ve oyun arasında köprü
  - Menü seçeneklerinin açıklanması
  - Oyun durumu özeti
  - Windows Automation API entegrasyonu
- **AccessibilityModule.cs**: Ana modül ve giriş noktası

### Önemli Notlar

1. **Bellek Okuma**: `AccessibilityHook.cs` içindeki bellek adresleri şu anda örnek adreslerdir. Mortal Kombat 11'in **çeşitli sürümleri için** doğru adresler bulmak gerekir:
   - Oyun version güncellemeleri adres ofsetini değiştirebilir
   - Dinamik bellek tahsisi şemaya göre değişebilir
   - Reverse engineering araçları (CheatEngine, Ghidra, IDA) kullanarak bellek haritası çıkarılabilir

2. **DLL Enjeksiyonu**: Modülü oyunun çalışma zamanında yüklemek için:
   - DLL injection tool
   - Oyun modding framework (varsa)
   - Steam overlay API'si

3. **İzin ve Yasal**: 
   - Bu modül **oyun içi rekabeti etkilemez**
   - Yaşlı sürümlerde başarısız olabilir
   - Yalnızca erişilebilirlik amacıyla tasarlanmıştır

## Kurulum

### DLL Derlemesi

```powershell
dotnet build src/MK11GameAccessibilityDLL/MK11AccessibilityDLL.csproj -c Release
```

### Oyundan Kullanım

DLL'i yüklemek için bir DLL injector veya oyun modding framework kullanın:

```csharp
// Örnek C# injection kodu
var module = AccessibilityModule.Instance;
if (module.Initialize())
{
    Console.WriteLine("Erişilebilirlik modülü başladı.");
    // Oyun çalışırken modül durumu izler
}
```

## Geliştirilecek Alanlar

- [ ] Mortal Kombat 11 bellek yapısının dökümantasyonu ve gerçek adresler
- [ ] DLL injection framework
- [ ] Menü yapılarının UIA üzerinden açıklanması
- [ ] Gerçek zamanlı karakter pozisyon açıklamaları
- [ ] Spesial hareket ve Fatality notifikasyonları
- [ ] Turnuva modu rehberi
- [ ] Ek diller (İngilizce ve Türkçe)

## Gereksinimler

- Windows 10/11
- .NET 8
- Mortal Kombat 11 (Steam)
- NVDA veya JAWS
- DLL injection aracı

Kod MIT lisansıyla sunulur. Mortal Kombat 11 ve Steam ilgili hak sahiplerine aittir.
