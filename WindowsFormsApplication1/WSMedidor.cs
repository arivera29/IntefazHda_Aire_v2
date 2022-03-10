using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InterfazHda
{
    class WSMedidor
    {
        public String Acta { set; get; }
        public String Nic { set; get; }
        public String Medidor { set; get; }
        public bool Conforme { set; get; }

        public String Respuesta { set; get; }

        public Protocolo protocolo = null;
        public WSMedidor()
        {
            Conforme = false;
        }

        public void CallWebService()
        {
            //var _url = "http://172.198.207.1:9090/ServicioWebRecaudos/webServiceRecaudos";
            // var _action = "http://presentacion.ws.recaudos.v2/consultarRecaudo";
            var _url = @Properties.Settings.Default.url_ws;
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
                    Console.Write(soapResult);
                    Respuesta = soapResult;
                    XmlDocument docXml = new XmlDocument();
                    docXml.LoadXml(soapResult);

                    XmlNodeList nodos = docXml.GetElementsByTagName("consultarMedidoresProcesadosResponse");
                    if (nodos != null)
                    {
                        if (nodos.Count > 0)
                        {
                            AgregarLog("Respuesta del servidor: " + nodos[0]["codigoResponse"].InnerText.Trim() + " " + nodos[0]["mensajeResponse"].InnerText.Trim());
                            if (nodos[0]["codigoResponse"].InnerText.Trim().Equals("SC000"))
                            {
                                if (nodos[0]["NumeroActa"] != null)
                                {
                                    
                                        XmlNode nodo = nodos[0];

                                        if (nodo["NumeroActa"].InnerText.Trim().Equals(this.Acta.ToString()))
                                        {
                                            protocolo = new Protocolo();

                                            protocolo.Nic = nodo["Nic"].InnerText.Trim();
                                            protocolo.Nis = nodo["Nis"].InnerText.Trim();
                                            protocolo.NumeroMedidor = nodo["NumeroMedidor"].InnerText.Trim();
                                            protocolo.ResultadoExactitud = nodo["ResultadoExactitud"].InnerText.Trim();
                                            protocolo.Fecha_Res_Exactitud = nodo["Fecha_Res_Exactitud"].InnerText.Trim();
                                            protocolo.TipoEnergia = nodo["TipoEnergia"].InnerText.Trim();
                                            protocolo.ResultadoPropieDialectrica = nodo["ResultadoPropieDialectrica"].InnerText.Trim();
                                            protocolo.ResultadoArranque = nodo["ResultadoArranque"].InnerText.Trim();
                                            protocolo.ResultadoEnsayoFuncioSinCarga = nodo["ResultadoEnsayoFuncioSinCarga"].InnerText.Trim();
                                            protocolo.ResultadoInspeccionVisual = nodo["ResultadoInspeccionVisual"].InnerText.Trim();
                                            protocolo.ResultadoVerificacionConstante = nodo["ResultadoVerificacionConstante"].InnerText.Trim();
                                            protocolo.ErrorPorcentual = double.Parse(nodo["ErrorPorcentual"].InnerText.Trim().Replace(".", ","));
                                            protocolo.ErrorporcenEnEnergiaReactiva = double.Parse(nodo["ErrorporcenEnEnergiaReactiva"].InnerText.Trim().Replace(".", ","));

                                            protocolo.NumCertificado = nodo["NumCertificado"].InnerText.Trim();
                                            protocolo.CodLaboratorio = nodo["CodLaboratorio"].InnerText.Trim();

                                            protocolo.Ensayo1Activa = double.Parse(nodo["Ensayo1Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre1Activa = double.Parse(nodo["Incertidumbre1Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo2Activa = double.Parse(nodo["Ensayo2Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre2Activa = double.Parse(nodo["Incertidumbre2Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo3Activa = double.Parse(nodo["Ensayo3Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre3Activa = double.Parse(nodo["Incertidumbre3Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo4Activa = double.Parse(nodo["Ensayo4Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre4Activa = double.Parse(nodo["Incertidumbre4Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo5Activa = double.Parse(nodo["Ensayo5Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre5Activa = double.Parse(nodo["Incertidumbre5Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo6Activa = double.Parse(nodo["Ensayo6Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre6Activa = double.Parse(nodo["Incertidumbre6Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo7Activa = double.Parse(nodo["Ensayo7Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre7Activa = double.Parse(nodo["Incertidumbre7Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo8Activa = double.Parse(nodo["Ensayo8Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre8Activa = double.Parse(nodo["Incertidumbre8Activa"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo1Reactiva = double.Parse(nodo["Ensayo1Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre1Reactiva = double.Parse(nodo["Incertidumbre1Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo2Reactiva = double.Parse(nodo["Ensayo2Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre2Reactiva = double.Parse(nodo["Incertidumbre2Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo3Reactiva = double.Parse(nodo["Ensayo3Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre3Reactiva = double.Parse(nodo["Incertidumbre3Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo4Reactiva = double.Parse(nodo["Ensayo4Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre4Reactiva = double.Parse(nodo["Incertidumbre4Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo5Reactiva = double.Parse(nodo["Ensayo5Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre5Reactiva = double.Parse(nodo["Incertidumbre5Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo6Reactiva = double.Parse(nodo["Ensayo6Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre6Reactiva = double.Parse(nodo["Incertidumbre6Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Ensayo7Reactiva = double.Parse(nodo["Ensayo7Reactiva"].InnerText.Trim().Replace(".", ","));
                                            protocolo.Incertidumbre7Reactiva = double.Parse(nodo["Incertidumbre7Reactiva"].InnerText.Trim().Replace(".", ","));

                                        }

                                }
                                else
                                {
                                    AgregarLog("No hay informacion de PROTOCOLO");
                                }
                            }
                            else
                            {
                                AgregarLog("Codigo de respuesta no valido");
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
            XmlDocument soapEnvelop = new XmlDocument();



            String xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>";
            xml += @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://gnf.com.co/2015-10-07"">";
            xml += "<soapenv:Header>";
            xml += "<ns:autenticarWSHGISRequest>";
            xml += "<username>" + Properties.Settings.Default.user_ws + "</username>";
            xml += "<password>" + Properties.Settings.Default.pass_ws + "</password>";
            xml += "</ns:autenticarWSHGISRequest>";
            xml += "</soapenv:Header>";
            xml += "<soapenv:Body>";
            xml += "<ns:consultarMedidoresProcesados>";
            xml += "<consultarMedidoresProcesadosRequest>";
            xml += "<nic>" + this.Nic + "</nic>";
            xml += "<numMedidor>" + this.Medidor + "</numMedidor>";
            xml += "</consultarMedidoresProcesadosRequest>";
            xml += "</ns:consultarMedidoresProcesados>";
            xml += "</soapenv:Body>";
            xml += "</soapenv:Envelope>";

            try
            {

                //AgregarLog("Cadena Xml enviada: " + xml);
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
            String filename = Environment.CurrentDirectory + @"\LOG\INTERFAZ_HDA_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
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

