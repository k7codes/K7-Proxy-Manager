using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using k7proxyyoneticisi.modeller;

namespace k7proxyyoneticisi.servisler
{
    public class proxykontrolcu
    {
        private static readonly HttpClient _geoHttp = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };

        public async Task kontrolEtAsync(proxy p, CancellationToken iptal)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                // TCP connect kontrolü
                using var tcp = new TcpClient();
                var connectTask = tcp.ConnectAsync(p.ip, p.port);
                var delayTask = Task.Delay(TimeSpan.FromSeconds(8), iptal);
                if (await Task.WhenAny(connectTask, delayTask) != connectTask)
                    throw new TimeoutException("TCP zaman aşımı");
                await connectTask;
                if (!tcp.Connected) throw new Exception("Bağlantı kurulamadı");

                // HTTP GET üzerinden kontrol
                string sema = p.tur.ToUpper() switch
                {
                    "SOCKS4" => "socks4",
                    "SOCKS5" => "socks5",
                    _ => "http"
                };
                var webProxy = new WebProxy($"{sema}://{p.ip}:{p.port}");
                var handler = new HttpClientHandler
                {
                    Proxy = webProxy,
                    UseProxy = true,
                    PreAuthenticate = false
                };
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
                var yanit = await client.GetAsync("http://httpbin.org/headers", iptal);
                sw.Stop();

                if (yanit.IsSuccessStatusCode)
                {
                    var icerik = await yanit.Content.ReadAsStringAsync(iptal);
                    p.calisiyor = true;
                    p.hiz = (int)sw.ElapsedMilliseconds;
                    p.anonimlik = AnonimlikBelirle(icerik);
                    // ülke bilgisi
                    _ = UlkeGetirAsync(p, iptal);
                }
                else
                {
                    p.calisiyor = false;
                    p.hiz = 0;
                }
            }
            catch
            {
                sw.Stop();
                p.calisiyor = false;
                p.hiz = 0;
            }
        }

        private static string AnonimlikBelirle(string icerik)
        {
            var kucuk = icerik.ToLowerInvariant();
            if (kucuk.Contains("x-forwarded-for") || kucuk.Contains("x-real-ip") || kucuk.Contains("client-ip"))
                return "Transparent";
            if (kucuk.Contains("via") || kucuk.Contains("proxy-connection") || kucuk.Contains("x-proxy-id"))
                return "Anonymous";
            return "Elite";
        }

        private static async Task UlkeGetirAsync(proxy p, CancellationToken iptal)
        {
            try
            {
                var yanit = await _geoHttp.GetStringAsync($"http://ip-api.com/json/{p.ip}?fields=country", iptal);
                using var doc = System.Text.Json.JsonDocument.Parse(yanit);
                if (doc.RootElement.TryGetProperty("country", out var c))
                {
                    var ulke = c.GetString();
                    if (!string.IsNullOrWhiteSpace(ulke)) p.ulke = ulke!;
                }
            }
            catch { }
        }
    }
}