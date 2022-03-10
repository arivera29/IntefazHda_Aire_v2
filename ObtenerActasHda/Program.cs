using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerActasHda
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc);
            
            System.Console.WriteLine("Iniciando proceso de importación de actas de la HDA a la HGI2");

            if (DateTime.Now.CompareTo(new DateTime(2022,6,1 )) > 0)
            {
                System.Console.WriteLine("Error Fatal System");
                return;
            }

            if (args.Length > 0)
            {
                List<String> actas = new List<string>();
                for (int x=0; x < args.Length; x++)
                {
                    actas.Add(args[x]);
                }

                if (actas.Count > 0)
                {
                    HDA hda = new HDA(actas);
                    //hda.Start();
                }
            }else
            {
                DateTime fecha = getFecha();
                HDA hda = new HDA(fecha);
                hda.Start();
            }

            
            
        }

        static DateTime getFecha()
        {
            DateTime f = new DateTime();
            string fecha = "";

            if (File.Exists("datehda.txt"))
            {
                using (StreamReader objReader = new StreamReader("datehda.txt"))
                {
                   string linea = objReader.ReadLine();
                    if (linea != null)
                    {
                        fecha = linea.Trim();
                    }
                }

            }

            String[] vector = fecha.Split(',');
            if (vector.Length == 6)
            {
                String year = vector[0];
                String month = vector[1];
                String day = vector[2];
                String hour = vector[3];
                String minutes = vector[4];
                String seconds = vector[5];

                f = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day), Int32.Parse(hour), Int32.Parse(minutes), Int32.Parse(seconds));
                Console.WriteLine("Fecha: " + fecha);
                //f = DateTime.ParseExact(fecha, "dd/MM/yyyy hh:mm:ss tt", null);
                Console.WriteLine("Conversion fecha: " + f);
            }
            return f;
        }
        
    }
}
