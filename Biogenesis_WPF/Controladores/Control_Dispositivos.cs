using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Google.Protobuf;
using Gsdk.Err;
using Gsdk.Gateway;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
//Clases
//using Biogenesis_WPF.Vistas;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
using Biogenesis_WPF.VistaModelo;
using Grpc.Core;
using System.Security.Policy;
using System.Windows.Media;

namespace Biogenesis_WPF.Controladores
{
    public class Control_Dispositivos
    {
        //Servicios
        private Service_ControlDB? controlDB;
        private Service_Gateway? gateway;
        private Service_GestorErrores? errores;

        //Modelos de Vistas
        ViewModel_Dispositivos vistaDispositivos = ViewModel_Dispositivos.viewModelDispositivos;
        ViewModel_Logs vistaLogs = ViewModel_Logs.viewModelLogs;

        //Control de Threads
        Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion necesaria entre los Hilos y la interfaz de Proceso Rutina
        ManualResetEvent pausar = new ManualResetEvent(true); //Thread que controla los tiempos de pausa de hilo en Proceso Rutina
        
        //Variable Control de Threads
        public CancellationTokenSource detenerThread = new CancellationTokenSource();
        private bool procesoIniciado = false;
        private bool procesoDetenido = false;

        public const int Error = -1;
        public const int Nuevo = 0;
        public const int Desconectado = 1;
        public const int Conectado = 2;

        public ObservableCollection<Modelo_Log> logs = new();            //
        public ObservableCollection<Modelo_Log> LogsFiltrados = new();  //

        //Variables Barras de Progreso
        private double cantidadEquipos = 0;
        private double porcentajePorEquipo = 0;
        private double porcentajePorLogs = 0;
        private DispatcherTimer? tiempoBarraLogs;
        public bool barrido = false;

        private string? filtro;
        private DispatcherTimer? tiempo;

        public void ProcesarDispositivos()
        {
            controlDB = new();
            gateway = new();
            errores = new();
            procesoIniciado = true;

            errores.Guardar("ProcesarDispositivos: ", "Log de 'Inicio de Proceso de Dispositivos'", " ");
            vistaLogs.AgregarLog(0, "Iniciando Proceso Dispositivos");
            vistaDispositivos.textoEstadoProceso.texto = "Inicio proceso de Rutina";

            pausar.WaitOne(1000);

            vistaDispositivos.barraRutina.valor = 5;

            //Variables de medicion de tiempo
            DateTime _inicioTiempoTranscurrido;
            int _demora = 0;
            TimeSpan _tiempotranscurrido = TimeSpan.Zero;

            //TODO: Generar el inicio del Gateway.exe con un "Shell" o un ".bat"
            // Esperar 5/10 segundos a que abra Gateway y se inicie

            // 1 - ConectarDB1 GateWay + Suscripcion Estado Gateway
            vistaDispositivos.textoEstadoProceso.texto = "Conectandose a Gateway";
            vistaLogs.AgregarLog(0, "Conectandose a Gateway");

            //do
            //{
            //    pausar.WaitOne(3000);
            //    gateway.ConectarGateway();
            //} while (gateway.channel.State == ChannelState.Shutdown || gateway.channel.State == ChannelState.TransientFailure);

            gateway.ConectarGateway();

            vistaDispositivos.barraRutina.valor = 10;

            pausar.Reset();
            pausar.WaitOne(1000);

            // 1.2 - Reconectar GateWay
            // if (statusGateway == shutdown)...
            // ReconectarGateway();
            vistaDispositivos.barraRutina.valor = 20;
            // if (Channel.State == "Ready" || Channel.State == "Idle")
            //{ Armar toda la rutina aca }
            uiDispatcher.Invoke(() =>
            {
                vistaDispositivos.textoEstadoProceso.texto = "Suscribiendose a Estado de Dispositivos";
            });
            gateway.SuscripcionEstadoDispositivos();

            pausar.Reset();
            pausar.WaitOne(1000);
            //// Apertura de conexion con DB
            //barraRutina.texto1 = "Conectando la Base de Datos";
            //controlDB.Conectar();
            //IF true => ....
            vistaDispositivos.barraRutina.valor = 30;

            pausar.Reset();
            pausar.WaitOne(1000);

            vistaDispositivos.barraRutina.valor = 40;

            // 3 - Marcar (DB) todos los dispositivos como "Nuevos"
            vistaDispositivos.textoEstadoProceso.texto = "Seteando Dispositivos como nuevos";
            controlDB.EjecutarNonQuery("UPDATE dispositivos SET estado = 'Nuevo', seteo_hora = 0, hay_eventos = 0;");

            // Modificamos la fecha de actualizacion de los usuarios en los dispositivos para que los cargue nuevamente.
            controlDB.EjecutarNonQuery("UPDATE usuarios_x_dispositivo SET actualizacion = '2000-01-01 00:00:00.000';");

            pausar.Reset();
            pausar.WaitOne(1000);

            vistaDispositivos.textoEstadoProceso.texto = "Cargando listado de Equipos";
            vistaDispositivos.barraRutina.valor = 50;

            pausar.Reset();
            pausar.WaitOne(1000);

            while (!procesoDetenido && !detenerThread.Token.IsCancellationRequested)
            {
                uiDispatcher.Invoke(() =>
                {
                    vistaDispositivos.dispositivos.Clear();
                    //vistaDispositivos.dispositivosFiltrados.Clear();
                     });
                var _listaDispositivos = controlDB.ListaDispositivos();
                foreach (var _item in _listaDispositivos)
                {
                    uiDispatcher.Invoke(() =>
                    {
                        vistaDispositivos.dispositivos.Add(_item);
                    });
                }
                vistaDispositivos.barraRutina.valor = 55;
                filtro = vistaDispositivos.Filtro;

                uiDispatcher.Invoke(() =>
                {
                    vistaDispositivos.FiltrarEquipos(filtro);
                });

                if (filtro == "" || filtro == null)
                {
                    uiDispatcher.Invoke(() =>
                    {
                        vistaDispositivos.dispositivosFiltrados.Clear();
                    });
                    foreach (var _item in vistaDispositivos.dispositivos)
                    {
                        uiDispatcher.Invoke(() =>
                        {
                            vistaDispositivos.dispositivosFiltrados.Add(_item);
                        });
                    }
                }
                else
                {
                    vistaDispositivos.FiltrarEquipos(filtro);
                }

                vistaDispositivos.barraRutina.valor = 60;

                _inicioTiempoTranscurrido = DateTime.Now;

                // 5 - Conectar Sincronicamente Todos los Dispositivos con estado = desconectado/nuevo
                uiDispatcher.Invoke(() =>
                {
                    vistaDispositivos.textoEstadoProceso.texto = "Conectando Dispositivos";
                });
                gateway.ConectarDispositivo(vistaDispositivos.dispositivosFiltrados);
                vistaLogs.AgregarLog(0, "Dispositivos Conectados: " + vistaDispositivos.dispositivosFiltrados.Count);
                vistaDispositivos.barraRutina.valor = 70;

                // 6.5 - Actualizacion de Fecha y Hora en Dispositivos
                //vistaDispositivos.textoEstadoProceso.texto = "Seteando Fecha y Hora en Dispositivos Conectados";
                //gateway.SetearFechaYHoraMulti();

                //pausar.Reset();
                //pausar.WaitOne(1000);
                vistaDispositivos.barraRutina.valor = 80;
                // 7 - Monitorear Dispositivos Conectados Sincronicamente
                vistaDispositivos.textoEstadoProceso.texto = "Iniciando Monitoreo de Dispositivos Conectados";
                gateway.MonitoreoDispositivos();

                //pausar.Reset();
                //pausar.WaitOne(1000);
                
                        vistaDispositivos.barraRutina.valor = 100;

                // 8 - Tomar 1 dispositivo de la lista (Foreach)
                int _i = 0;
                
                if(vistaDispositivos.dispositivos.Count > 0)
                { 
                    foreach (Modelo_Dispositivo _item in vistaDispositivos.dispositivos)
                    {
                        vistaDispositivos.barraRutina.valor = _i;
                        if (_item.Estado == "Conectado")
                        {
                           vistaLogs.AgregarLog(_item.Serial, "Accediendo a dispositivo");
                            vistaDispositivos.textoEstadoProceso.texto = "Accediendo a " + _item.Serial;
                            _item.Estado = "Accediendo..";
                            _item.Activa = true;

                            pausar.Reset();
                            pausar.WaitOne(500);

                            // 9 - Consultar (el Flag) si el dispositivo tiene nuevos logs
                            if (_item.HayEventos < 1)
                            {
                                vistaDispositivos.textoEstadoProceso.texto = "Descargando Logs...";          //                    
                                vistaLogs.AgregarLog(_item.Serial, "Descargando Logs");                      //
                                gateway.LogsPorFecha(_item.Serial, _item.UltimoLogFecha, _item.UltimoLogId); //



                                // TraerLogsDispositivo();...
                                // string QueryGuardarLog...
                                // log user enroll
                                // log user delete
                            }
                            controlDB.EjecutarNonQuery("UPDATE dispositivos SET hay_eventos = hay_eventos-1 WHERE serial = " + _item.Serial + "AND hay_eventos >= 0");


                            // 11 - Consultar Usuarios modificados (DB) para ese dispositivo + TimeStamp servidor
                            // string QueryUsuariosActualizados
                            // if (usuariosActualizados)
                            // EnrollUserList();
                            // En la DB (usuario_x_dispositivo) se guardara el TimeStamp del serividor

                            //vistaDispositivos.textoEstadoProceso.texto = "Eliminando Usuarios...";
                            //gateway.EliminarUsuarios(_item.Serial);
                            //pausar.Reset();
                            //pausar.WaitOne(3000);
                            vistaDispositivos.textoEstadoProceso.texto = "Actualizando Usuarios...";
                            gateway.ActualizarUsuarios(_item.Serial);


                            // 12 - Consultar Usuarios Dados de baja (DB) + TimeStamp servidor
                            // string QueryCambioEstadoActivo
                            // if (cambioEstado)
                            // DeleteUserList();
                            // En la DB (usuario_x_dispositivo) se guardara el TimeStamp del serividor

                            // 13 - Tomar Nuevo dispositivo
                            // Nueva vuelta del foreach

                            _item.Estado = "Conectado";
                            _item.Activa = false;

                            pausar.Reset();
                            pausar.WaitOne(1000);
                            _i++;

                           // vistaLogs.AgregarLog(_item.Serial,  "Dispositivo liberado");
                        }
                    }
                }
                vistaDispositivos.barraRutina.valor = 100;  

                // 14 - Tiempo restante de vuelta
                _tiempotranscurrido = DateTime.Now - _inicioTiempoTranscurrido;
                vistaDispositivos.textoTiempoProceso.texto = ((int)_tiempotranscurrido.TotalSeconds).ToString();
                vistaDispositivos.textoEstadoProceso.texto = "En Espera...";
                _demora = (int)(10000 - _tiempotranscurrido.TotalMilliseconds);

                if (_demora > 0 && _demora < 120000)
                {
                    pausar.Reset();
                    pausar.WaitOne(_demora);
                }
                vistaDispositivos.barraRutina.valor = 50;
            }
            //Desconecta Todos los Dispositivos
            gateway.DesconectarTodosDispositivos();

            //Desconexion a Gateway
            gateway.DesconectarGateway();

            // Cierra la conexion con la DB
            controlDB.Desconectar();

            // si errores en DB timmer=> on
            // alarma por falla DB

            vistaDispositivos.textoEstadoProceso.texto = "Rutina Detenida.";
        }

        public void DetenerRutina()
        {
            vistaDispositivos.textoEstadoProceso.texto = "Deteniendo Proceso..";
            detenerThread.Cancel(true);
        }

        
    }
}
