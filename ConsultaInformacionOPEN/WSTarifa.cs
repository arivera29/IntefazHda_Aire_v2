using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsultaInformacionOPEN
{
    class WSTarifa
    {
        public String nic { set; get; }
        public String fecha { set; get; }
        public String Respuesta { set; get; }
        public InformacionTarifa Tarifa = null;

        public void CallWebService()
        {
            //var _url = "http://172.198.207.1:9090/ServicioWebRecaudos/webServiceRecaudos";
            // var _action = "http://presentacion.ws.recaudos.v2/consultarRecaudo";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            var _url = ConfigVars.UrlWsOpen();
            var _action = "";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            try
            {
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                    //Console.Write(soapResult);
                    Respuesta = soapResult;
                    XmlDocument docXml = new XmlDocument();
                    docXml.LoadXml(soapResult);

                    XmlNodeList nodos = docXml.GetElementsByTagName("consultarUltSubsidiosConsumosResponse");
                    if (nodos != null)
                    {
                        if (nodos.Count > 0)
                        {
                                XmlNode nodo = nodos[0];
                                if (nodo["codigoResponse"].InnerText.Trim().Equals("SC000"))
                                {
                                    Tarifa = new InformacionTarifa();
                                    Tarifa.Codigo = nodo["codTarifa"].InnerText.Trim();
                                    Tarifa.Descripcion = nodo["descTarifa"].InnerText.Trim();
                                    Tarifa.CargaContratada = nodo["cargaContratada"].InnerText.Trim();
                                    Tarifa.ValorTarifa = nodo["cuTarifa"].InnerText.Trim();
                                }
                                else
                                {
                                    AgregarLog("Error: " + Respuesta);
                                }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                AgregarLog(DateTime.Now.ToString() + "-> Error: " + e.Message);
            }
        }

        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Headers.Add("SOAPAction", action);
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
            }
            catch (Exception ex)
            {
                AgregarLog("Error: " + ex.Message);
            }
            return webRequest;
        }

        private XmlDocument CreateSoapEnvelope()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            XmlDocument soapEnvelop = new XmlDocument();
            String xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>";
            xml += @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://gnf.com.co/2015-10-07"">";
            xml += "<soapenv:Header>";
            xml += "<ns:autenticarWSHGISRequest>";
            xml += "<username>" + ConfigVars.UserWsOpen() +"</username>";
            xml += "<password>" + ConfigVars.PasswordWsOpen() + "</password>";
            xml += "</ns:autenticarWSHGISRequest>";
            xml += "</soapenv:Header>";
            xml += "<soapenv:Body>";
            xml += "<ns:consultarTarifaCUContratada>";
            xml += "<consultarTarifaCUContratadaRequest>";
            xml += "<nic>" + nic + "</nic>";
            xml += "<fechaActa>" + fecha + "</fechaActa>";
            xml += "</consultarTarifaCUContratadaRequest>";
            xml += "</ns:consultarTarifaCUContratada>";
            xml += "</soapenv:Body>";
            xml += "</soapenv:Envelope>";

            try
            {

                //AgregarLog("Cadena Enviada: " + xml);
                soapEnvelop.LoadXml(xml);
            }
            catch (Exception ex)
            {
                AgregarLog("Error: " + ex.Message);
            }

            return soapEnvelop;
        }

        private void AgregarLog(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\CONSULTA_WS_OPEN_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            try
            {
                using (Stream stream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }
            }
            catch (Exception ex)
            {
                AgregarLog("Error: " + ex.Message);
            }
        }

        public class InformacionTarifa
        {
            public String Codigo { set; get; }
            public String Descripcion { set; get; }
            public String CargaContratada { set; get; }
            public String ValorTarifa { set; get; }

            public InformacionTarifa()
            {
                ValorTarifa = "0";
            }
        }

    }
}
