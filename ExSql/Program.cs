using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExSql
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Archivo de consulta no especificado");
                return;
            }

            string archivo = args[0];

            if (!File.Exists(archivo))
            {
                Console.WriteLine("Archivo de consulta no existe.");
                return;
            }

            Console.WriteLine("Conectansose a la base de datos.");
            Datos conexion = new Datos();
            if (conexion.getConection().State == System.Data.ConnectionState.Open)
            {

                using (StreamReader objReader = new StreamReader(archivo))
                {
                    String sLine = "";
                    long linea = 0;
                    while (sLine != null)
                    {
                        linea++;
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                        {
                            try
                            {
                                Console.WriteLine("Leyendo SQL: " + sLine);
                                if (conexion.ExecuteNonQuery(sLine, false))
                                {
                                    Console.WriteLine("Query OK");
                                }
                                else
                                {
                                    Console.WriteLine("Query FAIL");
                                }
                            }
                            catch (SqlException ex) {
                                Console.WriteLine("Error: " + ex.Message);
                                Console.WriteLine("Linea Error: " + linea);
                            }
                        }
                        

                    }
                }


                Console.WriteLine("Cerrando conexión con el servidor.");
                conexion.Close();
            }
            else
            {
                Console.WriteLine("Conexion NO establecida con el servidor.");
                
            }

        }
    }
}
