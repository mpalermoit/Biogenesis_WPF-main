using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Tracing;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
//Seteo de Tareas Asyncronicas
using System.Windows.Threading;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Timers;
//Enlace con Otros Scripts
using GestionDB;
using Biogenesis_WPF;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
//Enlace con Base de datos
using System.Data.SqlClient;
//Protobuf
using Google.Protobuf;
using Google.Protobuf.Collections;
//G-SDK
using Biogenesis_WPF.biostar.services;
using Grpc.Core.Utils;
using Grpc;
using Gsdk;
using Gsdk.Connect;
using Gsdk.Device;
using Gsdk.Event;
using Gsdk.Time;
using Gsdk.Finger;
using Gsdk.User;
using Gsdk.Auth;
using Channel = Grpc.Core.Channel;
using EventLog = Gsdk.Event.EventLog;
using Grpc.Core;
using Gsdk.Gateway;
using System.Collections.ObjectModel;

namespace Space_Biometricos
{

    public class Biometricos
    {
        //Service_ControlDB controlDB;
        //Service_GestorErrores error;

        //ModeloBarra barraGateway = ModeloBarra.BarraGateway;

        //Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion necesaria entre los Hilos y la interfaz
        ////ManualResetEvent pausar = new ManualResetEvent(true);   //Thread que controla los tiempos de pausa o detencion de hilos 


        ////public Biometricos(ControlDB cntrlDB)
        ////{
        ////    controlDB = cntrlDB;
        ////}

        ////Canales de Conexion para accesos a funcionalidades de Suprema G-SDK.
        //public Channel? channel;
        //public ConnectSvc? connectSvc;
        //public EventSvc? eventSvc;

        ////Variable de Suscripcion a eventos de Dispositivos
        //private bool alreadySuscribed = false;

        ////Variable de confirmacion de Cerrar Thread
        ////public CancellationTokenSource detenerThread = new CancellationTokenSource();

        ////Lista de dispositivos para acceso desde las distintas funcionalidades
        //private RepeatedField<SearchDeviceInfo> listaDispositivos;

        ////Lista de Huellas y Listado que contiene las huellas disponibles para solicitar
        //Modelo_DedoData?[] listaHuellas = new Modelo_DedoData[4];

        public Biometricos()
        {
            //controlDB = new();
            //error = new();
        }

        //public void ConectarGateway()
        //{
        //    //Ruta de busqueda LOCAL de los Certificados para la conexion del Gateway
        //    const string GATEWAY_CA_FILE = "./cert/ca.crt"; //TODO crear una carpeta dentro del proyecto (en el cliente) que contenga los certificados. //TODO: Config

        //    //IP Adress de Conexion con Gateway
        //    string gatewayAddr = "localhost"; //Debe ir el puerto IP al cual se conectara el Gateway //TODO: Config
        //    int gatewayPort = 4000; //Debe llevar el Puerto al cual se conectara el Gateway (400 por default) //TODO: Config
        //    var channelCredentials = new SslCredentials(File.ReadAllText(GATEWAY_CA_FILE)); //Certificado //TODO: Config

        //    try
        //    {
        //        channel = new Channel(gatewayAddr, gatewayPort, channelCredentials);
        //        connectSvc = new ConnectSvc(channel);
        //        eventSvc = new EventSvc(channel);
        //        eventSvc.InitCodeMap("./utils/event_code.json");

        //        //TODO: Suscripcion al estado del GateWay (https://supremainc.github.io/g-sdk/api/gateway/#subscribestatus)
        //        SuscripcionEstadoGateway(channel.State);

        //        if (channel.State == ChannelState.Shutdown || channel.State == ChannelState.TransientFailure || channel.State == ChannelState.Ready)
        //        {
        //            ConectarGateway();
        //        }
        //        barraGateway.texto1 = channel.State.ToString();
        //        //main.MostrarProcesoActual("Gateway Conectado.");
        //    }
        //    catch (Exception excGatewayConexion)
        //    {
        //        string? metodo = excGatewayConexion.TargetSite?.Name ?? "Desconocido";
        //        string? linea = excGatewayConexion.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, excGatewayConexion.Message);
        //    }
        //    connectSvc.DisconnectAll();
        //}

        //public void DesconectarGateway()
        //{
        //    try
        //    {
        //        if (channel != null)
        //        {
        //            connectSvc.DisconnectAll();
        //            alreadySuscribed = false;
        //            channel.ShutdownAsync().Wait();
        //            //main.MostrarProcesoActual("Desconexión del Gateway exitosa.");
        //        }
        //    }
        //    catch (Exception excDesconexion)
        //    {
        //        string? metodo = excDesconexion.TargetSite?.Name ?? "Desconocido";
        //        string? linea = excDesconexion.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, excDesconexion.Message);
        //        throw;
        //    }
        //}

        //public void DesconectarTodosDispositivos ()
        //{
        //    connectSvc.DisconnectAll();
        //}

        // (1.1)
        //private void SuscripcionEstadoGateway(ChannelState estado)
        //{
        //    string estadoGateway = string.Empty;
        //    switch (estado)
        //    {
        //        case ChannelState.Ready:
        //            uiDispatcher.Invoke(() =>
        //            {
        //                barraGateway.texto1 = "Listo";
        //            });
        //            break;
        //        case ChannelState.Idle:
        //            uiDispatcher.Invoke(() =>
        //            {
        //                barraGateway.texto1 = "Conexion Establecida";
        //            });
        //            break;
        //        case ChannelState.Connecting:
        //            uiDispatcher.Invoke(() =>
        //            {
        //                barraGateway.texto1 = "Conectando";
        //            });
        //            break;
        //        case ChannelState.Shutdown:
        //            uiDispatcher.Invoke(() =>
        //            {
        //                barraGateway.texto1 = "Apagado";
        //            });
        //            break;
        //        default:
        //            uiDispatcher.Invoke(() =>
        //            {
        //                barraGateway.texto1 = "Desconectado";
        //            });
        //            break;
        //    }
        //}

        // (2)
        //public void SuscripcionEstadoDispositivos()
        //{
        //    //main.MostrarProcesoActual("Suscribiendose al estado de Dispositivos.");
        //    try
        //    {
        //        //Suscripciones al estado de los dispositivos
        //        var stream = connectSvc.Subscribe(200);  //TODO: Ver la cantidad de suscripciones (min/max) y referencia (equipo/evento)
        //        SuscribirseEstados(stream);
        //        alreadySuscribed = true;
        //    }
        //    catch (Exception excSuscripcion)
        //    {
        //        alreadySuscribed= false;
        //        string? metodo = excSuscripcion.TargetSite?.Name ?? "Desconocido";
        //        string? linea = excSuscripcion.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, excSuscripcion.Message);
        //        throw;
        //    }
        //}

        // (2.1)
        //public async void SuscribirseEstados(IAsyncStreamReader<Gsdk.Connect.StatusChange> stream)
        //{
        //    try
        //    {
        //        if (alreadySuscribed == true)
        //        {
        //            while (await stream.MoveNext())
        //            {
        //                try
        //                {
        //                    uiDispatcher.Invoke(() =>
        //                    {
        //                        var statusLog = stream.Current;
        //                        //textCambiosEstado.Text += statusLog.DeviceID + Environment.NewLine;
        //                        //textCambiosEstado.Text += "\t" + statusLog.Status + Environment.NewLine;
        //                        //textCambiosEstado.Text += "\t" + DateTimeOffset.FromUnixTimeSeconds(statusLog.Timestamp).ToString("yyyy-MM-dd     HH:mm:ss") + Environment.NewLine;
        //                        //main.MostrarProcesoActual("Cambio de Estado de Dispositivo " + statusLog.DeviceID);

        //                        if (statusLog.Status == 0x00)
        //                        {
        //                            controlDB.EjecutarNonQuery("UPDATE dispositivos SET ultima_actualizacion = GETDATA(), estado = 'desconectado' WHERE serial = " + statusLog.DeviceID);
        //                           //foreach(Dispositivo item in equipo)
        //                           // {
        //                           //     if (item.Serial == statusLog.DeviceID)
        //                           //     {
        //                           //         item.Estado = "desconectado";
        //                           //     }
        //                           // }
        //                        }
        //                    });
        //                }
        //                catch (Exception errorEvento)
        //                {
        //                    string? metodo = errorEvento.TargetSite?.Name ?? "Desconocido";
        //                    string? linea = errorEvento.StackTrace ?? "Null";
        //                    error.Guardar(metodo, linea, errorEvento.Message);
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception excStream)
        //    {
        //        string? metodo = excStream.TargetSite?.Name ?? "Desconocido";
        //        string? linea = excStream.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, excStream.Message);
        //        throw;
        //    }
        //}

        // (4)
        //private void SetearDispNuevos()
        //{
        //    try
        //    {
        //        //Desconectar todos
        //        connectSvc.DisconnectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        string? metodo = ex.TargetSite?.Name ?? "Desconocido";
        //        string? linea = ex.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, ex.Message);
        //        throw;
        //    }
        //    //main.MostrarProcesoActual("Cargando Estado 'Nuevo' de Dispositivos en DB."); //Accion en la que se encuentra trabajando el programa

        //    //controlDB.EjecutarQuery("UPDATE dispositivos SET estado = 'nuevo'");
        //}

        // (5)
        //public void ConectarDispositivo(ObservableCollection<Modelo_Dispositivo> Equipos)
        //{
            
        //    //barra1.Value = 75;
        //    DateTime tiempoConexionDispositivos = DateTime.Now;

        //    int fallas = 0;
        //    int i = 0;
        //    //var listaEquipos = controlDB.ListaDispositivos();
        //    //barra1.Maximo = listaEquipos.Count();

        //    foreach (var item in Equipos)  //TODO: Cambiar por EQUIPO 
        //    { 
               

        //        if (item.Estado != "conectado" && item.Estado != "errores")
        //        {
        //            item.Estado="conectando...";
        //            item.Activa = true;
        //            uint newserial = 0;

        //            ConnectInfo connInfo = new ConnectInfo
        //            {
        //                IPAddr = item.Ip,
        //                Port = 51211,
        //                UseSSL = false
        //            };

        //            try
        //            {
        //                newserial = connectSvc.Connect(connInfo);
        //            }
        //            catch (Exception excEquipo)
        //            {
        //                string? metodo = excEquipo.TargetSite?.Name ?? "Desconocido";
        //                string? linea = excEquipo.StackTrace ?? "Null";
        //                error.Guardar(metodo, linea, excEquipo.Message);

        //                if (item.Estado == "nuevo")
        //                {
        //                    //TODO: Registrar alarma
        //                }

        //                controlDB.EjecutarNonQuery("UPDATE dispositivos SET estado = 'desconectado', ultima_conexion = GETDATE() WHERE serial = " + item.Serial);
        //                item.Estado = "desconectado";
        //                Console.WriteLine("Error: " + excEquipo);
        //                fallas++;
        //                newserial = 0;
        //            }

        //            if (newserial > 0)
        //            {
        //                if (item.Serial != newserial)
        //                {
        //                    //Array de dispositivo actual para poder pasarlo por parametro para desconexion
        //                    uint[] deviceID = { item.Serial };

        //                    try
        //                    {
        //                        //Desconectamos el dispositivo por diferencia de datos contra la DB
        //                        connectSvc.Disconnect(deviceID);
        //                    }
        //                    catch (Exception excDesconectar)
        //                    {
        //                        string? metodo = excDesconectar.TargetSite?.Name ?? "Desconocido";
        //                        string? linea = excDesconectar.StackTrace ?? "Null";
        //                        error.Guardar(metodo, linea, excDesconectar.Message);

        //                        return;
        //                    }

        //                    //TODO: Listado de alarmas para envio de mails (una vez que chequeemos el listado)

        //                    //TODO: este Try/Catch estan de mas, pero son para los logs y Alarmas. Quitarlos y agregarlo en "ControlDB" ???
        //                    try
        //                    {
        //                        //Áctualizar DB con el estado del dispositivo que dio errores al desconectar
        //                        controlDB.EjecutarNonQuery("UPDATE dispositivos SET estado = 'errores', ultima_conexion = GETDATE() WHERE serial = " + item.Serial);
        //                        item.Estado = "errores";
        //                        //TODO: generar alarma
        //                    }
        //                    catch (Exception excGuardarDBError)
        //                    {
        //                        string? metodo = excGuardarDBError.TargetSite?.Name ?? "Desconocido";
        //                        string? linea = excGuardarDBError.StackTrace ?? "Null";
        //                        error.Guardar(metodo, linea, excGuardarDBError.Message);
        //                        return;
        //                    }
        //                }

        //                else
        //                {
        //                    //Actualizamos la DB con el estado Conectado y la fecha y hora actual.
        //                    controlDB.EjecutarNonQuery("UPDATE dispositivos SET estado = 'conectado', seteo_hora = 0, ultima_conexion = GETDATE() WHERE serial = " + item.Serial);
        //                    item.Estado = "conectado";
        //                }
        //            }
        //            TimeSpan tiempotranscurrido = DateTime.Now - tiempoConexionDispositivos;
        //            if (tiempotranscurrido.TotalSeconds > 300)  //TODO: Archivo Configuraciones 
        //            {
        //                break;
        //            }
                    
        //            item.Activa = false;
        //        }

        //        //i++;
        //        //barra1.Value = i;
        //    }
          
        //}

        /// <summary>
        /// Seteo de Fecha y Hora en el Dispositivo Actual 
        /// donde el estado del mismo sea "Conectado" 
        /// y el seteo_hora sea 0 (seteo_hora < 0)
        /// </summary>
        //public void SetFechaYHora()
        //{
        //    TimeSvc timeSvc = new TimeSvc(channel);

        //    List<uint> listaDispositivos = new List<uint>();
        //    foreach (var item in controlDB.ListaDispositivos())
        //    {
        //        if (item.Estado == "conectado" && item.SeteoHora <1)
        //        {
        //            string dispositivoID = item.Serial.ToString();
        //            uint deviceID = uint.Parse(dispositivoID);
        //            DateTimeOffset date = DateTimeOffset.Now;
        //            ulong fecha = (ulong)date.ToUnixTimeSeconds();

        //            TimeConfig getHora = timeSvc.GetConfig(deviceID);
        //            try
        //            {
        //                timeSvc.Set(deviceID, fecha);
        //            }
        //            catch (Exception errSetHora)
        //            {
        //                string? metodo = errSetHora.TargetSite?.Name ?? "Desconocido";
        //                string? linea = errSetHora.StackTrace ?? "Null";
        //                error.Guardar(metodo, linea, errSetHora.Message);
        //                throw;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Seteo de Fecha y Hora en todos los Dispositivos 
        ///// donde el estado sea "Conectado" 
        ///// y el seteo_hora sea 0 (seteo_hora < 0)
        ///// </summary>
        //public void SetearFechaYHora()
        //{
        //    TimeSvc timeSvc = new TimeSvc(channel);

        //    List<uint> listDispositivos = new List<uint>();
        //    foreach (var item in controlDB.ListaDispositivos())
        //    {
        //        if (item.Estado == "conectado" && item.SeteoHora <1)
        //        {
        //        listDispositivos.Add(item.Serial);
        //        }
        //    }

        //    if (listDispositivos.Count > 0)
        //    {
        //        //Seteamos la fecha a los dispositivos.
        //        DateTimeOffset date = DateTimeOffset.Now;
        //        ulong fecha = (ulong)date.ToUnixTimeSeconds();
        //        try
        //        {
        //            timeSvc.SetMulti(listDispositivos, fecha);
        //            controlDB.EjecutarNonQuery("UPDATE dispositivos SET seteo_hora = 30 WHERE estado = 'conectado' AND seteo_hora < 1");
        //        }
        //        catch (Exception errorSetHora)
        //        {
        //            string? metodo = errorSetHora.TargetSite?.Name ?? "Desconocido";
        //            string? linea = errorSetHora.StackTrace ?? "Null";
        //            error.Guardar(metodo, linea, errorSetHora.Message);
        //            throw;
        //        }
        //    }
        //    controlDB.EjecutarNonQuery("UPDATE dispositivos SET seteo_hora = seteo_hora-1 WHERE estado = 'conectado'");
        //    //update seteo_hora=seteo_hora-1 where conectado
        //}

        
        //#region Botones => Reiniciar/Detener
        ////
        ////Botones de Reinicio y Detencion de Rutina
        ////

        //// (1.2.0)
        //private void butReiniciarRutina_Click(object sender, EventArgs e)
        //{
        //    //if (detenerRutina && !rutinaIniciada)
        //    //{
        //    //    MostrarProcesoActual("Empezando proceso automatizado");
        //    //    threadRutina = new Thread(ComenzarRutina);
        //    //    threadRutina.Start();
        //    //}
        //}

        //// (1.2.1)
        //private void butDetenerRutina_Click(object sender, EventArgs e)
        //{
        //    //MostrarProcesoActual("Deteniendo el proceso...");
        //    //detenerRutina = true;
        //    //DetenerRutina();
        //}
        //#endregion

        //public void MonitoreoDispositivos()
        //{
        //    //main.MostrarProcesoActual("Iniciando Monitoreo de Eventos");
            
        //    List<uint> idDispositivos = new List<uint>();

        //    foreach (var item in controlDB.ListaDispositivos())
        //    {
        //      if (item.Serial > 0 && item.Estado == "conectado")
        //        {
        //            idDispositivos.Add(item.Serial);
        //       }
        //    }

        //    try
        //    {
        //        if (idDispositivos.Count > 0)
        //        {
        //        eventSvc.SetCallback(RecibirEvento);
        //        eventSvc.StartMonitoringMulti(idDispositivos);
        //        }
        //    }
        //    catch (Exception errorMonitoring)
        //    {
        //        string? metodo = errorMonitoring.TargetSite?.Name ?? "Desconocido";
        //        string? linea = errorMonitoring.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, errorMonitoring.Message);
        //        return;
        //    }
        //}

        //public void RecibirEvento(EventLog logEvent)
        //{
        //    //TODO: filtrar por eventos que nos interesen.
        //    //eventListaAlarmas.Text += " " + DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime + Environment.NewLine;
        //    //eventListaAlarmas.Text += "ID Dispositivo: " + logEvent.DeviceID + Environment.NewLine;
        //    //eventListaAlarmas.Text += "ID Usuario: " + logEvent.UserID + Environment.NewLine;
        //    //eventListaAlarmas.Text += "ID HayEventos: " + logEvent.ID + Environment.NewLine;
        //    //eventListaAlarmas.Text += "HayEventos: " + eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode) + Environment.NewLine;

        //    DateTime tiempo = DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime;

        //    if (logEvent.EventCode == 0x2000)  //user enrollment success
        //    {
        //        controlDB.EjecutarNonQuery("UPDATE dispositivos SET hayeventos = 1 WHERE serial = " + logEvent.ID);
        //    }
        //    if (logEvent.SubCode == 0x02)  //fingerprint + PIN
        //    {
        //        //TODO: Terminar la definicion de Flag
        //    }
        //    //-GuardarLogsDB(logEvent.DeviceID, tiempo, logEvent.EventCode.ToString(), logEvent.UserID);
        //    //eventListaAlarmas.Text += string.Format("{0}: Device - {1}, User - {2}, HayEventos - {3}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, logEvent.UserID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode)) + Environment.NewLine;

        //    //TODO: Luego de reconocer el evento, debe hacer una Flag en la DB para poder traernos todos los logs de ESE dispositivo desde el ultimo evento (fecha) que tengamos guardados en la DB.

        //    // GUARDADO DE LOG PARA "ControlDB"
        //}

        #region Logs
        //public void LogPorFecha(uint dispositivo, DateTime fechaUltimoLog)
        //{
        //    // Convertimos DateTime a tipo uint => Fecha Ultimo Log
        //    uint startTime = ConvertirDateTimeAUint(fechaUltimoLog);
        //    DateTime logActual = fechaUltimoLog;
        //    DateTime tiempoEnd = DateTime.Now;
        //    uint endTime = ConvertirDateTimeAUint(tiempoEnd);

        //    string Query = "";

        //    // Creamos el filtro de Eventos.
        //    EventFilter filtroDeEventos = new EventFilter()
        //    {
        //        EventCode = 0,                  // Codigo de HayEventos a solicitar, 0 Significa todos ??
        //        StartTime = startTime + 1,      // Fecha del primer Log a buscar +1 segundo para no repetir logs
        //        EndTime = endTime,              // Fecha de ultimo Log a buscar o Fecha Actual para traer todos los logs.
        //        UserID = string.Empty           // La identificación del usuario, Empty es todos ??
        //    };
        //    try
        //    {
        //        // Filtramos los logs segun => ID Dispositivo, Desde Primer HayEventos, Cantidad de eventos y los filtros => filtroDeEventos.
        //        var logObtenidos = eventSvc.GetLogWithFilter(dispositivo, 0, 100, filtroDeEventos);

        //        foreach (var log in logObtenidos)
        //        {
        //            // Modificar Variables para asignarlas a la DB
        //            // DateTime para la fecha
        //            DateTime fechaLog = ConvertirUintADateTime(log.Timestamp);
        //            string fechaString1 = fechaLog.ToString("yyyy/MM/dd HH:mm:ss");

        //            // Varchar para Detalle de HayEventos
        //            string detalleEvento = log.SubCode.ToString();

        //            // Guardado de logs en DB
        //            Query = $"INSERT INTO logs (equipo_id, log_numero, fecha, codigo_evento, id_usuario) VALUES ({log.DeviceID},    {log.ID},'{fechaString1}',{log.EventCode}";

        //            if (log.UserID == "" || log.UserID == null)
        //            {
        //                Query = Query + ", Null)";  // {idUsuario} = Null
        //            }
        //            else
        //            {
        //                Query = Query + $"  ,{log.UserID})";  // {idUsuario} != 0
        //            }

        //            controlDB.EjecutarNonQuery(Query);

        //            if (logActual < fechaLog)
        //            {
        //                logActual = fechaLog;
        //            }
        //        }
        //        string fechaString = logActual.ToString("yyyy/MM/dd HH:mm:ss");

        //        if (logActual == fechaUltimoLog)
        //        {
        //            Query = $"UPDATE dispositivos SET ultimo_evento = '{fechaString}', hayeventos=0 WHERE serial = " + dispositivo;
        //        }
        //        else
        //        {
        //            Query = $"UPDATE dispositivos SET ultimo_evento = '{fechaString}' WHERE serial = " + dispositivo;
        //        }
        //        // Actualiza la fecha del ultimo Log consultado
        //        controlDB.EjecutarNonQuery(Query);
        //    }
        //    catch (Exception ex)
        //    {
        //        string? metodo = ex.TargetSite?.Name ?? "Desconocido";
        //        string? linea = ex.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, ex.Message);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Trae los Logs del equipo segun los filtros aplicados internamente. Busca logs y los guarda en la Db.
        ///// </summary>
        ///// <param name="dispositivo"> Uint del dispositivo que enviara los logs</param>
        ///// <param name="fechaUltimoLog">Fecha y hora del ultimo log consultado</param>
        //public void LogsPorFecha(uint dispositivo, DateTime fechaUltimoLog)
        //{
        //    // Convertimos DateTime a tipo uint => Fecha Ultimo Log y Fecha Actual
        //    uint startTime = ConvertirDateTimeAUint(fechaUltimoLog);
        //    DateTime logActual = fechaUltimoLog;
        //    DateTime tiempoEnd = DateTime.Now;
        //    uint endTime = ConvertirDateTimeAUint(tiempoEnd);

        //    //Lista de Event Codes(En total se Usan 8).
        //    uint eventCode0 = 0x1300; // Convert.ToUInt32("1300", 16);  //0x1300 => BS2_EVENT_IDENTIFY_SUCCESS
        //    uint eventCode1 = 0x1300; // Convert.ToUInt32("1400", 16);  //0x1300 => BS2_EVENT_IDENTIFY_SUCCESS
        //    uint eventCode2 = 0x1400; // Convert.ToUInt32("1800", 16);  //0x1400 => BS2_EVENT_IDENTIFY_FAIL
        //    uint eventCode3 = 0x1800; // Convert.ToUInt32("1900", 16);  //0x1800 => BS2_EVENT_AUTH_FAILED
        //    uint eventCode4 = 0x1A00; // Convert.ToUInt32("1A00", 16);  //0x1A00 => BS2_EVENT_FAKE_FINGER_DETECTED
        //    //uint eventCode5 = Convert.ToUInt32("1A00", 16);             //0x1900 => BS2_EVENT_ACCESS_DENIED
        //    //uint eventCode6 = Convert.ToUInt32("3300", 16);             //0x3300 => BS2_EVENT_DEVICE_LINK_CONNECTED
        //    //uint eventCode7 = Convert.ToUInt32("3400", 16);             //0x3400 => BS2_EVENT_DEVICE_LINK_DISCONNECTED

        //    //Creamos el filtro de Eventos y cargamos el primer Codigo del "textBox => eventEventCode.Text".
        //    EventFilter filtroDeEventos = new EventFilter() { EventCode = eventCode0, StartTime = startTime, EndTime = endTime };

        //    //Creamos la Lista de Filtros por la cual se filtrara en la busqueda y los demas codigos de eventos.
        //    List<EventFilter> listaFiltros = new List<EventFilter>();
        //    listaFiltros.Add(filtroDeEventos);
        //    filtroDeEventos = new EventFilter() { EventCode = eventCode1, StartTime = startTime, EndTime = endTime };
        //    listaFiltros.Add(filtroDeEventos);
        //    filtroDeEventos = new EventFilter() { EventCode = eventCode2, StartTime = startTime, EndTime = endTime };
        //    listaFiltros.Add(filtroDeEventos);
        //    filtroDeEventos = new EventFilter() { EventCode = eventCode3, StartTime = startTime, EndTime = endTime };
        //    listaFiltros.Add(filtroDeEventos);
        //    filtroDeEventos = new EventFilter() { EventCode = eventCode4, StartTime = startTime, EndTime = endTime };
        //    listaFiltros.Add(filtroDeEventos);
        //    //filtroDeEventos = new EventFilter() { EventCode = eventCode5, StartTime = startTime, EndTime = endTime };
        //    //listaFiltros.Add(filtroDeEventos);
        //    //filtroDeEventos = new EventFilter() { EventCode = eventCode6, StartTime = startTime, EndTime = endTime };
        //    //listaFiltros.Add(filtroDeEventos);
        //    //filtroDeEventos = new EventFilter() { EventCode = eventCode7, StartTime = startTime, EndTime = endTime };
        //    //listaFiltros.Add(filtroDeEventos);

        //    try
        //    {
        //        // Filtramos los logs segun => ID Dispositivo, Desde Primer HayEventos, Cantidad de eventos y los filtros => filtroDeEventos.
        //        var logsObtenidos = eventSvc.GetLogsWithFilter(dispositivo, 0, 100, listaFiltros.ToList()[0], listaFiltros.ToList()[1], listaFiltros.ToList()[2], listaFiltros.ToList()[3], listaFiltros.ToList()[4]);

        //        string Query = "";

        //        foreach (var log in logsObtenidos)
        //        {
        //            // Modificar Variables para asignarlas a la DB
        //            // DateTime para la fecha
        //            DateTime fechaLog = ConvertirUintADateTime(log.Timestamp);
        //            string fechaString1 = fechaLog.ToString("yyyy/MM/dd HH:mm:ss");

        //            // Varchar para Detalle de HayEventos
        //            string detalleEvento = log.SubCode.ToString();

        //            //GUARDAR LOGS EN DB
        //            Query = $"INSERT INTO logs (equipo_id, log_numero, fecha, codigo_evento, subcodigo_evento, id_usuario) VALUES   ({log.DeviceID},{log.ID},'{fechaString1}',{log.EventCode}, {log.SubCode}";

        //            if (log.UserID == "" || log.UserID == null)
        //            {
        //                Query = Query + ", Null)";  // {idUsuario} = Null
        //            }
        //            else
        //            {
        //                Query = Query + $"  ,{log.UserID})";  // {idUsuario} != 0
        //            }

        //            //controlDB.EjecutarNonQuery(Query);

        //            if (logActual < fechaLog)
        //            {
        //                logActual = fechaLog;
        //            }
        //        }

        //        string fechaString = logActual.ToString("yyyy/MM/dd HH:mm:ss");

        //        //Cargar la fecha del ultimo Log consultado
        //        if (logActual == fechaUltimoLog)
        //        {
        //            Query = $"UPDATE dispositivos SET ultimo_evento = '{fechaString}', hayeventos=0 WHERE serial = " + dispositivo;
        //        }
        //        else
        //        {
        //            Query = $"UPDATE dispositivos SET ultimo_evento = '{fechaString}' WHERE serial = " + dispositivo;
        //        }
        //        controlDB.EjecutarNonQuery(Query);
        //    }
        //    catch (Exception ex)
        //    {
        //        string? metodo = ex.TargetSite?.Name ?? "Desconocido";
        //        string? linea = ex.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, ex.Message);
        //        throw;
        //    }
        //}
        #endregion

        #region Metodos de Conversion
        //
        // Metodos de Conversion
        //
        //public uint ConvertirDateTimeAUint(DateTime fecha)
        //{
        //    // Obtener Fecha de tipo DateTime para convertir a uint
        //    DateTime fechaDateTime = fecha;

        //    // Crear una fecha de referencia (por ejemplo, el Epoch Unix)
        //    DateTime fechaAUint = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //    // Calcular la diferencia en segundos entre la fecha actual y la fecha de referencia
        //    TimeSpan diferencia = fechaDateTime - fechaAUint;

        //    uint fechaUint = 0;
        //    try
        //    {
        //        // Convertir la diferencia total de segundos a un valor uint (puede haber pérdida de precisión)
        //        fechaUint = (uint)diferencia.TotalSeconds;
        //    }
        //    catch (Exception exConvertToUint)
        //    {
        //        string? metodo = exConvertToUint.TargetSite?.Name ?? "Desconocido";
        //        string? linea = exConvertToUint.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, exConvertToUint.Message);
        //        throw;
        //    }
        //    return fechaUint;
        //}

        //public DateTime ConvertirUintADateTime(uint tiempoSpan)
        //{
        //    // Crear un objeto DateTime usando la fecha de referencia del Epoch Unix
        //    DateTime fechaUint = new DateTime(1970, 1, 1, 0, 0, (int)DateTimeKind.Utc);

        //    // Agregamos los segundos de la variable uint a la fecha de referencia Epoch Unix y lo guardamos en DateTime
        //    DateTime fechaConvertida;

        //    try
        //    {
        //        fechaConvertida = fechaUint.AddSeconds(tiempoSpan);
        //    }
        //    catch (Exception excConvertToDateTime)
        //    {
        //        string? metodo = excConvertToDateTime.TargetSite?.Name ?? "Desconocido";
        //        string? linea = excConvertToDateTime.StackTrace ?? "Null";
        //        error.Guardar(metodo, linea, excConvertToDateTime.Message);
        //        throw;
        //    }
        //    return fechaConvertida;
        //}

        #endregion

    }

    //public class ListaDataSql
    //{
    //    public uint Serial { get; set; }
    //    public string? Usuario { get; set; }
    //}

    //public class ListaDatosUsuario
    //{
    //    public string? Nombre { get; set; }
    //    public string? UsuarioID { get; set; }
    //    public ByteString? PIN { get; set; }
    //    public uint FechaInicio { get; set; }
    //    public uint FechaFin { get; set; }
    //}

}
