using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biogenesis_WPF.VistaModelo
{
    public class ViewModel_Estados
    {
        private static ViewModel_Estados instancia1;

        public ViewModel_Estados()
        {
            //Modelo_Texto ControlDispositosEstado = new Modelo_Texto();
            //Modelo_Texto ControlDispositosTiempo = new Modelo_Texto();
            //Modelo_Textbox ControlEventosEstado = new Modelo_Texto();
            //Modelo_Textbox ControlEventosTiempo = new Modelo_Texto();
            //Modelo_Textbox GatewayEstado = new Modelo_Texto();
       
        }

        public static ViewModel_Estados EstatusView
        {
            get
            {
                if (instancia1 == null)
                {
                    instancia1 = new ViewModel_Estados();
                }
                return instancia1;
            }
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs _e)
        {
            CollectionChanged?.Invoke(this, _e);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string _propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
