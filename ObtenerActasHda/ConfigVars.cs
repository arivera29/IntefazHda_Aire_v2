using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerActasHda
{
    class ConfigVars
    {

        public static String UrlWsHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["url_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }

        public static String UserWsHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["user_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }
        public static String PasswordWsHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["pass_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }
        public static String UrlWsDocHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["url_doc_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }
        public static String UrlWsFotosHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["url_fotos_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }
        public static String UrlWsFirmasHda()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hda"]["url_firmas_ws_hda"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            return parametro;
        }
        public static String UrlWsOpen()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["open"]["url_ws_open"];
                
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }

            

            return parametro;
        }

        public static String UserWsOpen()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["open"]["user_ws_open"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }



            return parametro;
        }

        public static String PasswordWsOpen()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["open"]["pass_ws_open"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }



            return parametro;
        }

        public static String FolderImagenesHGI2()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hgi"]["folder_fotos"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }



            return parametro;
        }
        public static String UrlConexionBdHGI2()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hgi"]["conexion"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }



            return parametro;
        }

        public static String RutaVirtualImagenesHGI2()
        {
            string parametro = "";
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                parametro = data["hgi"]["ruta_virtual_fotos"];

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error " + e.Message);
            }



            return parametro;
        }
    }
}
