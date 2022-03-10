using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HgiReports
{
    class ReporteGeneral
    {
        public String Separator {get; set;}
        public String Path { set; get; }
        public String Filename { set; get; }
        public String FilenameLog { set; get; }
        public String FilenameZip { set; get; }

        private String BeginFilename;
        public Boolean FiltroRechazadas {set; get; }
        public Boolean FiltroPendientes { set; get; }

        public Boolean FiltroMensajeria { set; get; }
        public Boolean ReporteLiquidacion { set; get; }

        public Boolean FiltroGeneral { set; get; }

        private List<int> preguntas;

        private Datos conex;

        public bool debug { set; get; }
        public ILog log { get; set; }

        public int year { get; set; }

        public ReporteGeneral(String beginFilename, ILog log)
        {
            this.log = log;
            
            if (beginFilename.Equals("")) {
                this.BeginFilename=  "RP_GENERAL";
            }
            else
            {
                this.BeginFilename = beginFilename;
            }
            this.FiltroRechazadas = false;
            this.FiltroPendientes = false;
            this.ReporteLiquidacion = false;
            this.FiltroGeneral = false;
            this.Filename = this.generateFilename();
            this.FilenameLog = this.generateFilenameLog();
            this.FilenameZip = Filename + ".zip";
            this.debug = false;
            this.year = DateTime.Now.Year; // Año Actual
            conex = new Datos();
        }

        private String parseValue(String cadena) {
            cadena = cadena.Replace(this.Separator, "_");
            cadena = cadena.Replace("\n", "");
            cadena = cadena.Replace("\r", "");
            cadena = cadena.Replace("\t", "");

            return cadena;
        }

        public Boolean Comprimir()
        {
            if (File.Exists(@Filename))
            {
                try
                {
                    log.Info("Creando archivo: " + FilenameZip);
                    FileStream fsOut = File.Create(FilenameZip);
                    ZipOutputStream zip = new ZipOutputStream(fsOut);

                    log.Info("Agregando archivo al ZIP: " + Filename);
                    FileInfo fi = new FileInfo(Filename);
                    ZipEntry newEntry = new ZipEntry(fi.Name);
                    newEntry.DateTime = fi.LastWriteTime;
                    newEntry.Size = fi.Length;
                    zip.PutNextEntry(newEntry);

                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(Filename))
                    {
                        StreamUtils.Copy(streamReader, zip, buffer);
                    }

                    zip.IsStreamOwner = true;
                    zip.Close();
                }
                catch (Exception e)
                {
                    log.Error("Error Comprimiendo Archivo. " + e.StackTrace);
                    return false;
                }

            }
            else
            {
                return false;
            }

            return true;
        }

        public void GenerarReporte()
        {
            
            this.RemoveFile();
            this.WriteLine(this.getHeader());
            
            
            String sql = "select _number," // 0
                    + "nic,"    // 1
                    + "direccion,"  // 2
                    + "codigoTarifa,"   // 3
                    + "tipoCliente,"    // 4
                    + "estrato,"    // 5
                    + "departamento,"   // 6
                    + "municipio,"  // 7
                    + "localidad,"  // 8
                    + "CONVERT(VARCHAR,ISNULL(cargaContratada,0)),"    // 9
                    + "CONVERT(VARCHAR,ISNULL(fechaInicioIrregularidad,''),103),"   // 10
                    + "codigoEmpresa,"  // 11
                    + "empresaOperario,"    // 12
                    + "tipoOrdenServicio,"  // 13
                    + "ISNULL(tipoServicio,'') tipoServicio,"   // 14
                    + "ISNULL(comentario1,'') comentario1,"    // 15
                    + "e2.EsAcDesc as EstadoActaAnterior," // 16
                    + "e1.EsAcDesc as EstadoActaActual,"    // 17
                    + "CONVERT(VARCHAR,protocolo),"  // 18
                    + "observaciones,"  // 19
                    + "nombreOperario," // 20
                    + "apellido1Operario,"  // 21
                    + "apellido2Operario,"  // 22
                    + "cedulaOperario," // 23
                    + "empresaOperario,"    // 24
                    + "zonas.ZonaDesc as Delegacion,"   // 25
                    + "Observaciones,"  // 26
                    + "tipoCenso,"  // 27
                    + "CONVERT(VARCHAR,ISNULL(censoCargaInstalada,0)),"    // 28
                    + "CONVERT(VARCHAR,ISNULL(ValorTarifa,0)),"    // 29
                    + "CONVERT(VARCHAR,ISNULL(ValorEcdf,0)),"   // 30
                    + "CONVERT(VARCHAR,osResuelta)," // 31
                    + "CONVERT(VARCHAR,ISNULL(fechaCarga,''),103)," // 32
                    + "usuarioCarga,"   // 33
                    + "TipoActa.Descripcion as TipoActa,"   // 34
                    + "CASE "
	                + "WHEN Actas.subnormal=1 THEN 'SUBNORMAL'"
	                + "WHEN Actas.ActaManual=1 THEN 'MANUAL'"
	                + "ELSE 'HDA' END AS FuenteIngreso,"    // 35
                    + "CONVERT(VARCHAR,ISNULL(FechaConfirmacion,''),103),"  // 36
                    + "CONVERT(VARCHAR,ISNULL(FechaAsignacionBandeja,''),103)," // 37
                    + "Incidencia," // 38
                    + "CONVERT(VARCHAR,Bandeja),"   // 39
                    + "CONVERT(VARCHAR,ISNULL(FRAnticipado,0)),"   // 40
                    + "CONVERT(VARCHAR,ISNULL(EnergiaAnticipada,0)),"   // 41
                    + "CONVERT(VARCHAR,ISNULL(_clientCloseTs,''),103),"   // 42
                    + "Bandejas.BandDesc,"  // 43
                    + "CONVERT(VARCHAR,Actas.BandejaAnterior),"  // 44
                    + "e1.EsAcDesc,"    // 45
                    + "Bandejas.BandResp,"  // 46
                    + "Bandejas.BandCoor,"  // 47
                    + "TipoBandeja.TiBaDesc as TipoBandeja,"    // 48
                    + "CONVERT(VARCHAR,ISNULL(fechaOrden,''),103)," // 49
                    + "CONVERT(VARCHAR,ISNULL(fechaOrden,''),108)," // 50
                    + "CONVERT(VARCHAR,ISNULL(fechaModificacionActa,''),103),"  // 51
                    + "Incidencia, " // 52
                    + "codNovedad," // 53
                    + "DescTiNov," // 54
                    + "estNovedad," // 55
                    + "obsNovedad," // 56
                    + "CONVERT(VARCHAR,ISNULL(fechaNovedad,''),103)," // 57
                    + "CONVERT(VARCHAR,ISNULL(fechaCNovedad,''),103), " // 58
                    + "CASE WHEN Confirmada=1 THEN 'SI' ELSE 'NO' END as Confirmada " // 59
                    + ",tipProcesoTec " // 60
                    + ",tipProceso " // 61
                    + " FROM Actas with(nolock),EstadosActas e1, EstadosActas e2, Zonas , TipoActa, Bandejas, TipoBandeja,TipoNovedad"
                    + " WHERE Actas.EstadoActa = e1.EsAcCodi"
                    + " and Actas.EstadoAnteriorActa = e2.EsAcCodi"
                    + " and Delegacion = Zonas.ZonaCodi"
                    + " and Actas.IdTipoActa = tipoActa.Id"
                    + " and Actas.codNovedad = TipoNovedad.Id"
                    + " and Actas.Bandeja = Bandejas.BandCodi"
                    + " and Bandejas.BandTiBa = TipoBandeja.TiBaCodi";



            if (this.FiltroRechazadas) { // Generar informe solo con actas rechazadas y en rechazo definitivo
                sql += " and Actas.estadoActa IN (5,14) " ;
                // Filtro de lois ultimos 6 meses.
                sql += " and Actas.FechaCarga >= DATEADD(month, -8, SYSDATETIME()) ";
            }

            if (this.FiltroGeneral)
            {
                //sql += " and YEAR(Actas.FechaCarga) = " + this.year;
                sql += " and Actas.FechaCarga >= DATEADD(month, -12, SYSDATETIME()) ";
            }
            
            if (this.FiltroPendientes)
            {
                sql += " and Actas.estadoActa IN (1,2,3,4,6) ";
            }

            if (this.ReporteLiquidacion)
            {
                //sql += " and YEAR(Actas.FechaCarga) = " + this.year;
                sql += " and Actas.FechaCarga >= DATEADD(month, -6, SYSDATETIME()) ";
            }

            if (this.FiltroMensajeria)
            {
                sql += " and Actas.estadoActa in (10,11) ";
                if (this.ReporteLiquidacion)
                {
                    sql += " AND  YEAR(Actas.FechaCarga) =  " + this.year;
                }
            }

            log.Info("Conectando al servidor de base de datos.");
            Datos conexion = new Datos();
            if (conexion != null)
            {
                log.Info("Conexion OK.");
                WriteLineLog("Ejecutando consulta SQL: " + sql);
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    WriteLineLog("Iniciando consulta SQL");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada. OK");
                        WriteLineLog("Leyendo registros");
                        int fila = 1;
                        while (reader.Read())
                        {
                            WriteLineLog("Leyendo fila: " + fila);
                            
                            Int32 acta = 0;
                            try
                            {
                                acta = reader.GetInt32(0);
                                WriteLineLog("Generando linea para acta " + acta);
                                
                                String linea = reader.GetInt32(0) + this.Separator; // Numero Acta
                                //System.Console.WriteLine("0:" + reader.GetInt32(0));
                                linea += parseValue(reader.IsDBNull(1) ? "" : reader.GetString(1)) + this.Separator;  // nic
                                //System.Console.WriteLine("1:" + reader.GetString(1));
                                linea += parseValue(reader.IsDBNull(2) ? "" : reader.GetString(2)) + this.Separator;  // Dirección
                                //System.Console.WriteLine("2:" + reader.GetString(2));
                                linea += parseValue(reader.IsDBNull(3) ? "" : reader.GetString(3)) + this.Separator;  // Codigo Tarifa
                                //System.Console.WriteLine("3:" + reader.GetString(3));
                                linea += parseValue(reader.IsDBNull(4) ? "" : reader.GetString(4)) + this.Separator;  // Tipo Cliente
                                //System.Console.WriteLine("4:" + reader.GetString(4));
                                linea += parseValue(reader.IsDBNull(5) ? "" : reader.GetString(5)) + this.Separator;  // Estrato
                                //System.Console.WriteLine("5:" + reader.GetString(5));
                                linea += parseValue(reader.IsDBNull(6) ? "" : reader.GetString(6)) + this.Separator;  // Departamento
                                //System.Console.WriteLine("6:" + reader.GetString(6));
                                linea += parseValue(reader.IsDBNull(7) ? "" : reader.GetString(7)) + this.Separator;  // Municipio
                                //System.Console.WriteLine("7:" + reader.GetString(7));
                                linea += parseValue(reader.IsDBNull(8) ? "" : reader.GetString(8)) + this.Separator;  // Localidad
                                //System.Console.WriteLine("8:" + reader.GetString(8));
                                // Obtener consumos
                                WriteLineLog("Consultando Consumos del acta " + acta);
                                linea += this.ObtenerConsumos(acta) + this.Separator;
                                linea += parseValue(reader.IsDBNull(9) ? "" : reader.GetString(9)) + this.Separator;  // Carga Contratada
                                //System.Console.WriteLine("9:" + reader.GetString(9));
                                linea += parseValue(reader.IsDBNull(10) ? "" : reader.GetString(10)) + this.Separator;  // Fecha Inicio Irregularidad
                                //System.Console.WriteLine("10:" + reader.GetString(10));
                                linea += parseValue(reader.IsDBNull(11) ? "" : reader.GetString(11)) + this.Separator; // Empresa
                                //System.Console.WriteLine("11:" + reader.GetString(11));
                                linea += parseValue(reader.IsDBNull(12) ? "" : reader.GetString(12)) + this.Separator;  // Contrata
                                //System.Console.WriteLine("12:" + reader.GetString(12));
                                linea += parseValue(reader.IsDBNull(13) ? "" : reader.GetString(13)) + this.Separator; // Tipo Orde De Servicio
                                //System.Console.WriteLine("13:" + reader.GetString(13));
                                linea += parseValue(reader.IsDBNull(14) ? "" : reader.GetString(14)) + this.Separator; // Tipo Servicio
                                //System.Console.WriteLine("14:" + reader.GetString(14));
                                linea += parseValue(reader.IsDBNull(15) ? "" : reader.GetString(15)) + this.Separator; // Comentario 1
                                //System.Console.WriteLine("15:" + reader.GetString(15));
                                linea += parseValue(reader.IsDBNull(16) ? "" : reader.GetString(16)) + this.Separator; // Estado_Anterior_del_Acta
                                //System.Console.WriteLine("16:" + reader.GetString(16));
                                linea += parseValue(reader.IsDBNull(17) ? "" : reader.GetString(17)) + this.Separator; // Estado_del_Acta
                                //System.Console.WriteLine("17:" + reader.GetString(17));
                                linea += parseValue(reader.IsDBNull(18) ? "" : reader.GetString(18)) + this.Separator;  // protocolo
                                //System.Console.WriteLine("18:" + reader.GetString(18));
                                linea += parseValue(reader.IsDBNull(19) ? "" : reader.GetString(19)) + this.Separator;  // Observaciones
                                //System.Console.WriteLine("19:" + reader.GetString(19));
                                linea += parseValue(reader.IsDBNull(20) ? "" : reader.GetString(20)) + this.Separator; // Nombre_Operario
                                //System.Console.WriteLine("20:" + reader.GetString(20));
                                linea += parseValue(reader.IsDBNull(21) ? "" : reader.GetString(21)) + this.Separator; // Primer_Apellido_Operario
                                //System.Console.WriteLine("21:" + reader.GetString(21));
                                linea += parseValue(reader.IsDBNull(22) ? "" : reader.GetString(22)) + this.Separator; // Segundo_Apellido_Operario
                                //System.Console.WriteLine("22:" + reader.GetString(22));
                                linea += parseValue(reader.IsDBNull(23) ? "" : reader.GetString(23)) + this.Separator; // Cedula_Operario
                                //System.Console.WriteLine("23:" + reader.GetString(23));
                                linea += parseValue(reader.IsDBNull(24) ? "" : reader.GetString(24)) + this.Separator; // Empresa_Operario
                                //System.Console.WriteLine("24:" + reader.GetString(24));
                                linea += parseValue(reader.IsDBNull(25) ? "" : reader.GetString(25)) + this.Separator; // Delegación
                                //System.Console.WriteLine("25:" + reader.GetString(25));
                                //Obtener información de Medidor
                                WriteLineLog("Consultando Información del medidor del acta " + acta);
                                linea += this.ObtenerInformacionMedidor(acta) + this.Separator; // Tipo_Rev_Apa_Exis
                                // Obtener Anomalias
                                WriteLineLog("Consultando Información de anomalias del acta " + acta);
                                linea += this.ObtenerAnomalias(acta) + this.Separator;  //

                                linea += parseValue(reader.IsDBNull(26) ? "" : reader.GetString(26)) + this.Separator;  // Observacion_Anomalia
                                //System.Console.WriteLine("26:" + reader.GetString(26));
                                linea += parseValue(reader.IsDBNull(27) ? "" : reader.GetString(27)) + this.Separator;  // tipoCenso
                                //System.Console.WriteLine("27:" + reader.GetString(27));
                                linea += parseValue(reader.IsDBNull(28) ? "" : reader.GetString(28)) + this.Separator;  // Carga_Instalada
                                //System.Console.WriteLine("28:" + reader.GetString(28));
                                linea += parseValue(reader.IsDBNull(29) ? "" : reader.GetString(29)) + this.Separator;  // Tarifa
                                //System.Console.WriteLine("29:" + reader.GetString(29));

                                linea += parseValue(reader.IsDBNull(30) ? "" : reader.GetString(30)) + this.Separator;  // ECDF
                                //linea += parseValue(reader.IsDBNull(30) ? "0" : ObtenerECDF(acta).ToString()) + this.Separator;
                                //System.Console.WriteLine("30:" + reader.GetString(30));
                                linea += parseValue(reader.IsDBNull(31) ? "" : reader.GetString(31)) + this.Separator;  // Orden_de_Servicio_Resuelta
                                //System.Console.WriteLine("31:" + reader.GetString(31));
                                linea += "" + this.Separator;  // Voltaje_R
                                linea += "" + this.Separator; // Voltaje_S
                                linea += "" + this.Separator; // Voltaje_T
                                linea += "" + this.Separator;  // Fase_R
                                linea += "" + this.Separator;  // Fase_S
                                linea += "" + this.Separator;  // Fase_T
                                linea += parseValue(reader.IsDBNull(32) ? "" : reader.GetString(32)) + this.Separator;  // Fecha_Carga
                                //System.Console.WriteLine("32:" + reader.GetString(32));
                                linea += parseValue(reader.IsDBNull(33) ? "" : reader.GetString(33)) + this.Separator;  // Usuario_Carga
                                //System.Console.WriteLine("33:" + reader.GetString(33));
                                linea += parseValue(reader.IsDBNull(34) ? "" : reader.GetString(34)) + this.Separator;  // Tipo Acta
                                //System.Console.WriteLine("34:" + reader.GetString(34));
                                linea += parseValue(reader.IsDBNull(35) ? "" : reader.GetString(35)) + this.Separator;  // Fuente de Ingreso
                                //System.Console.WriteLine("35:" + reader.GetString(35));
                                linea += parseValue(reader.IsDBNull(36) ? "" : reader.GetString(36)) + this.Separator;  // Fecha Confirmacion
                                //System.Console.WriteLine("36:" + (reader.IsDBNull(42) ? "" : reader.GetString(36)));
                                linea += parseValue(reader.IsDBNull(37) ? "" : reader.GetString(37)) + this.Separator;  // Fecha Asignacion Bandeja
                                //System.Console.WriteLine("37:" + reader.GetString(37));
                                //linea += parseValue(reader.IsDBNull(43) ? "" : reader.GetString(43)) + this.Separator;  // Bandeja
                                //System.Console.WriteLine("43:" + reader.GetString(43));
                                linea += parseValue(reader.IsDBNull(38) ? "" : reader.GetString(38)) + this.Separator;  // Incidencia
                                //System.Console.WriteLine("38:" + reader.GetString(38));
                                linea += parseValue(reader.IsDBNull(39) ? "" : reader.GetString(39)) + this.Separator;  // Codigo_Bandeja
                                //System.Console.WriteLine("39:" + reader.GetString(39));
                                //linea += parseValue(reader.IsDBNull(40) ? "" : reader.GetString(40)) + this.Separator;  // FR_Anticipado
                                //System.Console.WriteLine("40:" + reader.GetString(40));
                                linea += "" + this.Separator;  // Fecha Anticipada
                                linea += parseValue(reader.IsDBNull(41) ? "" : reader.GetString(41)) + this.Separator;  // Valor Energia_Anticipada
                                //System.Console.WriteLine("41:" + reader.GetString(41));
                                linea += parseValue(reader.IsDBNull(42) ? "" : reader.GetString(42)) + this.Separator;  // Fecha_de_Acta
                                //System.Console.WriteLine("42:" + reader.GetString(42));
                                linea += parseValue(reader.IsDBNull(43) ? "" : reader.GetString(43)) + this.Separator;  // Bandeja
                                //System.Console.WriteLine("43:" + reader.GetString(43));
                                linea += parseValue(reader.IsDBNull(44) ? "" : reader.GetString(44)) + this.Separator;  // Bandeja_Anterior
                                //System.Console.WriteLine("44:" + reader.GetString(44));
                                linea += parseValue(reader.IsDBNull(45) ? "" : reader.GetString(45)) + this.Separator;  // Estado
                                //System.Console.WriteLine("45:" + reader.GetString(45));
                                linea += parseValue(reader.IsDBNull(46) ? "" : reader.GetString(46)) + this.Separator;  // Responsable
                                //System.Console.WriteLine("46:" + reader.GetString(46));
                                linea += parseValue(reader.IsDBNull(47) ? "" : reader.GetString(47)) + this.Separator;  // Coordinador
                                //System.Console.WriteLine("47:" + reader.GetString(47));
                                linea += parseValue(reader.IsDBNull(48) ? "" : reader.GetString(48)) + this.Separator;  // Tipo_De_Bandeja
                                //System.Console.WriteLine("48:" + reader.GetString(48));
                                linea += parseValue(reader.IsDBNull(49) ? "" : reader.GetString(49)) + this.Separator;  // Fecha_de_Orden
                                //System.Console.WriteLine("49:" + reader.GetString(49));
                                linea += parseValue(reader.IsDBNull(50) ? "" : reader.GetString(50)) + this.Separator;  // Hora_de_Orden
                                //System.Console.WriteLine("50:" + reader.GetString(50));
                                linea += "" + this.Separator;  // FR
                                // Anotaciones
                                WriteLineLog("Consultando Anotaciones " + acta);
                                linea += this.ObtenerAnotaciones(acta) + this.Separator;
                                // Información Mensajeria
                                WriteLineLog("Consultando información de mensajeria del acta " + acta);
                                linea += ObtenerInformacionMensajeria(acta) + this.Separator;  // Fecha_Envio_a_Mensajeria
                                linea += parseValue(reader.IsDBNull(51) ? "" : reader.GetString(51)) + this.Separator;  // Fecha Ultima Actualización
                                //System.Console.WriteLine("51:" + reader.GetString(51));
                                WriteLineLog("Consultando información de rechazo del acta " + acta);
                                linea += this.ObtenerInformacionRechazo(acta) + this.Separator;  // Usuario Rechazo
                                //linea += parseValue(reader.IsDBNull(52) ? "" : reader.GetString(52)) + this.Separator;  // Incidencia
                                //System.Console.WriteLine("52:" + reader.GetString(52));
                                linea += parseValue(reader.IsDBNull(53) ? "" : reader.GetInt32(53).ToString()) + this.Separator;  // Codigo Novedad
                                linea += parseValue(reader.IsDBNull(54) ? "" : reader.GetString(54)) + this.Separator;  // Descripcion Novedad
                                linea += parseValue(reader.IsDBNull(55) ? "" : reader.GetString(55)) + this.Separator;  // estado Novedad
                                linea += parseValue(reader.IsDBNull(56) ? "" : reader.GetString(56)) + this.Separator;  // Observacion Novedad
                                linea += parseValue(reader.IsDBNull(57) ? "" : reader.GetString(57)) + this.Separator;  // fecha Novedad
                                linea += parseValue(reader.IsDBNull(58) ? "" : reader.GetString(58)) + this.Separator;  // fecha Cierre Novedad
                                linea += parseValue(reader.IsDBNull(59) ? "" : reader.GetString(59)) + this.Separator;  // Confirmada
                                linea += parseValue(reader.IsDBNull(60) ? "" : reader.GetString(60)) + this.Separator;  // tipo de proceso Tecnico
                                linea += parseValue(reader.IsDBNull(61) ? "" : reader.GetString(61)) + this.Separator;  // tipo de proceso

                                if (ReporteLiquidacion)
                                {
                                    
                                    linea += parseValue(reader.IsDBNull(41) ? "" : reader.GetString(41)) + this.Separator;  // Valor ECDF Anticiapda
                                    linea += parseValue(reader.IsDBNull(40) ? "" : reader.GetString(40)) + this.Separator;  // FR Anticipado
                                    linea += parseValue(reader.IsDBNull(30) ? "" : reader.GetString(30)) + this.Separator;  // ECDF liquidada
                                    linea += parseValue(reader.IsDBNull(3) ? "" : reader.GetString(3)) + this.Separator;  // Codigo Tarifa
                                    linea += parseValue(reader.IsDBNull(29) ? "" : reader.GetString(29)) + this.Separator;  // Valor Tarifa
                                    linea += parseValue(reader.IsDBNull(34) ? "" : reader.GetString(34)) + this.Separator;  // Tipo de Acta
                                    linea += parseValue(reader.IsDBNull(35) ? "" : reader.GetString(35)) + this.Separator;  // Clasificacion

                                    WriteLineLog("Consultando información de liquidación del acta " + acta);
                                    linea += this.ObtenerInfoLiquidacion(acta);
                                    WriteLineLog("Consultando información de preguntas del acta " + acta);
                                    linea += obtenerPreguntas(acta);


                                }

                                WriteLineLog("Escribiendo linea en el archivo del acta " + acta);
                                this.WriteLine(linea);
                            }
                            catch (SqlException e)
                            {
                                log.Error("Error Acta: " + acta + "\r\n" + e.StackTrace + "\r\nLinea:" + e.LineNumber);
                            }
                            catch (Exception e)
                            {
                                log.Error("Error Acta: " + acta + "\r\n" + e.StackTrace + "\r\nLinea:");
                            }
                            fila++;
                        }
                        log.Info("Finalizando lectura de registros.");
                        log.Info("Total registros. " + fila);
                        log.Info("Finalización escritura de archivo. OK");

                    }
                }
                log.Info("Cerrando conexión a la base de datos");
                conexion.Close();
            }
            else
            {
                log.Error("Error al conectarse con el servidor de datos");
            }

            if (conex != null)
            {
                conex.Close();
            }

        }

        private String ObtenerInfoLiquidacion(Int32 acta)
        {
            String linea = String.Format("{0}{0}{0}{0}{0}{0}", this.Separator);
            try
            {
                String sql = "SELECT AcLiUser, MeLiNomb, AcLiMese,AcLiDeFo,AcLiObse,AcLiVaC1 "
                + " FROM Acta_Liquidacion,MetodoLiquidacion "
                + " WHERE AcLiMeLi=MeLiCoDi AND AcLiActa = @acta";
                WriteLineLog("Ejecutando consulta SQL " + sql);
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada. OK");
                        WriteLineLog("Leyendo registro");
                        if (reader.Read())
                        {
                            WriteLineLog("Gerenado linea información de liquidación");
                            linea = String.Format("{0}{1}", reader.GetString(0), this.Separator);  // Usuario liquidacion
                            //this.WriteLineLog("Usuario Liquidacion");
                            linea += String.Format("{0}{1}", reader.GetString(1), this.Separator); // Metodo liquidacion
                            //this.WriteLineLog("Metodo Liquidacion");
                            linea += String.Format("{0}{1}", reader.GetInt32(2), this.Separator); // Meses liquidados
                            //this.WriteLineLog("Meses liquidados");
                            linea += String.Format("{0}{1}", reader.GetDecimal(5), this.Separator); // ECDF Mensual
                            //this.WriteLineLog("ECDF Mensual");

                            TextReader r = reader.GetTextReader(3);
                            String formula = r.ReadToEnd();
                            linea += String.Format("{0}{1}", parseValue(formula), this.Separator); // Descripcion Formula
                            //this.WriteLineLog("Descripcion formula");
                            r = reader.GetTextReader(4);
                            String observacion = r.ReadToEnd();
                            linea += String.Format("{0}{1}", parseValue(observacion), this.Separator); // Observacion
                            //this.WriteLineLog("Observacion");
                        }
                        WriteLineLog("Fin lectura de registro");
                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error. " + ex.Message);
            }

            return linea;
        }

        private double ObtenerECDF(Int32 acta)
        {
            double ecdf = 0;
            String sql = "SELECT AcLiVaC1 FROM Acta_Liquidacion WHERE AcLiActa = @acta";
            WriteLineLog("Ejecutando consulta: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada. OK");
                        if (reader.Read())
                        {
                            ecdf = reader.GetDouble(0);
                        }

                    }

                }
                WriteLineLog("Retornando valor ECDF: " + ecdf);
               
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }
            return ecdf;
        }

        private String ObtenerConsumos(Int32 acta)
        {
            string[] consumos = new string[10] { "", "", "", "", "", "", "", "", "", "" };
            String sql = "SELECT TOP(5) ConsValo, ConsFech FROM Consumo NOLOCK WHERE ConsActa = @acta";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int x = 0;
                        WriteLineLog("Consulta ejecutada. OK");
                        WriteLineLog("Leyendo registros");
                        while (reader.Read())
                        {
                            String valor = "";
                            String fecha = "";
                            try
                            {
                                valor = reader.GetInt32(0).ToString();
                                fecha = reader.GetString(1);

                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                            finally
                            {
                                consumos[x] = valor.ToString();
                                x++;
                                consumos[x] = fecha;
                                x++;
                            }


                        }
                        WriteLineLog("Lectura de registros finalizada.");
                    }

                }

            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }

            String cadena = "";
            for (int x = 0; x < consumos.Length; x++)
            {
                cadena += consumos[x];
                if (x < (consumos.Length-1))
                {
                    cadena += this.Separator;
                }
            }
            WriteLineLog("Retornando cadena de consumos: " + cadena);
            return cadena;
        }

        private String ObtenerInformacionMedidor(Int32 acta)
        {
            string[] medidor = new string[9] { "",	"","",	"","",	"",	"",	"","" };
            String sql = "SELECT TOP(1) ISNULL(tipoRevision,''),"
                + "ISNULL(numero,''),"
                + "ISNULL(tipo,''),"
                + "ISNULL(marca,''),"
                + "ISNULL(tecnologia,''),"
                + "ISNULL(nFases,''),"
                + "ISNULL(lecturaUltimaFecha,''),"
                + "ISNULL(lecturaUltima,''),"
                + "ISNULL(LecturaActual,'')  "
                + " FROM Acta_Medidor WHERE _number = @acta";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    WriteLineLog("Consulta ejecutada OK.");
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Leyendo registro");
                        if (reader.Read())
                        {
                            try
                            {
                                medidor[0] = parseValue(reader.GetString(0));
                                medidor[1] = parseValue(reader.GetString(1));
                                medidor[2] = parseValue(reader.GetString(2));
                                medidor[3] = parseValue(reader.GetString(3));
                                medidor[4] = parseValue(reader.GetString(4));
                                medidor[5] = parseValue(reader.GetString(5));
                                medidor[6] = parseValue(reader.GetString(6));
                                medidor[7] = parseValue(reader.GetString(7));
                                medidor[8] = parseValue(reader.GetString(8));
                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                        }
                        WriteLineLog("Lectura finalizada.");
                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }

            String cadena = "";
            for (int x = 0; x < medidor.Length; x++)
            {
                cadena += medidor[x];
                if (x < (medidor.Length-1))
                {
                    cadena += this.Separator;
                }
            }
            WriteLineLog("Retornando información de medidor. " + cadena);
            return cadena;
        }

        private String ObtenerInformacionRechazo(Int32 acta)
        {
            string[] rechazo = new string[7] { "", "", "", "", "", "", ""};
            String sql = "SELECT TOP(1) ISNULL(AcReUsua,''),"
                + "ISNULL(CaDeDesc,''),"
                + "ISNULL(AcReObse,''),"
                + "ISNULL(AcReUsDe,''),"
                + "ISNULL(AcReObs2,''),"
                + "'',"
                + "CONVERT(VARCHAR,ISNULL(AcReFech,''),103) "
                + " FROM ActasRechazadas NOLOCK, CausasDevolucion "
                + " WHERE AcReActa = @acta "
                + " AND AcReCaus = CaDeCodi ";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada OK.");
                        WriteLineLog("Leyendo registro");
                        if (reader.Read())
                        {
                            try
                            {
                                rechazo[0] = parseValue(reader.GetString(0));
                                rechazo[1] = parseValue(reader.GetString(1));
                                rechazo[2] = parseValue(reader.GetString(2));
                                rechazo[3] = parseValue(reader.GetString(3));
                                rechazo[4] = parseValue(reader.GetString(4));
                                rechazo[5] = parseValue(reader.GetString(5));
                                rechazo[6] = parseValue(reader.GetString(6));
                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                        }
                        WriteLineLog("Lectura registro finalizada.");
                    }

                }

            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }

            String cadena = "";
            for (int x = 0; x < rechazo.Length; x++)
            {
                cadena += rechazo[x];
                if (x < (rechazo.Length - 1))
                {
                    cadena += this.Separator;
                }
            }

            WriteLineLog("Retornando información de registro.");
            return cadena;
        }

        private String ObtenerInformacionMensajeria(Int32 acta)
        {
            string[] mensajeria = new string[14] { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            String sql = "SELECT TOP(1) CONVERT(VARCHAR,ISNULL(MensFeSi,''),103),"
                + "CONVERT(VARCHAR,ISNULL(MensFeSi,''),108),  "
                + "ISNULL(Direccion,''),  "
                + "ISNULL(Departamento,''),  "
                + "ISNULL(Municipio,''),  "
                + "ISNULL(Localidad,''),  "
                + "ZonaDesc,  "
                + "ISNULL(EstadoEntrega,''),  "
                + "CONVERT(VARCHAR,ISNULL(FechaEntregaExpe,''),103),"
                + "ISNULL(Latitud,''),  "
                + "ISNULL(Longitud,''),  "
                + "CONVERT(VARCHAR,ISNULL(MensNoFo,0)),"
                + "CONVERT(VARCHAR,ISNULL(UploadImagen,0)),"
                + "CONVERT(VARCHAR,ISNULL(MensCaDe,'')) "
                + " FROM Mensajeria,Zonas WHERE MensActa = @acta "
                + " AND Delegacion=ZonaCodi";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada OK.");
                        WriteLineLog("Leyendo registro");
                        if (reader.Read())
                        {
                            try
                            {
                                mensajeria[0] = parseValue(reader.GetString(0));
                                mensajeria[1] = parseValue(reader.GetString(1));
                                mensajeria[2] = parseValue(reader.GetString(2));
                                mensajeria[3] = parseValue(reader.GetString(3));
                                mensajeria[4] = parseValue(reader.GetString(4));
                                mensajeria[5] = parseValue(reader.GetString(5));
                                mensajeria[6] = parseValue(reader.GetString(6));
                                mensajeria[7] = parseValue(reader.GetString(7));
                                mensajeria[8] = parseValue(reader.GetString(8));
                                mensajeria[9] = parseValue(reader.GetString(9));
                                mensajeria[10] = parseValue(reader.GetString(10));
                                mensajeria[11] = parseValue(reader.GetString(11));
                                mensajeria[12] = parseValue(reader.GetString(12));
                                mensajeria[13] = parseValue(reader.GetString(13));
                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                        }
                        WriteLineLog("Lectura registro finalizada.");
                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }


            String cadena = "";
            for (int x = 0; x < mensajeria.Length; x++)
            {
                cadena += mensajeria[x];
                if (x < (mensajeria.Length - 1))
                {
                    cadena += this.Separator;
                }
            }

            WriteLineLog("Retornando información de mensajeria: " + cadena);
            return cadena;
        }

        private String ObtenerAnomalias(Int32 acta)
        {
            string[] anomalias = new string[5] { "", "", "", "", "" };
            String sql = "SELECT TOP 5 AcAnDesc FROM Anomalias NOLOCK WHERE  AcAnNuAc = @acta";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada OK.");
                        WriteLineLog("Leyendo registros.");
                        int x = 0;
                        while (reader.Read())
                        {
                            try
                            {
                                anomalias[x] = parseValue(reader.GetString(0));
                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                            x++;
                        }
                        WriteLineLog("Lectura de registros finalizada.");
                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }


            String cadena = "";
            for (int x = 0; x < anomalias.Length; x++)
            {
                cadena += anomalias[x];
                if (x < (anomalias.Length -1))
                {
                    cadena += this.Separator;
                }
            }
            WriteLineLog("Retornando información de Anomalias: " + cadena);
            return cadena;
        }

        private void WriteLine(String linea)
        {
            using (StreamWriter outfile = new StreamWriter(@Filename, true, Encoding.UTF8))
            {
                outfile.Write(linea + "\r\n");
            }
        }

        public void WriteLineLog(String linea)
        {
            if (debug)
            {
                String f = String.Format("{0:G}", DateTime.Now);
                using (StreamWriter outfile = new StreamWriter(@FilenameLog, true, Encoding.UTF8))
                {
                    outfile.Write(f + "\t" + linea + "\r\n");
                    //System.Console.WriteLine(f + "\t" + linea);
                }
            }
            
        }

        private String ObtenerAnotaciones(Int32 acta)
        {
            string[] anotaciones = new string[5] { "", "", "", "", "" };
            String sql = "SELECT TOP 5 AnotDesc FROM AnotacionActa NOLOCK WHERE  AnotActa = @acta AND AnotDesc NOT LIKE '%LECTA%' ORDER BY AnotFeSi DESC";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Cosulta ejecutada OK.");
                        int x = 0;
                        while (reader.Read())
                        {
                            try
                            {
                                anotaciones[x] = parseValue(reader.GetString(0));
                            }
                            catch (Exception e)
                            {
                                this.WriteLineLog("Acta " + acta + "\r\nError: " + e.StackTrace);
                            }
                            x++;
                        }
                        WriteLineLog("Lectura de registros finalizada.");
                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }


            String cadena = "";
            for (int x = 0; x < anotaciones.Length; x++)
            {
                cadena += anotaciones[x];
                if (x < (anotaciones.Length-1))
                {
                    cadena += this.Separator;
                }
            }

            WriteLineLog("Retornando información de anotaciones: " + cadena);
            return cadena;
        }

        private void RemoveFile()
        {
            if (File.Exists(@Filename))
            {
                File.Delete(@Filename);
            }
        }

        private String generateFilename()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\REPORTS"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\REPORTS");
            }
            return  Environment.CurrentDirectory + @"\REPORTS\" + this.BeginFilename + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
           
        }

        private String generateFilenameLog()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\REPORTS"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\REPORTS");
            }
            return Environment.CurrentDirectory + @"\LOG\" + this.BeginFilename + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";

        }
        
        
        private String getHeader()
        {
            WriteLineLog("Generando encabezado el archivo");
            String header = String.Format("Nro Acta{0}NIC{0}Dirección{0}Codigo Tarifa{0}Tipo Cliente{0}Estrato{0}Departamento{0}Municipio{0}Localidad{0}Csmo1{0}fechaCsmo1{0}Csmo2{0}fechaCsmo2{0}Csmo3{0}fechaCsmo3{0}Csmo4{0}fechaCsmo4{0}Csmo5{0}fechaCsmo5{0}CargaContratada{0}Fecha Inicio Irregularidad{0}Empresa{0}Contrata{0}Tipo Orde De Servicio{0}Tipo Servicio{0}Comentario 1{0}Estado_Anterior_del_Acta{0}Estado_del_Acta{0}protocolo{0}Observaciones{0}Nombre_Operario{0}Primer_Apellido_Operario{0}Segundo_Apellido_Operario{0}Cedula_Operario{0}Empresa_Operario{0}Delegación{0}Tipo_Rev_Apa_Exis{0}Numero Medidor{0}Medida_Tipo{0}Marca_Apa_Exis{0}Tecnología_Apa_Exis{0}NFases_Apa_Exis{0}FECHA_LECTURA_ULTIMA_APA_EXIS{0}LECTURA_ULTIMA_APA_EXIS{0}LECTURA_ACTUAL_APA_EXIS{0}ANOMALIAS{0}ANOMALIA_1{0}ANOMALIA_1_1{0}ANOMALIA_2{0}ANOMALIA_2_1{0}Observacion_Anomalia{0}tipoCenso{0}Carga_Instalada{0}Tarifa{0}ECDF{0}Orden_de_Servicio_Resuelta{0}Voltaje_R{0}Voltaje_S{0}Voltaje_T{0}Fase_R{0}Fase_S{0}Fase_T{0}Fecha_Carga{0}Usuario_Carga{0}Tipo Acta{0}Fuente de Ingreso{0}Fecha Confirmacion{0}Fecha Asignacion{0}Incidencia{0}Codigo_Bandeja{0}Fecha Anticipada{0}Energia_Anticipada{0}Fecha_de_Acta{0}Bandeja{0}Bandeja_Anterior{0}Estado{0}Responsable{0}Coordinador{0}Tipo_De_Bandeja{0}Fecha_de_Orden{0}Hora_de_Orden{0}FR{0}Anotación1{0}Anotación2{0}Anotación3{0}Anotación4{0}Anotación5{0}Fecha_Envio_a_Mensajeria{0}Hora_de_Envio_a_Mensajeria{0}Direccion Notificacion{0}Departamento Notificacion{0}Municipio Notificacion{0}Localidad Notificacion{0}Delegacion Notificacion{0}Estado Entrega{0}Fecha Entrega Exp.{0}Latitud{0}Longitud{0}NroFolios{0}Upload Guia{0}Causal Devolucion{0}Fecha Ultima Actualizacion{0}Usuario Rechazo{0}Causal de Rechazo{0}Observación Rechazo{0}Usuario Retorna{0}Observación Retorno{0}Tipo Rechazo{0}Fecha Rechazo{0}Cod.Novedad{0}Desc.Novedad{0}Estado Nov.{0}Obs.Novedad{0}Fecha Apertura Nov.{0}Fecha Cierre Nov.{0}Confirmada{0}Tipo Proceso Técnico{0}Tipo Proceso HGI2", Separator);
            
            
            
            if (ReporteLiquidacion)
            {
                WriteLineLog("Generando encabezado de informe de liquidación.");
                header += String.Format("{0}VALOR_ECDF_ANTICIPADA{0}FR_ANTICIPADO{0}ECDF_LIQUIDADA{0}CODIGO_TARIFA{0}VALOR_TARIFA{0}TIPO_ACTA{0}CLASIFICACION{0}USUARIO_LIQUIDADOR{0}METODO_LIQUIDACION{0}MESES_LIQUIDADOS{0}ECDF_MENSUAL_CALCULADA{0}DESCIPCION_FORMULA{0}OBSERVACION", Separator);
                header += this.getHeaderPreguntas();
            }
            return header;
        }

        private String getHeaderPreguntas()
        {
            preguntas = new List<int>();
            String header = "";
            String sql = "SELECT CodiLich FROM listaChequeo";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conex.getConection();
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    WriteLineLog("Consulta ejecutada OK.");
                    WriteLineLog("Leyendo registros.");
                    while (reader.Read())
                    {
                        header += String.Format("{0}PREG_{1}",Separator, reader.GetInt32(0));
                        preguntas.Add(reader.GetInt32(0));
                    }
                    WriteLineLog("Lectura de registros finalizada.");
                }
            }
            WriteLineLog("Retornando encabezado de preguntas. " + header);
            return header;
        }

        private String obtenerPreguntas(Int32 acta)
        {
            String fila = "";
            String[] respuesta = new String[preguntas.Count];

            String sql = "SELECT LiRePregu,LiReResp FROM RespuestaLista NOLOCK WHERE LiReActa=@acta ";
            WriteLineLog("Ejecutando consulta SQL: " + sql);
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conex.getConection();
                    cmd.Parameters.Add("@acta", System.Data.SqlDbType.Int, 11).Value = acta;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        WriteLineLog("Consulta ejecutada OK.");
                        WriteLineLog("Leyendo registros.");
                        while (reader.Read())
                        {
                            for (int i = 0; i < preguntas.Count; i++)
                            {
                                if (preguntas[i] == reader.GetInt32(0))
                                {
                                    respuesta[i] = reader.GetString(1);
                                }

                            }
                        }
                        WriteLineLog("Lectura de registros finalizada.");
                    }
                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }

            for (int i = 0; i < respuesta.Length; i++)
            {
                fila += String.Format("{0}{1}",respuesta[i],Separator);
            }

                return fila;
        }

        public Boolean AgregarRegistroBD(String filename, String path, String url, long size)
        {
            log.Info("Agregando registro de reporte en la BD");
            Boolean resultado = false;
            Datos conexion = new Datos();
            try
            {
                if (conexion != null)
                {
                    String sql = "INSERT INTO FileReporte (Fecha,Usuario,Filename,Path,Url,Size) VALUES (SYSDATETIME(),'interfaz',@filename, @path,@url,@size)";
                    WriteLineLog("Ejecutando consulta SQL: " + sql);
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@filename", System.Data.SqlDbType.VarChar, 100).Value = filename;
                        cmd.Parameters.Add("@path", System.Data.SqlDbType.VarChar, 100).Value = path;
                        cmd.Parameters.Add("@url", System.Data.SqlDbType.VarChar, 100).Value = url;
                        cmd.Parameters.Add("@size", System.Data.SqlDbType.Int, 11).Value = size;

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            log.Info("Registro agregado correctamente");
                            resultado = true;
                        }

                    }

                }
            }
            catch (SqlException ex)
            {
                log.Error("Error BD: " + ex.Message);
            }

            return resultado;
        }

    }
}
