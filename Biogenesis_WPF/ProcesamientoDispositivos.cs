using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Gsdk.Err;
using Gsdk.Gateway;
using System.Windows.Controls;
//using Biogenesis_WPF.Vistas;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
//Clases
using Space_Biometricos;
using System.Collections.ObjectModel;
using Google.Protobuf;
using System.ComponentModel;
using System.Windows.Data;

namespace Biogenesis_WPF
{
    //public class ProcesamientoDispositivos
    //{
    //    Service_ControlDB controlDB;
    //    Biometricos biometricos;
    //    Service_GestorErrores errores = new();
    //    Modelo_Barra barraRutina = Modelo_Barra.BarraRutina;
    //    Modelo_Barra barraLogs = Modelo_Barra.BarraLogs;
    //    Modelo_Estado estadoRutina = Modelo_Estado.EstadoRutina;
    //    Modelo_Estado estadoLogs = Modelo_Estado.EstadoLogs;
        

    //    // Creacion de Thread de Logs
    //    private bool procesoIniciado = false;
    //    private bool procesoDetenido = false;

    //    //Control de Threads
    //    Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion necesaria entre los Hilos y la interfaz de Proceso Rutina
    //    ManualResetEvent pausar = new ManualResetEvent(true); //Thread que controla los tiempos de pausa de hilo en Proceso Rutina

    //    //Variable de confirmacion de Cerrar Threads
    //    public CancellationTokenSource detenerThread = new CancellationTokenSource();

    //    public const int Error = -1;
    //    public const int Nuevo = 0;
    //    public const int Desconectado = 1;
    //    public const int Conectado = 2;

    //    //public ObservableCollection<Dispositivo> dispositivos = new ObservableCollection<Dispositivo>();
    //    //private List<Dispositivo> dispositivosFiltrados = new List<Dispositivo>();
    //    private ObservableCollection<Modelo_Dispositivo> dispositivos = new ObservableCollection<Modelo_Dispositivo>();

    //    public ObservableCollection<Modelo_Log> logs = new ObservableCollection<Modelo_Log>();
    //    private List<Modelo_Log> LogsFiltrados = new List<Modelo_Log>();

    //    //Variables Barras de Progreso
    //    private double cantidadEquipos = 0;
    //    private double porcentajePorEquipo = 0;
    //    private double porcentajePorLogs = 0;
    //    private DispatcherTimer? tiempoBarraLogs;
    //    public bool barrido = false;

    //    private DispatcherTimer tiempo;
     
    //    public void ProcesarDispositivos()
    //    {
    //        controlDB = new();
    //        biometricos = new();

    //        procesoIniciado = true;
    //        errores.Guardar("Inicio de Proceso Automatizado", "---", "---");

    //        pausar.Reset();
    //        pausar.WaitOne(1000);

    //        barraRutina.valor = 5;

    //        estadoRutina.texto1 = "Inicio proceso de Rutina";

    //        //Variables de medicion de tiempo
    //        DateTime inicioTiempoTranscurrido;
    //        int demora = 0;
    //        TimeSpan tiempotranscurrido = TimeSpan.Zero;

    //        //TODO: Generar el inicio del Gateway.exe con un "Shell" o un ".bat"
    //        // Esperar 5/10 segundos a que abra Gateway y se inicie

    //        // 1 - ConectarDB1 GateWay + Suscripcion Estado Gateway
    //        estadoRutina.texto1 = "Conectandose a Gateway";
    //        biometricos.ConectarGateway();
    //        //GatewayInfo gatewayInfo = new GatewayInfo();
    //        //barraRutina.texto3 = gatewayInfo.IsConnected.ToString();
    //        //if (gatewayInfo.IsConnected)
    //        //{

    //        //}
    //        //else
    //        //{
    //        //    gateway.ConectarGateway();
    //        //}

    //        barraRutina.valor = 10;

    //        pausar.Reset();
    //        pausar.WaitOne(1000);

    //        // 1.2 - Reconectar GateWay
    //        // if (statusGateway == shutdown)...
    //        // ReconectarGateway();
    //        barraRutina.valor = 20;
    //        // if (Channel.State == "Ready" || Channel.State == "Idle")
    //        //{ Armar toda la rutina aca }
    //        estadoRutina.texto1 = "Suscribiendose a Estado de Dispositivos";
    //        biometricos.SuscripcionEstadoDispositivos();

    //        pausar.Reset();
    //        pausar.WaitOne(1000);
    //        //// Apertura de conexion con DB
    //        //barraRutina.texto1 = "Conectando la Base de Datos";
    //        //controlDB.Conectar();
    //        //IF true => ....
    //        barraRutina.valor = 30;
    //        pausar.Reset();
    //        pausar.WaitOne(1000);

    //        barraRutina.valor = 40;
    //        // 2 - Suscripcion a Estados de Dispositivos
    //        //alreadySuscribed = true;                        //Seteamos el estado de suscripcion en True para poder suscribirnos.


    //        // 3 - Marcar (DB) los dispositivos como "Desconectados/Nuevos"
    //        estadoRutina.texto1 = "Seteando Dispositivos como nuevos";
    //        controlDB.EjecutarNonQuery("UPDATE dispositivos SET estado = 'nuevo', seteo_hora = 0");

    //        pausar.Reset();
    //        pausar.WaitOne(1000);

    //        estadoRutina.texto1 = "Cargando listado de Equipos";
    //        barraRutina.valor = 50;

    //        pausar.Reset();
    //        pausar.WaitOne(3000);


    //        //dispositivosFiltrados = dispositivos;

    //        while (!procesoDetenido && !detenerThread.Token.IsCancellationRequested)
    //        {
    //            dispositivos.Clear();
    //            foreach (var item in controlDB.ListaDispositivos())
    //            {
    //                dispositivos.Add(item);
    //            }
    //            barraRutina.valor = 55;
    //            uiDispatcher.Invoke(() =>
    //            {
    //                //filtro = FiltrarEquipos(filtro);
    //            });

    //            //if (filtro == "" || filtro == null)
    //            //{
    //            //    FiltrarEquipos(filtro, equipoDataGrid)
    //            //    dispositivosFiltrados = dispositivos.ToList();
    //            //}
    //            //else
    //            //{
    //            //    //FiltrarEquipos(filtro,equipoDataGrid);
    //            //}
    //            barraRutina.valor = 60;


    //            inicioTiempoTranscurrido = DateTime.Now;

    //            // 5 - ConectarDB1 Sincronicamente Todos los Dispositivos con estado = desconectado/nuevo
    //            estadoRutina.texto1 = "Conectando Dispositivos";
    //            biometricos.ConectarDispositivo(dispositivos);
    //            barraRutina.valor = 70;
    //            // 6.5 - Actualizacion de Fecha y Hora en Dispositivos
    //            estadoRutina.texto1 = "Seteando Fecha y Hora en Dispositivos Conectados";
    //            biometricos.SetearFechaYHora();

    //            pausar.Reset();
    //            pausar.WaitOne(1000);
    //            barraRutina.valor = 80;
    //            // 7 - Monitorear Dispositivos Conectados Sincronicamente
    //            estadoRutina.texto1 = "Iniciando Monitoreo de Dispositivos Conectados";
    //            biometricos.MonitoreoDispositivos();

    //            pausar.Reset();
    //            pausar.WaitOne(1000);

    //            barraRutina.valor = 100;

    //            // 8 - Tomar 1 dispositivo de la lista (Foreach)
    //            int i = 0;
    //            barraRutina.maximo = dispositivos.Count();
    //            foreach (Modelo_Dispositivo item in dispositivos)
    //            {
    //                barraRutina.valor = i;
    //                if (item.Estado == "conectado")
    //                {
    //                    uiDispatcher.Invoke(() => logs.Add(new Modelo_Log { TiempoLog = DateTime.Now, Serial = item.Serial, Detalle = "Acceso al equipo" }));

    //                    item.Estado = "Accediendo..";
    //                    item.Activa = true;

    //                    pausar.Reset();
    //                    pausar.WaitOne(3000);

    //                    // 9 - Consultar (el Flag) si el dispositivo tiene nuevos logs
    //                    if (item.HayEventos == true)
    //                    {
    //                        estadoRutina.texto1 = "Descargando Logs...";
    //                        biometricos.LogsPorFecha(item.Serial, item.UltimoLogFecha);
    //                        uiDispatcher.Invoke(() => logs.Add(new Modelo_Log { TiempoLog = DateTime.Now, Serial = item.Serial, Detalle = "Descargando Logs" }));
    //                        // TraerLogsDispositivo();...
    //                        // string QueryGuardarLog...
    //                        // log user enroll
    //                        // log user delete
    //                    }

    //                    // 11 - Consultar Usuarios modificados (DB) para ese dispositivo + TimeStamp servidor
    //                    // string QueryUsuariosActualizados
    //                    // if (usuariosActualizados)
    //                    // EnrollUserList();
    //                    // En la DB (usuario_x_dispositivo) se guardara el TimeStamp del serividor

    //                    // 12 - Consultar Usuarios Dados de baja (DB) + TimeStamp servidor
    //                    // string QueryCambioEstadoActivo
    //                    // if (cambioEstado)
    //                    // DeleteUserList();
    //                    // En la DB (usuario_x_dispositivo) se guardara el TimeStamp del serividor

    //                    // 13 - Tomar Nuevo dispositivo
    //                    // Nueva vuelta del foreach


    //                    item.Estado = "conectado";
    //                    item.Activa = false;

    //                    pausar.Reset();
    //                    pausar.WaitOne(1000);
    //                    i++;

    //                    uiDispatcher.Invoke(() => logs.Add(new Modelo_Log { TiempoLog = DateTime.Now, Serial = item.Serial, Detalle = "Liberando el equipo" }));
    //                }
    //            }
    //            //barraRutina.valor = 100;

    //            // 14 - Tiempo restante de vuelta
    //            tiempotranscurrido = DateTime.Now - inicioTiempoTranscurrido;
    //            estadoRutina.texto2 = ((int)tiempotranscurrido.TotalSeconds).ToString();
    //            estadoRutina.texto1 = "En Espera...";

    //            demora = (int)(30000 - tiempotranscurrido.TotalMilliseconds);
    //            //barraRutina.texto2 = demora.ToString();
    //            if (demora > 0)
    //            {
    //                pausar.Reset();
    //                pausar.WaitOne(demora);
    //            }
    //            barraRutina.valor = 50;
    //        }
    //        //Desconecta Todos los Dispositivos
    //        biometricos.DesconectarTodosDispositivos();

    //        //Desconexion a Gateway
    //        biometricos.DesconectarGateway();

    //        // Cierra la conexion con la DB
    //        controlDB.Desconectar();

    //        // si errores en DB timmer=> on
    //        // alarma por falla DB

    //        estadoRutina.texto1 = "Rutina Detenida.";

    //    }

    //    public void DetenerRutina()
    //    {
    //        estadoRutina.texto1 = "Deteniendo Rutina..";
    //        detenerThread.Cancel(true);
    //    }

    //    public void FiltrarEquipos(string filtro, DataGrid data)
    //    {
    //        ICollectionView col = CollectionViewSource.GetDefaultView(data.ItemsSource);

    //        col.Filter = (obj) =>
    //        {
    //            Modelo_Dispositivo? dispositivo = obj as Modelo_Dispositivo;
    //            if (dispositivo == null) 
    //            { 
    //                return false; 
    //            }

    //            // Se muestra solo los dispositivos cuyos valores coincidan con texto del TextBox.
    //            return dispositivo.Serial.ToString().Contains(filtro) || 
    //                   dispositivo.Estado.Contains(filtro) || 
    //                   dispositivo.Ip.ToString().Contains(filtro);
    //            //};

    //            //dispositivosFiltrados = dispositivos.Where(equipo =>
    //            //    equipo.Serial.ToString().ToLower().Contains(filtro)
    //            //).ToList();


    //        };
    //        //CollectionViewSource.GetDefaultView(data.ItemsSource).Refresh();
    //    }
    //}
}
