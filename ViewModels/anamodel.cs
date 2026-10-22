using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using k7proxyyoneticisi.modeller;
using k7proxyyoneticisi.servisler;

namespace k7proxyyoneticisi.gorunummodelleri
{
    public class anamodel : INotifyPropertyChanged
    {
        private readonly veritabani _veritabani;
        private readonly proxycekici _cekici;
        private readonly proxykontrolcu _kontrolcu;
        private readonly Dispatcher _dispatcher;

        public ObservableCollection<proxy> proxyler { get; } = new();
        public ObservableCollection<kaynak> kaynaklar { get; } = new();
        public ObservableCollection<string> loglar { get; } = new();

        private string _secilitur = "HTTP";
        public string secilitur
        {
            get => _secilitur;
            set { _secilitur = value; bildir(); }
        }

        private int _istenensayi = 100;
        public int istenensayi
        {
            get => _istenensayi;
            set { _istenensayi = value; bildir(); }
        }

        private bool _cekiliyor;
        public bool cekiliyor
        {
            get => _cekiliyor;
            set { _cekiliyor = value; bildir(); bildir(nameof(mesgul)); komutDurumGuncelle(); }
        }

        private bool _kontrolEdiliyor;
        public bool kontrolEdiliyor
        {
            get => _kontrolEdiliyor;
            set { _kontrolEdiliyor = value; bildir(); bildir(nameof(mesgul)); komutDurumGuncelle(); }
        }

        public bool mesgul => _cekiliyor || _kontrolEdiliyor;

        private double _ilerleme;
        public double ilerleme
        {
            get => _ilerleme;
            set { _ilerleme = value; bildir(); }
        }

        private string _durummetni = "Hazır";
        public string durummetni
        {
            get => _durummetni;
            set { _durummetni = value; bildir(); }
        }

        private int _toplam;
        public int toplam { get => _toplam; set { _toplam = value; bildir(); } }

        private int _calisan;
        public int calisan { get => _calisan; set { _calisan = value; bildir(); } }

        private int _calismayan;
        public int calismayan { get => _calismayan; set { _calismayan = value; bildir(); } }

        private int _paralel = 50;
        public int paralel { get => _paralel; set { _paralel = value; bildir(); } }

        private string _yeniKaynakAd = string.Empty;
        public string yeniKaynakAd { get => _yeniKaynakAd; set { _yeniKaynakAd = value; bildir(); } }

        private string _yeniKaynakUrl = string.Empty;
        public string yeniKaynakUrl { get => _yeniKaynakUrl; set { _yeniKaynakUrl = value; bildir(); } }

        private string _yeniKaynakTur = "HTTP";
        public string yeniKaynakTur { get => _yeniKaynakTur; set { _yeniKaynakTur = value; bildir(); } }

        // grafik serileri
        public ObservableCollection<ISeries> turSerisi { get; } = new();
        public ObservableCollection<ISeries> durumSerisi { get; } = new();
        public ObservableCollection<ISeries> ulkeSerisi { get; } = new();
        public ObservableCollection<ISeries> hizSerisi { get; } = new();

        public Axis[] turXAxes { get; set; } = new Axis[] { new Axis { Labels = new[] { "HTTP", "HTTPS", "SOCKS4", "SOCKS5" } } };
        public Axis[] durumXAxes { get; set; } = new Axis[] { new Axis { Labels = new[] { "Çalışıyor", "Çalışmıyor" } } };

        public komut cekKomutu { get; }
        public komut kontrolKomutu { get; }
        public komut temizleKomutu { get; }
        public komut kaydetKomutu { get; }
        public komut kaynakEkleKomutu { get; }
        public komut kaynakSilKomutu { get; }
        public komut secTumuKomutu { get; }
        public komut secTemizleKomutu { get; }
        public komut secilenleriSilKomutu { get; }
        public komut grafikGuncelleKomutu { get; }
        public komut disaAktarKomutu { get; }

        public string[] turSecenekleri { get; } = new[] { "HTTP", "HTTPS", "SOCKS4", "SOCKS5" };
        public string[] anonimlikSecenekleri { get; } = new[] { "Tümü", "Elite", "Anonymous", "Transparent" };

        private string _anonimlikFiltre = "Tümü";
        public string anonimlikFiltre { get => _anonimlikFiltre; set { _anonimlikFiltre = value; bildir(); } }

        private string _ulkeFiltre = string.Empty;
        public string ulkeFiltre { get => _ulkeFiltre; set { _ulkeFiltre = value; bildir(); } }

        private int _minHiz;
        public int minHiz { get => _minHiz; set { _minHiz = value; bildir(); } }

        public anamodel()
        {
            _dispatcher = Application.Current.Dispatcher;
            _veritabani = new veritabani(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "k7proxy.db"));
            _cekici = new proxycekici();
            _kontrolcu = new proxykontrolcu();

            // kaynakları yükle
            var ks = _veritabani.kaynaklariGetir();
            if (ks.Count == 0)
            {
                varsayilanKaynaklariEkle();
                ks = _veritabani.kaynaklariGetir();
            }
            foreach (var k in ks) kaynaklar.Add(k);

            // proxyleri yükle
            foreach (var p in _veritabani.proxyleriGetir()) proxyler.Add(p);
            istatistikGuncelle();
            grafikGuncelle();

            cekKomutu = new komut(() => _ = cekAsync(), () => !cekiliyor && !kontrolEdiliyor);
            kontrolKomutu = new komut(() => _ = kontrolEtAsync(), () => !cekiliyor && !kontrolEdiliyor && proxyler.Count > 0);
            temizleKomutu = new komut(() => temizle(), () => !cekiliyor && !kontrolEdiliyor);
            kaydetKomutu = new komut(() => kaydet(), () => true);
            kaynakEkleKomutu = new komut(() => kaynakEkle(), () => true);
            kaynakSilKomutu = new komut(p => kaynakSil(p as kaynak), _ => true);
            secTumuKomutu = new komut(() => secTumu(), () => true);
            secTemizleKomutu = new komut(() => secTemizle(), () => true);
            secilenleriSilKomutu = new komut(() => secilenleriSil(), () => true);
            grafikGuncelleKomutu = new komut(() => grafikGuncelle(), () => true);
            disaAktarKomutu = new komut(() => disaAktar(), () => true);
        }

        private void varsayilanKaynaklariEkle()
        {
            var liste = new List<kaynak>
            {
                new kaynak { ad = "ProxyScrape HTTP", url = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all", tur = "HTTP", aktif = true },
                new kaynak { ad = "ProxyScrape SOCKS4", url = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks4&timeout=10000&country=all", tur = "SOCKS4", aktif = true },
                new kaynak { ad = "ProxyScrape SOCKS5", url = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks5&timeout=10000&country=all", tur = "SOCKS5", aktif = true },
                new kaynak { ad = "Geonode", url = "https://proxylist.geonode.com/api/proxy-list?limit={adet}&page=1&sort_by=lastChecked&sort_type=desc&protocols={tur}", tur = "HTTP", aktif = true },
                new kaynak { ad = "Proxy-List.Download HTTP", url = "https://www.proxy-list.download/api/v1/get?type=http", tur = "HTTP", aktif = true },
                new kaynak { ad = "Proxy-List.Download SOCKS4", url = "https://www.proxy-list.download/api/v1/get?type=socks4", tur = "SOCKS4", aktif = true },
                new kaynak { ad = "Proxy-List.Download SOCKS5", url = "https://www.proxy-list.download/api/v1/get?type=socks5", tur = "SOCKS5", aktif = true },
                new kaynak { ad = "OpenProxyList HTTP", url = "https://api.openproxylist.xyz/http.txt", tur = "HTTP", aktif = true },
                new kaynak { ad = "OpenProxyList SOCKS4", url = "https://api.openproxylist.xyz/socks4.txt", tur = "SOCKS4", aktif = true },
                new kaynak { ad = "OpenProxyList SOCKS5", url = "https://api.openproxylist.xyz/socks5.txt", tur = "SOCKS5", aktif = true },
            };
            foreach (var k in liste) _veritabani.kaynakEkle(k);
        }

        private async Task cekAsync()
        {
            cekiliyor = true;
            ilerleme = 0;
            durummetni = "Proxy'ler çekiliyor...";
            var aktifKaynaklar = kaynaklar.Where(k => k.aktif && (k.tur == secilitur || secilitur == "TÜMÜ")).ToList();
            // tur eşleşmesi: kaynak turu secilen ile aynı olmalı; HTTPS için HTTP kaynakları da kullanılabilir
            if (secilitur == "HTTPS")
                aktifKaynaklar = kaynaklar.Where(k => k.aktif && (k.tur == "HTTP" || k.tur == "HTTPS")).ToList();

            int tamamlanan = 0;
            var toplamKaynak = aktifKaynaklar.Count;
            var tumbulunan = new List<proxy>();

            var gorevler = aktifKaynaklar.Select(async k =>
            {
                try
                {
                    var liste = await _cekici.cekAsync(k, secilitur, istenensayi, CancellationToken.None);
                    lock (tumbulunan) tumbulunan.AddRange(liste);
                    _dispatcher.Invoke(() => k.cekilensayi = liste.Count);
                }
                catch { }
                tamamlanan++;
                _dispatcher.Invoke(() =>
                {
                    ilerleme = toplamKaynak == 0 ? 100 : (double)tamamlanan / toplamKaynak * 100;
                    durummetni = $"{tamamlanan}/{toplamKaynak} kaynak tamamlandı - {tumbulunan.Count} proxy";
                });
            }).ToList();

            await Task.WhenAll(gorevler);

            // tekilleştir
            var benzersiz = tumbulunan
                .GroupBy(p => $"{p.ip}:{p.port}")
                .Select(g => g.First())
                .ToList();

            // limit
            if (benzersiz.Count > istenensayi)
                benzersiz = benzersiz.Take(istenensayi).ToList();

            _dispatcher.Invoke(() =>
            {
                foreach (var p in benzersiz) proxyler.Add(p);
                istatistikGuncelle();
                grafikGuncelle();
                durummetni = $"{benzersiz.Count} proxy eklendi. Toplam: {proxyler.Count}";
                ilerleme = 100;
                loglar.Insert(0, $"{DateTime.Now:HH:mm:ss} - {benzersiz.Count} proxy çekildi");
                if (loglar.Count > 60) loglar.RemoveAt(loglar.Count - 1);
            });

            cekiliyor = false;
        }

        private async Task kontrolEtAsync()
        {
            kontrolEdiliyor = true;
            ilerleme = 0;
            durummetni = "Proxy'ler kontrol ediliyor...";
            var hedefler = proxyler.Where(filtreUygun).ToList();
            int tamamlanan = 0;
            int toplam = hedefler.Count;
            var semafor = new SemaphoreSlim(paralel);

            var gorevler = hedefler.Select(async p =>
            {
                await semafor.WaitAsync();
                try
                {
                    await _kontrolcu.kontrolEtAsync(p, CancellationToken.None);
                }
                finally
                {
                    semafor.Release();
                    tamamlanan++;
                    _dispatcher.Invoke(() =>
                    {
                        ilerleme = toplam == 0 ? 100 : (double)tamamlanan / toplam * 100;
                        durummetni = $"{tamamlanan}/{toplam} kontrol edildi";
                        istatistikGuncelle();
                    });
                }
            }).ToList();

            await Task.WhenAll(gorevler);

            _dispatcher.Invoke(() =>
            {
                grafikGuncelle();
                durummetni = $"Kontrol tamamlandı. Çalışan: {calisan}";
                ilerleme = 100;
                _dispatcher.Invoke(() =>
                {
                    loglar.Insert(0, $"{DateTime.Now:HH:mm:ss} - Kontrol tamamlandı: {calisan} çalışan");
                    if (loglar.Count > 60) loglar.RemoveAt(loglar.Count - 1);
                });
            });
            kontrolEdiliyor = false;
        }

        private bool filtreUygun(proxy p)
        {
            if (secilitur != "TÜMÜ" && p.tur != secilitur) 
            {
                // HTTPS isteniyorsa HTTP proxyler de kontrol edilebilir; şimdilik tam eşleşme
            }
            if (anonimlikFiltre != "Tümü" && p.anonimlik != anonimlikFiltre) return false;
            if (!string.IsNullOrWhiteSpace(ulkeFiltre) && !p.ulke.Contains(ulkeFiltre, StringComparison.OrdinalIgnoreCase)) return false;
            if (minHiz > 0 && p.hiz > 0 && p.hiz > minHiz) return false;
            return true;
        }

        private void temizle()
        {
            proxyler.Clear();
            _veritabani.proxyleriTemizle();
            istatistikGuncelle();
            grafikGuncelle();
            durummetni = "Liste temizlendi";
        }

        private void kaydet()
        {
            _veritabani.proxyleriTemizle();
            foreach (var p in proxyler) _veritabani.proxyEkle(p);
            durummetni = $"{proxyler.Count} proxy kaydedildi";
        }

        private void kaynakEkle()
        {
            if (string.IsNullOrWhiteSpace(yeniKaynakUrl)) return;
            var k = new kaynak { ad = string.IsNullOrWhiteSpace(yeniKaynakAd) ? "Yeni Kaynak" : yeniKaynakAd, url = yeniKaynakUrl, tur = yeniKaynakTur, aktif = true };
            _veritabani.kaynakEkle(k);
            // id almak için yeniden yükle
            kaynaklar.Clear();
            foreach (var x in _veritabani.kaynaklariGetir()) kaynaklar.Add(x);
            yeniKaynakAd = string.Empty;
            yeniKaynakUrl = string.Empty;
            durummetni = "Kaynak eklendi";
        }

        private void kaynakSil(kaynak? k)
        {
            if (k == null) return;
            _veritabani.kaynakSil(k.id);
            kaynaklar.Remove(k);
            durummetni = "Kaynak silindi";
        }

        private void secTumu()
        {
            foreach (var p in proxyler) p.secili = true;
        }

        private void secTemizle()
        {
            foreach (var p in proxyler) p.secili = false;
        }

        private void secilenleriSil()
        {
            var secilenler = proxyler.Where(p => p.secili).ToList();
            foreach (var p in secilenler) proxyler.Remove(p);
            istatistikGuncelle();
            grafikGuncelle();
            durummetni = $"{secilenler.Count} proxy silindi";
        }

        private void disaAktar()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Metin Dosyası (*.txt)|*.txt|CSV Dosyası (*.csv)|*.csv",
                FileName = "proxyler.txt"
            };
            if (dialog.ShowDialog() != true) return;
            try
            {
                if (dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    var satirlar = new List<string> { "ip,port,tur,ulke,anonimlik,hiz,durum,kaynak" };
                    satirlar.AddRange(proxyler.Select(p => $"{p.ip},{p.port},{p.tur},{p.ulke},{p.anonimlik},{p.hiz},{p.durummetni},{p.kaynak}"));
                    System.IO.File.WriteAllLines(dialog.FileName, satirlar);
                }
                else
                {
                    System.IO.File.WriteAllLines(dialog.FileName, proxyler.Select(p => $"{p.ip}:{p.port}"));
                }
                durummetni = $"{proxyler.Count} proxy dışa aktarıldı";
            }
            catch (Exception ex)
            {
                durummetni = "Dışa aktarma hatası: " + ex.Message;
            }
        }

        private void istatistikGuncelle()
        {
            toplam = proxyler.Count;
            calisan = proxyler.Count(p => p.calisiyor);
            calismayan = proxyler.Count(p => !p.calisiyor && p.durummetni == "Çalışmıyor");
        }

        public void grafikGuncelle()
        {
            // tür dağılımı
            var turSayilari = new Dictionary<string, int>
            {
                ["HTTP"] = proxyler.Count(p => p.tur == "HTTP"),
                ["HTTPS"] = proxyler.Count(p => p.tur == "HTTPS"),
                ["SOCKS4"] = proxyler.Count(p => p.tur == "SOCKS4"),
                ["SOCKS5"] = proxyler.Count(p => p.tur == "SOCKS5"),
            };

            turSerisi.Clear();
            turSerisi.Add(new ColumnSeries<int>
            {
                Values = new[] { turSayilari["HTTP"], turSayilari["HTTPS"], turSayilari["SOCKS4"], turSayilari["SOCKS5"] },
                Name = "Proxy Sayısı",
                Fill = new SolidColorPaint(SKColors.DodgerBlue)
            });

            // durum
            var calisanSayi = proxyler.Count(p => p.calisiyor);
            var calismayanSayi = proxyler.Count(p => !p.calisiyor);
            durumSerisi.Clear();
            durumSerisi.Add(new PieSeries<int> { Values = new[] { calisanSayi }, Name = "Çalışıyor", Fill = new SolidColorPaint(SKColors.LimeGreen) });
            durumSerisi.Add(new PieSeries<int> { Values = new[] { calismayanSayi }, Name = "Çalışmıyor", Fill = new SolidColorPaint(SKColors.OrangeRed) });

            // ülke dağılımı (top 8)
            var ulkeGrup = proxyler.GroupBy(p => p.ulke).OrderByDescending(g => g.Count()).Take(8).ToList();
            ulkeSerisi.Clear();
            ulkeSerisi.Add(new ColumnSeries<int>
            {
                Values = ulkeGrup.Select(g => g.Count()).ToArray(),
                Name = "Ülke",
                Fill = new SolidColorPaint(SKColors.MediumPurple)
            });
            // x ekseni etiketleri
            // hiz histogramı
            var hizlar = proxyler.Where(p => p.hiz > 0).Select(p => p.hiz).ToArray();
            hizSerisi.Clear();
            hizSerisi.Add(new LineSeries<int>
            {
                Values = hizlar.OrderBy(x => x).ToArray(),
                Name = "Gecikme (ms)",
                Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 2 },
                Fill = null
            });
        }

        private void komutDurumGuncelle()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void bildir([CallerMemberName] string? ad = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ad));
    }
}