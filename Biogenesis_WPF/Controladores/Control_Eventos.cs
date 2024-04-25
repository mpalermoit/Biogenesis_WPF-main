using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Biogenesis_WPF.biostar.services;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;
using Biogenesis_WPF.VistaModelo;
using Gsdk.Event;
using System.Xml;
using System.Drawing;
using System.Windows.Media;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Eventing.Reader;
using System.Collections.ObjectModel;


namespace Biogenesis_WPF.Controladores
{
    public class Control_Eventos 
    {

        ViewModel_Eventos vistaEventos = ViewModel_Eventos.viewModelEventos;


        // new barra 
        //Modelo_Barra buffer = new Modelo_Barra(); //




        //Servicios
        private Service_ControlDB? controlDB;
        private Service_Gateway? gateway;
        private Service_GestorErrores? errores;


        //Control de Threads
        Dispatcher uiDispatcher = Application.Current.Dispatcher;   //Gestion necesaria entre los Hilos y la interfaz de Proceso Rutina
        ManualResetEvent pausar = new ManualResetEvent(true);       //Controla los tiempos de pausa de hilo en Proceso Rutina

        //Variable Control de Threads
        public CancellationTokenSource detenerThread = new CancellationTokenSource();
        private bool procesoIniciado = false;
        private bool procesoDetenido = false;

        // Variables
        int i = 0;


        //int j = 0; //


        public void ProcesarEventos()
        {
            
            controlDB = new();
            gateway = new();
            errores = new();

            controlDB.Conectar();
            procesoIniciado = true;
            vistaEventos.textoEstadoProceso.texto = "Proceso Iniciado";
            vistaEventos.barraEventos.valor = i;


            //vistaEventos.buffer.valor = j; //


            while (procesoIniciado)
             {
                if (i >= 5)
                {
                    i = 0;


                   // j++; //aumenta segunda barra 
                    
                }
                i++;
              
                vistaEventos.barraEventos.valor = i;


                //vistaEventos.buffer.valor = j; //


                pausar.Reset();
                //pausar.WaitOne(2000);
           
                pausar.Reset();
                pausar.WaitOne(1000);

                //string _query = "SELECT * FROM logs";
                uint disp = controlDB.ListaLogs();

                 vistaEventos.textoTiempoProceso.texto = disp.ToString();
            }

            procesoDetenido = true;
            procesoIniciado = false;
        }

        public void DetenerRutina()
        {
            procesoIniciado = false;
            vistaEventos.textoEstadoProceso.texto = "Deteniendo Proceso..";
            detenerThread.Cancel(true);
        }
    }
}
