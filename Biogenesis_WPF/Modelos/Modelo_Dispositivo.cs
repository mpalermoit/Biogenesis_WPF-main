using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Dispositivo : INotifyPropertyChanged
    {
        public Modelo_Dispositivo()
        {
            Serial = 0;
            Ip = string.Empty;
            HayEventos = 0;
            Estado = string.Empty;
            UltimoLogFecha = DateTime.UnixEpoch;
            UltimoLogId = 0;
            SeteoHora = 0;
            Activa = false;
        }

        public uint Serial { get; set; }

        public string? Ip { get; set; }

        public int HayEventos { get; set; }

        private string? _estado;
        public string? Estado
        {
            get { return _estado; }
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }

        public DateTime UltimoLogFecha { get; set; }

        public uint UltimoLogId { get; set; }

        public int SeteoHora { get; set; }

        private bool _activa;

        public bool Activa
        {
            get { return _activa; }
            set
            {
                if (_activa != value)
                {
                    _activa = value;
                    OnPropertyChanged(nameof(Activa));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
