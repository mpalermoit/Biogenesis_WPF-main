using Biogenesis_WPF.biostar.services;
using System;
using System.Windows;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
//Protobuf
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
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
using Gsdk.Gateway;
using Gsdk.Err;
using Biogenesis_WPF.Modelos;
using System.Windows.Threading;
using GestionDB;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;
using Biogenesis_WPF.VistaModelo;
//
using System.Transactions;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using Google.Protobuf.WellKnownTypes;

namespace Biogenesis_WPF.Servicios
{
    public class Service_Gateway
    {
        //Instancias de Clases
        private Service_GestorErrores? errores = new();
        private Service_ControlDB? controlDB = new();

        //Acceso a la vista de Estado y Barra del Gateway
        //Modelo_Barra barraGateway = Modelo_Barra.BarraGateway;
        public Modelo_Texto estadoGateway = new();

        List<uint> listaDispconLogs = new List<uint>();

        //Vistas
        ViewModel_Logs vistaLogs = ViewModel_Logs.viewModelLogs;
        ViewModel_Dispositivos listaDispositivosAct = ViewModel_Dispositivos.viewModelDispositivos;

        //Canales de Conexion para accesos a funcionalidades de Suprema G-SDK.
        public Channel? channel;
        public ConnectSvc? connectSvc;
        public EventSvc? eventSvc;

        //Gestion necesaria entre los Hilos y la interfaz
        Dispatcher uiDispatcher = Application.Current.Dispatcher;

        //Lista de Huellas y Listado que contiene las huellas disponibles para solicitar
        Modelo_DedoData?[] listaHuellas = new Modelo_DedoData[4];

        //Variable de Suscripcion a eventos de Dispositivos
        private bool alreadySuscribed = false;

        //Lista de Codigos para filtros de Logs
        uint[] codigosRegistroEventos = { 0x1000, 0x1100, 0x1300, 0x1400, 0x1600, 0x1700, 0x1800, 0x1900, 0x1A00 };
        /*
         * 0x1000:  //0x1000 => BS2_EVENT_VERIFY_SUCCESS - Authentication Success
         * 0x1100:  //0x1100 => BS2_EVENT_VERIFY_FAIL - Authentication Failure
         * 0x1300:  //0x1300 => BS2_EVENT_IDENTIFY_SUCCESS - Authentication Success
         * 0x1400:  //0x1400 => BS2_EVENT_IDENTIFY_FAIL - Authentication Failure
         * 0x1600:  //0x1600 => BS2_EVENT_DUAL_AUTH_SUCCESS - Dual Authentication Success
         * 0x1700:  //0x1700 => BS2_EVENT_DUAL_AUTH_FAIL - Dual Authentication Failure
         * 0x1800:  //0x1800 => BS2_EVENT_AUTH_FAILED - Unregistered Credential
         * 0x1900:  //0x1900 => BS2_EVENT_ACCESS_DENIED - User Without Access Privileges or Violation of Zone Rules
         * 0x1A00:  //0x1A00 => BS2_EVENT_FAKE_FINGER_DETECTED - Fake Finger Detected
         * 0x2000:  //0x2000 => BS2_EVENT_USER_ENROLL_SUCCESS - User Enroll Succes
         */

        public void ConectarGateway()
        {
            //Ruta de busqueda LOCAL de los Certificados para la conexion del Gateway
            const string GATEWAY_CA_FILE = "./cert/ca.crt"; //TODO crear una carpeta dentro del proyecto (en el cliente) que contengalos certificados.

            //IP Adress de Conexion con Gateway
            string gatewayAddr = "localhost"; //Debe ir el puerto IP al cual se conectara el Gateway //TODO: Config
            int gatewayPort = 4000; //Debe llevar el Puerto al cual se conectara el Gateway (400 por default) //TODO: Config
            var channelCredentials = new SslCredentials(File.ReadAllText(GATEWAY_CA_FILE)); //Certificado //TODO: Config 

            try
            {
                channel = new Channel(gatewayAddr, gatewayPort, channelCredentials);
                connectSvc = new ConnectSvc(channel);
                eventSvc = new EventSvc(channel);
                eventSvc.InitCodeMap("./utils/event_code.json");
                uiDispatcher.Invoke(new Action(() =>
                {
                    estadoGateway.texto = "hola";
                }));
                //TODO: Suscripcion al estado del GateWay (https://supremainc.github.io/g-sdk/api/gateway/#subscribestatus)
                SuscripcionEstadoGateway(channel.State);

                if (channel.State == ChannelState.Shutdown || channel.State == ChannelState.TransientFailure/* || channel.State == ChannelState.Ready*/)
                {
                    ConectarGateway();
                }
                uiDispatcher.Invoke(new Action(() =>
                {
                    estadoGateway.texto = channel.State.ToString();
                }));

            }
            catch (Exception excGatewayConexion)
            {
                string? metodo = excGatewayConexion.TargetSite?.Name ?? "Desconocido";
                string? linea = excGatewayConexion.StackTrace ?? "Null";
                errores.Guardar(metodo, linea, excGatewayConexion.Message);
                return;
            }
            connectSvc.DisconnectAll();
        }

        public void DesconectarGateway()
        {
            try
            {
                uiDispatcher.Invoke(new Action(() =>
                {
                    estadoGateway.texto = "Desconectado";
                }));
                if (channel != null)
                {
                    connectSvc.DisconnectAll();
                    alreadySuscribed = false;
                    channel.ShutdownAsync().Wait();

                    uiDispatcher.Invoke(new Action(() =>
                    {
                        estadoGateway.texto = "Apagado";
                    }));
                }
            }
            catch (Exception excDesconexion)
            {
                string? metodo = excDesconexion.TargetSite?.Name ?? "Desconocido";
                string? linea = excDesconexion.StackTrace ?? "Null";
                errores.Guardar(metodo, linea, excDesconexion.Message);
                uiDispatcher.Invoke(new Action(() =>
                {
                    estadoGateway.texto = "No se pudo desconectar Gateway";
                }));
                throw;
            }
        }

        public void DesconectarTodosDispositivos()
        {
            connectSvc.DisconnectAll();
        }

        private void SuscripcionEstadoGateway(ChannelState estado)
        {
            switch (estado)
            {
                case ChannelState.Ready:
                    uiDispatcher.Invoke(() =>
                    {
                        estadoGateway.texto = "Listo";
                    });
                    break;
                case ChannelState.Idle:
                    uiDispatcher.Invoke(() =>
                    {
                        estadoGateway.texto = "Conexion Establecida";
                    });
                    break;
                case ChannelState.Connecting:
                    uiDispatcher.Invoke(() =>
                    {
                        estadoGateway.texto = "Conectando";
                    });
                    break;
                case ChannelState.Shutdown:
                    uiDispatcher.Invoke(() =>
                    {
                        estadoGateway.texto = "Apagado";
                    });
                    break;
                default:
                    uiDispatcher.Invoke(() =>
                    {
                        estadoGateway.texto = "Desconectado";
                    });
                    break;
            }
        }

        public void SuscripcionEstadoDispositivos()
        {
            //main.MostrarProcesoActual("Suscribiendose al estado de Dispositivos.");
            try
            {
                //Suscripciones al estado de los dispositivos
                var _stream = connectSvc.Subscribe(200);  //TODO: Ver la cantidad de suscripciones (min/max) y referencia (equipo/evento)
                SuscribirseEstados(_stream);
                alreadySuscribed = true;
            }
            catch (Exception _excSuscripcion)
            {
                alreadySuscribed = false;
                string? _metodo = _excSuscripcion.TargetSite?.Name ?? "Desconocido";
                string? _linea = _excSuscripcion.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, _excSuscripcion.Message);
                throw;
            }
        }

        public async void SuscribirseEstados(IAsyncStreamReader<Gsdk.Connect.StatusChange> stream)
        {
            try
            {


                if (alreadySuscribed == true)
                {
                    while (await stream.MoveNext())
                    {
                        try
                        {
                            //uiDispatcher.Invoke(() =>
                            //{
                            //    _statusLog = stream.Current;
                            //    //textCambiosEstado.Text += statusLog.DeviceID + Environment.NewLine;
                            //    //textCambiosEstado.Text += "\t" + statusLog.Status + Environment.NewLine;
                            //    //textCambiosEstado.Text += "\t" + DateTimeOffset.FromUnixTimeSeconds(statusLog.Timestamp).ToString("yyyy-MM-dd     HH:mm:ss") + Environment.NewLine;
                            //    //main.MostrarProcesoActual("Cambio de Estado de Dispositivo " + statusLog.DeviceID);
                            //});

                            var _statusLog = stream.Current;


                            if (_statusLog.Status == 0x00)
                            {
                                controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET ultima_actualizacion = GETDATA(), estado ='Desconectado' WHERE serial = " + _statusLog.DeviceID);
                                //TODO: Agregar en cambio de estado de los dispositivos en las vistas de WPF
                            }
                        }
                        catch (Exception _errorEvento)
                        {
                            string? _metodo = _errorEvento.TargetSite?.Name ?? "Desconocido";
                            string? _linea = _errorEvento.StackTrace ?? "Null";
                            errores.Guardar(_metodo, _linea, _errorEvento.Message);
                            return;
                        }
                    }
                }
            }
            catch (Exception excStream)
            {
                string? metodo = excStream.TargetSite?.Name ?? "Desconocido";
                string? linea = excStream.StackTrace ?? "Null";
                errores.Guardar(metodo, linea, excStream.Message);
                return;
            }
        }

        private void SetearDispNuevos()
        {
            try
            {
                //Desconectar todos
                connectSvc.DisconnectAll();

                //Setear Todos los Dispositivos como Nuevos
                controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET ultima_actualizacion = GETDATA(), estado ='Nuevo'");
            }
            catch (Exception _ex)
            {
                string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                string? _linea = _ex.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, _ex.Message);
                return;
            }
        }

        public void ConectarDispositivo(ObservableCollection<Modelo_Dispositivo> _dispositivos)
        {
            //barra1.Value = 75;
            DateTime _tiempoConexionDispositivos = DateTime.Now;

            int _fallas = 0;

            //var listaEquipos = controlDB.ListaDispositivos();
            //barra1.Maximo = listaEquipos.Count();

            foreach (var _item in _dispositivos)  //TODO: Cambiar por EQUIPO 
            {
                if (_item.Estado != "Conectado" && _item.Estado != "Error")
                {
                    _item.Estado = "Conectando...";
                    _item.Activa = true;
                    uint _newserial = 0;

                    ConnectInfo _connInfo = new ConnectInfo
                    {
                        IPAddr = _item.Ip,
                        Port = 51211,
                        UseSSL = false
                    };

                    try
                    {
                        _newserial = connectSvc.Connect(_connInfo);
                    }
                    catch (Exception _excEquipo)
                    {
                        string? _metodo = _excEquipo.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _excEquipo.StackTrace ?? "Null";
                        errores.Guardar(_metodo, _linea, _excEquipo.Message);

                        if (_item.Estado == "Nuevo")
                        {
                            //TODO: Registrar alarma
                        }

                        controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET estado = 'Desconectado', ultima_conexion = GETDATE() WHERE serial = " + _item.Serial);
                        _item.Estado = "Desconectado";
                        Console.WriteLine("Error: " + _excEquipo);
                        _fallas++;
                        _newserial = 0;
                    }

                    if (_newserial > 0)
                    {
                        if (_item.Serial != _newserial)
                        {
                            //Array de dispositivo actual para poder pasarlo por parametro para desconexion
                            uint[] _deviceID = { _item.Serial };

                            try
                            {
                                //Desconectamos el dispositivo por diferencia de datos contra la DB
                                connectSvc.Disconnect(_deviceID);
                            }
                            catch (Exception _excDesconectar)
                            {
                                string? _metodo = _excDesconectar.TargetSite?.Name ?? "Desconocido";
                                string? _linea = _excDesconectar.StackTrace ?? "Null";
                                errores.Guardar(_metodo, _linea, _excDesconectar.Message);

                                return;
                            }

                            //TODO: Listado de alarmas para envio de mails (una vez que chequeemos el listado)

                            //TODO: este Try/Catch estan de mas, pero son para los logs y Alarmas. Quitarlos y agregarlo en "ControlDB" ???
                            try
                            {
                                //Áctualizar DB con el estado del dispositivo que dio errores al desconectar
                                controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET estado = 'Falla', ultima_conexion = GETDATE() WHERE serial = " + _item.Serial);
                                _item.Estado = "errores";

                                //TODO: generar alarma
                            }
                            catch (Exception _excGuardarDBError)
                            {
                                string? _metodo = _excGuardarDBError.TargetSite?.Name ?? "Desconocido";
                                string? _linea = _excGuardarDBError.StackTrace ?? "Null";
                                errores.Guardar(_metodo, _linea, _excGuardarDBError.Message);
                                return;
                            }
                        }

                        else
                        {
                            //Actualizamos la DB con el estado Conectado y la fecha y hora actual.
                            controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET estado = 'Conectado', seteo_hora = 0, ultima_conexion = GETDATE() WHERE serial = " + _item.Serial);
                            _item.Estado = "Conectado";
                        }
                    }
                    TimeSpan _tiempotranscurrido = DateTime.Now - _tiempoConexionDispositivos;
                    if (_tiempotranscurrido.TotalSeconds > 300)  //TODO: Archivo Configuraciones 
                    {
                        break;
                    }

                    _item.Activa = false;
                }
            }
        }

        public void MonitoreoDispositivos()
        {
            List<uint> _idDispositivos = new List<uint>();
            var _listaDispositivos = controlDB.ListaDispositivos();
            foreach (var _item in _listaDispositivos)
            {
                if (_item.Serial > 0 && _item.Estado == "Conectado")
                {
                    _idDispositivos.Add(_item.Serial);
                }
            }

            try
            {
                if (_idDispositivos.Count > 0)
                {
                    eventSvc.StartMonitoringMulti(_idDispositivos);
                    eventSvc.SetCallback(RecibirEvento);
                }
            }
            catch (Exception _errorMonitoring)
            {
                //TODO: Agregar Estado:FALLA
                string? _metodo = _errorMonitoring.TargetSite?.Name ?? "Desconocido";
                string? _linea = _errorMonitoring.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, _errorMonitoring.Message);
                return;
            }
        }

        public void RecibirEvento(EventLog logEvent)
        {
            //TODO: filtrar por eventos que nos interesen.
            //eventListaAlarmas.Text += " " + DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime + Environment.NewLine;
            //eventListaAlarmas.Text += "ID Dispositivo: " + logEvent.DeviceID + Environment.NewLine;
            //eventListaAlarmas.Text += "ID Usuario: " + logEvent.UserID + Environment.NewLine;
            //eventListaAlarmas.Text += "ID HayEventos: " + logEvent.ID + Environment.NewLine;
            //eventListaAlarmas.Text += "HayEventos: " + eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode) + Environment.NewLine;

            //DateTime _tiempo = DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime;

            try
            {
                //  controlDB.Conectar();

                foreach (var _item in codigosRegistroEventos)
                {
                    if (logEvent.EventCode == _item)
                    {
                        controlDB.EjecutarNonQueryGateway("UPDATE dispositivos SET hay_eventos = 0 WHERE serial = " + logEvent.DeviceID);
                        listaDispconLogs.Add(logEvent.DeviceID + logEvent.SubCode);

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string? _metodo = ex.TargetSite?.Name ?? "Desconocido";
                string? _linea = ex.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, ex.Message);
                return;
            }

            //TODO: Luego de reconocer el evento, debe hacer una Flag en la DB para poder traernos todos los logs de ESE dispositivo desde el ultimo evento (fecha) que tengamos guardados en la DB.

            // GUARDADO DE LOG PARA "ControlDB"
        }

        #region Seteo Fecha y Hora
        /// <summary>
        /// Seteo de Fecha y Hora en el Dispositivo Actual 
        /// donde el estado del mismo sea "Conectado" 
        /// y el seteo_hora sea 0 (seteo_hora < 0)
        /// </summary>
        public void SeteoFechaYHora()
        {
            TimeSvc _timeSvc = new TimeSvc(channel);

            List<uint> listaDispositivos = new List<uint>();
            var _dispositivos = controlDB.ListaDispositivos();

            foreach (var _item in _dispositivos)
            {
                if (_item.Estado == "Conectado" && _item.SeteoHora < 1)
                {
                    string _dispositivoID = _item.Serial.ToString();
                    uint _deviceID = uint.Parse(_dispositivoID);
                    DateTimeOffset _date = DateTimeOffset.Now;
                    ulong _fecha = (ulong)_date.ToUnixTimeSeconds();

                    TimeConfig _getHora = _timeSvc.GetConfig(_deviceID);
                    try
                    {
                        _timeSvc.Set(_deviceID, _fecha);
                    }
                    catch (Exception _errSetHora)
                    {
                        string? _metodo = _errSetHora.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _errSetHora.StackTrace ?? "Null";
                        errores.Guardar(_metodo, _linea, _errSetHora.Message);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Seteo de Fecha y Hora en todos los Dispositivos 
        /// donde el estado sea "Conectado" 
        /// y el seteo_hora sea 0 (seteo_hora < 0)
        /// </summary>
        public void SetearFechaYHoraMulti()
        {
            TimeSvc _timeSvc = new TimeSvc(channel);

            List<uint> _listaDispositivosAactualizar = new List<uint>();
            var _listaDispositivos = controlDB.ListaDispositivos();
            foreach (var _item in _listaDispositivos)
            {
                if (_item.Estado == "Conectado" && _item.SeteoHora < 1)
                {
                    _listaDispositivosAactualizar.Add(_item.Serial);
                }
            }

            if (_listaDispositivosAactualizar.Count > 0)
            {
                //Seteamos la fecha a los dispositivos.
                DateTimeOffset _date = DateTimeOffset.Now;
                ulong _fecha = (ulong)_date.ToUnixTimeSeconds();
                try
                {
                    _timeSvc.SetMulti(_listaDispositivosAactualizar, _fecha);
                    controlDB.EjecutarNonQuery("UPDATE dispositivos SET seteo_hora = 30 WHERE estado = 'Conectado' AND seteo_hora < 1");
                }
                catch (Exception errorSetHora)
                {
                    string? metodo = errorSetHora.TargetSite?.Name ?? "Desconocido";
                    string? linea = errorSetHora.StackTrace ?? "Null";
                    errores.Guardar(metodo, linea, errorSetHora.Message);
                    return;
                }
            }
            controlDB.EjecutarNonQuery("UPDATE dispositivos SET seteo_hora = seteo_hora-1 WHERE estado = 'Conectado'");
        }
        #endregion

        #region Logs

        /// <summary>
        /// Trae los Logs del equipo segun los filtros aplicados internamente. Busca logs y los guarda en la Db.
        /// </summary>
        /// <param name="dispositivo"> Uint del dispositivo que enviara los logs</param>
        /// <param name="UltimoLogFecha">Fecha y hora del ultimo log consultado</param>
        public void LogsPorFecha(uint dispositivo, DateTime UltimoLogFecha, uint UltimoLogID)
        {
            // Ruta de busqueda LOCAL de los Certificados para la conexion del Gateway
            const string GATEWAY_CA_FILE = "./cert/ca.crt"; //TODO crear una carpeta dentro del proyecto (en el cliente) que contengalos certificados.

            // IP Adress de Conexion con Gateway
            string gatewayAddr = "localhost"; // Puerto IP al cual se conectara el Gateway //TODO: Config
            int gatewayPort = 4000; // Puerto al cual se conectara el Gateway (4000 por default) //TODO: Config
            var channelCredentials = new SslCredentials(File.ReadAllText(GATEWAY_CA_FILE)); //Certificado //TODO: Config 

            channel = new Channel(gatewayAddr, gatewayPort, channelCredentials);
            connectSvc = new ConnectSvc(channel);
            eventSvc = new EventSvc(channel);
            eventSvc.InitCodeMap("./utils/event_code.json");

            // Devolucion del Estado de la Consulta a DB (1 => Ok o 2627 => OK)
            int? _estado = null;


            DateTime _fechaUltimoLogNueva = UltimoLogFecha;
            DateTime _fechaUltimoLogValido = UltimoLogFecha;
            DateTime _tiempoEnd = DateTime.UtcNow;

            // Convertimos DateTime a tipo uint => Fecha Ultimo Log y Fecha Actual     
            //uint _startTime = ConvertirDateTimeAUint(DateTime.SpecifyKind(UltimoLogFecha, DateTimeKind.Utc));
            uint _fechaInicio = ConvertirDateTimeAUint(UltimoLogFecha);
            uint _endTime = ConvertirDateTimeAUint(_tiempoEnd);

            int cant_logs = 0;
            int _cantErrores = 0;

            uint _nuevoLogId = UltimoLogID;
            uint _nuevoLogIdValido = UltimoLogID;
            string _fechaString = string.Empty;
           
            //Creamos el filtro de Eventos y cargamos el primer Codigo del "textBox => eventEventCode.Text".
            EventFilter _filtroDeEventos = new EventFilter();

            //Creamos la Lista de Filtros por la cual se filtrara en la busqueda y los demas codigos de eventos.
            List<EventFilter> _listaFiltros = new List<EventFilter>();

            // Carga de Filtros de Eventos => Codigo de Evento/Fecha Inicio de Eventos/Fecha de Final de Eventos.
            foreach (var _item in codigosRegistroEventos)
            {
                _filtroDeEventos = new EventFilter() { EventCode = _item, StartTime = _fechaInicio, EndTime = _endTime };
                _listaFiltros.Add(_filtroDeEventos);
            }

            try
            {
                // Filtramos logs segun => ID Dispositivo/Desde Primer HayEventos/Cantidad maxima de eventos por busqueda/filtros
                var _logsObtenidos = eventSvc.GetLogsFilter(dispositivo, 0, 100, _listaFiltros.ToArray());

                string _query = "";

                foreach (var _log in _logsObtenidos)
                {
                    // Modificar Variables para asignarlas a la DB
                    // DateTime para la fecha
                    DateTime _fechaLog = ConvertirUintADateTime(_log.Timestamp - (60 * 60 * 3));
                    string _fechaStringCorregida = _fechaLog.ToString("yyyy/MM/dd HH:mm:ss");
                    // Varchar para Detalle de HayEventos
                    //string _detalleEvento = _log.SubCode.ToString();
                    string _detalleEvento = eventSvc.GetEventString(_log.EventCode, _log.SubCode);

                    if (_fechaLog > _fechaUltimoLogNueva || _log.ID > _nuevoLogId)
                    {
                        _fechaUltimoLogNueva = _fechaLog;
                        _nuevoLogId = _log.ID;
                    }

                    _fechaString = _fechaUltimoLogNueva.ToString("yyyy/MM/dd HH:mm:ss");

                    //Cargar la fecha del ultimo Log consultado
                    if (_fechaLog == UltimoLogFecha && _log.ID == UltimoLogID)
                    {
                        _query = $"UPDATE dispositivos SET ultimo_log_fecha = '{_fechaString}', ultimo_log_id = '{_nuevoLogId}' , hay_eventos = 30 WHERE serial = " + dispositivo;
                    }
                    else
                    {
                        _query = $"INSERT INTO logs (equipo_id, log_numero, fecha, codigo_evento, subcodigo_evento, id_usuario) VALUES ({_log.DeviceID},{_log.ID},'{_fechaStringCorregida}',{_log.EventCode}, {_log.SubCode}";

                        if (_log.UserID == "" || _log.UserID == null)
                        {
                            _query = _query + ", Null)";  // {idUsuario} = Null
                        }
                        else
                        {
                            _query = _query + $",{_log.UserID})";  // {idUsuario} != 0
                        }
                        cant_logs += 1;
                    }

                    _estado = controlDB.EjecutarNonQuery(_query);

                    if (_estado != 1 && _estado != 2627)
                    {
                        _cantErrores++;
                        break;
                    }
                    else
                    {
                        vistaLogs.AgregarLog(_log.DeviceID, _fechaStringCorregida + " " + _detalleEvento);

                        _nuevoLogIdValido = _nuevoLogId;
                        _fechaUltimoLogValido = _fechaUltimoLogNueva;
                    }
                }

                if (cant_logs > 0)
                { 
                    _query = $"UPDATE dispositivos SET ultimo_log_fecha = '{_fechaUltimoLogValido.ToString("yyyy/MM/dd HH:mm:ss")}', ultimo_log_id = '{_nuevoLogIdValido}', hay_eventos = 0  WHERE serial = " + dispositivo;
                    controlDB.EjecutarNonQuery(_query);

                    if (_cantErrores == 0)
                    {
                        vistaLogs.AgregarLog(dispositivo, "Logs descargados: " + cant_logs);
                    }
                    else
                    {
                        vistaLogs.AgregarLog(dispositivo, "Errores en Logs");
                    }
                }
            }
            
            catch (Exception _ex)
            {
                //TODO: Cambiar estado disp: FALLA y return error
                string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                string? _linea = _ex.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, _ex.Message);
                return;
            }
        }

        #endregion

        #region Usuarios
        public void ActualizarUsuarios(uint dispositivoId)
        {
            //Instanciamos la Clase UserSvc
            UserSvc _userSvc = new UserSvc(channel);

            List<int> _listaUsuariosId = new List<int>();
            //int usuarioID = 0;                  //Int32.Parse(textIDUsuario.Text);

            FingerData _huellaDedo = new FingerData();
            ByteString _pin = ByteString.Empty;
            bool _habilitarIDyPIN = false;

            string? _ultimoID = string.Empty;
            List<Modelo_Usuario> _listaUsuariosDB = new List<Modelo_Usuario>();
            List<UserInfo> _UsuariosNuevos = new List<UserInfo>();

            UserHdr _userHeader = new UserHdr();
            UserSetting _userSetting = new UserSetting();
            UserInfo _infoUsuario = new UserInfo();
            _infoUsuario.Hdr = _userHeader;
            _infoUsuario.Setting = _userSetting;

           //string _queryUsuarios = $"SELECT * FROM view_usuarios WHERE activo = '1' and serial_equipo={dispositivoId} ORDER BY id, nro_huella ASC"; //WHERE activo = 1 ORDER BY id ASC


            string _queryUsuarios = $"SELECT * FROM view_usuarios WHERE activo = '1' and serial_equipo = {dispositivoId} and actualizacion >= acualizacion_en_disp  ORDER BY id, nro_huella ASC";
            try
            {
                
                _listaUsuariosDB = controlDB.EjecutarConsultaUsuarios(_queryUsuarios);

                if (_listaUsuariosDB != null || _listaUsuariosDB.Count != 0)
                {
                    foreach (var _item in _listaUsuariosDB)
                    {
                        string? _idTemporal = _item.Id;

                       

                        if (_ultimoID != _idTemporal)
                      
                        {
                         
                            if(_ultimoID != string.Empty)
                            {
                                _UsuariosNuevos.Add(_infoUsuario);
                                
                                _userHeader = new UserHdr();
                                _userSetting = new UserSetting();
                                _infoUsuario = new UserInfo();
                              
                                _infoUsuario.Hdr = _userHeader;
                                _infoUsuario.Setting = _userSetting;
                                _infoUsuario.Fingers.Clear();
                            }

                            

                            _infoUsuario.Hdr.ID = _item.Id;
                            _ultimoID = _item.Id;

                            _infoUsuario.Name = _item.Nombre; //TODO Reemplazar por Apellido+" "+Nombre(48 caracteres) - Apellid+nombre en unavariable.
                            _infoUsuario.Setting.StartTime = ConvertirDateTimeAUint((DateTime)_item.FechaInicio);
                            _infoUsuario.Setting.EndTime = ConvertirDateTimeAUint((DateTime)_item.FechaFin);

                            //Obtenemos el Hash de la contraseña
                            //ByteString _pinByteString = _item.PIN;
                            //ByteString pinString2 = _userSvc.GetPINHash(_pinByteString);

                            _infoUsuario.PIN = _item.PIN;
                            
                            _infoUsuario.Setting.BiometricAuthMode = 0xFF; //Undefined
                            _infoUsuario.Setting.SecurityLevel = 0;        //Undefined
                            if (_item.Acceso == null || _item.Acceso == false)
                            {
                                _infoUsuario.AccessGroupIDs.Add(4);
                            }
                            else
                            {
                                _infoUsuario.AccessGroupIDs.Add((uint)_item.GrupoDeAcceso);
                            }
                            //Armado de la Informacion de Usuario
                            _habilitarIDyPIN = _item.HabilitarIDyPIN;
                
                            if (_habilitarIDyPIN == false)
                            {
                                _infoUsuario.Setting.IDAuthMode = 0xFF;    //Undefined
                            }
                            else
                            {
                                _infoUsuario.Setting.IDAuthMode = 8;
                            }

                        }

                        // Armado de Huellas (2 Templates por cada Huella)
                        
                        _huellaDedo = new FingerData();


                        _huellaDedo.Templates.Clear();
                        _huellaDedo.Index = 0;
                        _huellaDedo.Flag = 0;
                        byte[] templateByte1 = (byte[])_item.Template1;
                        _huellaDedo.Templates.Add(ByteString.CopyFrom(templateByte1));
                        byte[] templateByte2 = (byte[])_item.Template2;
                        _huellaDedo.Templates.Add(ByteString.CopyFrom(templateByte2));

                        _infoUsuario.Fingers.Add(_huellaDedo);
                        _infoUsuario.Hdr.NumOfFinger = _infoUsuario.Fingers.Count;
                    
                    }
                    
                }

                if (_infoUsuario != null || _infoUsuario.Hdr.ID != "")
                {
                    _UsuariosNuevos.Add(_infoUsuario);
                }

                UserInfo[] _usuarionuevo = new UserInfo[1];

                foreach (var _item in _UsuariosNuevos)
                {
                    _usuarionuevo[0] = _item;

                    try
                    {
                        if (_usuarionuevo[0].Hdr.ID != null && _usuarionuevo[0].Hdr.ID != "")
                        {
                            _userSvc.Enroll(dispositivoId, _usuarionuevo, true);
                        
                            // Acutalizamos la tabla de usuarios_x_dispositivos para confirmar que se actualizaron los usuarios
                            _queryUsuarios = $"UPDATE usuarios_x_dispositivo SET actualizacion = GETDATE() WHERE serial_equipo = {dispositivoId} AND id_usuario = {_usuarionuevo[0].Hdr.ID}";
                            controlDB.EjecutarConsultaUsuarios(_queryUsuarios);
                        }
                    }
                    catch (Exception _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        errores.Guardar(_metodo, _linea, "--- Error al cargar Usuario '" + _usuarionuevo[0].Hdr.ID + "': " + _ex.Message);
                        return;
                    }
                    

                }

                _UsuariosNuevos.Clear();
            }
            catch (Exception exception)
            {
                string? _metodo = exception.TargetSite?.Name ?? "Desconocido";
                string? _linea = exception.StackTrace ?? "Null";
                errores.Guardar(_metodo, _linea, "--- Error en el armado de Usuarios" + exception.Message);
                //MessageBox.Show("Error: " + exception);
                return;
            }
        }

        public void EliminarUsuarios(uint dispositivoId)
        {
            ///
            /// Eliminar Usuario
            ///

            UserSvc userSvc = new UserSvc(channel);

            uint _dispositivoId = dispositivoId;
            //string usuarioID = userUsuarioID.Text;
            var _listaUsuarios = userSvc.GetList(_dispositivoId);
            List<string> _usuarios = new List<string> { _listaUsuarios.ToString() };

            userSvc.Delete(_dispositivoId, _usuarios.ToArray());

            ///
            /// Eliminar Multi
            ///

            //UserSvc userSvc = new UserSvc(channel);

            //// Get connected devices
            //var deviceList = connectSvc.GetDeviceList();

            //uint deviceID = uint.Parse(userDispositivoID.Text);
            //string usuarioID = userUsuarioID.Text;

            //List<string> usuarios = new List<string> { usuarioID };

            //// Get device IDs of the devices
            //List<uint> devicesIDs = new List<uint>();

            //foreach (var device in deviceList)
            //{
            //    devicesIDs.Add(device.DeviceID);
            //}
            //userSvc.DeleteMulti(devicesIDs.ToArray(), usuarios.ToArray());
        }

        #region Huellas

        #endregion

        #endregion

        #region Metodos de Conversion
        //
        // Metodos de Conversion
        //
        public uint ConvertirDateTimeAUint(DateTime fecha)
        {
            // Obtener Fecha de tipo DateTime para convertir a uint
            //DateTime _fechaDateTime = fecha;

            //// Crear una fecha de referencia (por ejemplo, el Epoch Unix)
            //DateTime _fechaAUint = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            //// Calcular la diferencia en segundos entre la fecha actual y la fecha de referencia
            //TimeSpan _diferencia = _fechaDateTime - _fechaAUint;

            //uint _fechaUint = 0;
            //try
            //{
            //    // Convertir la diferencia total de segundos a un valor uint (puede haber pérdida de precisión)
            //    _fechaUint = (uint)_diferencia.TotalSeconds;
            //}
            //catch (Exception _exConvertToUint)
            //{
            //    string? _metodo = _exConvertToUint.TargetSite?.Name ?? "Desconocido";
            //    string? _linea = _exConvertToUint.StackTrace ?? "Null";
            //    errores.Guardar(_metodo, _linea, _exConvertToUint.Message);
            //    throw;
            //}

            // Convierte la fecha y hora a un objeto DateTimeOffset
            DateTimeOffset dateTimeOffset = new DateTimeOffset(fecha);

            // Obtiene el Unix timestamp en segundos
            uint _fechaUint = (uint)dateTimeOffset.ToUnixTimeSeconds();
            // _fechaUint = _fechaUint - (60 * 60 * 3);

            return _fechaUint;
        }

        public DateTime ConvertirUintADateTime(uint unixTimestampInSeconds)
        {
            // Crear un objeto DateTime usando la fecha de referencia del Epoch Unix
            //DateTime _fechaUint = new DateTime(1970, 1, 1, 0, 0, (int)DateTimeKind.Utc);

            //// Agregamos los segundos de la variable uint a la fecha de referencia Epoch Unix y lo guardamos en DateTime
            //DateTime _fechaConvertida;

            //try
            //{
            //    _fechaConvertida = _fechaUint.AddSeconds(tiempoSpan);
            //}
            //catch (Exception _excConvertToDateTime)
            //{
            //    string? _metodo = _excConvertToDateTime.TargetSite?.Name ?? "Desconocido";
            //    string? _linea = _excConvertToDateTime.StackTrace ?? "Null";
            //    errores.Guardar(_metodo, _linea, _excConvertToDateTime.Message);
            //    throw;
            //}

            // Convierte el Unix timestamp a un objeto DateTimeOffset
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestampInSeconds);

            // Convierte el DateTimeOffset a un objeto DateTime
            DateTime _fechaConvertida = dateTimeOffset.DateTime;

            return _fechaConvertida;
        }

        #endregion
    }
}
