using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace k7proxyyoneticisi.modeller
{
    public class kaynak : INotifyPropertyChanged
    {
        private int _id;
        private string _ad = string.Empty;
        private string _url = string.Empty;
        private string _tur = "HTTP";
        private bool _aktif = true;
        private int _cekilensayi;

        public int id { get => _id; set { _id = value; bildir(); } }
        public string ad { get => _ad; set { _ad = value; bildir(); } }
        public string url { get => _url; set { _url = value; bildir(); } }
        public string tur { get => _tur; set { _tur = value; bildir(); } }
        public bool aktif { get => _aktif; set { _aktif = value; bildir(); } }
        public int cekilensayi { get => _cekilensayi; set { _cekilensayi = value; bildir(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void bildir([CallerMemberName] string? ad = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ad));
    }
}