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

namespace GestionDB
{
    public class ControlDB
    {
    //    //ModeloDispositivo Dispositivo = ModeloDispositivo.Dispositivo;

    //    public List<Dispositivo> dispositivos = new List<Dispositivo>();
    //    public List<Usuario> usuarios = new List<Usuario>();

    //    //private static string connectionString;
    //    private SqlConnection? connection;

    //    //private MainWindow? main;
    //    //private Biometricos gateway;
    //    private Service_GestorErrores error;

    //    public ControlDB()
    //    {
    //        error = new();
    //    }

        

    //    public bool Conectar()
    //    {
    //        //TODO Archivo de Configuraciones !!
    //        //config.coneccciondb
    //        //config.DBServer
    //        //config.DBName
    //        //cinfig.DBUser
    //        //config.DBPass
    //        //config.DBCertificate

    //        string stringDeConexionDB = "Server=SQ-IT-SRV\\SQLD_TEST; Database=BioTest;User ID=MaxiP;Password=123456;TrustServerCertificate=True";

    //        try
    //        {
    //            if (connection == null)
    //            {
    //                connection = new SqlConnection(stringDeConexionDB);
    //                connection.Open();
    //            }
    //            else if (connection.State == ConnectionState.Closed)
    //            {
    //                connection.Open();
    //            }
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            error.Guardar(metodo, linea, ex.Message);
    //            return false;
    //        }
    //    }
        
    //    public void CerrarConexion()
    //    {
    //        if (connection != null && connection.State == ConnectionState.Open)
    //        {
    //            connection.Close();
    //        }
    //    }

    //    public SqlDataReader? EjecutarConsulta(string query)
    //    {
    //        SqlDataReader? reader = null;
    //        var respReader = reader;
    //        Conectar();
            
    //        if (connection != null)
    //        {
    //            try
    //            {
    //                SqlCommand command1 = new SqlCommand(query, connection);

    //                reader = command1.ExecuteReader();
    //                respReader = reader;
    //                reader.Close();
    //            }
    //            catch (Exception excConsultaDB)
    //            {
    //                string? metodo = excConsultaDB.TargetSite?.Name ?? "Desconocido";
    //                string? linea = excConsultaDB.StackTrace ?? "Null";
    //                error.Guardar(metodo, linea, excConsultaDB.Message + " --- " + query);
    //            }
    //        }
    //        return respReader;
    //    }

    //    private static readonly object lockObject = new object();

    //    public void EjecutarNonQuery(string query)
    //    {
    //       Conectar();
    //        if (connection != null)
    //        {
    //            try
    //            {
    //                SqlCommand command2 = new SqlCommand(query, connection);
    //                command2.ExecuteNonQuery();
                    
    //            }
    //            catch (SqlException ex)
    //            {
    //                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //                string? linea = ex.StackTrace ?? "Null";
    //                error.Guardar(metodo, linea, ex.Message + " --- " + query);
    //                return;
    //            }
    //            catch (DbException ex)
    //            {
    //                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //                string? linea = ex.StackTrace ?? "Null";
    //                error.Guardar(metodo, linea, ex.Message + " --- " + query);
    //                return;
    //            }
    //            catch (Exception ex)
    //            {
    //                string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //                string? linea = ex.StackTrace ?? "Null";
    //                error.Guardar(metodo, linea, ex.Message + " --- " + query);
    //                return;
    //            }
    //        }
            
    //    }

    //    //TODO: Hacer el metodo con la Query para la Consulta de Alarmas (TimeSpan + Detalle de Alarma) 

    //    public void EjecutarGuardadoLog(string query, int numeroConexion) //TODO: Debe pasar los datos por parametro => Serial - Fecha - HayEventos - Sub HayEventos - Usuario (null o no) - Detalle ... etc.
    //    {           
    //            Conectar();
          
    //        string guardaLog = "";
    //        try
    //        {
    //            //SqlCommand command = new SqlCommand(query, connection);
    //            //command.Parameters.AddWithValue("@equipo_id", equipo_id);
    //            //command.Parameters.AddWithValue("@fecha", fecha);
    //            //command.Parameters.AddWithValue("@codigo_evento", codigo_evento);
    //            ////TODO Agregar Sub HayEventos - Sub Codigo
    //            //command.Parameters.AddWithValue("@id_usuario", id_usuario);
    //            //command.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            error.Guardar(metodo, linea, ex.Message);
    //            return;
    //        }
    //    }

    //    public List<Dispositivo> ListaDispositivos()
    //    {
    //        Conectar();
           
    //        dispositivos.Clear();

    //        string Query = "SELECT * FROM dispositivos ORDER BY serial ASC"; //TODO Todas las Query deben ser desde la solicitud y no desde la funcion (armar la Query por afuera)
    //        try
    //        {
    //            SqlCommand command = new SqlCommand(Query, connection);
    //            SqlDataReader reader = command.ExecuteReader();

    //            while (reader.Read())
    //            {
    //                long serialEqLong = (long)reader["serial"];

    //                dispositivos.Add(new Dispositivo
    //                {
    //                    Serial = (uint)serialEqLong,
    //                    Ip = (string)reader["ip"],
    //                    HayEventos = (bool)reader["hayeventos"],
    //                    Estado = (string)reader["estado"],
    //                    UltimoLogFecha = (DateTime)reader["ultimo_evento"],
    //                    SeteoHora = (int)reader["seteo_hora"]

    //                    //TODO: Agregar todo el detalle que tienen los dispositivos para el armado completo de la lista
    //                });
    //            }
    //            reader.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            error.Guardar(metodo, linea, ex.Message);
    //            throw;
    //        }
    //        return dispositivos;
    //    }

    //    //TODO: Consulta de Usuarios Modificados (int dispositivoID)

    //    /*public List<Dispositivo> ListaDispDesconectados()
    //    {
    //        dispositivos.Clear();

    //        string consulta = "SELECT * FROM dispositivos WHERE estado = 'desconectado' OR estado = 'nuevo'  ORDER BY serial ASC";
    //        try
    //        {
    //            SqlCommand command = new SqlCommand(consulta, connection);
    //            SqlDataReader reader = command.ExecuteReader();

    //            while (reader.Read())
    //            {
    //                long serialEqLong = (long)reader["serial"];

    //                dispositivos.Add(new Dispositivo
    //                {
    //                    Serial = (uint)serialEqLong,
    //                    Ip = (string)reader["ip"],
    //                    HayEventos = (bool)reader["hayeventos"],
    //                    Estado = (string)reader["estado"]
    //                });
    //            }
    //            reader.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            errores.Guardar(metodo, linea, ex.Message);
    //            throw;
    //        }
    //        return dispositivos;
    //    }*/

    //    /*public List<Dispositivo> ListaDispHora()
    //    {
    //        dispositivos.Clear();

    //        string QueryConectados = "SELECT * FROM dispositivos WHERE estado = 'conectado' and seteo_hora < 1 ORDER BY serial ASC";
    //        try
    //        {
    //            SqlCommand command = new SqlCommand(QueryConectados, connection);
    //            SqlDataReader reader = command.ExecuteReader();

    //            while (reader.Read())
    //            {

    //                long serialEqLong = (long)reader["serial"];

    //                dispositivos.Add(new Dispositivo
    //                {
    //                    Serial = (uint)serialEqLong,
    //                    Ip = (string)reader["ip"],
    //                    HayEventos = (bool)reader["hayeventos"],
    //                    Estado = (string)reader["estado"]
    //                });
    //            }
    //            reader.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            gateway.Guardar(metodo, linea, ex.Message);
    //            throw;
    //        }
    //        return dispositivos;
    //    }*/

    //    /*public List<Dispositivo> ListaTodosLosDisp()
    //    {
            
    //        string consulta = "SELECT * FROM dispositivos ORDER BY serial ASC";

    //        dispositivos.Clear();
    //        try
    //        {
    //            SqlCommand command = new SqlCommand(consulta, connection);
    //            SqlDataReader reader = command.ExecuteReader();

    //            while (reader.Read())
    //            {
    //                long serialEqLong = (long)reader["serial"];

    //                dispositivos.Add(new Dispositivo
    //                {
    //                    Serial = (uint)serialEqLong,
    //                    Ip = (string)reader["ip"],
    //                    HayEventos = (bool)reader["hayeventos"],
    //                    Estado = (string)reader["estado"]
    //                });
    //            }
    //            reader.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            string? metodo = ex.TargetSite?.Name ?? "Desconocido";
    //            string? linea = ex.StackTrace ?? "Null";
    //            gateway.Guardar(metodo, linea, ex.Message);
    //            throw;
    //        }
    //        return dispositivos;
    //    }*/
    //}

    //public class Dispositivo : INotifyPropertyChanged
    //{
    //    public Dispositivo()
    //    {
    //        Serial = 0;
    //        Ip = string.Empty;
    //        HayEventos = (bool)false;
    //        Estado = string.Empty;
    //        UltimoLogFecha = DateTime.UnixEpoch;
    //        SeteoHora = 0;
    //        Activa = false;
    //    }

    //    public uint Serial { get; set; }

    //    public string Ip { get; set; }

    //    public bool HayEventos { get; set; }

    //    private string _estado;
    //    public string Estado
    //    {
    //        get { return _estado; }
    //        set
    //        {
    //            if (_estado != value)
    //            {
    //                _estado = value;
    //                OnPropertyChanged(nameof(Estado));
    //            }
    //        }
    //    }

    //    public DateTime UltimoLogFecha { get; set; }

    //    public int SeteoHora { get; set; }

    //    private bool _activa;

    //    public bool Activa
    //    {
    //        get { return _activa; }
    //        set
    //        {
    //            if (_activa != value)
    //            {
    //                _activa = value;
    //                OnPropertyChanged(nameof(Activa));
    //            }
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}

    //public class Usuario
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public DateTime FechaInicio { get; set; }
    //    public DateTime FechaFin { get; set; }
    //    public string Password { get; set; }
    //    public DateTime Actualizacion { get; set; }
    //    public bool Activo { get; set; }
    }

}
