# Değişiklik Günlüğü

Tüm önemli değişiklikler bu dosyada belgelenir.

Format: [Keep a Changelog](https://keepachangelog.com/tr/1.0.0/)
Sürümleme: [Semantic Versioning](https://semver.org/lang/tr/)

## [1.0.0] - 2026-10-22

### Eklendi
- Çoklu API kaynaklarından proxy çekme (ProxyScrape, Geonode, Proxy-List.Download, OpenProxyList)
- HTTP / HTTPS / SOCKS4 / SOCKS5 tür desteği
- TCP + HTTP üzerinden proxy kontrolü
- Gecikme (ms) ölçümü
- Anonimlik seviyesi tespiti (Elite / Anonymous / Transparent)
- Ülke tespiti (ip-api.com)
- LiveCharts2 grafikleri (tür, durum, ülke, gecikme)
- SQLite kalıcılık (proxy'ler + kaynaklar)
- Kullanıcı tanımlı kaynak yönetimi
- Filtreleme (tür, anonimlik, ülke)
- Dışa aktarma (TXT, CSV)
- Panoya kopyalama
- HeraProtector monokrom tema (derin siyah + parlayan beyaz glow)
- Özel pencere chrome (başlıksız, resize)
- Sekme geçiş animasyonları
- Yüklenme overlay'i (spinner)
- Aktivite logu
- Context menu (IP kopyala, seç, sil)

### Değiştirildi
- Arayüz Material Design'dan HeraProtector monokrom temaya geçirildi
- Navigasyon TabControl sol rail olarak yeniden tasarlandı

### Bilinen Sorunlar
- Bazı proxy API'leri zaman zaman 502 döndürebilir
- SOCKS desteği .NET 8 SocketsHttpHandler'a bağlıdır