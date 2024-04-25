using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using System.ComponentModel;
//Grpc & G-SDK
using Grpc.Core;
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
//Enlace con Base de Datos
using System.Data.SqlClient;
//Enlace con Otros Scripts
using Space_Biometricos;
using Biogenesis_WPF;
using Biogenesis_WPF.biostar.services;
using Gsdk.Action;
using System.Reflection.PortableExecutable;
using Google.Protobuf.WellKnownTypes;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
using System.Windows.Input;
//
using System.Runtime.Serialization;
using Google.Protobuf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biogenesis_WPF.Servicios
{
    public class Service_ControlDB
    {
        private Service_GestorErrores error = new();

        public List<Modelo_Dispositivo> dispositivos = new List<Modelo_Dispositivo>();
        public List<Modelo_Usuario> usuarios = new List<Modelo_Usuario>();

        private static readonly object lockObject = new object();

        //private static string connectionString;
        private SqlConnection? connection;

        public Service_ControlDB()
        {

        }

        public bool Conectar()
        {
            //TODO Archivo de Configuraciones !!
            //config.coneccciondb
            //config.DBServer
            //config.DBName
            //cinfig.DBUser
            //config.DBPass
            //config.DBCertificate

            string stringDeConexionDB = "Server=SQ-IT-SRV\\SQLD_TEST; Database=BioTest;User ID=MaxiP;Password=123456;TrustServerCertificate=True;Min Pool Size=20";

            try
            {
                if (connection == null)
                {
                    connection = new SqlConnection(stringDeConexionDB);
                    connection.Open();
                }
                else if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
                string? linea = ex.StackTrace ?? "Null";
                error.Guardar(metodo, linea, ex.Message);
                return false;
            }
        }

        public void Desconectar()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Ejecuta una consulta (enviada por parametro) en MS SQL
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Devuelve los datos obtenidos de la consulta</returns>
        public SqlDataReader? EjecutarConsulta(string query)
        {
            lock (lockObject)
            {
                SqlDataReader? _reader = null;
                SqlDataReader? _datosReader = null;

                Conectar();

                if (connection != null)
                {
                    try
                    {
                        SqlCommand _command1 = new SqlCommand(query, connection);

                        _reader = _command1.ExecuteReader();
                        _datosReader = _reader;
                        _command1.Cancel();
                        _reader.Close();
                    }
                    catch (Exception _excConsultaDB)
                    {
                        string? _metodo = _excConsultaDB.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _excConsultaDB.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _excConsultaDB.Message + " --- " + query);
                    }
                }
                return _datosReader;
            }
        }


        public SqlDataReader? EjecutarConsulta2(string query)
        {
            lock (lockObject)
            {
                SqlDataReader? _datosReader = null;

                Conectar();

                if (connection != null)
                {
                    try
                    {
                        using (SqlCommand _command1 = new SqlCommand(query, connection))
                        {
                            _datosReader = _command1.ExecuteReader();
                        }
                    }
                    catch (Exception _excConsultaDB)
                    {
                        string? _metodo = _excConsultaDB.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _excConsultaDB.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _excConsultaDB.Message + " --- " + query);
                    }
                }
                return _datosReader;
            }
        }

        /// <summary>
        /// Ejecuta una query (enviada por parametro) para realizar modificaciones en MS SQL.
        /// </summary>
        /// <param name="query"></param>
        public int EjecutarNonQuery(string query)
        {
            lock (lockObject)
            {
                int _estadoQuery = 0;

                Conectar();
                if (connection != null)
                {
                    try
                    {
                        SqlCommand _command2 = new SqlCommand(query, connection);
                        _command2.ExecuteNonQuery();
                        _estadoQuery = 1;
                        _command2.Cancel();
                    }
                    catch (SqlException _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = _ex.Number;
                    }
                    catch (DbException _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = 2;
                    }
                    catch (Exception _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = 3;
                    }
                }
                return _estadoQuery;
            }      
        }

        public int EjecutarNonQueryGateway(string query)
        {
            lock (lockObject)
            {
                int _estadoQuery = 0;

                Conectar();
                if (connection != null)
                {
                    try
                    {
                        SqlCommand _command2 = new SqlCommand(query, connection);
                        _command2.ExecuteNonQuery();
                        _estadoQuery = 1;
                        _command2.Cancel();
                    }
                    catch (SqlException _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = _ex.Number;
                    }
                    catch (DbException _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = 2;
                    }
                    catch (Exception _ex)
                    {
                        string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                        string? _linea = _ex.StackTrace ?? "Null";
                        error.Guardar(_metodo, _linea, _ex.Message + " --- " + query);

                        _estadoQuery = 3;
                    }
                }
                return _estadoQuery;
            }
        }


        //TODO: Hacer el metodo con la Query para la Consulta de Alarmas (TimeSpan + Detalle de Alarma) 

        public void EjecutarGuardadoLog(string query, int numeroConexion) //TODO: Debe pasar los datos por parametro => Serial - Fecha - HayEventos - Sub HayEventos - Usuario (null o no) - Detalle ... etc.
        {
            lock (lockObject)
            {
                string _guardaLog = "";
                try
                {
                    //SqlCommand command = new SqlCommand(query, connection);
                    //command.Parameters.AddWithValue("@equipo_id", equipo_id);
                    //command.Parameters.AddWithValue("@fecha", fecha);
                    //command.Parameters.AddWithValue("@codigo_evento", codigo_evento);
                    ////TODO Agregar Sub HayEventos - Sub Codigo
                    //command.Parameters.AddWithValue("@id_usuario", id_usuario);
                    //command.ExecuteNonQuery();
                }
                catch (Exception _ex)
                {
                    string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                    string? _linea = _ex.StackTrace ?? "Null";
                    error.Guardar(_metodo, _linea, _ex.Message);
                    return;
                }
            }
        }

        public uint  ListaLogs()
        {
            lock (lockObject)
            {
                Conectar();
                uint i = 0;
                try
                {
                    string _query = "SELECT * FROM logs ORDER BY equipo_id, fecha ASC";

                    SqlCommand _command = new SqlCommand(_query, connection);

                    _command.CommandTimeout = 300;  //(0) Sin limite de tiempo - 30 Default, en 60 Fallaba tambien, Se recomiendan 300.
                    using (SqlDataReader _reader = _command.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            i += 1;
                        }
                    }
                    _command.Cancel();
                }
                catch (Exception _ex)
                {
                    string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                    string? _linea = _ex.StackTrace ?? "Null";
                    error.Guardar(_metodo, _linea, _ex.Message);
                }
                return i;
            }
        }

        public List<Modelo_Dispositivo> ListaDispositivos()
        {
            lock (lockObject)
            {
                Conectar();

                dispositivos.Clear();

                try
                {
                    string _query = "SELECT * FROM dispositivos ORDER BY serial ASC";
                    ////Codigo Anterior donde en alguna ocasion fallo el cierre del SqlDataReader
                    //SqlCommand _command = new SqlCommand(_query, connection);
                    //_command.CommandTimeout = 300;
                    //SqlDataReader _reader = _command.ExecuteReader();

                    //while (_reader.Read())
                    //{
                    //    long _serialEqLong = (long)_reader["serial"];

                    //    dispositivos.Add(new Modelo_Dispositivo
                    //    {
                    //        Serial = (uint)_serialEqLong,
                    //        Ip = (string)_reader["ip"],
                    //        HayEventos = (bool)_reader["hayeventos"],
                    //        Estado = (string)_reader["estado"],
                    //        UltimoLogFecha = (DateTime)_reader["ultimo_evento"],
                    //        SeteoHora = (int)_reader["seteo_hora"]

                    //        //TODO: Agregar todo el detalle que tienen los dispositivos para el armado completo de la lista
                    //    });
                    //}
                    //_reader.Close();

                    //Codigo Corredigo por AI para manejar correctamente el cierre de SqlDataReader ya que arrojo error con el anterior codigo
                    SqlCommand _command = new SqlCommand(_query, connection);
                    // Seteamos el Tiempo de espera hasta que complete la consulta porque fallaba en el predefinido
                    //_command.CommandTimeout = 300;  //(0) Sin limite de tiempo - 30 Default, en 60 Fallaba tambien, Se recomiendan 300.
                    using (SqlDataReader _reader = _command.ExecuteReader()) //prender tablero
                    {
                        while (_reader.Read())
                        {
                            long _serialEqLong = (long)_reader["serial"];
                            long _logId = (long)_reader["ultimo_log_id"];

                            dispositivos.Add(new Modelo_Dispositivo
                            {
                                Serial = (uint)_serialEqLong,
                                Ip = (string)_reader["ip"],
                                HayEventos = (int)_reader["hay_eventos"],
                                Estado = (string)_reader["estado"],
                                UltimoLogFecha = (DateTime)_reader["ultimo_log_fecha"],
                                UltimoLogId = (uint)_logId,
                                SeteoHora = (int)_reader["seteo_hora"]
                            });
                        }
                    }
                    _command.Cancel();
                }
                catch (SqlException ex)
                {
                    foreach (SqlError sqlError in ex.Errors)
                    {
                        if (sqlError.Class == 11)
                        {
                            string? _metodo = ex.TargetSite?.Name ?? "Desconocido";
                            string? _linea = ex.StackTrace ?? "Null";
                            error.Guardar(_metodo, _linea, sqlError.ToString());
                        }
                    }
                }

                //catch (SqlTimeOutException _ex)
                //{
                //    string? _metodo = _ex.TargetSite?.Name ?? "Desconocido";
                //    string? _linea = _ex.StackTrace ?? "Null";
                //    error.Guardar(_metodo, _linea, _ex.Message);
                //    return dispositivos;
                //}
                return dispositivos;
            }
        }


        public List<Modelo_Usuario> EjecutarConsultaUsuarios(string query)
        {
            lock (lockObject)
            {
                Conectar();

                List<Modelo_Usuario> usuarios = new List<Modelo_Usuario>();

                try
                {
                    SqlCommand _command = new SqlCommand(query, connection);

                    using (SqlDataReader _reader = _command.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            //string pinString = (string)_reader["password"];
                            //ByteString pinString2 = _userSvc.GetPINHash(pinString);
                            //string _pin = pinString;

                            //De Varchar a ByteString
                            //byte[] datosBinarios = Encoding.UTF8.GetBytes((string)_reader["password"]);
                            string byteString = string.Empty;
                            ByteString demo = null;
                            int binary = _reader.GetOrdinal("pin");
                            // Verifica si la columna no es nula
                            if (!_reader.IsDBNull(binary))
                            {
                                // Obtiene el valor varbinary como un array de bytes
                                byte[] bytes = (byte[])_reader["pin"];
                                //ByteString byteString = bytes;
                                //ByteString byteString = new MemoryStream(bytes).ToByteString();
                                // Convierte los bytes a una cadena de texto usando Encoding
                                //string popo = Encoding.Default.GetString(bytes);
                                //demo = ByteString.CopyFromUtf8(popo);
                                demo = ByteString.CopyFrom(bytes);

                            }

                            usuarios.Add(new Modelo_Usuario
                            {
                                Id = _reader["id"].ToString(),
                                Nombre = (string)_reader["name"], //TODO Reemplazar por Apellido+" "+Nombre(48 caracteres) - Apellid+nombre en unavariable.
                                FechaInicio = (DateTime)_reader["fecha_inicio"],
                                FechaFin = (DateTime)_reader["fecha_fin"],
                                PIN = demo,
                                GrupoDeAcceso = 3,
                                HabilitarIDyPIN = (bool)_reader["sin_huella"],
                                Template1 = (byte[])_reader["template1"],
                                Template2 = (byte[])_reader["template2"],
                                Acceso = (bool)_reader["tiene_acceso"]
                            });
                        }
                    }
                    _command.Cancel();
                }
                catch (SqlException ex)
                {
                    string? _metodo = ex.TargetSite?.Name ?? "Desconocido";
                    string? _linea = ex.StackTrace ?? "Null";
                    error.Guardar(_metodo, _linea, ex.ToString());
                }
                return usuarios;
            }
        }


        //TODO: Consulta de Usuarios Modificados (int dispositivoID)

        /*public List<Dispositivo> ListaDispDesconectados()
        {
            dispositivos.Clear();

            string consulta = "SELECT * FROM dispositivos WHERE estado = 'desconectado' OR estado = 'nuevo'  ORDER BY serial ASC";
            try
            {
                SqlCommand command = new SqlCommand(consulta, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    long serialEqLong = (long)reader["serial"];

                    dispositivos.Add(new Dispositivo
                    {
                        Serial = (uint)serialEqLong,
                        Ip = (string)reader["ip"],
                        HayEventos = (bool)reader["hayeventos"],
                        Estado = (string)reader["estado"]
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
                string? linea = ex.StackTrace ?? "Null";
                errores.Guardar(metodo, linea, ex.Message);
                throw;
            }
            return dispositivos;
        }*/

        /*public List<Dispositivo> ListaDispHora()
        {
            dispositivos.Clear();

            string QueryConectados = "SELECT * FROM dispositivos WHERE estado = 'conectado' and seteo_hora < 1 ORDER BY serial ASC";
            try
            {
                SqlCommand command = new SqlCommand(QueryConectados, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    long serialEqLong = (long)reader["serial"];

                    dispositivos.Add(new Dispositivo
                    {
                        Serial = (uint)serialEqLong,
                        Ip = (string)reader["ip"],
                        HayEventos = (bool)reader["hayeventos"],
                        Estado = (string)reader["estado"]
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
                string? linea = ex.StackTrace ?? "Null";
                gateway.Guardar(metodo, linea, ex.Message);
                throw;
            }
            return dispositivos;
        }*/

        /*public List<Dispositivo> ListaTodosLosDisp()
        {
            
            string consulta = "SELECT * FROM dispositivos ORDER BY serial ASC";

            dispositivos.Clear();
            try
            {
                SqlCommand command = new SqlCommand(consulta, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    long serialEqLong = (long)reader["serial"];

                    dispositivos.Add(new Dispositivo
                    {
                        Serial = (uint)serialEqLong,
                        Ip = (string)reader["ip"],
                        HayEventos = (bool)reader["hayeventos"],
                        Estado = (string)reader["estado"]
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
                string? linea = ex.StackTrace ?? "Null";
                gateway.Guardar(metodo, linea, ex.Message);
                throw;
            }
            return dispositivos;
        }*/
    }
}
