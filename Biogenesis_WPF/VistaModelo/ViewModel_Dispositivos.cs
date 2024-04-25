using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Biogenesis_WPF.Servicios;
using Biogenesis_WPF.Controladores;
using Biogenesis_WPF.Modelos;
using System.Windows;

namespace Biogenesis_WPF.VistaModelo
{
    public class ViewModel_Dispositivos : INotifyPropertyChanged
    {
        private static ViewModel_Dispositivos? instanciaViewModelDispositivos;

        // Modelos
        public Modelo_Barra barraRutina = new Modelo_Barra();
        public Modelo_Texto textoEstadoProceso = new Modelo_Texto();
        public Modelo_Texto textoTiempoProceso = new Modelo_Texto();
        public Modelo_Texto textoEstadoGateway = new Modelo_Texto();

        // Listas
        public ObservableCollection<Modelo_Dispositivo> dispositivos = new();
        public ObservableCollection<Modelo_Dispositivo> dispositivosFiltrados = new();

        // Notificaciones de Cambios
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        // Acceso a Hilo principal
        Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion entre los Hilos y la interfaz de Sub-Proceso

        // Variables
        public string filtro = string.Empty;
        private DispatcherTimer tiempoDeFiltro;

        public ViewModel_Dispositivos()
        {
            dispositivos.CollectionChanged += equipos_CollectionChanged;
            barraRutina.valor = 0;
            textoEstadoProceso.texto = "Listo para iniciar";
            textoTiempoProceso.texto = "0";
            textoEstadoGateway.texto = "Apagado";

            tiempoDeFiltro = new DispatcherTimer();
            tiempoDeFiltro.Interval = TimeSpan.FromMilliseconds(300);
            tiempoDeFiltro.Tick += (sender, e) =>
            {
                // Cuando se complete el temporizador, aplica el filtro
                FiltrarEquipos(Filtro);
                tiempoDeFiltro.Stop();
            };
        }

        public static ViewModel_Dispositivos viewModelDispositivos
        {
            get
            {
                if (instanciaViewModelDispositivos == null)
                {
                    instanciaViewModelDispositivos = new ViewModel_Dispositivos();
                }
                return instanciaViewModelDispositivos;
            }
        }

        private void equipos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FiltrarEquipos(Filtro);
        }

        public string Filtro
        {
            get { return filtro; }
            set
            {
                filtro = value;

                // Llama al método de filtrado cuando cambia el valor del filtro
                //FiltrarEquipos(filtro);
                OnPropertyChanged(nameof(Filtro));

                tiempoDeFiltro.Stop();
                tiempoDeFiltro.Start();
            }
        }

        public void FiltrarEquipos(string _filtro)
        {
            uiDispatcher.Invoke(() =>
            {
                dispositivosFiltrados.Clear();
            });

            if (_filtro != "" && _filtro != null)
            {
                foreach (Modelo_Dispositivo _dispositivo in dispositivos)
                {
                    if (
                        _dispositivo.Serial.ToString().ToLower().Contains(filtro.ToLower()) ||
                        _dispositivo.Ip.ToString().ToLower().Contains(filtro.ToLower()) ||
                        _dispositivo.Estado.ToString().ToLower().Contains(filtro.ToLower())
                        )
                    {
                        uiDispatcher.Invoke(() =>
                        {
                            dispositivosFiltrados.Add(_dispositivo);
                        });
                    }
                }
            }
            else
            {
                foreach (Modelo_Dispositivo _disp in dispositivos)
                {
                    uiDispatcher.Invoke(() =>
                    {
                        dispositivosFiltrados.Add(_disp);
                    });
                }
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
