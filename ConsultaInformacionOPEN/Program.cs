using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaInformacionOPEN
{
    class Program
    {
        static void Main(string[] args)
        {
            Datos conexion = new Datos();
            if (conexion.getConection().State == System.Data.ConnectionState.Open)
            {
                Consultar consulta = new Consultar(conexion);
                consulta.Start();
                conexion.Close();
            }
            else
            {
                Console.WriteLine("Error al conectarse con el servidor");
            }
        }
    }
}
