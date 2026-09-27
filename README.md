# v0.7 — Evrensel ekran okuyucu köprüsü

Eklenen destek:

- NVDA Controller Client DLL mevcutsa doğrudan NVDA konuşma çıktısı
- JAWS çalışıyorsa SAPI uyumlu çıktı
- Windows Narrator çalışıyorsa SAPI uyumlu çıktı
- Ekran okuyucu yoksa SAPI fallback
- Tek bir `UniversalAccessibilityAnnouncer` API'si
- Mevcut yerel JSON olay köprüsü korunur

Örnek olay:

```json
{"Type":"round_start","Round":1}
{"Type":"health","Player1Health":80,"Player2Health":65}
{"Type":"combo","Player1Combo":4,"Player2Combo":0}
```

## Gerçek oyun entegrasyonu hakkında

Bu sürüm ekran okuyuculara ses çıktısı vermeye hazır bir köprü sağlar; fakat oyun içindeki gerçek olayları sağlayacak izinli bir MK11 eklentisi gerekir. Kod, rastgele bellek okuma, DLL enjeksiyonu veya anti-hile atlatma yapmaz. Bu nedenle NVDA/JAWS desteği oyuna otomatik olarak “enjekte edilmiş” değildir.

NVDA doğrudan bağlantısı için kullanıcının resmi NVDA Controller Client DLL'sini sistemde bulundurması gerekir. JAWS ve Narrator için güvenli genel fallback SAPI'dir; üreticiye özel SDK olmadan sahte bir API çağrısı yapılmaz.
