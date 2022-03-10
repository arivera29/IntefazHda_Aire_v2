using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HgiReports
{
    class Program
    {
        public static String idReporte = "";
        public static ILog log { get; set; }

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(ConfigVars.S3Region());
        private static IAmazonS3 s3Client;
        private static string bucketName = ConfigVars.S3BucketHgi();
        private static String fileOut = "";

        static void Main(string[] args)
        {
            log = LogManager.GetLogger(Assembly.GetExecutingAssembly().GetTypes().First());
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Iniciando generación de reportes de la HGI2");

            String opcion = "-1";  // Todos los informes
            bool debug = false;
            int year = -1;
            if (args.Length > 0)
            {
                for (int x = 0; x < args.Length; x++)
                {
                    switch (args[x])
                    {
                        case "--general":
                            opcion = args[x];  // General
                            break;
                        case "--mensajeria":
                            opcion = args[x]; // Mensajeria
                            break;
                        case "--rechazadas":
                            opcion = args[x]; // Rechazadas
                            break;
                        case "--pendientes":
                            opcion = args[x];  // Pendientes
                            break;
                        case "--liquidacion":
                            opcion = args[x];  // Liquidación
                            break;
                        case "--actas-diario":
                            opcion = args[x];  // Actas diario
                            break;
                        case "--actas-anual":
                            opcion = args[x]; // Actas anual
                            break;
                        case "--debug":
                            debug = true;
                            break;
                        case "--year":
                            String p = args[x + 1];
                            try
                            {
                                year = Int32.Parse(p);
                            }
                            catch (Exception ex)
                            {

                            }
                            break;

                    }
                }
            }
            else
            {
                log.Error("ERROR. No hay parametros de entrada.");
                return;
            }

            if (opcion.Equals(""))
            {
                log.Error("ERROR. Parametros no válidos.");
                return;
            }

            //System.Console.ReadKey();

            if (opcion.Equals("--mensajeria"))
            {
                ReporteMensajeria reporte2 = new ReporteMensajeria(log);
                try
                {
                    RegistroHisorico("RP_MENSAJERIA");
                    log.Info("Iniciando generación de Informe de mensajeria HGI2. " + DateTime.Now.ToString());
                    reporte2.Separator = ";";
                    reporte2.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte2.Filename;
                    if (reporte2.Comprimir())
                    {
                        System.Console.WriteLine("Archivo comprimido");
                        reporte2.WriteLineLog("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte2.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        log.Info("Tamaño del archivo: " + fi.Length);
                        long size = fi.Length;
                        String Name = fi.Name;

                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Copy(reporte2.FilenameZip, @"G:\FTP\MENSAJERIAMASIVA\REPORT\" + fi.Name);
                        //System.IO.File.Move(reporte2.FilenameZip, @"G:\REPORTES\" + fi.Name);

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);

                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte2.FilenameZip, keyName, Name, size).Wait();

                        //if (reporte2.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte2.Filename);
                        //    log.Info("Archivo " + reporte2.Filename + " eliminado correctamente");

                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }


            }


            if (opcion.Equals("--general"))
            {
                RegistroHisorico("RP_GENERAL");
                ReporteGeneral reporte = new ReporteGeneral("RP_GENERAL", log);
                try
                {
                    reporte.Separator = ";";
                    reporte.debug = debug;
                    if (year != -1)
                    {
                        reporte.year = year;
                    }
                    log.Info("Iniciando generación de Informe General HGI2. " + DateTime.Now.ToString());
                    reporte.FiltroGeneral = true;
                    reporte.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte.Filename;
                    if (reporte.Comprimir())
                    {
                        log.Info("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        log.Info("Tamaño del archivo: " + fi.Length);
                        long size = fi.Length;
                        String Name = fi.Name;

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);
                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte.FilenameZip, keyName, Name, size).Wait();

                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Move(reporte.FilenameZip, @"G:\REPORTES\" + fi.Name);
                        //if (reporte.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte.Filename);
                        //    log.Info("Archivo " + reporte.Filename + " eliminado correctamente");
                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }

            }

            if (opcion.Equals("--rechazadas"))
            {
                RegistroHisorico("RP_RECHAZADAS");
                ReporteGeneral reporte = new ReporteGeneral("RP_RECHAZADAS", log);
                try
                {
                    reporte.FiltroRechazadas = true;
                    reporte.debug = debug;
                    if (year != -1)
                    {
                        reporte.year = year;
                    }
                    reporte.Separator = ";";
                    log.Info("Iniciando generación de Informe de rechazadas HGI2. " + DateTime.Now.ToString());
                    reporte.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte.Filename;
                    if (reporte.Comprimir())
                    {
                        log.Info("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        long size = fi.Length;
                        String Name = fi.Name;

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);

                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte.FilenameZip, keyName, Name, size).Wait();

                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Move(reporte.FilenameZip, @"G:\REPORTES\" + fi.Name);

                        //if (reporte.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte.Filename);
                        //    log.Info("Archivo " + reporte.Filename + " eliminado correctamente");
                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }

            }

            if (opcion.Equals("--pendientes"))
            {
                RegistroHisorico("RP_PENDIENTES");
                ReporteGeneral reporte = new ReporteGeneral("RP_PENDIENTES", log);
                try
                {
                    reporte.FiltroRechazadas = false;
                    reporte.FiltroPendientes = true;
                    reporte.debug = debug;
                    if (year != -1)
                    {
                        reporte.year = year;
                    }
                    reporte.Separator = ";";
                    log.Info("Iniciando generación de Informe Pendientes HGI2. " + DateTime.Now.ToString());
                    reporte.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte.Filename;
                    if (reporte.Comprimir())
                    {
                        log.Info("Archivo comprimido");
                        reporte.WriteLineLog("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        log.Info("Tamaño del archivo: " + fi.Length);
                        long size = fi.Length;
                        String Name = fi.Name;

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);

                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte.FilenameZip, keyName, Name, size).Wait();

                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Move(reporte.FilenameZip, @"G:\REPORTES\" + fi.Name);
                        //if (reporte.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte.Filename);
                        //    log.Info("Archivo " + reporte.Filename + " eliminado correctamente");
                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }

            }

            if (opcion.Equals("--liquidacion"))
            {
                RegistroHisorico("RP_LIQUIDACION");
                ReporteGeneral reporte = new ReporteGeneral("RP_LIQUIDACION", log);
                try
                {
                    reporte.FiltroRechazadas = false;
                    reporte.FiltroPendientes = false;
                    reporte.FiltroMensajeria = true;
                    reporte.ReporteLiquidacion = true;
                    reporte.debug = debug;
                    if (year != -1)
                    {
                        reporte.year = year;
                    }
                    reporte.Separator = ";";
                    log.Info("Iniciando generación de Informe Liquidacion HGI2. " + DateTime.Now.ToString());
                    reporte.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte.Filename;
                    if (reporte.Comprimir())
                    {
                        System.Console.WriteLine("Archivo comprimido");
                        log.Info("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        log.Info("Tamaño del archivo: " + fi.Length);
                        long size = fi.Length;
                        String Name = fi.Name;

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);
                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte.FilenameZip, keyName, Name, size).Wait();

                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Move(reporte.FilenameZip, @"G:\REPORTES\" + fi.Name);
                        //if (reporte.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte.Filename);
                        //    log.Info("Archivo " + reporte.Filename + " eliminado correctamente");
                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }

            }

            if (opcion.Equals("--actas-diario") || opcion.Equals("--actas-anual"))
            {

                int tipo = 0;

                if (opcion == "--actas-diario")
                {
                    tipo = 1; // diario
                }
                if (opcion == "--actas-anual")
                {
                    tipo = 2; // Anual
                }
                RegistroHisorico("RP_ACTAS");
                ReporteActas reporte = new ReporteActas(tipo);
                try
                {
                    reporte.Separator = ";";
                    log.Info("Iniciando generación de Informe Actas HGI2. " + DateTime.Now.ToString());
                    reporte.GenerarReporte();
                    log.Info("Proceso finalizado " + DateTime.Now.ToString());
                    fileOut = reporte.Filename;
                    if (reporte.Comprimir())
                    {
                        log.Info("Archivo comprimido");

                        FileInfo fi = new FileInfo(reporte.FilenameZip);
                        log.Info("Nombre del archivo: " + fi.Name);
                        long size = fi.Length;
                        String Name = fi.Name;

                        String keyName = System.Guid.NewGuid().ToString();
                        log.Info("Key of file: " + keyName);
                        StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                        s3Client = new AmazonS3Client(credentials, bucketRegion);
                        //s3Client = new AmazonS3Client(bucketRegion);
                        log.Info("Send File a S3:  " + Name + "Key Name: " + keyName);

                        UploadFileAsync(reporte.FilenameZip, keyName, Name, size).Wait();


                        //log.Info("Moviendo archivo " + fi.Name + " a " + @"G:\REPORTES\" + fi.Name);
                        //System.IO.File.Move(reporte.FilenameZip, @"G:\REPORTES\" + fi.Name);
                        //if (reporte.AgregarRegistroBD(Name, @"G:\REPORTES\" + fi.Name, "Reportes/" + Name, size))
                        //{
                        //    log.Info("Archivo movido correctamente. OK");
                        //    System.IO.File.Delete(reporte.Filename);
                        //    log.Info("Archivo " + reporte.Filename + " eliminado correctamente");
                        //}
                        //else
                        //{
                        //    log.Error("Error al mover el archivo. FAIL");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error generando informe: " + ex.Message);
                }
                finally
                {
                    ActualizarHisorico();
                }
            }

            if (!fileOut.Equals(""))
            {
                if (File.Exists(fileOut))
                {
                    File.Delete(fileOut);  // Eliminado archivo de reporte generado
                }
            }

        }

        public static Boolean RegistroHisorico(String reporte) {
            Boolean resultado = false;
            Datos conexion = new Datos();
            if (conexion != null)
            {
                idReporte = Guid.NewGuid().ToString();
                log.Info("Registrando histórico en la BD, ID=" + idReporte);
                String sql = "INSERT INTO HistoricoReportes (id,fecha_inicio,fecha_fin,reporte,error) VALUES (@id,SYSDATETIME(),NULL,@reporte,0)";
                //WriteLineLog("Ejecutando consulta SQL: " + sql);
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar, 100).Value = idReporte;
                    cmd.Parameters.Add("@reporte", System.Data.SqlDbType.VarChar, 100).Value = reporte;


                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        log.Info("Registro guardado correctamente. ID= " + idReporte);
                        resultado = true;
                    }

                }

                conexion.Close();
            }
            return resultado;
        }

        public static Boolean ActualizarHisorico()
        {
            Boolean resultado = false;
            if (!idReporte.Equals(""))
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    log.Info("Actualizando histórico en la BD, ID=" + idReporte);
                    String sql = "UPDATE HistoricoReportes SET fecha_fin = SYSDATETIME() WHERE id=@id";
                    //WriteLineLog("Ejecutando consulta SQL: " + sql);
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar, 100).Value = idReporte;
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            //WriteLineLog("Consulta ejecutada OK.");
                            log.Info("Registrado actualizado correctamente, ID=" + idReporte);
                            resultado = true;
                        }
                    }

                    conexion.Close();
                }
            }
            return resultado;
        }

        private static async Task UploadFileAsync(String filePath, String keyName, String Name, long size)
        {
            Boolean transfered = false;
            try
            {
                var fileTransferUtility =
                    new TransferUtility(s3Client);

                // Option 1. Upload a file. The file name is used as the object key name.
                //await fileTransferUtility.UploadAsync(filePath, bucketName);
                //Console.WriteLine("Upload 1 completed");

                // Option 2. Specify object key name explicitly.
                await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
                log.Info("Upload completed");

                // Option 3. Upload data from a type of System.IO.Stream.
                //using (var fileToUpload =
                //    new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //{
                //    await fileTransferUtility.UploadAsync(fileToUpload,
                //                               bucketName, keyName);
                //}
                //Console.WriteLine("Upload 3 completed");

                //// Option 4. Specify advanced settings.
                //var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                //{
                //    BucketName = bucketName,
                //    FilePath = filePath,
                //    StorageClass = S3StorageClass.StandardInfrequentAccess,
                //    PartSize = 6291456, // 6 MB.
                //    Key = keyName,
                //    CannedACL = S3CannedACL.PublicRead
                //};
                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                //await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                //Console.WriteLine("Upload 4 completed");



                transfered = true;

            }
            catch (AmazonS3Exception e)
            {
                log.Error("Error encountered on server. Message:'{0}' when writing an object" + e.Message);
            }
            catch (Exception e)
            {
                log.Error("Unknown encountered on server. Message:'{0}' when writing an object" + e.Message);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }



            if (transfered)  // Archivo transferido
            {
                // Actualizar base de datos
                log.Info("Actualizando el registro del reporte.");
                AgregarRegistroBD(Name, "", "" + Name, size, keyName, 1);
            }


        }

        private static Boolean AgregarRegistroBD(String filename, String path, String url, long size, String keyname, int tipo)
        {
            Boolean resultado = false;
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "INSERT INTO FileReporte (Fecha,Usuario,Filename,Path,Url,Size,Tipo,KeyName) VALUES (SYSDATETIME(),'interfaz',@filename, @path,@url,@size,@Tipo, @keyname)";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@filename", System.Data.SqlDbType.VarChar, 100).Value = filename;
                    cmd.Parameters.Add("@path", System.Data.SqlDbType.VarChar, 100).Value = path;
                    cmd.Parameters.Add("@url", System.Data.SqlDbType.VarChar, 100).Value = url;
                    cmd.Parameters.Add("@size", System.Data.SqlDbType.Int, 11).Value = size;
                    cmd.Parameters.Add("@Tipo", System.Data.SqlDbType.Int, 11).Value = tipo;
                    cmd.Parameters.Add("@keyname", System.Data.SqlDbType.VarChar, 100).Value = keyname;

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }

                }

            }
            return resultado;
        }
    }
}
