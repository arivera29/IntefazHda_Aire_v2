using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmObtenerFotosHDA : Form
    {
        List<Foto> lista;
        Datos conexion;
        public FrmObtenerFotosHDA()
        {
            InitializeComponent();
        }

        private void btnObtener_Click(object sender, EventArgs e)
        {
            if (txtActa.Text != "")
            {
                String cadena = txtActa.Text.Trim();
                String[] actas = Regex.Split(cadena, "\r\n");
                if (actas.Length > 0)
                {
                    txtLog.Clear();
                    for (int i = 0; i < actas.Length; i++)
                    {
                        
                        try
                        {
                            //Get method
                            string url = "http://hgiservice.herramientasgnf.com:8080/api/serviceOrders/byNumber/" + actas[i].Trim();
                            WebRequest req = WebRequest.Create(url);
                            LOG("Procesando url: " + url);
                            req.Method = "GET";
                            req.ContentType = "application/json; charset=utf-8";

                            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
                            req.Headers.Add("Authorization", "Basic " + credentials);

                            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                using (Stream respStream = resp.GetResponseStream())
                                {
                                    StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                                    StringBuilder sb = new StringBuilder(reader.ReadToEnd());
                                    Console.WriteLine(sb.ToString());
                                    ProcesarActa(sb, actas[i]);
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
            else
            {
                MessageBox.Show("Debe ingresar No. del Actas");
            }
        }

        private void LOG(string cadena)
        {
            txtLog.AppendText(cadena + "\r\n");
        }

        private void ProcesarActa(StringBuilder sb, String acta)
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
                     conexion.Close();

                 }
                 else
                 {
                     MessageBox.Show("Error al conectarse con la base de datos");
                 }


            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Trace: " + ex.StackTrace.ToString());
            }
        }

        private void RegistrarFotos(List<Foto> fotos, String _number)
        {
            try
            {

                if (fotos.Count > 0)
                {
                    String SQL = "DELETE FROM Documentos WHERE DocuActa=@orden and DocuTiDo in (3,4,5,6,7,8,10,11,12,9,19,2,1)";
                    using (SqlCommand cmd = new SqlCommand(SQL))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                        cmd.ExecuteNonQuery();

                    }

                }
                
                String sql = "INSERT INTO Documentos (DocuActa,DocuTiDo,DocuUrRe,DocuUsCa,DocuFeCa,DocuUrLo,DocuSincro,DocuVeri,DocuUsVe,DocuFeVe) "
                    + "VALUES (@orden,@tipo,@url_remoto,'interfaz',SYSDATETIME(),@url_local,1,0,'',NULL)";

                foreach (Foto foto in fotos)
                {
                    if (foto.Firma == 0)
                    {
                        if (this.RecuperarArchivoFoto(foto.Id, foto.Url))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();

                                cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                                cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "File/Documentos/" + foto.Id.Trim() + ".jpg";
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Foto en el gestor documental");
                                }

                            }
                        }
                    }
                    else
                    {
                        if (this.RecuperarArchivoFirma(_number, foto.Id, foto.Url))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();

                                cmd.Parameters.Add("@orden", SqlDbType.VarChar, 20).Value = _number;
                                cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = foto.Tipo;
                                cmd.Parameters.Add("@url_remoto", SqlDbType.VarChar, 200).Value = foto.Url;
                                cmd.Parameters.Add("@url_local", SqlDbType.VarChar, 200).Value = "File/Documentos/" + _number + "_" + foto.Id.Trim() + ".jpg";
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    // Se guardó el registro
                                }
                                else
                                {
                                    LOG("Error al guardar el registro de la Firma en el gestor documental");
                                }

                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                LOG(ex.Message + " Line Number: " + ex.LineNumber + " Procedure: " + ex.Procedure + " Trace: " + ex.StackTrace);
            }
        }

        private bool RecuperarArchivoFirma(string acta, string campo, string url)
        {
            String file = @Properties.Settings.Default.dir_imagenes + "\\" + acta + "_" + campo + ".jpg";
            if (File.Exists(file))
            {
              File.Delete(file);   
            }
                LOG("Descargado archivo Firma, url: " + url);
                try
                {
                    WebClient webClient = new WebClient();
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
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
            String file = @Properties.Settings.Default.dir_imagenes + "\\" + id + ".jpg";
            if (File.Exists(file))
            {
              File.Delete(file);   
            }
                
                LOG("Descargado Foto, url: " + url);
                try
                {
                    WebClient webClient = new WebClient();
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Properties.Settings.Default.user_hda + ":" + Properties.Settings.Default.pass_hda));
                    webClient.Headers.Add("Authorization", "Basic " + credentials);
                    webClient.DownloadFile(url, @Properties.Settings.Default.dir_imagenes + id + ".jpg");
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

        private void cmdClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }
    }
}
