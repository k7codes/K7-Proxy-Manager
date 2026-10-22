using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using k7proxyyoneticisi.modeller;

namespace k7proxyyoneticisi.servisler
{
    public class proxycekici
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        static proxycekici()
        {
            _http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) K7ProxyManager/1.0");
        }

        public async Task<List<proxy>> cekAsync(kaynak k, string istenenTur, int adet, CancellationToken iptal)
        {
            var sonuc = new List<proxy>();
            try
            {
                var url = k.url
                    .Replace("{tur}", TurParametresi(k.tur, istenenTur))
                    .Replace("{adet}", Math.Max(adet, 100).ToString())
                    .Replace("{TUR}", TurParametresi(k.tur, istenenTur).ToUpper());

                using var yanit = await _http.GetAsync(url, iptal);
                if (!yanit.IsSuccessStatusCode) return sonuc;
                var icerik = await yanit.Content.ReadAsStringAsync(iptal);

                var tur = istenenTur.ToUpper();
                if (tur == "TÜMÜ") tur = k.tur.ToUpper();
                sonuc = Ayrıştır(icerik, tur, k.ad);
            }
            catch { }

            // tekilleştir
            sonuc = sonuc
                .GroupBy(p => $"{p.ip}:{p.port}")
                .Select(g => g.First())
                .ToList();

            if (sonuc.Count > adet)
                sonuc = sonuc.Take(adet).ToList();

            return sonuc;
        }

        private static string TurParametresi(string kaynakTur, string istenenTur)
        {
            var t = istenenTur.ToLower();
            if (t == "https") t = "http";
            // bazı kaynaklar turu kendisi içerir
            return t;
        }

        private static List<proxy> Ayrıştır(string icerik, string varsayilanTur, string kaynakAd)
        {
            var liste = new List<proxy>();

            // JSON deneme
            icerik = icerik.Trim();
            if (icerik.StartsWith("{") || icerik.StartsWith("["))
            {
                try
                {
                    using var doc = JsonDocument.Parse(icerik);
                    JsonElement root = doc.RootElement;

                    JsonElement dizi = root;
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Array) dizi = d;
                        else if (root.TryGetProperty("proxies", out var p) && p.ValueKind == JsonValueKind.Array) dizi = p;
                        else if (root.TryGetProperty("results", out var r) && r.ValueKind == JsonValueKind.Array) dizi = r;
                        else
                        {
                            // tek obje? skip
                            dizi = default;
                        }
                    }

                    if (dizi.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in dizi.EnumerateArray())
                        {
                            var p = JsonProxy(item, varsayilanTur, kaynakAd);
                            if (p != null) liste.Add(p);
                        }
                        return liste;
                    }
                }
                catch { }
            }

            // Plain text / HTML: ip:port satırları
            var regex = new Regex(@"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})[:\s]+(\d{2,5})\b");
            foreach (Match m in regex.Matches(icerik))
            {
                if (!IPAddress.TryParse(m.Groups[1].Value, out _)) continue;
                var ip = m.Groups[1].Value;
                if (!int.TryParse(m.Groups[2].Value, out var port)) continue;
                if (port <= 0 || port > 65535) continue;
                liste.Add(new proxy
                {
                    ip = ip,
                    port = port,
                    tur = varsayilanTur.ToUpper(),
                    kaynak = kaynakAd,
                    tarih = DateTime.Now
                });
            }
            return liste;
        }

        private static proxy? JsonProxy(JsonElement item, string varsayilanTur, string kaynakAd)
        {
            try
            {
                string ip = string.Empty;
                if (item.TryGetProperty("ip", out var ipEl)) ip = ipEl.ToString();
                else if (item.TryGetProperty("host", out var h)) ip = h.ToString();
                else if (item.TryGetProperty("address", out var a)) ip = a.ToString();
                if (string.IsNullOrWhiteSpace(ip)) return null;

                int port = 0;
                if (item.TryGetProperty("port", out var portEl))
                {
                    if (portEl.ValueKind == JsonValueKind.Number) port = portEl.GetInt32();
                    else int.TryParse(portEl.GetString(), out port);
                }
                if (port <= 0) return null;

                string tur = varsayilanTur.ToUpper();
                if (item.TryGetProperty("protocols", out var prots) && prots.ValueKind == JsonValueKind.Array)
                {
                    var ilk = prots.EnumerateArray().FirstOrDefault();
                    if (ilk.ValueKind == JsonValueKind.String) tur = ilk.GetString()!.ToUpper();
                }
                else if (item.TryGetProperty("protocol", out var prot) && prot.ValueKind == JsonValueKind.String)
                {
                    tur = prot.GetString()!.ToUpper();
                }
                else if (item.TryGetProperty("type", out var tp) && tp.ValueKind == JsonValueKind.String)
                {
                    tur = tp.GetString()!.ToUpper();
                }

                string ulke = "Bilinmiyor";
                if (item.TryGetProperty("country", out var c) && c.ValueKind == JsonValueKind.String) ulke = c.GetString()!;
                else if (item.TryGetProperty("country_code", out var cc) && cc.ValueKind == JsonValueKind.String) ulke = cc.GetString()!;

                string anon = "Bilinmiyor";
                if (item.TryGetProperty("anonymity", out var an) && an.ValueKind == JsonValueKind.String) anon = an.GetString()!;
                else if (item.TryGetProperty("anonymityLevel", out var al) && al.ValueKind == JsonValueKind.String) anon = al.GetString()!;

                return new proxy
                {
                    ip = ip,
                    port = port,
                    tur = tur,
                    ulke = ulke,
                    anonimlik = anon,
                    kaynak = kaynakAd,
                    tarih = DateTime.Now
                };
            }
            catch { return null; }
        }
    }
}