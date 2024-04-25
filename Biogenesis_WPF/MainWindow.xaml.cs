//Enlace con Otros Scripts
//Enlace con Base de datos
//Protobuf
//G - SDK
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
using Biogenesis_WPF.Controladores;
using Biogenesis_WPF.VistaModelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Gsdk.Gateway;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Biogenesis_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Modelo_Log> logs = new();            //
        public ObservableCollection<Modelo_Log> LogsFiltrados = new();  //


        //Servicios
        Service_Gateway gateway = new();
        //Service_GestorErrores? errores = new();

        //Controladores
        Control_Dispositivos controlDispositivos = new();
        Control_Eventos controlEventos = new();

        //Modelos de Vista
        ViewModel_Dispositivos? viewModelDispositivos = ViewModel_Dispositivos.viewModelDispositivos;
        ViewModel_Logs? viewModelLogs = ViewModel_Logs.viewModelLogs;
        ViewModel_Eventos? viewModelEventos = ViewModel_Eventos.viewModelEventos;
        //ViewModel_ControlesOnOff viewModelControlOnOff = ViewModel_ControlesOnOFf.viewModelControlesOnOff;

        // Creacion de Thread
        private Thread threadRutina;
        private Thread threadLogs;
        private Thread threadbuffer;

        //Variable de confirmacion de Cerrar Threads
        public CancellationTokenSource detenerThreadRutina = new CancellationTokenSource();

        public const int Error = -1;
        public const int Nuevo = 0;
        public const int Desconectado = 1;
        public const int Conectado = 2;

        object coleccion;
        ICollectionView coleccionEquipos;
        string filtro;

        //Variables Barras de Progreso
        private double cantidadEquipos = 0;
        private double porcentajePorEquipo = 0;
        private double porcentajePorLogs = 0;
        private DispatcherTimer? tiempoBarraLogs;
        public bool barrido = false;

        private DispatcherTimer timer;
        private Timer? tiempoDeFiltrado;

        //Variables Nuevas
        private object valorCeldaSeleccionada;
        List<string> elementosSeleccionados = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            
            //
            //IncreaseProgressBarValue();                         



            //BarraProcesoRutina.DataContext = viewModelDispositivos.barraRutina;     //error, la barra esta comentada en MainWindow.xaml
            //BarraProcesoEventos.DataContext = viewModelEventos.barraEventos;        //error, la barra esta comentada en MainWindow.xaml

            //buffer.DataContext = viewModelEventos.buffer;

            Text_EstadoDispositivos.DataContext = viewModelDispositivos.textoEstadoProceso;
            Text_EstadoEventos.DataContext = viewModelEventos.textoEstadoProceso;
            Text_GateWayState.DataContext = gateway.estadoGateway;
            TiempoDispositivos.DataContext = viewModelDispositivos.textoTiempoProceso;
            TiempoEventos.DataContext = viewModelEventos.textoTiempoProceso;

            DataGridDispositivos.ItemsSource = viewModelDispositivos.dispositivosFiltrados;
            DataGridLogs.ItemsSource = viewModelLogs.logsFiltrados;

            filtro = string.Empty;

            Dispatcher.Invoke(() =>
            {
                DataGridDispositivos.ItemsSource = viewModelDispositivos.dispositivosFiltrados;
                DataGridLogs.ItemsSource = viewModelLogs.logsFiltrados;
            });

            threadRutina = new Thread(InicioProcesoDispositivos);
            threadRutina.Start();

            threadLogs = new Thread(InicioProcesoEventos);
            threadLogs.Start();

        }

        public void InicioProcesoEventos()
        {
            controlEventos.ProcesarEventos();
        }

        public void InicioProcesoDispositivos()
        {
            controlDispositivos.ProcesarDispositivos();
        }

        public void DetenerRutina()
        {
            Text_EstadoDispositivos.Text = "Deteniendo Rutina..";
            detenerThreadRutina.Cancel(true);
        }

        //private void BarridoButton_Click(object sender, RoutedEventArgs e)
        //{
        //    BarridoButton.IsEnabled = false;
        //    DetenerBarrido.IsEnabled = true;
        //    barrido = true;

        //    Thread barridoThread = new Thread(() =>
        //    {
        //        while (barrido)
        //        {
        //            this.Dispatcher.Invoke(() => BarraRutina.Value = 0, DispatcherPriority.Background);

        //            foreach (Dispositivo equipo in dispositivos)
        //            {
        //                equipo.Activa = true;
        //                this.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

        //                Thread.Sleep(500);

        //                equipo.Activa = false;
        //                this.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

        //                this.Dispatcher.Invoke(() => BarraRutina.Value += porcentajePorEquipo, DispatcherPriority.Background);

        //                this.Dispatcher.Invoke(() => logs.Add(new Log { Serial = equipo.Serial, TiempoLog = DateTime.Now, Detalle = "Actualizar Hora" }), DispatcherPriority.Background);
        //            }
        //            Thread.Sleep(2000);
        //        }

        //        this.Dispatcher.Invoke(() =>
        //        {
        //            BarraRutina.Value = 0;
        //            BarridoButton.IsEnabled = true;
        //            BarridoButton.Content = "Iniciar Barrido";
        //        }, DispatcherPriority.Background);
        //    });

        //    barridoThread.Start();
        //}

        //private void DetenerBarrido_Click(object sender, RoutedEventArgs e)
        //{
        //    barrido = false;
        //    DetenerBarrido.IsEnabled = false;
        //    BarridoButton.Content = "Terminando barrido...";
        //}

        private void ButtonClearDispositivos_Click(object _sender, RoutedEventArgs _e)
        {
            DataGridDispositivos.SelectedItems.Clear();
            TextBox_FiltroDispositivos.Clear();
            tiempoDeFiltrado?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void ButtonClearLogs_Click(object _sender, RoutedEventArgs _e)
        {
            DataGridLogs.SelectedItems.Clear();
            TextBox_FiltroLogs.Clear();
            tiempoDeFiltrado?.Change(Timeout.Infinite, Timeout.Infinite);
            FiltroLogs(_sender);
        }

        //private void FiltroEquipos_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (tiempo != null)
        //    {
        //        tiempo.Stop();
        //    }

        //    // Crea un nuevo temporizador con un intervalo de 300 ms.
        //    tiempo = new DispatcherTimer();
        //    tiempo.Interval = TimeSpan.FromMilliseconds(300);

        //    // Asocia un controlador de eventos al temporizador.
        //    tiempo.Tick += (s, args) =>
        //    {
        //        string searchText = string.Empty;

        //        Dispatcher.Invoke(() =>
        //        {
        //            searchText = TextBox_FiltroEquipos.Text.ToLower();
        //        });
        //        // Cuando el temporizador expire, ejecuta la función AplicarFiltro.
        //        procesamientoDispositivos.FiltrarEquipos(searchText, EquiposDataGrid);
        //        // Detén el temporizador después de ejecutar la función.
        //        tiempo.Stop();
        //    };

        //    // Inicia el temporizador.
        //    tiempo.Start();
        //}

        //private void FiltroLog_TextChanged(object _sender, TextChangedEventArgs _e)
        //{
        //    if (timer != null)
        //    {
        //        timer.Stop();
        //    }

        //    // Crea un nuevo temporizador con un intervalo de 300 ms.
        //    timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMilliseconds(300);

        //    // Asocia un controlador de eventos al temporizador.
        //    timer.Tick += (_s, _args) =>
        //    {
        //        // Cuando el temporizador expire, ejecuta la función AplicarFiltro.
        //        //ProcesamientoDispositivos.FiltrarEquipos();
        //        // Detén el temporizador después de ejecutar la función.
        //        timer.Stop();
        //    };

        //    // Inicia el temporizador.
        //    timer.Start();
        //}

        //private void FiltrarLog(object _sender)
        //{
        //    //Filtro_logs();

        //    //string searchText = string.Empty;

        //    //Dispatcher.Invoke(() =>
        //    //{
        //    //    searchText = FiltroLog.Text.ToLower();
        //    //});

        //    //LogsFiltrados = logs.Where(Log =>
        //    //    Log.Detalle.ToLower().Contains(searchText) ||
        //    //    Log.Serial.ToString().ToLower().Contains(searchText) ||
        //    //    Log.TiempoLog.ToString("dd/MM HH:mm:ss").Contains(searchText)
        //    //).ToList();

        //    //Dispatcher.Invoke(() =>
        //    //{
        //    //    LogsDatagrid.ItemsSource = LogsFiltrados;
        //    //});
        //} 

        private void EquiposDataGrid_SelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            //var serialNumbers = new[] { "1", "7" };
            uint serialNumbers = (uint)_sender;

            // Verifica que al menos haya una fila seleccionada
            if (DataGridDispositivos.SelectedItem != null)
            {
                foreach (Modelo_Dispositivo _item in DataGridDispositivos.SelectedItems)
                {
                    elementosSeleccionados.Add($"{_item}");
                }
            }
            //Filtro_logs();
        }

        private void FiltroDispositivos(object _sender)
        {
            filtro = string.Empty;

            Dispatcher.Invoke(() =>
            {
                filtro = TextBox_FiltroDispositivos.Text.ToLower();
            });
            viewModelDispositivos.Filtro = filtro;

        }

        private void FiltroLogs(object _sender)
        {
            filtro = string.Empty;

            Dispatcher.Invoke(() =>
            {
                filtro = TextBox_FiltroLogs.Text.ToLower();
            });
            viewModelLogs.Filtro = filtro;
        }

        private void TiempoFiltradoDispositivos(object sender, TextChangedEventArgs e)
        {
            tiempoDeFiltrado?.Change(Timeout.Infinite, Timeout.Infinite);
            tiempoDeFiltrado = new Timer(FiltroDispositivos, null, 300, Timeout.Infinite);
        }

        private void TiempoFiltradoLogs(object sender, TextChangedEventArgs e)
        {
            tiempoDeFiltrado?.Change(Timeout.Infinite, Timeout.Infinite);
            tiempoDeFiltrado = new Timer(FiltroLogs, null, 300, Timeout.Infinite);
        }










        //// cambio de color 

        //private int j = 0;
        //private SolidColorBrush currentColor;

        ////aumenta variable y llama cambio color 
        //private async void IncreaseProgressBarValue()
        //{
        //    for (; j <= 10; j++)  //numero max barra buffer/ 10 = Num max segunda barra
        //    {
        //        // Incrementar la variable j
                
        //        // Actualizar la barra de progreso

        //        //buffer.Value = j;


        //        SolidColorBrush newColor = GetColorForValue(j);
        //        if (newColor != currentColor)
        //        {
        //            currentColor = newColor;
        //            buffer.Foreground = currentColor;

        //        }
        //        //UpdateProgressBarStyle();

        //        await Task.Delay(4552); // 

        //    }
        //}
        //// cambio de color 
        //public SolidColorBrush GetColorForValue(int value)
        //{
        //    if (value <= 7)  //numero cambio de color   / 7 = donde se quiere cambiar       
        //        return Brushes.Orange;   
        //    else
        //        return Brushes.Red;
        //}

        //// detele logs grid
        //private void ButtonDeleteLogs_Click(object sender, RoutedEventArgs e)
        //{
        //    //viewModelLogs.logsFiltrados.Clear();
        //    viewModelLogs.logs.Clear();
        //}

    }
}
