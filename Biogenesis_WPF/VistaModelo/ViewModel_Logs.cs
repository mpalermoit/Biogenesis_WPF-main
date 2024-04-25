using Biogenesis_WPF.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Biogenesis_WPF.VistaModelo
{
    public class ViewModel_Logs : INotifyPropertyChanged
    {
        private static ViewModel_Logs instanciaLogs;

        // Listas
        public ObservableCollection<Modelo_Log> logs = new();
        public ObservableCollection<Modelo_Log> logsFiltrados = new();

        // Notificaciones de Cambios
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        // Gestion de Hilos
        Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion entre Hilos

        // Variables
        int maximo = 1000;                                           //TODO: Config
        public string filtro = string.Empty;
        private DispatcherTimer tiempoDeFiltro;

        public ViewModel_Logs()
        {
            logs.CollectionChanged += logs_CollectionChanged;
            tiempoDeFiltro = new DispatcherTimer();
            tiempoDeFiltro.Interval = TimeSpan.FromMilliseconds(300);
            tiempoDeFiltro.Tick += (sender, e) =>
            {
                FiltrarLogs(Filtro);
                tiempoDeFiltro.Stop();
            };
        }

        public static ViewModel_Logs viewModelLogs
        {
            get
            {
                if (instanciaLogs == null)
                {
                    instanciaLogs = new ViewModel_Logs();
                }
                return instanciaLogs;
            }
        }

        public void AgregarLog(uint _serial, string _detalle)
        {
            Modelo_Log nuevoLog = new Modelo_Log
            {
                Serial = _serial,
                TiempoLog = DateTime.Now,
                Detalle = _detalle
            };
            if (logs.Count >= maximo)
            {
                logs.RemoveAt(0);
            } 
            logs.Add(nuevoLog);
            //FiltrarLogs(Filtro);
        }

        public string Filtro
        {
            get { return filtro; }
            set
            {
                filtro = value;

                // Llama al método de filtrado cuando cambia el valor del filtro
                FiltrarLogs(filtro);
                OnPropertyChanged(nameof(Filtro));

                tiempoDeFiltro.Stop();
                tiempoDeFiltro.Start();
            }
        }

        private void logs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FiltrarLogs(Filtro);
        }

        public void FiltrarLogs(string _filtro)
        {
            uiDispatcher.Invoke(() =>
            {
                logsFiltrados.Clear();
            });

            if (_filtro != "" && _filtro != null)
            {
                foreach (Modelo_Log _log in logs)
                {
                    if (
                        _log.Serial.ToString().ToLower().Contains(_filtro.ToLower()) ||
                        _log.TiempoLog.ToString("dd/MM HH:mm:ss").ToLower().Contains(_filtro.ToLower()) ||
                        _log.Detalle.ToString().ToLower().Contains(_filtro.ToLower()))
                    {
                        uiDispatcher.Invoke(() =>
                        {
                            logsFiltrados.Add(_log);
                        });
                    }
                }
            }
            else
            {
                foreach (Modelo_Log _log in logs)
                {
                    uiDispatcher.Invoke(() =>
                    {
                        logsFiltrados.Add(_log);
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
