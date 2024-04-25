using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Log
    {
        public Modelo_Log()
        {
            Serial = 0;
            Detalle = string.Empty;
        }

        public uint Serial { get; set; }
        public DateTime TiempoLog { get; set; }
        public string? Detalle { get; set; }

    }
}
