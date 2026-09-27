# Mortal Kombat 11 Erişilebilirlik Modu v0.8 - MK11Hook Menü Desteği

## Yeni Özellikleri:

### 1. MK11Hook Lua Scriptleri
- `MK11MenuHook.lua` - Menü olaylarını algılayan ve Named Pipe'a gönderen script
- Menü açılması/kapanması
- Menü seçim değişimleri
- Tur başlama, can durumu, kombo olayları
- Tur ve maç sonu bildirileri

### 2. Menü Erişilebilirliği
- `MenuAccessibilityModule.cs` - Menü navigasyon ve okuma
- Menü seçeneklerini NVDA/JAWS'a iletme
- Seçim indeksini ve toplam seçenek sayısını okuma
- Menü kapatma/açma sesli bildirimleri

### 3. Oyun Olayları
- `GameEventBroadcaster.cs` - MK11Hook'tan gelen olayları dinleme
- Named Pipe üzerinden JSON olaylarını işleme
- Menü ve oyun durumu olaylarını ekran okuyuculara iletme

## Kurulum Adımları

### 1. MK11Hook Kurulumu

```powershell
# MK11Hook'u indirin
git clone https://github.com/ermaccer/MK11Hook.git

# Dosyaları MK11 oyun klasörüne kopyalayın
Copy-Item MK11Hook\* "C:\Program Files (x86)\Steam\steamapps\common\Mortal Kombat 11\" -Recurse
```

### 2. Lua Scriptini Kopyalayın

```powershell
# MK11MenuHook.lua'yı Mods klasörüne kopyalayın
Copy-Item MK11MenuHook.lua "C:\Program Files (x86)\Steam\steamapps\common\Mortal Kombat 11\Mods\"
```

### 3. Erişilebilirlik DLL'sini Derleyin

```powershell
dotnet build src/MK11GameAccessibilityDLL/MK11GameAccessibilityDLL.csproj -c Release
```

### 4. NVDA/JAWS ile Test

1. NVDA veya JAWS'u başlatın
2. MK11'i başlatın
3. Menülerde gezinin (ok tuşları)
4. NVDA/JAWS menü seçeneklerini okuyacaktır

## Nasıl Çalışır

```
MK11 Menü
   ↓ (MK11Hook Lua Script)
Named Pipe: "MK11AccessibilityEvents"
   ↓ (JSON Olayları)
GameEventBroadcaster
   ↓
MenuAccessibilityModule
   ↓
UniversalAccessibilityAnnouncer
   ↓
NVDA / JAWS / Narrator / SAPI
   ↓
🔊 Sesli Bildirim
```

## Örnek Olay Akışı

```json
{"Type":"menu","Text":"Ana Menü","Items":4}
{"Type":"menu_select","Text":"Eğitim Modu","Index":1,"Total":4}
{"Type":"menu_select","Text":"Karakter Seç","Index":2,"Total":4}
{"Type":"round_start","Round":1}
{"Type":"health","Player1Health":100,"Player2Health":100}
{"Type":"combo","Player1Combo":3,"Player2Combo":0}
{"Type":"fight_end","Text":"Oyuncu 1 kazandı"}
```

## Desteklenen Ekran Okuyucular

✅ NVDA (NVDA Controller Client DLL)
✅ JAWS (SAPI fallback)
✅ Windows Narrator (SAPI fallback)
✅ Diğer SAPI uyumlu okuyucular

## Teknik Bilgiler

- **MK11Hook**: Community modding framework
- **Lua**: Script tarafında menü olayları
- **Named Pipe**: Oyun ve dış aplikasyon iletişimi
- **.NET 8**: Erişilebilirlik köprüsü
- **SAPI 5.1**: Sesli bildirim
- **NVDA Controller Client**: NVDA doğrudan entegrasyonu

## Sınırlamalar

- MK11Hook sadece çevrim dışı oyunda çalışır
- Çevrim içi rankında kullanılmamalidir
- Oyunun her güncellemesinde test edilmelidir
- Anti-hile sistemi tarafından engellenebilir

## Gelecek Sürümleri

- [ ] Karakter seçim ekranı navigasyonu
- [ ] Hikaye modu rehberi
- [ ] Turnuva modu sesli rehberi
- [ ] İstatistik raporu seslendirmesi
- [ ] Özelleştirilebilir kısayollar

MIT Lisansı - Mortal Kombat 11 ve Steam ilgili hak sahiplerine aittir.
