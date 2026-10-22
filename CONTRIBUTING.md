# Katkıda Bulunma Rehberi

K7 Proxy Yöneticisi'ne katkıda bulunmak istediğiniz için teşekkürler! 🎉

## Başlangıç

1. Depoyu fork'layın
2. Yerel makinenize klonlayın
3. Yeni bir branch oluşturun: `git checkout -b feature/yeni-ozellik`

## Geliştirme Ortamı

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 veya Rider

```bash
dotnet restore
dotnet build
dotnet run --project K7ProxyYoneticisi
```

## Kod Standartları

- Tüm özel tanımlayıcılar **Türkçe küçük harf** (örn: `proxycekici`, `veritabani`)
- Arayüz etiketleri **Türkçe büyük harf** (örn: "PROXY ÇEK", "KONTROL ET")
- MVVM deseni korunmalı — View code-behind minimal tutulmalı
- Her yeni özellik için README güncellenmeli
- Commit mesajları açıklayıcı olmalı

## Pull Request Süreci

1. Değişikliklerinizi test edin
2. README'yi güncelleyin (gerekirse)
3. Açıklayıcı bir PR açıklaması yazın
4. Review bekleyin

## Hata Bildirimi

GitHub Issues kullanın. Lütfen şunları ekleyin:
- İşletim sistemi ve .NET sürümü
- Yeniden üretme adımları
- Beklenen vs. gerçekleşen davranış
- Ekran görüntüleri (varsa)

## Lisans

Katkılarınız MIT Lisansı altında lisanslanacaktır.