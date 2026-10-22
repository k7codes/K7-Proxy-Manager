using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace k7proxyyoneticisi.modeller
{
    public class proxy : INotifyPropertyChanged
    {
        private int _id;
        private string _ip = string.Empty;
        private int _port;
        private string _tur = "HTTP";
        private string _ulke = "Bilinmiyor";
        private string _anonimlik = "Bilinmiyor";
        private int _hiz;
        private bool _calisiyor;
        private string _kaynak = string.Empty;
        private DateTime _tarih = DateTime.Now;
        private bool _secili;
        private string _durummetni = "Bekliyor";

        public int id { get => _id; set { _id = value; bildir(); } }
        public string ip { get => _ip; set { _ip = value; bildir(); } }
        public int port { get => _port; set { _port = value; bildir(); } }
        public string tur { get => _tur; set { _tur = value; bildir(); } }
        public string ulke { get => _ulke; set { _ulke = value; bildir(); } }
        public string anonimlik { get => _anonimlik; set { _anonimlik = value; bildir(); } }
        public int hiz { get => _hiz; set { _hiz = value; bildir(); } }
        public bool calisiyor { get => _calisiyor; set { _calisiyor = value; bildir(); durummetni = value ? "Çalışıyor" : "Çalışmıyor"; } }
        public string kaynak { get => _kaynak; set { _kaynak = value; bildir(); } }
        public DateTime tarih { get => _tarih; set { _tarih = value; bildir(); } }
        public bool secili { get => _secili; set { _secili = value; bildir(); } }
        public string durummetni { get => _durummetni; set { _durummetni = value; bildir(); } }

        public string adres => $"{ip}:{port}";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void bildir([CallerMemberName] string? ad = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ad));
    }
}