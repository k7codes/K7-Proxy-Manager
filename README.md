# 🛡️ K7 Proxy Yöneticisi

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/WPF-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white" alt="WPF"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License"/>
  <img src="https://img.shields.io/badge/PRs-Welcome-brightgreen?style=for-the-badge" alt="PRs Welcome"/>
  <img src="https://img.shields.io/github/stars/k7proxy/k7-proxy-yoneticisi?style=for-the-badge" alt="Stars"/>
</p>

<p align="center">
  <b>Modern, hızlı ve aşırı detaylı bir proxy yönetim aracı.</b><br/>
  Çoklu API kaynaklarından proxy çekme · Otomatik kontrol · Anonimlik seviyesi · Ülke tespiti · Gecikme ölçümü · LiveCharts grafikleri · SQLite kalıcılık
</p>

---

## 📸 Ekran Görüntüleri

| Ana Sayfa | Proxy Çekme |
|-----------|-------------|
| ![Ana Sayfa](screenshots/anasayfa.png) | ![Proxy Çekme](screenshots/cekme.png) |

| Proxy Listesi | Grafikler |
|---------------|-----------|
| ![Proxy Listesi](screenshots/liste.png) | ![Grafikler](screenshots/grafikler.png) |

> 📌 Ekran görüntüleri `screenshots/` klasörüne eklenecektir.

---

## ✨ Özellikler

### 🔌 Çoklu Kaynak Desteği
- **ProxyScrape** (HTTP / SOCKS4 / SOCKS5)
- **Geonode** (JSON API, ülke & anonimlik bilgisi)
- **Proxy-List.Download** (HTTP / SOCKS4 / SOCKS5)
- **OpenProxyList** (HTTP / SOCKS4 / SOCKS5)
- Kullanıcı tanımlı özel kaynak ekleme / silme / aktif-pasif yönetimi

### 🎯 Detaylı Proxy Çekme
- İstenen **tür** seçimi (HTTP, HTTPS, SOCKS4, SOCKS5)
- İstenen **adet** (10 – 1000+)
- Paralel bağlantı sayısı ayarı (1 – 200)
- Otomatik tekilleştirme (IP:Port bazlı)
- İlerleme çubuğu + canlı durum metni

### 🔍 Proxy Kontrolü (Checking)
- TCP bağlantı testi (zaman aşımı ile)
- HTTP GET üzerinden canlılık doğrulama (`httpbin.org/headers`)
- **Gecikme (ms)** ölçümü (Stopwatch)
- **Anonimlik seviyesi** tespiti (Elite / Anonymous / Transparent) — `X-Forwarded-For`, `Via`, `Proxy-Connection` başlık analizi
- **Ülke** tespiti (ip-api.com)
- Paralel kontrol (SemaphoreSlim ile sınırlı eşzamanlılık)

### 📊 Grafikler & İstatistikler
- **Tür dağılımı** (sütun grafik)
- **Durum dağılımı** (pasta grafik — çalışan / ölü)
- **Ülke dağılımı** (top 8, sütun grafik)
- **Gecikme** grafiği (çizgi grafik)
- Anlık sayaçlar: Toplam / Çalışan / Ölü / Kaynak

### 💾 Kalıcılık
- SQLite veritabanı (`k7proxy.db`)
- Proxy'leri ve kaynakları kalıcı olarak saklama
- Uygulama kapatılıp açılınca veriler korunur

### 🎨 Modern Arayüz
- **HeraProtector monokrom tema**: derin siyah (#040405) + parlayan beyaz glow
- Glassmorphism kartlar, hover glow efektleri
- Özel pencere chrome (başlıksız, sürüklenebilir, resize)
- Sol navigasyon rail (ikon + metin)
- Sekme geçiş animasyonları (fade-in + slide-up)
- Nabız atan durum noktası, dönen spinner overlay
- Arka plan glow ellipse animasyonları
- Context menu, tooltip, expander, slider bileşenleri

### 📤 Dışa Aktarma
- TXT (IP:Port listesi)
- CSV (tüm alanlar)
- Panoya kopyalama (çalışan proxy'ler)

---

## 🏗️ Mimari

```
K7ProxyYoneticisi/
├── App.xaml / App.xaml.cs          # Uygulama başlangıcı, tema kaynakları
├── MainWindow.xaml / .cs           # Ana pencere, custom chrome, navigasyon
├── Models/
│   ├── proxy.cs                    # Proxy modeli (INotifyPropertyChanged)
│   └── kaynak.cs                   # Kaynak (API) modeli
├── Services/
│   ├── proxycekici.cs              # Çoklu API'den proxy çekme & ayrıştırma
│   ├── proxykontrolcu.cs           # TCP + HTTP kontrol, anonimlik, ülke
│   └── veritabani.cs               # SQLite CRUD işlemleri
├── ViewModels/
│   ├── anamodel.cs                 # Ana ViewModel (komutlar, durum, grafikler)
│   └── komut.cs                    # RelayCommand implementasyonu
├── Converters/
│   └── boolGorunurluk.cs           # bool → Visibility converter
├── Themes/
│   └── DarkTheme.xaml              # (legacy) koyu tema
└── K7ProxyYoneticisi.csproj
```

### Tasarım Deseni
- **MVVM** (Model-View-ViewModel)
- **RelayCommand** ile komut bağlama
- **ObservableCollection** ile canlı UI güncelleme
- **INotifyPropertyChanged** ile veri bağlama

---

## 🛠️ Teknoloji Yığını

| Teknoloji | Kullanım |
|-----------|----------|
| **.NET 8** | Hedef framework |
| **WPF** | Masaüstü arayüz |
| **LiveChartsCore.SkiaSharpView.WPF** | Grafikler (sütun, pasta, çizgi) |
| **Microsoft.Data.Sqlite** | Yerel veritabanı |
| **System.Text.Json** | JSON ayrıştırma |
| **SkiaSharp** | Grafik render motoru |

---

## 📦 Kurulum

### Gereksinimler
- Windows 10/11
- .NET 8 SDK ([indir](https://dotnet.microsoft.com/download/dotnet/8.0))
- Visual Studio 2022 (önerilen) veya Rider

### Adımlar

```bash
# 1. Depoyu klonla
git clone https://github.com/kullaniciadi/k7-proxy-yoneticisi.git
cd k7-proxy-yoneticisi

# 2. Bağımlılıkları geri yükle
dotnet restore

# 3. Derle
dotnet build

# 4. Çalıştır
dotnet run --project K7ProxyYoneticisi
```

### Visual Studio ile
1. `K7ProxyYoneticisi.sln` dosyasını açın (veya .csproj)
2. `F5` ile çalıştırın

---

## 🚀 Kullanım

### 1. Proxy Çekme
1. **PROXY ÇEK** sekmesine gidin
2. Tür seçin (HTTP / HTTPS / SOCKS4 / SOCKS5)
3. Adet belirleyin (slider veya textbox)
4. **ÇEKMEYİ BAŞLAT** butonuna tıklayın
5. İlerleme çubuğunu izleyin

### 2. Proxy Kontrolü
1. **KONTROL** sekmesine gidin
2. Paralel bağlantı sayısını ayarlayın
3. **KONTROLÜ BAŞLAT** butonuna tıklayın
4. Çalışan / ölü sayılarını ve grafikleri izleyin

### 3. Filtreleme
- **PROXY LİSTESİ** sekmesinde tür, anonimlik, ülke filtreleri
- "TÜMÜNÜ SEÇ" / "SEÇİMİ TEMİZLE" / "SEÇİLENLERİ SİL"

### 4. Dışa Aktarma
- TXT: `IP:Port` listesi
- CSV: tüm alanlar
- "ÇALIŞANLARI KOPYALA" → panoya kopyalar

### 5. Kaynak Yönetimi
- **KAYNAKLAR** sekmesinden yeni API kaynağı ekleyin
- URL'de `{tur}` ve `{adet}` yer tutucuları kullanabilirsiniz
- Kaynakları aktif/pasif yapın

---

## 🔌 Desteklenen API Kaynakları

| Kaynak | Format | Türler |
|--------|--------|--------|
| ProxyScrape | Plain text | HTTP, SOCKS4, SOCKS5 |
| Geonode | JSON | HTTP, SOCKS4, SOCKS5 |
| Proxy-List.Download | Plain text | HTTP, SOCKS4, SOCKS5 |
| OpenProxyList | Plain text | HTTP, SOCKS4, SOCKS5 |

---

## ⚙️ Yapılandırma

Veritabanı dosyası (`k7proxy.db`) uygulama klasöründe oluşturulur. İlk açılışta varsayılan 10 kaynak otomatik eklenir.

### Özel Kaynak URL Formatı
```
https://example.com/proxies?type={tur}&limit={adet}
```
- `{tur}` → seçilen tür (http, socks4, socks5)
- `{adet}` → istenen proxy sayısı

---

## 🤝 Katkıda Bulunma

1. Fork'layın
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Commit'leyin (`git commit -m 'Yeni özellik: ...'`)
4. Push'layın (`git push origin feature/yeni-ozellik`)
5. Pull Request açın

### Kod Standartları
- Tüm özel tanımlayıcılar **Türkçe küçük harf**
- Arayüz etiketleri **Türkçe büyük harf**
- MVVM deseni korunmalı
- Her yeni özellik için README güncellenmeli

---

## 📋 Yol Haritası

- [ ] Proxy rotasyon (rotation) desteği
- [ ] Web panel / REST API
- [ ] Otomatik zamanlanmış çekme
- [ ] Proxy kalite skorlama
- [ ] Çoklu dil desteği (i18n)
- [ ] Tema seçici (açık/koyu)
- [ ] Export: JSON, XML

---

## 🐛 Bilinen Sorunlar

- Bazı proxy API'leri (Proxy-List.Download) zaman zaman 502 döndürebilir — kaynak pasif yapılabilir.
- SOCKS proxy desteği .NET 8'in SocketsHttpHandler'ına bağlıdır; eski sistemlerde çalışmayabilir.
- `httpbin.org` erişilemezse anonimlik tespiti "Elite" olarak varsayılan döner.

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.

---

## 🙏 Teşekkürler

- [LiveCharts](https://livecharts.dev/) — harika grafik kütüphanesi
- [HeraProtector](https://github.com/) — monokrom tema ilhamı
- [MaterialDesignInXamlToolkit](https://materialdesigninxaml.net/) — referans tasarım

---

<p align="center">
  <b>K7 Proxy Yöneticisi</b> ile proxy yönetimini bir üst seviyeye taşıyın! 🚀
</p>