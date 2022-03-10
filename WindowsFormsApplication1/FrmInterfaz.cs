using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;

namespace InterfazHda
{
    public partial class FrmInterfaz : Form
    {
        private const int MINUTOS = 1000 * 60;
        private int Estado = 0;
        private List<InfoActa> IdActas;
        private List<Acta> listaActas;
        private int numPages = 0;
        private String AppDir = "";
        private DateTime FechaUltimaConsulta;
        private DateTime FechaActual;
        private String idCiclo = "";
        public FrmInterfaz()
        {
            InitializeComponent();
        }

        private void FrmInterfaz_Load(object sender, EventArgs e)
        {
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc);
            //fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            //long ticks = fecha.Ticks;
            //double milliseconds = fecha.ToUniversalTime().Subtract(fecha).TotalMilliseconds;
            //long milliseconds = fecha.ticks  / TimeSpan.TicksPerMillisecond;
            long ts = (long)Math.Floor((fecha - new DateTime(1970, 1, 1)).TotalMilliseconds);

            
            txtTiempo.Value = 30;
            timer1.Interval = MINUTOS * (int)txtTiempo.Value;
            timer2.Interval = MINUTOS * 45;

            IdActas = new List<InfoActa>();

            AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

        }

        private void Ocultar()
        {
            this.Hide();

            //MessageBox.Show("Observa en el area de notificación\nla aplicación fue minimizada ahí.\n has click derecho sobre el icono para ver las opciones");

            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipText = "Interfaz HDA Iniciada";
            notifyIcon1.BalloonTipTitle = "Interfaz HDA";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(5000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Get method
            string url = @Properties.Settings.Default.url_hda;
            WebRequest req = WebRequest.Create(url);

            req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";

            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("hgi:hgi"));
            req.Headers.Add("Authorization", "Basic " + credentials);

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (Stream respStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                    //Console.WriteLine(reader.ReadToEnd());
                    LOG(reader.ReadToEnd());
                }


            }
            else
            {
                Console.WriteLine(string.Format("Status Code: {0}, Status Description: {1}", resp.StatusCode, resp.StatusDescription));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listaActas = new List<Acta>();
            IdActas = new List<InfoActa>();
           
            cmdIniciar.Enabled = false;
            cmdStop.Enabled = true;

            timer1.Interval = MINUTOS * (int)txtTiempo.Value;
            timer1.Enabled = true;

            //timer2.Enabled = true;
            //timer2.Interval = MINUTOS * (int)txtTiempo.Value;


            Estado = 1;
            txtTiempo.Enabled = false;
            this.RegistroInicio();
            LOG("UPLOADING Actas");
            numPages = 0;
            FechaUltimaConsulta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc);
            if (checkBox1.Checked)
            {
                FechaUltimaConsulta = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, (int)txtHora.Value , 0, 0, DateTimeKind.Utc);
            }
            FechaActual = DateTime.Now;
            txtUltimaConsulta.Text = "Ultima Consulta: " + FechaActual;

            Consultar(0);
            //this.backgroundWorker1.RunWorkerAsync();
            ProcesoDistribucionActas();
            this.RegistroFinal();
        }

        public void Consultar(int page)
        {

            LOG("Consultando Web Service HDA");
            try
            {
                //Get method
                //DateTime FechaUltimaConsulta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc);
                
                long ts = (long)Math.Floor((FechaUltimaConsulta - new DateTime(1970, 1, 1)).TotalMilliseconds);
                string url = @Properties.Settings.Default.url_hda + "?fromMs=" + ts + "&page=" + page;

                String filename = Environment.CurrentDirectory + @"\petciones_ws.txt";
                using (StreamWriter outfile = new StreamWriter(@filename, true))
                {
                    outfile.Write(DateTime.Now.ToString() + "\t" + url + "\t" + FechaUltimaConsulta.ToString() + "\r\n");
                }
                
                LOG("Procesando url: " + url);
                WebRequest req = WebRequest.Create(url);

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
                        //Console.WriteLine(reader.ReadToEnd());
                        JObject obj = JObject.Parse(sb.ToString());
                        numPages = (int)obj["numPages"];
                        int total = (int)obj["total"];
                        JArray record = (JArray)obj["records"];
                        if (record.Count > 0)
                        {
                            Procesar(sb);
                            Consultar(page + 1);
                        }
                        else
                        {
                            ConsultarIdActa();
                        }
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

        public void ConsultarIdActa()
        {
            LOG("Consultando Web Service HDA id Acta");
            if (IdActas.Count > 0)
            {
                foreach (InfoActa acta in IdActas)
                {
                    try
                    {
                        //Get method
                        string url = @Properties.Settings.Default.url_hda + "/" + acta.Id;
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
                                ProcesarActa(sb, acta.Id, acta.fechaModificacion);
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

                GuardarActaBD();
               
            }
        }

        public void ActualizarSofatec(string acta, string idActa,string fecha)
        {
            try
            {
                string url = @"http://64.37.63.226:8080/sofatec/SrvConsultarIdOrdenHda?orden=" + acta + "&id=" + idActa + "&fecha=" + fecha;
                WebRequest req = WebRequest.Create(url);
                LOG("Procesando url: " + url);
                req.Method = "GET";
                 HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                 if (resp.StatusCode == HttpStatusCode.OK)
                 {
                     LOG("Respuesta OK SOFATEC");
                     resp.Close();
                 }
            }
            catch (Exception ex)
            {
                LOG("Error: " + ex.Message);
                //EnviarAlertaCorreo("Alerta Interfaz HDA", ex.Message);
            }

        }

        public void Procesar(StringBuilder sb)
        {
            LOG("Procesando JSON");

            try
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    JObject obj = JObject.Parse(sb.ToString());
                    //listBox1.Items.Add("Status=" + obj["status"]);
                    //listBox1.Items.Add("Total=" + obj["total"]);
                    //listBox1.Items.Add("PageSize=" + obj["pageSize"]);
                    //listBox1.Items.Add("NumPages=" + obj["numPages"]);

                    JArray a = (JArray)obj["records"];

                    for (int x = 0; x < a.Count; x++)
                    {
                        
                        String orden = (string)a[x]["numeroOS"];
                        String idActa = (string)a[x]["id"];
                        
                        if (!ExisteActaId((string)a[x]["id"],conexion))
                        {
                            InfoActa acta = new InfoActa();
                            acta.Id = (string)a[x]["id"];
                            string strFecha = (string)a[x]["fechaModificacion"]["COT"];
                            DateTime fecha = Convert.ToDateTime(strFecha);
                            acta.fechaModificacion = fecha;
                            IdActas.Add(acta);
                        }
                        else
                        {
                            LOG("Orden de servicio Id " + (string)a[x]["id"] + " ya se encuentra registrada");
                        }

                    }

                    conexion.Close();
                }
                else
                {
                    LOG("Error al conectarse con el servidor de base de datos");
                }

            }
            catch (Exception ex)
            {
                LOG(ex.Message);
            }
        
        }


        public void ProcesarActa(StringBuilder sb, string id, DateTime fechaModificacion)
        {
            LOG("Procesando JSON Acta " + id);
            Acta acta = new Acta();
            try
            {
                JObject o = JObject.Parse(sb.ToString());
                LOG("Validando estructura JSON");
                try
                {
                    if ((JToken)o.SelectToken("error") != null)
                    {
                        if ((bool)(JToken)o.SelectToken("error") == true)
                        {
                            LOG("Error al procesar JSON del acta, informar al administrador de la HDA");
                            return;
                        }
                    }
                }
                catch (Exception)
                {

                }
                LOG("Estructura JSON OK");
                LOG("Validando estado de la(s) anomalia(s)");
                
                if ((bool)(JToken)o["noAnomalias"] == true)  // El Acta tiene Anomalia
                {
                    LOG("Acta No. " + acta.Id + " SIN ANOMALIA");
                }
                else
                {
                    LOG("Acta con anomalia");
                }

                    
                    LOG("Recuperando informacion basica");
                    #region InformacionBasica
                    acta.Id = id;
                    acta.fechaModificacion = fechaModificacion;
                    acta._number = ConvertString(o["_number"], "_number");
                    acta.numeroLote = ConvertString(o["numeroLote"], "numeroLote");
                    acta._clientCloseTs = ConvertString(o["_clientCloseTs"], "_clientCloseTs");
                    acta.codigoEmpresa = ConvertString(o["codigoEmpresa"], "codigoEmpresa");
                    acta.CodigoContrata = acta.codigoEmpresa.Substring(1, 6);
                    acta.comentario1 = ConvertString(o["comentario1"], "comentario1");
                    acta.comentario2 = ConvertString(o["comentario2"], "comentario2");
                    acta.nic = ConvertString(o["nic"], "nic");
                    acta.direccion = ConvertString(o["direccion"], "direccion");
                    acta.departamento = ConvertString(o["departamento"], "departamento");
                    acta.municipio = ConvertString(o["municipio"], "municipio");
                    acta.localidad = ConvertString(o["localidad"], "localidad");
                    if (!isNull(o["tipoVia"]))
                    {
                        acta.tipoVia = ConvertString(o["tipoVia"]["displayName"], "tipoVia->displayName");
                    }
                    if (!isNull(o["tipoOrdenServicio"]))
                    {
                        acta.tipoOrdenServicio = ConvertString(o["tipoOrdenServicio"]["displayName"], "tipoOrdenServicio->displayName");
                    }
                    if (!isNull(o["tipoServicio"]))
                    {
                        acta.tipoServicio = ConvertString(o["tipoServicio"]["displayName"], "tipoServicio->displayName");
                    }
                    acta.tarifa = ConvertString(o["codigoTarifa"], "codigoTarifa");
                    
                    if (!isNull(o["estrato"]))
                    {
                        acta.estrato = ConvertString(o["estrato"]["displayName"], "estrato->displayName");
                    }

                    if (!isNull(o["calCenCar_tipo"]))
                    {
                        acta.tipoCenso = ConvertString(o["calCenCar_tipo"]["displayName"], "calCenCar_tipo->displayName");
                    }

                    if (!isNull(o["calCenCar_cargaInstalada"]))
                    {
                        try
                        {
                            acta.censoCargaInstalada = (double)o["calCenCar_cargaInstalada"];
                        }
                        catch (Exception e)
                        {
                            LOG("Error: " + e.Message + " Key: calCenCar_cargaInstalada");
                        }
                    }

                    if (!isNull(o["tipoCliente"]))
                    {
                        acta.tipoCliente = ConvertString(o["tipoCliente"]["displayName"], "tipoCliente->displayName");
                    }

                    acta.cargaContratada = ConvertString(o["cargaContratada"], "cargaContratada");
                    acta.calle = ConvertString(o["calle"], "calle");
                    acta.numeroPuerta = ConvertString(o["numeroPuerta"], "numeroPuerta");
                    acta.duplicador = ConvertString(o["duplicador"], "duplicador");
                    acta.piso = ConvertString(o["piso"], "piso");
                    acta.referenciaDireccion = ConvertString(o["referenciaDireccion"], "referenciaDireccion");
                    acta.acceso = ConvertString(o["acceso"], "acceso");
                    acta.numeroCircuito = ConvertString(o["numeroCircuito"], "numeroCircuito");
                    acta.matriculaCT = ConvertString(o["matriculaCT"], "matriculaCT");
                    acta.nombreTitularContrato = ConvertString(o["nombreTitularContrato"], "nombreTitularContrato");
                    acta.apellido1TitularContrato = ConvertString(o["apellido1TitularContrato"], "apellido1TitularContrato");
                    acta.apellido2TitularContrato = ConvertString(o["apellido2TitularContrato"], "apellido2TitularContrato");
                    acta.cedulaTitularContrato = ConvertString(o["cedulaTitularContrato"], "cedulaTitularContrato");
                    acta.telefonoFijoTitularContrato = ConvertString(o["telefonoFijoTitularContrato"], "telefonoFijoTitularContrato");
                    acta.telefonoMovilTitularContrato = ConvertString(o["telefonoMovilTitularContrato"], "telefonoMovilTitularContrato");
                    acta.emailTitularContrato = ConvertString(o["emailTitularContrato"], "emailTitularContrato");
                    
                    if (!isNull(o["relacionReceptorVisita"]))
                    {
                        acta.relacionReceptorVisita = ConvertString(o["relacionReceptorVisita"]["displayName"], "relacionReceptorVisita->displayName");
                    }
                    
                    acta.solicitaTecnicoReceptorVisita = ConvertString(o["solicitaTecnicoReceptorVisita"], "solicitaTecnicoReceptorVisita");
                    acta.aportaTestigo = ConvertString(o["aportaTestigo"], "aportaTestigo");
                    acta.nombreReceptorVisita = ConvertString(o["nombreReceptorVisita"], "nombreReceptorVisita");
                    acta.apellido1ReceptorVisita = ConvertString(o["apellido1ReceptorVisita"], "apellido1ReceptorVisita");
                    acta.apellido2ReceptorVisita = ConvertString(o["apellido2ReceptorVisita"], "apellido2ReceptorVisita");
                    acta.cedulaReceptorVisita = ConvertString(o["cedulaReceptorVisita"], "cedulaReceptorVisita");
                    acta.telefonoFijoReceptorVisita = ConvertString(o["telefonoFijoReceptorVisita"], "telefonoFijoReceptorVisita");
                    acta.telefonoMovilReceptorVisita = ConvertString(o["telefonoMovilReceptorVisita"], "telefonoMovilReceptorVisita");
                    acta.emailReceptorVisita = ConvertString(o["emailReceptorVisita"], "emailReceptorVisita");
                    acta.observacionAnomalia = ConvertString(o["anomalias_observaciones"], "anomalias_observaciones");
                    String fecha = ConvertString(o["fechaInicioIrregularidad"], "fechaInicioIrregularidad");

                    acta.actaSinAnomalia = (bool)(JToken)o["noAnomalias"];

                    if (!fecha.Equals(""))
                    {
                        acta.fechaInicioIrregularidad = DateTime.Parse(fecha.Substring(0, 10));
                    }
                    acta.residuosRecolectados = ConvertString(o["residuosRecolectados"], "residuosRecolectados");
                    acta.clasificacionResiduos = ConvertString(o["clasificacionResiduos"], "clasificacionResiduos");
                    acta.ordenAseo = ConvertString(o["ordenAseo"], "ordenAseo");
                    acta.recibidoQuejas = ConvertString(o["recibidoQuejas"], "recibidoQuejas");
                    acta.atendidoQuejas = ConvertString(o["atendidoQuejas"], "atendidoQuejas");
                    acta.observaciones = ConvertString(o["observaciones"], "observaciones");
                    acta.tipoCalculo = ConvertString(o["tipoCalculo"], "tipoCalculo");

                    if (!isNull(o["medidaAnomaliaTipo"]))
                    {
                        acta.medidaAnomaliaTipo = ConvertString(o["medidaAnomaliaTipo"]["displayName"], "medidaAnomaliaTipo->displayName");
                    }

                    acta.medidaAnomaliaIR = ConvertString(o["medidaAnomaliaIR"], "medidaAnomaliaIR");
                    acta.medidaAnomaliaIS = ConvertString(o["medidaAnomaliaIS"], "medidaAnomaliaIS");
                    acta.medidaAnomaliaIT = ConvertString(o["medidaAnomaliaIT"], "medidaAnomaliaIT");
                    acta.medidaAnomaliaVR = ConvertString(o["medidaAnomaliaVR"], "medidaAnomaliaVR");
                    acta.medidaAnomaliaVS = ConvertString(o["medidaAnomaliaVS"], "medidaAnomaliaVS");
                    acta.medidaAnomaliaVT = ConvertString(o["medidaAnomaliaVT"], "medidaAnomaliaVT");

                    acta.cedulaOperario = ConvertString(o["cedulaOperario"], "cedulaOperario");
                    acta.nombreOperario = ConvertString(o["nombreOperario"], "nombreOperario");
                    acta.apellido1Operario = ConvertString(o["apellido1Operario"], "apellido1Operario");
                    acta.apellido2Operario = ConvertString(o["apellido2Operario"], "apellido2Operario");
                    acta.empresaOperario = ConvertString(o["empresaOperario"], "empresaOperario");



                    #endregion
                    LOG("Recuperando informacion de trabajos ejecutados");
                    #region TrabajosEjecutados
                    if (isJArray(o["trabajos_ejecutados"]))
                    {
                        JArray trabajosEjecutados = (JArray)o["trabajos_ejecutados"];
                        if (trabajosEjecutados.Count > 0)
                        {
                            for (int i = 0; i < trabajosEjecutados.Count; i++)
                            {
                                if ((bool)trabajosEjecutados[i]["ejecutada"] == true)
                                { 
                                    Accion accion = new Accion();
                                    accion.CodigoAccion = ConvertString(trabajosEjecutados[i]["codigo"]);
                                    accion.CodigoPaso = ConvertString(trabajosEjecutados[i]["accion"]);
                                    accion.DescripcionAccion = ConvertString(trabajosEjecutados[i]["valor"]);
                                    accion.NuevoPaso = 0;

                                    LOG("Validando materiales Accion " + accion.CodigoAccion  + " Paso " + accion.CodigoPaso);
                                    if (isJArray(trabajosEjecutados[i]["materiales"]))
                                    {
                                        JArray materiales = (JArray)trabajosEjecutados[i]["materiales"];
                                        if (materiales.Count > 0)
                                        {
                                            for (int j = 0; j < materiales.Count; j++)
                                            {
                                                Material material = new Material();
                                                material.CodigoMaterial = ConvertString(materiales[j]["codigo"]);
                                                material.DescripcionMaterial = ConvertString(materiales[j]["valor"]);
                                                material.Cantidad = (long)materiales[j]["cantidad"];
                                                material.Cobro = (bool)materiales[j]["cobrable"] == true ? 1 : 0;
                                                LOG("Material " + material.CodigoMaterial + " Agregado");
                                                accion.Materiales.Add(material);
                                            }

                                        }
                                        else
                                        {
                                            LOG("Items de materiales es cero(0)");
                                        }
                                    }else
                                    {
                                        LOG("No se registra información de materiales");
                                    }

                                    acta.acciones.Add(accion);

                                    LOG("Validando Nuevos Pasos " + accion.CodigoAccion + " Paso " + accion.CodigoPaso);
                                    if (isJArray(trabajosEjecutados[i]["children"]))
                                    {
                                        JArray children = (JArray)trabajosEjecutados[i]["children"];
                                        if (children.Count > 0)
                                        {
                                            for (int j = 0; j < children.Count; j++)
                                            {
                                                Accion NuevaAccion = new Accion();
                                                NuevaAccion.CodigoAccion = ConvertString(children[j]["codigo"]);
                                                NuevaAccion.CodigoPaso = ConvertString(children[j]["accion"]);
                                                NuevaAccion.DescripcionAccion = ConvertString(children[j]["valor"]);
                                                NuevaAccion.NuevoPaso = 1;

                                                if ((bool)children[j]["ejecutada"] == true)
                                                {

                                                    LOG("Validando materiales Accion " + NuevaAccion.CodigoAccion + " Nuevo Paso " + NuevaAccion.CodigoPaso);
                                                    if (isJArray(children[j]["materiales"]))
                                                    {
                                                        JArray materiales = (JArray)children[j]["materiales"];
                                                        if (materiales.Count > 0)
                                                        {
                                                            for (int h = 0; h < materiales.Count; h++)
                                                            {
                                                                Material material = new Material();
                                                                material.CodigoMaterial = ConvertString(materiales[h]["codigo"]);
                                                                material.DescripcionMaterial = ConvertString(materiales[h]["valor"]);
                                                                material.Cantidad = (long)materiales[h]["cantidad"];
                                                                material.Cobro = (bool)materiales[h]["cobrable"] == true ? 1 : 0;
                                                                LOG("Material " + material.CodigoMaterial + " Agregado");
                                                                NuevaAccion.Materiales.Add(material);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            LOG("Items de materiales es cero(0)");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        LOG("No se registra información de materiales");
                                                    }

                                                    acta.acciones.Add(NuevaAccion);

                                                }

                                            }


                                        }

                                    }


                                }
                            }

                        }

                    }



                    #endregion

                    LOG("Recuperando informacion de medidores existentes");
                    #region AparatoExistente

                    acta.medidorRetirado = 0;
                    acta.protocolo = 0;
                    if (isJArray(o["aparatosExistentes"]))
                    {
                        JArray Aparatos = (JArray)o["aparatosExistentes"];
                        if (Aparatos.Count > 0)
                        {
                            if (!isNull(Aparatos[0]["retirado"]))
                            {
                                if ((bool)Aparatos[0]["retirado"] == true)
                                {
                                    acta.medidorRetirado = 1;
                                    if (!isNull(Aparatos[0]["envioLab"]))
                                    {
                                        if (!isNull(Aparatos[0]["envioLab"]["id"]))
                                        {
                                            acta.protocolo = 0;
                                            if ((string)Aparatos[0]["envioLab"]["id"] == "__ret_lab")
                                            {

                                                acta.medidorEnviadoLaboratorio = 1;
                                                acta.protocolo = 1;
                                            }
                                        }
                                    }
                                }
                            }

                            for (int x = 0; x < Aparatos.Count; x++)
                            {
                                MedidorExistente medidor = new MedidorExistente();

                                medidor.tipoRevision = ConvertString(Aparatos[x]["tipoRevision"]["displayName"], "aparatosExistentes->tipoRevision->displayName");
                                medidor.numero = ConvertString(Aparatos[x]["numero"], "numero");

                                if (!isNull(Aparatos[x]["marca"]))
                                {
                                    medidor.marca = ConvertString(Aparatos[x]["marca"]["displayName"], "marca->displayName");
                                }
                                else
                                {
                                    medidor.marca = "SIN MARCA";
                                }
                                if (!isNull(Aparatos[x]["tipo"]))
                                {
                                    medidor.tipo = ConvertString(Aparatos[x]["tipo"]["displayName"], "tipo->displayName");
                                }
                                else
                                {
                                    medidor.tipo = "SIN TIPO";
                                }

                                if (!isNull(Aparatos[x]["tecnologia"]))
                                {
                                    medidor.tecnologia = ConvertString(Aparatos[x]["tecnologia"]["displayName"], "tecnologia->displayName");
                                }

                                medidor.lecturaUltimaFecha = ConvertString(Aparatos[x]["lecturaUltimaFecha"], "lecturaUltimaFecha");
                                medidor.lecturaUltima = ConvertString(Aparatos[x]["lecturaUltima"], "lecturaUltima");
                                medidor.lecturaActual = ConvertString(Aparatos[x]["lecturaActual"], "lecturaActual");
                                if (!isNull(Aparatos[x]["kdkh_tipo"]))
                                {
                                    medidor.kdkh_tipo = ConvertString(Aparatos[x]["kdkh_tipo"]["displayName"], "kdkh_tipo->displayName");
                                }
                                medidor.kdkh_value = ConvertString(Aparatos[x]["kdkh_value"], "kdkh_value");
                                if (!isNull(Aparatos[x]["digitos"]))
                                {
                                    medidor.digitos = ConvertString(Aparatos[x]["digitos"]["displayName"], "digitos->displayName");
                                }
                                if (!isNull(Aparatos[x]["decimales"]))
                                {
                                    medidor.decimales = ConvertString(Aparatos[x]["decimales"]["displayName"], "decimales->displayName");
                                }
                                if (!isNull(Aparatos[x]["nFases"]))
                                {
                                    medidor.nFases = ConvertString(Aparatos[x]["nFases"]["displayName"], "nFases->displayName");
                                }
                                if (!isNull(Aparatos[x]["voltajeNominal"]))
                                {
                                    medidor.voltajeNominal = ConvertString(Aparatos[x]["voltajeNominal"]["displayName"], "voltajeNominal->displayName");
                                }
                                medidor.rangoCorrienteMin = ConvertString(Aparatos[x]["rangoCorrienteMin"], "rangoCorrienteMin");
                                medidor.rangoCorrienteMax = ConvertString(Aparatos[x]["rangoCorrienteMax"], "rangoCorrienteMax");
                                medidor.corrienteN_mec = ConvertString(Aparatos[x]["corrienteN_mec"], "corrienteN_mec");
                                medidor.corrienteFN_mec = ConvertString(Aparatos[x]["corrienteFN_mec"], "corrienteFN_mec");
                                medidor.voltageNT_mec = ConvertString(Aparatos[x]["voltageNT_mec"], "voltageNT_mec");
                                medidor.voltageRS_mec = ConvertString(Aparatos[x]["voltageRS_mec"], "voltageRS_mec");
                                medidor.voltageFNR_mec = ConvertString(Aparatos[x]["voltageFNR_mec"], "voltageFNR_mec");
                                medidor.voltageFTR_mec = ConvertString(Aparatos[x]["voltageFTR_mec"], "voltageFTR_mec");
                                medidor.corrienteR_mec = ConvertString(Aparatos[x]["corrienteR_mec"], "corrienteR_mec");
                                medidor.voltageFNS_mec = ConvertString(Aparatos[x]["voltageFNS_mec"], "voltageFNS_mec");
                                medidor.voltageFTS_mec = ConvertString(Aparatos[x]["voltageFTS_mec"], "voltageFTS_mec");
                                medidor.corrienteS_mec = ConvertString(Aparatos[x]["corrienteS_mec"], "corrienteS_mec");

                                medidor.voltageFNT_mec = ConvertString(Aparatos[x]["voltageFNT_mec"], "voltageFNT_mec");
                                medidor.voltageFTT_mec = ConvertString(Aparatos[x]["voltageFTT_mec"], "voltageFTT_mec");
                                medidor.corrienteT_mec = ConvertString(Aparatos[x]["corrienteT_mec"], "corrienteT_mec");

                                medidor.pruebaAlta = "";
                                if (!isNull(Aparatos[x]["pruebaAlta"]))
                                {
                                    medidor.pruebaAlta = ConvertString(Aparatos[x]["pruebaAlta"]["displayName"], "pruebaAlta->displayName");
                                }
                                medidor.voltageFNR_alta = ConvertString(Aparatos[x]["voltageFNR_alta"], "voltageFNR_alta");
                                medidor.corrienteR_alta = ConvertString(Aparatos[x]["corrienteR_alta"], "corrienteR_alta");
                                medidor.vueltasR_alta = ConvertString(Aparatos[x]["vueltasR_alta"], "vueltasR_alta");
                                medidor.tiempoR_alta = ConvertString(Aparatos[x]["tiempoR_alta"], "tiempoR_alta");
                                medidor.voltageFNS_alta = ConvertString(Aparatos[x]["voltageFNS_alta"], "voltageFNS_alta");
                                medidor.corrienteS_alta = ConvertString(Aparatos[x]["corrienteS_alta"], "corrienteS_alta");
                                medidor.vueltasS_alta = ConvertString(Aparatos[x]["vueltasS_alta"], "vueltasS_alta");
                                medidor.tiempoS_alta = ConvertString(Aparatos[x]["tiempoS_alta"], "tiempoS_alta");
                                medidor.errorPruebaR_alta = ConvertString(Aparatos[x]["errorPruebaR_alta"], "errorPruebaR_alta");
                                medidor.errorPruebaS_alta = ConvertString(Aparatos[x]["errorPruebaS_alta"], "errorPruebaS_alta");
                                medidor.pruebaBaja = "";
                                if (!isNull(Aparatos[x]["pruebaBaja"]))
                                {
                                    medidor.pruebaBaja = ConvertString(Aparatos[x]["pruebaBaja"]["displayName"], "pruebaBaja->displayName");
                                }
                                medidor.voltageFNR_baja = ConvertString(Aparatos[x]["voltageFNR_baja"], "voltageFNR_baja");
                                medidor.corrienteR_baja = ConvertString(Aparatos[x]["corrienteR_baja"], "corrienteR_baja");
                                medidor.vueltasR_baja = ConvertString(Aparatos[x]["vueltasR_baja"], "vueltasR_baja");
                                medidor.tiempoR_baja = ConvertString(Aparatos[x]["tiempoR_baja"], "tiempoR_baja");
                                medidor.voltageFNS_baja = ConvertString(Aparatos[x]["voltageFNS_baja"], "voltageFNS_baja");
                                medidor.corrienteS_baja = ConvertString(Aparatos[x]["corrienteS_baja"], "corrienteS_baja");
                                medidor.vueltasS_baja = ConvertString(Aparatos[x]["vueltasS_baja"], "vueltasS_baja");
                                medidor.tiempoS_baja = ConvertString(Aparatos[x]["tiempoS_baja"], "tiempoS_baja");
                                medidor.errorPruebaR_baja = ConvertString(Aparatos[x]["errorPruebaR_baja"], "errorPruebaR_baja");
                                medidor.errorPruebaS_baja = ConvertString(Aparatos[x]["errorPruebaS_baja"], "errorPruebaS_baja");
                                medidor.pruebaDosificacion = "";
                                if (!isNull(Aparatos[x]["pruebaDosificacion"]))
                                {
                                    medidor.pruebaDosificacion = ConvertString(Aparatos[x]["pruebaDosificacion"]["displayName"], "pruebaDosificacion->displayName");
                                }
                                medidor.voltageFNR_dosif = ConvertString(Aparatos[x]["voltageFNR_dosif"], "voltageFNR_dosif");
                                medidor.corrienteR_dosif = ConvertString(Aparatos[x]["corrienteR_dosif"], "corrienteR_dosif");
                                medidor.lecturaInicialR_dosif = ConvertString(Aparatos[x]["lecturaInicialR_dosif"], "lecturaInicialR_dosif");
                                medidor.lecturaFinalR_dosif = ConvertString(Aparatos[x]["lecturaFinalR_dosif"], "lecturaFinalR_dosif");
                                medidor.tiempoR_dosif = ConvertString(Aparatos[x]["tiempoR_dosif"], "tiempoR_dosif");
                                medidor.errorPruebaR_dosif = ConvertString(Aparatos[x]["errorPruebaR_dosif"], "errorPruebaR_dosif");
                                medidor.giroNormal = ConvertString(Aparatos[x]["giroNormal"], "giroNormal");
                                medidor.rozamiento = ConvertString(Aparatos[x]["rozamiento"], "rozamiento");
                                medidor.medidorFrena = ConvertString(Aparatos[x]["medidorFrena"], "medidorFrena");
                                medidor.estadoConexiones = ConvertString(Aparatos[x]["estadoConexiones"], "estadoConexiones");
                                medidor.continuidad = ConvertString(Aparatos[x]["continuidad"], "continuidad");
                                medidor.pruebaPuentes = ConvertString(Aparatos[x]["pruebaPuentes"], "pruebaPuentes");
                                medidor.display = ConvertString(Aparatos[x]["display"], "display");
                                medidor.estadoIntegrador = ConvertString(Aparatos[x]["estadoIntegrador"], "estadoIntegrador");
                                medidor.retirado = ConvertString(Aparatos[x]["retirado"], "retirado");



                                medidor.envioLabNumCustodia = ConvertString(Aparatos[x]["envioLabNumCustodia"], "envioLabNumCustodia");

                                if (isJArray(Aparatos[x]["sellosExistentes"]))
                                {
                                    JArray sellosExistentes = (JArray)Aparatos[x]["sellosExistentes"];
                                    for (int y = 0; y < sellosExistentes.Count; y++)
                                    {
                                        Sellos sello = new Sellos();
                                        sello.Acta = acta._number;
                                        sello.Medidor = medidor.numero;
                                        sello.Clasificacion = 1;  // encontrado
                                        sello.Serie = ConvertString(sellosExistentes[y]["numeroSerie"], "numeroSerie");
                                        if (!isNull(sellosExistentes[y]["posicion"]))
                                        {
                                            sello.Posicion = ConvertString(sellosExistentes[y]["posicion"]["displayName"], "posicion->displayName");
                                        }
                                        if (!isNull(sellosExistentes[y]["estado"]))
                                        {
                                            sello.Estado = ConvertString(sellosExistentes[y]["estado"]["displayName"], "estado->displayName");
                                        }
                                        if (!isNull(sellosExistentes[y]["tipo"]))
                                        {
                                            sello.Tipo = ConvertString(sellosExistentes[y]["tipo"]["displayName"], "tipo->displayName");
                                        }
                                        if (!isNull(sellosExistentes[y]["color"]))
                                        {
                                            sello.Color = ConvertString(sellosExistentes[y]["color"]["displayName"],"color->displayName");
                                        }

                                        medidor.sellos.Add(sello);
                                    }

                                }

                                if (isJArray(Aparatos[x]["sellosInstalados"]))
                                {
                                    JArray sellosInstalados = (JArray)Aparatos[x]["sellosInstalados"];
                                    for (int y = 0; y < sellosInstalados.Count; y++)
                                    {
                                        if (!isNull(sellosInstalados[y]["numeroSerie"]))
                                        {
                                            Sellos sello = new Sellos();
                                            sello.Acta = acta._number;
                                            sello.Medidor = medidor.numero;
                                            sello.Clasificacion = 1;  // instalado
                                            sello.Serie = ConvertString(sellosInstalados[y]["numeroSerie"]);

                                            if (!isNull(sellosInstalados[y]["posicion"]))
                                            {
                                                sello.Posicion = ConvertString(sellosInstalados[y]["posicion"]["displayName"]);
                                            }
                                            if (!isNull(sellosInstalados[y]["estado"]))
                                            {
                                                sello.Estado = ConvertString(sellosInstalados[y]["estado"]["displayName"]);
                                            }
                                            if (!isNull(sellosInstalados[y]["tipo"]))
                                            {
                                                sello.Tipo = ConvertString(sellosInstalados[y]["tipo"]["displayName"]);
                                            }
                                            if (!isNull(sellosInstalados[y]["color"]))
                                            {
                                                sello.Color = ConvertString(sellosInstalados[y]["color"]["displayName"]);
                                            }

                                            medidor.sellos.Add(sello);
                                        }
                                    }

                                }


                                if (isJArray(Aparatos[x]["fotosAparato"]))
                                {
                                    JArray fotosAparato = (JArray)Aparatos[x]["fotosAparato"];
                                    for (int y = 0; y < fotosAparato.Count; y++)
                                    {
                                        Foto foto = new Foto();
                                        foto.Id = ConvertString(fotosAparato[y]["id"]);
                                        foto.Url = ConvertString(fotosAparato[y]["__URL__"]);
                                        foto.Tipo = 3;  // 3-Foto Aparato
                                        acta.fotos.Add(foto);
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
                                        acta.fotos.Add(foto);
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
                                        acta.fotos.Add(foto);
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
                                        acta.fotos.Add(foto);
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
                                        acta.fotos.Add(foto);
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
                                        foto.Tipo = 8;  // 7-Foto Envio Laboratorio
                                        acta.fotos.Add(foto);
                                    }
                                }




                                acta.medidorExistente.Add(medidor);


                            }


                        }
                    }
                    #endregion
                    LOG("Recuperando informacion de Anomalias");
                    #region Anomalias
                    if (acta.actaSinAnomalia == false)
                    {
                        if (!(o["anomalias"] == null))
                        {
                            if (isJArray(o["anomalias"]))
                            {
                                JArray Anomalias = (JArray)o["anomalias"];
                                if (Anomalias.Count > 0)
                                {
                                    for (int x = 0; x < Anomalias.Count; x++)
                                    {
                                        Anomalia anomalia = new Anomalia();
                                        anomalia.Id = ConvertString(Anomalias[x]["id"]);
                                        anomalia.Descripcion = ConvertString(Anomalias[x]["displayName"]);
                                        acta.anomalias.Add(anomalia);
                                    }

                                }

                            }
                        }
                    }
                    #endregion
                    LOG("Recuperando censo de carga");
                    #region CensodeCarga

                    if (isJArray(o["calCenCar_items"]))
                    {
                        JArray censoCarga = (JArray)o["calCenCar_items"];
                        if (censoCarga.Count > 0)
                        {
                            for (int x = 0; x < censoCarga.Count; x++)
                            {
                                Censo censo = new Censo();
                                censo.descripcion = ConvertString(censoCarga[x]["displayName"]);
                                censo.cantidad = ConvertString(censoCarga[x]["intValue"]);
                                acta.censo.Add(censo);
                            }

                        }

                    }

                    #endregion
                    LOG("Recuperando Fotos");
                    #region Fotos

                    if (isJArray(o["fotoFachada"]))
                    {
                        JArray fotoFachada = (JArray)o["fotoFachada"];
                        for (int x = 0; x < fotoFachada.Count; x++)
                        {
                            Foto foto = new Foto();
                            foto.Id = ConvertString(fotoFachada[x]["id"]);
                            foto.Url = ConvertString(fotoFachada[x]["__URL__"]);
                            foto.Tipo = 1;  // 1-Foto Fachada
                            acta.fotos.Add(foto);
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
                            acta.fotos.Add(foto);
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
                            acta.fotos.Add(foto);
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
                        acta.fotos.Add(foto);
                    }

                    if (!isNull(o["firmaOperario"]))
                    {
                        String url = ConvertString(o["firmaOperario"]["__URL__"]);
                        Foto foto = new Foto();
                        foto.Id = "firmaOperario";
                        foto.Url = url;
                        foto.Tipo = 10;  // 10 - Foto Firma Operario
                        foto.Firma = 1;
                        acta.fotos.Add(foto);
                    }

                    if (!isNull(o["firmaTestigo"]))
                    {
                        String url = ConvertString(o["firmaTestigo"]["__URL__"]);
                        Foto foto = new Foto();
                        foto.Id = "firmaTestigo";
                        foto.Url = url;
                        foto.Tipo = 11;  // 11 - Foto Firma Testigo
                        foto.Firma = 1;
                        acta.fotos.Add(foto);
                    }

                    if (!isNull(o["firmaTecnicoParticular"]))
                    {
                        String url = ConvertString(o["firmaTecnicoParticular"]["__URL__"]);
                        Foto foto = new Foto();
                        foto.Id = "firmaTecnicoParticular";
                        foto.Url = url;
                        foto.Tipo = 12;  // 12 - Foto Firma Tecnico Particular
                        foto.Firma = 1;
                        acta.fotos.Add(foto);
                    }

                    #endregion

                    listaActas.Add(acta);


                
            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Trace: " + ex.StackTrace.ToString());
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
                LOG("Iniciando Ciclo");
                this.RegistroInicio();
                this.IniciarProceso();
                ProcesoDistribucionActas();
                this.RegistroFinal();
        }

        public void RegistroInicio()
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                this.idCiclo = Guid.NewGuid().ToString();
                String sql = "INSERT INTO interfaz (id,fechaInicio,fechaFin) values (@id,@fecha1,null)";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 100).Value = idCiclo;
                    cmd.Parameters.Add("@fecha1", SqlDbType.DateTime, 8).Value = DateTime.Now;

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                       // Registro guardado
                    }

                }

                sql = "EXEC UpdateDelegacionContrata;";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        // Registro guardado
                    }

                }


                conexion.Close();
            }


        }

        public void RegistroFinal()
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "UPDATE interfaz SET fechaFin=@fecha1 WHERE id=@id";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 100).Value = idCiclo;
                    cmd.Parameters.Add("@fecha1", SqlDbType.DateTime, 8).Value = DateTime.Now;

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        // Registro guardado
                    }

                }
                conexion.Close();
            }


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

        public string ConvertString(JToken obj, string nota)
        {
            string cadena = "";
            try
            {
                cadena = NullToString((object)obj);
            }
            catch (Exception e)
            {
                LOG("Error al convertir valor " + nota);
            }
            return cadena;
        }

        static string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;


        }
        public void LOG(string log)
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

        public void EnviarAlertaCorreo(string asunto, string mensaje)
        {
            MailMessage email = new MailMessage();
            email.To.Add(new MailAddress("aimerrivera@yahoo.es"));
            email.From = new MailAddress("aimer.rivera@are-soluciones.com");
            email.Subject = asunto + " ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";
            email.Body = "<h2>Notificacion de Alerta Interfaz HDA</h2><p><strong>Mensaje Alerta: </strong>" + mensaje + "</p><br/><p>Favor comunicarse con el administrador del sistema</p><br/><br/><p>Este mensaje se ha generado automaticamente, favor no responder.</p><br/><p>Todos los derechos reservados 2015.  SOLUCIONES INTEGRALES ARE.  www.are-soluciones.com</p>";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("aimer.rivera@are-soluciones.com", "pantera2341");

            string output = null;

            try
            {
                smtp.Send(email);
                email.Dispose();
                output = "Corre electrónico fue enviado satisfactoriamente.";
            }
            catch (Exception ex)
            {
                output = "Error enviando correo electrónico: " + ex.Message;
            }

            LOG(output);

        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            cmdIniciar.Enabled = true;
            cmdStop.Enabled = false;
            timer1.Enabled = false;
            txtTiempo.Enabled = true;
            Estado = 0;
            LOG("Terminando proceso");
        }

        private void GuardarActaBD()
        {
            LOG("Iniciando carga de actas en el base de datos");
            LOG("Numero de actas descargadas: " + listaActas.Count);
            Datos conexion = new Datos();
            if (conexion != null)
            {

                foreach (Acta acta in listaActas)
                {

                    if (!ExisteActa(acta._number,conexion))
                    {

                        LOG("Guardando en la base de datos acta " + acta.Id);
                        GestionActa ga = new GestionActa();
                        ga.conexion = conexion;
                        if (ga.AgregarActa(acta))
                        {
                            LOG("Acta guardada correctamente " + acta.Id);
                            FechaUltimaConsulta = acta.fechaModificacion;
                            String filename = Environment.CurrentDirectory + @"\date_hgi_found.txt";
                            using (StreamWriter outfile = new StreamWriter(@filename,true))
                            {
                                outfile.Write( DateTime.Now.ToString() + "\t" + acta.Id + "\t" + acta.fechaModificacion + "\r\n");
                            }
                        }
                        else
                        {
                            LOG("Acta " + acta.Id + " NO registrada en el servidor");
                        }
                        
                    }
                    else
                    {
                        LOG("Orden de servicio ya se encuentra registrada. Acta numero " + acta._number);
                    }

                }
                conexion.Close();
            }
            else
            {
                LOG("Error al conectarse con el servidor");
            }

        }

       

        private bool ExisteActa(String _number, Datos conexion)
        {
            bool resultado = false;
            if (conexion != null)
            {
                String sql = "SELECT _number FROM Actas WHERE _number = @_number";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@_number", SqlDbType.VarChar, 20).Value = _number;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado = true;
                        }
                    }

                }

            }

            return resultado;
        }

        private bool ExisteActaId(String id, Datos conexion)
        {
            bool resultado = false;
            if (conexion != null)
            {
                String sql = "SELECT _number FROM Actas WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 100).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado = true;
                        }
                    }

                }

            }

            return resultado;
        }

        
        

        private string ConsultaBandejaDisponible(String delegacion, String tipo, Datos conexion)
        {
            string bandeja = null;
            if (conexion != null)
            {

                
                String sql = "SELECT TOP 1 A.BandCodi, (SELECT count(_number) total "
	                    + " FROM Actas "
	                    + " WHERE A.BandCodi = Actas.Bandeja "
	                    + " AND Actas.EstadoActa <> '10') as Total"
                        + " FROM Bandejas A, BandejaZona "
                        + " WHERE A.BandTiBa = @tipo "
                        + " AND A.BandCodi = BandejaZona.BazoBand "
                        + " AND BandejaZona.BazoZona = @delegacion "
                        + " AND A.BandEsta = 1 "
                        + " AND A.BandTope > (SELECT count(_number) total "
	                    + " FROM Actas "
	                    + " WHERE A.BandCodi = Actas.Bandeja "
	                    + " AND Actas.EstadoActa <> '10') "
                        + " ORDER BY Total ASC ";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@delegacion", SqlDbType.VarChar, 100).Value = delegacion;
                    cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 100).Value = tipo;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bandeja = reader.GetString(0);
                        }
                    }

                }

            }


            return bandeja;
        }

        

        

        private void button1_Click_1(object sender, EventArgs e)
        {
            WSTarifa eca = new WSTarifa();
            eca.nic = "2210687";
            eca.fecha = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
            eca.CallWebService();
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ProcesoDistribucionActas();
        }

        private void ProcesoDistribucionActas()
        {
            LOG("INICIANDO DISTRIBUCION DE ACTAS");
            Datos conexion = new Datos();
            if (conexion != null)
            {
                GestionActa ga = new GestionActa();
                ga.conexion = conexion;
                LOG("1. Actualizando Tarifas");
                ga.ActualizarTarifaOrdenServicio();
                LOG("2. Actualizando Consumos");
                ga.ActualizarConsumosOrdenServicio();
                //LOG("3. Actualizando Protocolos");
                //ga.ActualizarProtocoloOrdenServicio();
                LOG("4. Actualizando Estados");
                ga.ActualizarEstadoOrdenServicio();
                LOG("5. Distribuyendo a bandejas de proceso/Supervisor");
                ga.DistribuirActas();
                LOG("5. Distribuyendo actas Subnormal");
                ga.DistribuirActasSubnormal();
                LOG("5. Distribuyendo actas Manuales");
                ga.DistribuirActasManuales();
                //LOG("6.Anticipando Energia");
                //ga.AnticipardaActas();
                LOG("7.Distribuyendo a bandejas de liquidación anticipada");
                ga.DistribuirLiquidacionAnticipadaActas();
                LOG("8.Distribuyendo a bandejas de Sin Anomalia");
                ga.DistribuirActasSinAnomalia();
                conexion.Close();
                LOG("CICLO FINALIZADO");

            }
            else
            {
                LOG("Error conectar base de datos, proceso distribucion");
            }
        }

        private void parametrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmConfig frm = new FrmConfig();
            frm.ShowDialog();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            FrmVerLog frm = new FrmVerLog();
            frm.ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.

            LOG("Iniciando DoWork Process Background");
            BackgroundWorker bw = (BackgroundWorker)sender;

            // Extract the argument.
            int arg = (int)e.Argument;

            // Start the time-consuming operation.
            e.Result = IniciarProceso();

            // If the operation was canceled by the user, 
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        public int IniciarProceso()
        {
            //FechaUltimaConsulta = FechaActual;
            FechaActual = DateTime.Now;
            txtUltimaConsulta.Text = "Ultima Consulta: " + FechaActual;
            listaActas = new List<Acta>();
            IdActas = new List<InfoActa>();
            Consultar(0);
            
            return 0;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // The user canceled the operation.
                LOG("Operation was canceled");
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                LOG(msg);
            }
            else
            {
                // The operation completed normally.
                string msg = String.Format("Result = {0}", e.Result);
                LOG(msg);
            }
        }

        

        public DateTime FromUnixTime(long unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(unixTime);
        }

        private void consumosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmWsConsumos frm = new FrmWsConsumos();
            frm.ShowDialog();
        }

        private void utilidadArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUtilidadFile frm = new FrmUtilidadFile();
            frm.ShowDialog();
        }

        private void actualizarTarifaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUpdateTarifa frm = new FrmUpdateTarifa();
            frm.ShowDialog();
        }

        private void convertirPDFATIFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmConvertPDFtoTIFF frm = new FrmConvertPDFtoTIFF();
            frm.ShowDialog();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
            notifyIcon1.Visible = !this.Visible;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Ocultar();
        }

        private void eliminarArchivosHGIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCleanDocumentos frm = new FrmCleanDocumentos();
            frm.ShowDialog();
        }

        private void distribuirActasRechazadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDistribuir frm = new FrmDistribuir();
            frm.ShowDialog();
        }

        private void subirActasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UploadActas frm = new UploadActas();
            frm.ShowDialog();
        }

        private void actualizarProtocolosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUpdateActasProtocolo frm = new FrmUpdateActasProtocolo();
            frm.ShowDialog();
        }

        private void distribuirActasAsignadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDistribuirAsigando frm = new FrmDistribuirAsigando();
            frm.ShowDialog();
        }

        private void distribuirActasSinAnomaliaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDistribuirBandejaSinAnomalia frm = new FrmDistribuirBandejaSinAnomalia();
            frm.ShowDialog();
        }

        private void actualizarOrdenesResueltasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmActualizarEstadoOrden frm = new FrmActualizarEstadoOrden();
            frm.ShowDialog();
        }

        private void obtenerFotosActaHDAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmObtenerFotosHDA frm1 = new FrmObtenerFotosHDA();
            frm1.ShowDialog();
        }

        private void subirArchivoClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadFileClientes frm = new FrmUploadFileClientes();
            frm.Show();
        }

        private void distribuirActasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDistribuirActas frm = new FrmDistribuirActas();
            frm.Show();
        }

        private void cargaImagenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmImagenGuia frm = new FrmImagenGuia();
            frm.Show();
        
        
        }

        private void cargaArchivoImagenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadFileImagen frm = new FrmUploadFileImagen();
            frm.Show();
        }

        private void cargaGuiasHGI2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadGuia frm = new FrmUploadGuia();
            frm.Show();
        }

        private void extraerDocumentosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmExtraerDocumentosActas frm = new FrmExtraerDocumentosActas();
            frm.Show();
        }

        private void subirDocumentosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSubirDocumentos frm = new FrmSubirDocumentos();
            frm.Show();
        }

       

    }


}
