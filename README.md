# Mortal Kombat 11 Erişilebilirlik Modu

Bu depo, görme engelli oyuncular için Mortal Kombat 11'e erişilebilirlik özellikleri eklemek üzere hazırlanmış **açık kaynak bir başlangıç projesidir**.

> Önemli: Bu sürüm kurulum altyapısını ve erişilebilir kurulum arayüzünü içerir. Oyunun sesli oyun içi bildirimleri için Mortal Kombat 11'in sürümüne uygun, izinli bir mod bileşeni ayrıca geliştirilmelidir. Çevrim içi rekabeti etkileyen hile/otomasyon özellikleri bu projeye dahil edilmeyecektir.

## Özellikler

- ✅ Steam kütüphanelerini ve Mortal Kombat 11 kurulumunu otomatik bulma
- ✅ Kurulumdan önce modifiye edilecek dosyaları yedekleme
- ✅ Geri alma (restore) desteği
- ✅ NVDA ve JAWS ile kullanılabilen standart Windows erişilebilirlik kontrolleri
- ✅ Windows SAPI ile kurulum durumunu sesli bildirme
- ✅ Türkçe ve İngilizce arayüz metinleri için genişletilebilir yapı

## Gereksinimler

- Windows 10 veya 11
- .NET 8 Desktop Runtime
- Steam üzerinden kurulmuş Mortal Kombat 11
- NVDA veya JAWS (isteğe bağlı)

## Derleme

```powershell
dotnet build src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj -c Release
```

## Çalıştırma

```powershell
dotnet run --project src/MK11AccessibilityInstaller/MK11AccessibilityInstaller.csproj
```

`ModPayload` klasörüne gerçek ve test edilmiş mod dosyaları konulduğunda kurulum düğmesi bunları oyun klasörüne kopyalar. Gerçek mod dosyaları eklenmeden kurulum yapmak yalnızca yedekleme/kurulum altyapısını test eder.

## Proje Yapısı

```
├── src/
│   └── MK11AccessibilityInstaller/
│       ├── MK11AccessibilityInstaller.csproj
│       ├── Program.cs
│       ├── MainForm.cs
│       ├── SteamLocator.cs
│       └── InstallerService.cs
├── ModPayload/
│   └── README.txt
├── README.md
├── LICENSE
└── .gitignore
```

### Dosya Açıklamaları

- **SteamLocator.cs**: Steam kurulum yolunu bulur, kütüphane dizinlerini tarar
- **InstallerService.cs**: Mod dosyalarını kopyalar, yedekleme ve geri yükleme yapır
- **MainForm.cs**: Erişilebilir Windows Forms arayüzü, NVDA/JAWS uyumlu
- **ModPayload/**: Kurulum yapılacak mod dosyalarının yerleştirildiği dizin

## Güvenlik ve Kullanım

- Kurulumdan önce oyun klasörünün yedeğini alın.
- Modu yalnızca çevrim dışı/izin verilen kullanım senaryolarında kullanın.
- Steam güncellemeleri mod dosyalarını değiştirebilir; güncelleme sonrası yeniden kurulum gerekebilir.
- Proje, lisanslı Mortal Kombat 11 dosyalarını içermez; yalnızca kullanıcı tarafından sağlanan mod dosyalarını kopyalar.

## Erişilebilirlik Özellikleri (Gelecek Sürümler)

- [ ] Sesli oyun durumu bildirimleri
- [ ] Karakter konumu ve durum açıklamaları
- [ ] Kombo rehberi ve eğitim modu
- [ ] Menü ve ayarlar erişilebilirliği
- [ ] Oyun içi şampiyon ve harita açıklamaları
- [ ] Turnuva ve hikaye modu rehberi

## Lisans

Kod MIT lisansı ile sunulur. Mortal Kombat 11 ve Steam, ilgili hak sahiplerine aittir.

## Katkı

Erişilebilirlik iyileştirmeleri ve bug raporları için issue açabilirsiniz.
