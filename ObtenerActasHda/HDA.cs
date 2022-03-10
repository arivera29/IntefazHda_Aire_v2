using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerActasHda
{
    
    class HDA
    {
        private List<InfoActa> IdActas;
        private List<Acta> listaActas;
        public DateTime FechaUltimaConsulta { set; get; }
        private int numPages = 0;
        CultureInfo MyCultureInfo = new CultureInfo("es-CO");
        long ts = 0;


        public HDA() 
        {
            listaActas = new List<Acta>();
            IdActas = new List<InfoActa>();
            FechaUltimaConsulta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc);
            Console.WriteLine("Inciando consulta desde la fecha: " + FechaUltimaConsulta);
            LOG("Inciando consulta desde la fecha: " + FechaUltimaConsulta);
            //UpdateDelegacionContrata();
            ts = (long)Math.Floor((FechaUltimaConsulta - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }
        public HDA(DateTime fecha)
        {
            listaActas = new List<Acta>();
            IdActas = new List<InfoActa>();
            Console.WriteLine("Inciando consulta desde la fecha: " + fecha);
            LOG("Inciando consulta desde la fecha: " + fecha);
            FechaUltimaConsulta = fecha;
            //UpdateDelegacionContrata();
            ts = (long)Math.Floor((FechaUltimaConsulta - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public HDA(List<String> actas)
        {
            listaActas = new List<Acta>();
            IdActas = new List<InfoActa>();
            LOG("Procesando ordenes de servicio. Cantidad: " + actas.Count);
            ConsultarOrdenServicio(actas);
        }

        public void Start()
        {

            Consultar(0);

        }

        private void UpdateDelegacionContrata()
        {
            Datos conexion = new Datos();
            if (conexion.getConection().State == ConnectionState.Open)
            {
                String sql = "EXEC UpdateDelegacionContrata;";
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
            else
            {
                Console.WriteLine("Error al conectarse con el servidor");
            }
        }

        public void Consultar(int page)
        {

            LOG("Consultando Web Service HDA");
            try
            {

                //long ts = (long)Math.Floor((FechaUltimaConsulta - new DateTime(1970, 1, 1)).TotalMilliseconds);
                string url = ConfigVars.UrlWsHda() + "?fromMs=" + ts + "&page=" + page;

                LOG("Procesando url: " + url);
                WebRequest req = WebRequest.Create(url);

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

        public void ConsultarOrdenServicio(List<String> actas)
        {
            LOG("Consultando Web Service HDA orden de servicio");
            if (actas.Count > 0)
            {
                foreach (String acta in actas)
                {
                    try
                    {
                        //Get method
                        string url = ConfigVars.UrlWsHda() + "/byNumber/" + acta;
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
                                Console.WriteLine(sb.ToString());
                                ProcesarOrdenServicio(sb,acta);
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

                GuardarActaBD(false);

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
                        string url = ConfigVars.UrlWsHda() + "/" + acta.Id;
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

                GuardarActaBD(true);

            }
        }
        private void GuardarActaBD(Boolean updateFecha)
        {
            LOG("Iniciando carga de actas en el base de datos");
            LOG("Numero de actas descargadas: " + listaActas.Count);
            Datos conexion = new Datos();
            if (conexion != null)
            {

                foreach (Acta acta in listaActas)
                {

                    if (!ExisteActa(acta._number, conexion))
                    {

                        LOG("Guardando en la base de datos acta " + acta.Id);
                        GestionActa ga = new GestionActa();
                        ga.conexion = conexion;
                        if (ga.AgregarActa(acta))
                        {
                            LOG("Acta guardada correctamente " + acta.Id);
                            FechaUltimaConsulta = acta.fechaModificacion;
                            if (updateFecha)
                            {
                                UpdateFechaUltimaConsulta(FechaUltimaConsulta);
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

        private void UpdateFechaUltimaConsulta(DateTime fecha)
        {
            String filename = Environment.CurrentDirectory + @"\datehda.txt";
            using (StreamWriter outfile = new StreamWriter(@filename))
            {
                LOG(String.Format("Actualizando fecha consulta WS: Año {0} Mes {1} dia {2} hora {3} minutos {4}", fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute));
                String data = String.Format("{0},{1},{2},{3},{4},{5}",fecha.Year,fecha.Month,fecha.Day,fecha.Hour,fecha.Minute,fecha.Second);
                outfile.Write(data);
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

        public void Procesar(StringBuilder sb)
        {
            LOG("Procesando JSON");

            try
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    JObject obj = JObject.Parse(sb.ToString());

                    JArray a = (JArray)obj["records"];

                    for (int x = 0; x < a.Count; x++)
                    {

                        String orden = (string)a[x]["numeroOS"];
                        String idActa = (string)a[x]["id"];

                        if (!ExisteActaId((string)a[x]["id"], conexion))
                        {
                            InfoActa acta = new InfoActa();
                            acta.Id = (string)a[x]["id"];
                            string strFecha = (string)a[x]["fechaModificacion"]["COT"];
                            DateTime fecha = DateTime.ParseExact(strFecha,"dd/MM/yyyy HH:mm:ss K",MyCultureInfo);
                            //DateTime fecha = Convert.ToDateTime(strFecha);
                            LOG(String.Format("Fecha de modificacion: Año {0} Mes {1} dia {2} hora {3} minutos {4}",fecha.Year,fecha.Month, fecha.Day, fecha.Hour, fecha.Minute));
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

        private bool ExisteActaId(String id, Datos conexion)
        {
            bool resultado = false;
            if (conexion != null)
            {
                String sql = "SELECT _number FROM Actas WITH(NOLOCK) WHERE id = @id";
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
                LOG("Fecha _clientCloseTS: " + o["_clientCloseTs"]);
                acta.codigoEmpresa = ConvertString(o["codigoEmpresa"], "codigoEmpresa");
                acta.CodigoContrata = acta.codigoEmpresa.Substring(1, 6);
                acta.comentario1 = ConvertString(o["comentario1"], "comentario1");
                acta.comentario2 = ConvertString(o["comentario2"], "comentario2");
                acta.nic = ConvertString(o["nic"], "nic");
                acta.direccion = ConvertString(o["direccion"], "direccion");
                acta.departamento = ConvertString(o["departamento"], "departamento");
                acta.municipio = ConvertString(o["municipio"], "municipio");
                acta.localidad = ConvertString(o["localidad"], "localidad");

                LOG("ID de delegacion recibida: " + ConvertString(o["territorioCodigo"], "territorioCodigo"));
                if (!isNull(o["territorioCodigo"]))
                {
                    acta.delegacion = ConvertString(o["territorioCodigo"], "territorioCodigo");
                }else
                {
                    acta.delegacion = "-1"; // Delegacion no encontrada
                }

                LOG("ID de delegeacion obteniedo: " + acta.delegacion);

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

                                LOG("Validando materiales Accion " + accion.CodigoAccion + " Paso " + accion.CodigoPaso);
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
                                }
                                else
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
                                        sello.Color = ConvertString(sellosExistentes[y]["color"]["displayName"], "color->displayName");
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2()  + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2()  + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
                    foto.Url = url;
                    foto.Tipo = 12;  // 12 - Foto Firma Tecnico Particular
                    foto.Firma = 1;
                    acta.fotos.Add(foto);
                }

                #endregion

                LOG("Agregando acta a la cola:" + acta._number);
                LOG("Delegacion: " + acta.delegacion);
                listaActas.Add(acta);



            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Trace: " + ex.StackTrace.ToString());
            }

        }

        public void ProcesarOrdenServicio(StringBuilder sb,String num_os)
        {
            LOG("Procesando JSON Orden de servicio " + num_os);
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
                acta.Id = num_os;
                acta._clientCloseTs = ConvertString(o["_clientCloseTs"], "_clientCloseTs");
                acta.fechaModificacion = DateTime.ParseExact(ConvertString(o["_clientCloseTs"], "_clientCloseTs"),"dd/MM/yyyy HH:mm:ss zzz",null);
                acta._number = ConvertString(o["_number"], "_number");
                acta.numeroLote = ConvertString(o["numeroLote"], "numeroLote");
                
                LOG("Fecha _clientCloseTS: " + o["_clientCloseTs"]);
                acta.codigoEmpresa = ConvertString(o["codigoEmpresa"], "codigoEmpresa");
                acta.CodigoContrata = acta.codigoEmpresa.Substring(1, 6);
                acta.comentario1 = ConvertString(o["comentario1"], "comentario1");
                acta.comentario2 = ConvertString(o["comentario2"], "comentario2");
                acta.nic = ConvertString(o["nic"], "nic");
                acta.direccion = ConvertString(o["direccion"], "direccion");
                acta.departamento = ConvertString(o["departamento"], "departamento");
                acta.municipio = ConvertString(o["municipio"], "municipio");
                acta.localidad = ConvertString(o["localidad"], "localidad");

                LOG("ID de delegacion recibida: " + ConvertString(o["territorioCodigo"], "territorioCodigo"));
                if (!isNull(o["territorioCodigo"]))
                {
                    acta.delegacion = ConvertString(o["territorioCodigo"], "territorioCodigo");
                }
                else
                {
                    acta.delegacion = "-1"; // Delegacion no encontrada
                }

                LOG("ID de delegeacion obteniedo: " + acta.delegacion);

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

                                LOG("Validando materiales Accion " + accion.CodigoAccion + " Paso " + accion.CodigoPaso);
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
                                }
                                else
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
                                        sello.Color = ConvertString(sellosExistentes[y]["color"]["displayName"], "color->displayName");
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                                    foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                        foto.Path = ConfigVars.FolderImagenesHGI2() + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
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
                    foto.Path = ConfigVars.FolderImagenesHGI2() + acta._number + "_" + foto.Id + ".jpg";
                    foto.Url = url;
                    foto.Tipo = 12;  // 12 - Foto Firma Tecnico Particular
                    foto.Firma = 1;
                    acta.fotos.Add(foto);
                }

                #endregion

                LOG("Agregando acta a la cola:" + acta._number);
                LOG("Delegacion: " + acta.delegacion);
                listaActas.Add(acta);



            }
            catch (Exception ex)
            {
                LOG(ex.Message + " Trace: " + ex.StackTrace.ToString());
            }

        }
        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\OBTENER_ACTAS_HDA_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
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

        static string NullToString(object Value)
        {
            return Value == null ? "" : Value.ToString();
        }


    }

    class InfoActa
    {
        public String Id { set; get; }
        public DateTime fechaModificacion { set; get; }
    }
}
