using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Biogenesis_WPF.Servicios
{
    public class Service_GestorErrores
    {
        private static readonly object lockObject = new object();

        public void Guardar(string metodoError, string lineaError, string errorEvento)
        {
            //Ruta Archivo TXT: .\repos\suprema-gsdk-ADO\suprema-gsdk-ADO\winforms-suprema\bin\Debug\net6.0-windows\logs
            string rutaArchivotxt = "./errores_del_sistema.log";

            lock (lockObject)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(rutaArchivotxt, true))
                    {
                        writer.WriteLine(DateTime.Now.ToString() + ";");
                        writer.WriteLine("Metodo: " + metodoError + ";" + "Linea: " + lineaError + ";" + "Error: " + errorEvento + ";");
                        writer.WriteLine("");
                        writer.Flush();
                        writer.Close();
                    }
                }
                catch (Exception log)
                {
                    MessageBox.Show(log.Message);
                    return;
                }
            }
        }
    }
}
