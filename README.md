# Mortal Kombat 11 Erişilebilirlik Modu

## v0.6 — güvenli oyun-içi entegrasyon temeli

Bu sürümde sesli bildirim katmanı, rastgele bellek adreslerine veya DLL enjeksiyonuna dayanmayacak şekilde yeniden düzenlendi.

### Neler çalışır?

- SAPI üzerinden Türkçe sesli bildirim kuyruğu
- Yerel Named Pipe üzerinden JSON olaylarını alma
- Menü, tur, sağlık, kombo ve maç sonu olaylarını seslendirme
- Oyun eklentisi/izinli entegrasyon bulunana kadar güvenli biçimde bekleme

### Olay kanalı

Kanal adı: `MK11AccessibilityEvents`

Her satır bir JSON olayıdır. Örnek:

```json
{"Type":"round_start","Round":1}
{"Type":"health","Player1Health":80,"Player2Health":65}
{"Type":"combo","Player1Combo":4,"Player2Combo":0}
{"Type":"menu","Text":"Training mode"}
{"Type":"fight_end","Text":"Oyuncu 1 kazandı"}
```

### Önemli sınırlama

Mortal Kombat 11 için yayımlanmış, desteklenen bir oyun-içi ekran okuyucu API'si olmadan bu depo tek başına oyunun görüntüsünü okuyamaz. Gerçek oyun olaylarını sağlayan izinli bir eklenti veya mod entegrasyonu gerekir. Bu nedenle v0.6 **oyuna enjekte olan hazır bir DLL değildir**; bilinmeyen bellek adreslerini okumaz, anti-hileyi aşmaz ve çevrim içi oyuna müdahale etmez.

Bu tercih kasıtlıdır: örnek adreslerle bellek okumak oyunu bozabilir, güncellemelerde çalışmaz ve çevrim içi hesap açısından risk oluşturabilir. Oyun içi taraf için sonraki çalışma, yalnızca çevrim dışı/eğitim modu ve oyunun izin verdiği modlama yüzeyi üzerinde yapılmalıdır.

## Derleme

```powershell
dotnet build src/MK11GameAccessibilityDLL/MK11AccessibilityDLL.csproj -c Release
```

Kod MIT lisansı ile sunulur. Mortal Kombat 11 ve Steam ilgili hak sahiplerine aittir.
