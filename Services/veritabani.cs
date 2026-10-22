using Microsoft.Data.Sqlite;
using k7proxyyoneticisi.modeller;

namespace k7proxyyoneticisi.servisler
{
    public class veritabani
    {
        private readonly string _yol;

        public veritabani(string yol)
        {
            _yol = yol;
            baslat();
        }

        private void baslat()
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = @"
                CREATE TABLE IF NOT EXISTS proxyler (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ip TEXT NOT NULL,
                    port INTEGER NOT NULL,
                    tur TEXT,
                    ulke TEXT,
                    anonimlik TEXT,
                    hiz INTEGER,
                    calisiyor INTEGER,
                    kaynak TEXT,
                    tarih TEXT
                );
                CREATE TABLE IF NOT EXISTS kaynaklar (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ad TEXT,
                    url TEXT,
                    tur TEXT,
                    aktif INTEGER
                );";
            komut.ExecuteNonQuery();
        }

        public List<proxy> proxyleriGetir()
        {
            var liste = new List<proxy>();
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "SELECT id,ip,port,tur,ulke,anonimlik,hiz,calisiyor,kaynak,tarih FROM proxyler ORDER BY id DESC";
            using var okuyucu = komut.ExecuteReader();
            while (okuyucu.Read())
            {
                liste.Add(new proxy
                {
                    id = okuyucu.GetInt32(0),
                    ip = okuyucu.GetString(1),
                    port = okuyucu.GetInt32(2),
                    tur = okuyucu.IsDBNull(3) ? "HTTP" : okuyucu.GetString(3),
                    ulke = okuyucu.IsDBNull(4) ? "Bilinmiyor" : okuyucu.GetString(4),
                    anonimlik = okuyucu.IsDBNull(5) ? "Bilinmiyor" : okuyucu.GetString(5),
                    hiz = okuyucu.IsDBNull(6) ? 0 : okuyucu.GetInt32(6),
                    calisiyor = !okuyucu.IsDBNull(7) && okuyucu.GetInt32(7) == 1,
                    kaynak = okuyucu.IsDBNull(8) ? "" : okuyucu.GetString(8),
                    tarih = okuyucu.IsDBNull(9) ? DateTime.Now : DateTime.Parse(okuyucu.GetString(9))
                });
            }
            return liste;
        }

        public void proxyEkle(proxy p)
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = @"INSERT INTO proxyler (ip,port,tur,ulke,anonimlik,hiz,calisiyor,kaynak,tarih)
                                  VALUES ($ip,$port,$tur,$ulke,$anonimlik,$hiz,$calisiyor,$kaynak,$tarih)";
            komut.Parameters.AddWithValue("$ip", p.ip);
            komut.Parameters.AddWithValue("$port", p.port);
            komut.Parameters.AddWithValue("$tur", p.tur);
            komut.Parameters.AddWithValue("$ulke", p.ulke);
            komut.Parameters.AddWithValue("$anonimlik", p.anonimlik);
            komut.Parameters.AddWithValue("$hiz", p.hiz);
            komut.Parameters.AddWithValue("$calisiyor", p.calisiyor ? 1 : 0);
            komut.Parameters.AddWithValue("$kaynak", p.kaynak);
            komut.Parameters.AddWithValue("$tarih", p.tarih.ToString("o"));
            komut.ExecuteNonQuery();
        }

        public void proxyleriTemizle()
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "DELETE FROM proxyler";
            komut.ExecuteNonQuery();
        }

        public void proxySil(int id)
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "DELETE FROM proxyler WHERE id=$id";
            komut.Parameters.AddWithValue("$id", id);
            komut.ExecuteNonQuery();
        }

        public List<kaynak> kaynaklariGetir()
        {
            var liste = new List<kaynak>();
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "SELECT id,ad,url,tur,aktif FROM kaynaklar";
            using var okuyucu = komut.ExecuteReader();
            while (okuyucu.Read())
            {
                liste.Add(new kaynak
                {
                    id = okuyucu.GetInt32(0),
                    ad = okuyucu.IsDBNull(1) ? "" : okuyucu.GetString(1),
                    url = okuyucu.IsDBNull(2) ? "" : okuyucu.GetString(2),
                    tur = okuyucu.IsDBNull(3) ? "HTTP" : okuyucu.GetString(3),
                    aktif = !okuyucu.IsDBNull(4) && okuyucu.GetInt32(4) == 1
                });
            }
            return liste;
        }

        public void kaynakEkle(kaynak k)
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "INSERT INTO kaynaklar (ad,url,tur,aktif) VALUES ($ad,$url,$tur,$aktif)";
            komut.Parameters.AddWithValue("$ad", k.ad);
            komut.Parameters.AddWithValue("$url", k.url);
            komut.Parameters.AddWithValue("$tur", k.tur);
            komut.Parameters.AddWithValue("$aktif", k.aktif ? 1 : 0);
            komut.ExecuteNonQuery();
        }

        public void kaynakSil(int id)
        {
            using var baglanti = new SqliteConnection($"Data Source={_yol}");
            baglanti.Open();
            using var komut = baglanti.CreateCommand();
            komut.CommandText = "DELETE FROM kaynaklar WHERE id=$id";
            komut.Parameters.AddWithValue("$id", id);
            komut.ExecuteNonQuery();
        }
    }
}