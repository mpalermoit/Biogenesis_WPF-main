using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biogenesis_WPF.Modelos
{
    public class Modelo_Usuario
    {
        public Modelo_Usuario() 
        {
            Id = string.Empty;
            Nombre = string.Empty;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public ByteString? PIN { get; set; }
        public DateTime? Actualizacion { get; set; }
        public bool Activo { get; set; }
        public int GrupoDeAcceso { get; set; }
        public bool HabilitarIDyPIN { get; set; }
        public byte[]? Template1 { get; set; }
        public byte[]? Template2 { get; set; }
        public bool Acceso { get; set; }
    }
}
