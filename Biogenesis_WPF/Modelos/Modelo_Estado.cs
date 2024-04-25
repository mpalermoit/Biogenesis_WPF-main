using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Estado : INotifyPropertyChanged
    {
        private static Modelo_Estado? instanciaEstado;

        private string? _estado;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string estado
        {
            get { return _estado; }
            set
            {
                if (_estado != null || _estado != "")
                {
                    _estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Modelo_Estado Estado
        {
            get
            {
                if (instanciaEstado == null)
                {
                    instanciaEstado = new Modelo_Estado();
                }
                return instanciaEstado;
            }
        }

    }
}
