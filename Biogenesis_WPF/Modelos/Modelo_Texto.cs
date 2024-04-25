using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Texto : INotifyPropertyChanged
    {
        private static Modelo_Texto? instanciaTexto;

        private string? _texto;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string texto
        {
            get { return _texto; }
            set
            {
                if (_texto != null || _texto != "")
                {
                    _texto = value;
                    OnPropertyChanged(nameof(texto));
                }
            }
        }

        public static Modelo_Texto Texto
        {
            get
            {
                if (instanciaTexto == null)
                {
                    instanciaTexto = new Modelo_Texto();
                }
                return instanciaTexto;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
