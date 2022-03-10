using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerFotosHda
{
    class ObtenerFotos
    {
        public List<int> Actas { set; get; }
        public bool debug { set; get; }
        List<Foto> lista;
        Datos conexion;

        public ObtenerFotos()
        {
            debug = true;
        }

        public void Start()
        {
            if (Actas.Count > 0)
            {
                for (int i = 0; i < Actas.Count; i++)
                {
                    Console.WriteLine("Consulta información del acta en la HDA. " + Actas[i].ToString());
                    try
                    {
                        //Get method
                        string url = ConfigVars.UrlWsHda() + "/byNumber/" + Actas[i].ToString();
                        WebRequest req = WebRequest.Create(url);
                        LOG("Procesando url: " + url);
                        req.Method = "GET";
                        req.ContentType = "application/json; charset=utf-8";

                        string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigVars.UserWsHda() + ":" + ConfigVars.PasswordWsHda()));
                        req.Headers.Add("Authorization", "Basic " + credentials);

                        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream respStream = resp.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                                StringBuilder sb = new StringBuilder(reader.ReadToEnd());
                                //Console.WriteLine(sb.ToString());
                                ProcesarActa(sb, Actas[i]);
                            }

                            resp.Close();

                        }
                        else
                        {
                            LOG(string.Format("Status Code: {0}, Status Description: {1}", resp.StatusCode, resp.StatusDescription));

                        }
                    }
                    catch (Exception ex)
                    {
                        LOG("Error: " + ex.Message);
                        //EnviarAlertaCorreo("Alerta Interfaz HDA", ex.Message);
                    }


                }
            }
        }


        private void ProcesarActa(StringBuilder sb, int acta)
        {
            try
            {
                lista = new List<Foto>();
                JObject o = JObject.Parse(sb.ToString());

                if (isJArray(o["aparatosExistentes"]))
                {
                    JArray Aparatos = (JArray)o["aparatosExistentes"];

                    for (int x = 0; x < Aparatos.Count; x++)
                    {
                        if (isJArray(Aparatos[x]["fotosAparato"]))
                        {
                            JArray fotosAparato = (JArray)Aparatos[x]["fotosAparato"];
                            for (int y = 0; y < fotosAparato.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(fotosAparato[y]["id"]);
                                foto.Url = ConvertString(fotosAparato[y]["__URL__"]);
                                foto.Tipo = 3;  // 3-Foto Aparato
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                        if (isJArray(Aparatos[x]["fotos_mec"]))
                        {
                            JArray fotos_mec = (JArray)Aparatos[x]["fotos_mec"];
                            for (int y = 0; y < fotos_mec.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(fotos_mec[y]["id"]);
                                foto.Url = ConvertString(fotos_mec[y]["__URL__"]);
                                foto.Tipo = 4;  // 4-Foto Mediciones
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                        if (isJArray(Aparatos[x]["fotosPruebaAlta"]))
                        {
                            JArray fotosPruebaAlta = (JArray)Aparatos[x]["fotosPruebaAlta"];
                            for (int y = 0; y < fotosPruebaAlta.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(fotosPruebaAlta[y]["id"]);
                                foto.Url = ConvertString(fotosPruebaAlta[y]["__URL__"]);
                                foto.Tipo = 5;  // 5-Foto Prueba Alta
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                        if (isJArray(Aparatos[x]["fotosPruebaBaja"]))
                        {
                            JArray fotosPruebaBaja = (JArray)Aparatos[x]["fotosPruebaBaja"];
                            for (int y = 0; y < fotosPruebaBaja.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(fotosPruebaBaja[y]["id"]);
                                foto.Url = ConvertString(fotosPruebaBaja[y]["__URL__"]);
                                foto.Tipo = 6;  // 6-Foto Prueba Baja
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                        if (isJArray(Aparatos[x]["fotosPruebaDosificacion"]))
                        {
                            JArray fotosPruebaDosificacion = (JArray)Aparatos[x]["fotosPruebaDosificacion"];
                            for (int y = 0; y < fotosPruebaDosificacion.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(fotosPruebaDosificacion[y]["id"]);
                                foto.Url = ConvertString(fotosPruebaDosificacion[y]["__URL__"]);
                                foto.Tipo = 7;  // 7-Foto Prueba Dosificacion
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                        if (isJArray(Aparatos[x]["envioLabFotos"]))
                        {
                            JArray envioLabFotos = (JArray)Aparatos[x]["envioLabFotos"];
                            for (int y = 0; y < envioLabFotos.Count; y++)
                            {
                                Foto foto = new Foto();
                                foto.Id = ConvertString(envioLabFotos[y]["id"]);
                                foto.Url = ConvertString(envioLabFotos[y]["__URL__"]);
                                foto.Tipo = 8;  // 8-Foto Envio Laboratorio
                                foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                                lista.Add(foto);
                            }
                        }

                    }



                }

                if (isJArray(o["fotoFachada"]))
                {
                    JArray fotoFachada = (JArray)o["fotoFachada"];
                    for (int x = 0; x < fotoFachada.Count; x++)
                    {
                        Foto foto = new Foto();
                        foto.Id = ConvertString(fotoFachada[x]["id"]);
                        foto.Url = ConvertString(fotoFachada[x]["__URL__"]);
                        foto.Tipo = 1;  // 1-Foto Fachada
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                        lista.Add(foto);
                    }
                }


                if (isJArray(o["fotosAnomalias"]))
                {
                    JArray fotoAnomalia = (JArray)o["fotosAnomalias"];
                    for (int x = 0; x < fotoAnomalia.Count; x++)
                    {
                        Foto foto = new Foto();
                        foto.Id = ConvertString(fotoAnomalia[x]["id"]);
                        foto.Url = ConvertString(fotoAnomalia[x]["__URL__"]);
                        foto.Tipo = 2;  // 2-Foto Anomalia
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                        lista.Add(foto);
                    }
                }

                if (isJArray(o["fotosResguardo"]))
                {
                    JArray fotosResguardo = (JArray)o["fotosResguardo"];
                    for (int x = 0; x < fotosResguardo.Count; x++)
                    {
                        Foto foto = new Foto();
                        foto.Id = ConvertString(fotosResguardo[x]["id"]);
                        foto.Url = ConvertString(fotosResguardo[x]["__URL__"]);
                        foto.Tipo = 19;  // 19-Foto Resguardo
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
                        lista.Add(foto);
                    }
                }

                if (!isNull(o["firmaReceptorVisita"]))
                {
                    String url = ConvertString(o["firmaReceptorVisita"]["__URL__"]);
                    Foto foto = new Foto();
                    foto.Id = "firmaTecnicoParticular";
                    foto.Url = url;
                    foto.Tipo = 9;  // 9 - Foto Firma Receptor Visita
                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + "_" +  acta +  ".jpg";
                    foto.Firma = 1;
                    lista.Add(foto);
                }

                if (!isNull(o["firmaOperario"]))
                {
                    String url = ConvertString(o["firmaOperario"]["__URL__"]);
                    Foto foto = new Foto();
                    foto.Id = "firmaOperario";
                    foto.Url = url;
                    foto.Tipo = 10;  // 10 - Foto Firma Operario
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta + "_" +  foto.Id.Trim()  + ".jpg";
                    foto.Firma = 1;
                    lista.Add(foto);
                }

                if (!isNull(o["firmaTestigo"]))
                {
                    String url = ConvertString(o["firmaTestigo"]["__URL__"]);
                    Foto foto = new Foto();
                    foto.Id = "firmaTestigo";
                    foto.Url = url;
                    foto.Tipo = 11;  // 11 - Foto Firma Testigo
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta + "_" + foto.Id.Trim() + ".jpg";
                    foto.Firma = 1;
                    lista.Add(foto);
                }

                if (!isNull(o["firmaTecnicoParticular"]))
                {
                    String url = ConvertString(o["firmaTecnicoParticular"]["__URL__"]);
                    Foto foto = new Foto();
                    foto.Id = "firmaTecnicoParticular";
                    foto.Url = url;
                    foto.Tipo = 12;  // 12 - Foto Firma Tecnico Particular
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta + "_" + foto.Id.Trim() + ".jpg";
                    foto.Firma = 1;
                    lista.Add(foto);
                }

                foreach (Foto foto in lista)
                {
                    LOG(foto.Url);
                }
                conexion = new Datos();
                if (conexion != null)
                {

                    RegistrarFotos(lista, acta);

                    String sql = "UPDATE SolicitudesFotos SET EstadoSolicitud = 2 WHERE NroActa = @acta";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();

                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            LOG("Fotos actualizadas correctamente. Acta " + acta);
                        }
                        else
                        {
                            LOG("Error al guardar el registro de la Firma en el gestor documental");
                        }

                    }

                    conexion.Close();

                }
                else
                {
                    LOG("Error al conectarse con la base de datos");
                }


            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Trace: " + ex.StackTrace.ToString());
            }
        }

        private void RegistrarFotos(List<Foto> fotos, int _number)
        {
            try
            {

                if (fotos.Count > 0)
                {
                    System.Console.WriteLine("Eliminado fotos del acta " + _number);
                    String SQL = "DELETE FROM Documentos WHERE DocuActa=@orden and DocuTiDo in (3,4,5,6,7,8,10,11,12,9,19,2,1)";
                    using (SqlCommand cmd = new SqlCommand(SQL))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@orden", SqlDbType.Int, 11).Value = _number;
                        cmd.ExecuteNonQuery();

                    }

                }

                String sql = "INSERT INTO Documentos (DocuActa,DocuTiDo,DocuUrRe,DocuUsCa,DocuFeCa,DocuUrLo,DocuSincro,DocuVeri,DocuUsVe,DocuFeVe,DocuPath,DocuSize, DocuSAWS, DocuIAWS) "
                    + "VALUES (@orden,@tipo,@url_remoto,'interfaz',SYSDATETIME(),@url_local,1,0,'',NULL,@path, @size,@state_aws, @id_aws)";

                foreach (Foto foto in fotos)
                {
                    if (foto.Firma == 0)
                    {
                        //if (this.RecuperarArchivoFoto(foto.Id, foto.Url))
                        //{
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                if (File.Exists(foto.Path))
                                {
                                    FileInfo f = new FileInfo(foto.Path);
                                    foto.Size = f.Length;
                                }

                                cmd.Parameters.Add("@orden", SqlDbType.Int, 11).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                            //cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = ConfigVars.RutaVirtualImagenesHGI2() + foto.Id.Trim() + ".jpg";
                            cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "";
                                //cmd.Parameters.Add("@path", SqlDbType.VarChar, 200).Value =foto.Path;
                                cmd.Parameters.Add("@path", SqlDbType.VarChar, 200).Value = "";
                                cmd.Parameters.Add("@size", SqlDbType.BigInt, 11).Value = foto.Size;
                                cmd.Parameters.Add("@state_aws", SqlDbType.Int, 11).Value = 1;
                                cmd.Parameters.Add("@id_aws", SqlDbType.VarChar, 200).Value = foto.Id;


                            if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                    System.Console.WriteLine("Registro de foto  " + foto.Path + " del acta " + _number + "Guardado correctamente ");
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Foto en el gestor documental");
                                }

                            }
                        //}
                    }
                    else
                    {
                        //if (this.RecuperarArchivoFirma(_number, foto.Id, foto.Url))
                        //{
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                if (File.Exists(foto.Path))
                                {
                                    FileInfo f = new FileInfo(foto.Path);
                                    foto.Size = f.Length;
                                }

                                cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                                //cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = ConfigVars.RutaVirtualImagenesHGI2() + _number + "_" + foto.Id.Trim() + ".jpg";
                                cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "";
                                //cmd.Parameters.Add("@path", SqlDbType.VarChar, 200).Value = foto.Path;
                                cmd.Parameters.Add("@path", SqlDbType.VarChar, 200).Value = "";
                                cmd.Parameters.Add("@size", SqlDbType.BigInt, 11).Value = foto.Size;
                                cmd.Parameters.Add("@state_aws", SqlDbType.Int, 11).Value = 2;
                                cmd.Parameters.Add("@id_aws", SqlDbType.VarChar, 200).Value = "";
                            if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Firma en el gestor documental");
                                }

                            }
                        //}
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }
        }

        private bool RecuperarArchivoFirma(int acta, string campo, string url)
        {
            String file = ConfigVars.FolderImagenesHGI2() + "\\" + acta + "_" + campo + ".jpg";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            
            LOG("Descargado archivo Firma, url: " + url);
            try
            {
                WebClient webClient = new WebClient();
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigVars.UserWsHda() + ":" + ConfigVars.PasswordWsHda()));
                webClient.Headers.Add("Authorization", "Basic " + credentials);
                webClient.DownloadFile(url, file);
                return true;

            }
            catch (Exception ex)
            {
                LOG("Problem: " + ex.Message);
            }

            return false;
        }

        private bool RecuperarArchivoFoto(String id, string url)
        {
            String file = ConfigVars.FolderImagenesHGI2() + "\\" + id + ".jpg";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            Console.WriteLine("Descargando foto url: " + url);
            LOG("Descargado Foto, url: " + url);
            try
            {
                WebClient webClient = new WebClient();
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigVars.UserWsHda() + ":" + ConfigVars.PasswordWsHda()));
                webClient.Headers.Add("Authorization", "Basic " + credentials);
                webClient.DownloadFile(url, ConfigVars.FolderImagenesHGI2() + id + ".jpg");
                return true;

            }
            catch (Exception ex)
            {
                LOG("Problem: " + ex.Message);
            }

            return false;
        }

        public bool isJArray(JToken obj)
        {
            bool result = true;
            try
            {
                JArray a = (JArray)obj;
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public bool isNull(JToken obj)
        {
            bool result = false;

            try
            {
                if (obj.Type == JTokenType.Null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                LOG(e.Message);
            }
            return result;
        }

        public bool isNull(JValue obj)
        {
            bool result = false;

            try
            {
                if (obj.Type == JTokenType.Null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                LOG(e.Message);
            }
            return result;
        }

        public string ConvertString(JToken obj)
        {
            string cadena = "";
            try
            {
                if (obj == null)
                {
                    cadena = "";
                }
                else
                {
                    cadena = (string)obj;
                }
            }
            catch (Exception e)
            {
                LOG(e.Message);
            }
            return cadena;
        }

        public void LOG(string log)
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
                    //Console.WriteLine(cadena);
                    using (StreamWriter outfile = new StreamWriter(@filename, true))
                    {
                        outfile.Write(cadena);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al escribir el archivo LOG " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine(log);
            }

        }
    }
}
