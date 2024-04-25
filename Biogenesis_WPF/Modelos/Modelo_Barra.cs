using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Barra : INotifyPropertyChanged
    {
        private static Modelo_Barra? instanciaBarra;

        private int _valor;
        public int maximo = 100;
        internal int Value;

        public int valor
        {
            get { return _valor; }
            set
            {
                if (_valor != (value * maximo) / maximo)
                {
                    _valor = (value * maximo) / maximo;
                    OnPropertyChanged(nameof(valor));
                }
            }
        }
       
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string _propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        public static Modelo_Barra Barra
        {
            get
            {
                if (instanciaBarra == null)
                {
                    instanciaBarra = new Modelo_Barra();
                }
                return instanciaBarra;
            }
        }
    }
}
