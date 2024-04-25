using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biogenesis_WPF.Controladores;
using Biogenesis_WPF.Modelos;
using Biogenesis_WPF.Servicios;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biogenesis_WPF.VistaModelo
{
    public class ViewModel_Eventos
    {
        private static ViewModel_Eventos instanciaEventos;

        // Modelos
        public Modelo_Evento evento = new();
        public Modelo_Barra barraEventos = new();
        public Modelo_Barra buffer = new();
        public Modelo_Texto textoEstadoProceso = new();
        public Modelo_Texto textoTiempoProceso = new();

        public ViewModel_Eventos()
        {
            barraEventos.valor = 0;
        }
        public static ViewModel_Eventos viewModelEventos
        {
            get
            {
                if (instanciaEventos == null)
                {
                    instanciaEventos = new ViewModel_Eventos();
                }
                return instanciaEventos;
            }
        }
    }
}
