using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribucionActas
{
    class Program
    {
        static void Main(string[] args)
        {

            if (DateTime.Now.CompareTo(new DateTime(2022, 6, 1)) > 0)
            {
                System.Console.WriteLine("Error Fatal System");
                return;
            }

            Datos conexion = new Datos();
            if (conexion.getConection().State == System.Data.ConnectionState.Open)
            {
                System.Console.WriteLine("Iniciando Proceso de Distribucion de actas");
                Distribuir distribuir = new Distribuir();
                distribuir.conexion = conexion;
                LOG("Actualizando delegación contratas...");
                distribuir.UpdateDelegacionContrata();
                LOG("Iniciando Proceso de Distribucion de Actas HDA");
                distribuir.ActualizarActasBrigadaElite();
                distribuir.DistribuirActas();
                LOG("Iniciando Proceso de Distribucion de Actas Manuales");
                distribuir.DistribuirActasManuales();
                LOG("Iniciando Proceso de Distribucion de Actas Subnormales");
                distribuir.DistribuirActasSubnormal();
                LOG("Iniciando Proceso de Distribucion de Actas a anticipar");
                distribuir.DistribuirLiquidacionAnticipadaActas();
                LOG("Iniciando Proceso de Distribucion Sin Anomalias");
                distribuir.DistribuirActasSinAnomalia();
                LOG("Iniciando Proceso Transferencia actas proceso PARE por cadicidad de terminos");
                distribuir.EnviarSeleccionProceso();

                LOG("Proceso finalizado");
                


                conexion.Close();
            }
            else
            {
                System.Console.WriteLine("Error al conectarse al servidor de base de datos");
            }
        }

        public static void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\DISTRIBUCION_HGI2_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

        }
    }
}
