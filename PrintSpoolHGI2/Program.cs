using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrintSpoolHGI2
{
    class Program
    {
        public static ILog log { get; set; }

        static void Main(string[] args)
        {
            log = LogManager.GetLogger(Assembly.GetExecutingAssembly().GetTypes().First());
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Iniciando generación Spool de impresion de la HGI2");



        }
    }
}
