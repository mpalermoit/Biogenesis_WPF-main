using GestionDB;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Gsdk.Err;
using System.Windows.Controls;
//using Biogenesis_WPF.Vistas;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;

namespace Biogenesis_WPF
{
    //public class ProcesamientoLogs
    //{
    //    Service_GestorErrores error = new();
    //    Modelo_Barra barraLogs = Modelo_Barra.BarraLogs;
       
    //    // Creacion de Thread de Logs
    //    private bool logsIniciados = false;
    //    private bool detenerLogs = false;

    //    //Variable de confirmacion de Cierre de Thread
    //    public CancellationTokenSource detenerThreadLogs = new CancellationTokenSource();

    //    Dispatcher logsDispatcher = Application.Current.Dispatcher;   //Gestion entre Hilos y la interfaz en Organizador de Logs
    //    ManualResetEvent pauseLogs = new ManualResetEvent(true);   //Controla los tiempos de pausa del hilo en Organizador de Logs

    //    public void ProcesarLogs()
    //    {
    //        logsIniciados = true;
    //        ControlDB controlDB = new ();

    //        DateTime inicioTiempoTranscurrido;
    //        int demora = 0;
    //        TimeSpan tiempotranscurrido = TimeSpan.Zero;

    //        barraLogs.valor = 0;
    //        barraLogs.texto1 = "Inicio Proceso logs";
    //        logsIniciados = true;
    //        int i = 0;

    //        while (!detenerLogs && !detenerThreadLogs.Token.IsCancellationRequested)
    //        {
    //            inicioTiempoTranscurrido = DateTime.Now;

    //            i++;
    //            barraLogs.valor = i;
    //            barraLogs.texto1 = "Buscando dispositivos";

    //            controlDB.EjecutarConsulta("SELECT * FROM dispositivos ORDER BY serial ASC");

    //            if (i>= 100)
    //            {
    //                i = 0;
    //            }
    //            barraLogs.texto1 = "Modificando dispositivos";

    //            pauseLogs.Reset();
    //            pauseLogs.WaitOne(1000);

    //            controlDB.EjecutarNonQuery("UPDATE dispositivos SET hayeventos = 1");

    //            pauseLogs.Reset();
    //            pauseLogs.WaitOne(1000);

    //            tiempotranscurrido = DateTime.Now - inicioTiempoTranscurrido;
    //            barraLogs.texto2 = ((int)tiempotranscurrido.TotalSeconds).ToString();
    //        }

    //        //MostrarEstadoLogsActual("Logs Detenidos");
    //        controlDB.CerrarConexion();
    //        pauseLogs.Reset();
    //        pauseLogs.WaitOne(10000);
    //        detenerThreadLogs.Cancel(true);
    //    }
       
    //}
}
