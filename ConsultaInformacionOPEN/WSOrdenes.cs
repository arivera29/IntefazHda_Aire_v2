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
    class WSOrdenes
    {
        public String Nic { set; get; }
        public String OrdenServicio { set; get; }
        public String fecha { set; get; }
        public Boolean Resuelta = false;
        public String Respuesta { set; get; }

        public void CallWebService()
        {
            //var _url = "http://172.198.207.1:9090/ServicioWebRecaudos/webServiceRecaudos";
            // var _action = "http://presentacion.ws.recaudos.v2/consultarRecaudo";
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
                    //AgregarLog("Respuesta WS: " + Respuesta);
                    XmlDocument docXml = new XmlDocument();
                    docXml.LoadXml(soapResult);

                    XmlNodeList nodos = docXml.GetElementsByTagName("infoOrdenes");
                    if (nodos != null)
                    {
                        if (nodos.Count > 0)
                        {
                            for (int x = 0; x < nodos.Count; x++)
                            {
                                XmlNode nodo = nodos[x];
                                
                                if (nodo["numOrden"].InnerText.Trim().Equals(this.OrdenServicio))
                                {
                                    if (nodo["estado"].InnerText.Trim().Equals("Resuelta"))
                                    {
                                        this.Resuelta = true;
                                        break;
                                    }
                                }
                                
                            }

                        }

                    }
                    else
                    {
                        AgregarLog("Error Xml: Respuesta " + Respuesta);
                    }
                        
                }
            }
            catch (Exception e)
            {
                AgregarLog("Error: " + e.Message);
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
            XmlDocument soapEnvelop = new XmlDocument();
            String strFecha = this.fecha;

            String xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>";
            xml += @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://gnf.com.co/2015-10-07"">";
            xml += "<soapenv:Header>";
            xml += "<ns:autenticarWSHGISRequest>";
            xml += "<username>" + ConfigVars.UserWsOpen() + "</username>";
            xml += "<password>" + ConfigVars.PasswordWsOpen() +"</password>";
            xml += "</ns:autenticarWSHGISRequest>";
            xml += "</soapenv:Header>";
            xml += "<soapenv:Body>";
            xml += "<ns:consultarUltimasInfoOS>";
            xml += "<consultarUltimasInfoOSRequest>";
            xml += "<nic>" + this.Nic + "</nic>";
            //xml += "<fechaActa>" + this.fecha + "</fechaActa>";
            xml += "<fechaActa>" + strFecha + "</fechaActa>";
            xml += "</consultarUltimasInfoOSRequest>";
            xml += "</ns:consultarUltimasInfoOS>";

            xml += "</soapenv:Body>";
            xml += "</soapenv:Envelope>";

            try
            {

                //AgregarLog("Xml enviado: " + xml);
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
    }
}
