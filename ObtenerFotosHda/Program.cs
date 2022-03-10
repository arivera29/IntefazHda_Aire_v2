using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerFotosHda
{
    class Program
    {
        private static bool debug = true;
        static void Main(string[] args)
        {
            
            
            if (args.Length > 0)
            {
                Console.WriteLine("Obtener fotos de parametros");

                List<int> Actas = new List<int>();
                String filename = "";
                for (int x = 0; x < args.Length; x++)
                {
                    if (args[x].Equals("-f"))
                    {
                        filename = args[x + 1];
                    }

                    if (args[x].Equals("--debug-disabled"))
                    {
                        Console.WriteLine("Debug enabled = false");
                        debug = false;
                    }
                }

                Console.WriteLine("Debug enabled = true");
                

                if (filename.Equals(""))
                {

                    for (int x = 0; x < args.Length; x++)
                    {
                        LOG("Procesando acta " + args[x]);
                        Actas.Add(Int32.Parse(args[x]));
                        
                    }
                    if (Actas.Count > 0)
                    {
                        ObtenerFotos proceso = new ObtenerFotos();
                        proceso.debug = debug;
                        proceso.Actas = Actas;
                        proceso.Start();
                    }
                }
                else
                {
                    Console.WriteLine("Leyendo archivo " + filename);
                    using (StreamReader objReader = new StreamReader(filename))
                    {
                        String sLine = "";
                        while (sLine != null)
                        {
                            sLine = objReader.ReadLine();
                            if (sLine != null) {
                                Console.WriteLine("Leyendo acta " + sLine.Trim());
                                Actas.Add(Int32.Parse(sLine.Trim()));
                            }
                                
                        }

                        if (Actas.Count > 0)
                        {
                            Console.WriteLine("Iniciando proceso de transferencia de actas. Total: " + Actas.Count);
                            ObtenerFotos proceso = new ObtenerFotos();
                            proceso.debug = debug;
                            proceso.Actas = Actas;
                            proceso.Start();
                        }
                    }

                }
            }
            else
            {

                Datos conexion = new Datos();
                if (conexion.getConection().State == System.Data.ConnectionState.Open)
                {
                    List<int> Actas = new List<int>();
                    String sql = "SELECT NroActa FROM SolicitudesFotos WHERE EstadoSolicitud=1";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Actas.Add(reader.GetInt32(0));
                            }
                        }

                    }


                    if (Actas.Count > 0)
                    {
                        ObtenerFotos proceso = new ObtenerFotos();
                        proceso.Actas = Actas;
                        proceso.Start();
                    }
                    else
                    {
                        LOG("No hay actas pendientes de foto");
                    }

                    conexion.Close();

                }
                else
                {
                    LOG("Error al conectarse con el servidor de base de datos");
                }
            }
        }

        public static void LOG(string log)
        {
            if (debug)
            {
                try
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
                    {
                        Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
                    }

                    string fecha = DateTime.Now.ToString();
                    String filename = Environment.CurrentDirectory + @"\LOG\OBTENER_FOTOS_HDA_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    String cadena = fecha + " " + log + "\r\n";
                    using (StreamWriter outfile = new StreamWriter(@filename, true))
                    {
                        outfile.Write(cadena);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al escribir en el archivo LOG: " + ex.Message);
                }
            }
        }
    }
}
