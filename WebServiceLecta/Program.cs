using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceLecta
{
    class Program
    {
        private static string bucketName = ConfigVars.S3BucketHgi();
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(ConfigVars.S3Region());
        private static IAmazonS3 s3Client;

        static void Main(string[] args)
        {
            if (DateTime.Now.CompareTo(new DateTime(2021, 10, 1)) > 0)
            {
                System.Console.WriteLine("Error Fatal System");
                return;
            }

            String filtro = "";
            if (args.Length > 0)
            {
                for (int x = 0; x < args.Length; x++)
                {
                    filtro += args[0];
                    if ((x+1) < args.Length)
                    {
                        filtro += ",";
                    }
                }
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("acta");
            dt.Columns.Add("fecha");
            dt.Columns.Add("delegacion");
            dt.Columns.Add("estado");
            dt.Columns.Add("imagen");

            String sql = "SELECT MensActa,MensFesi,Delegacion, ZonaDesc, estadoEntrega, uploadImagen "
               + " FROM Mensajeria WITH (nolock),Zonas "
               + " WHERE Delegacion = ZonaCodi"
               + " AND (EstadoEntrega = '' OR UploadImagen=0) "
               + " AND YEAR(MensFesi) >= 2017 "
               + " AND DATEDIFF(DAY,MensFesi,SYSDATETIME())  > 0";

            if (!filtro.Equals(""))
            {
                sql += " AND MensActa IN(" + filtro + ")";
            }

               sql +=  " ORDER BY MensFesi ASC ";

            Datos conexion = new Datos();
            if (conexion != null)
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Prepare();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["acta"] = reader.GetInt32(0).ToString();
                            row["fecha"] = reader.GetDateTime(1).ToString("yyyy-MM-dd");
                            row["delegacion"] = reader.GetString(3);
                            row["estado"] = reader.GetString(4);
                            row["imagen"] = reader.GetInt32(5);

                            dt.Rows.Add(row);
                        }
                    }
                }
                conexion.Close();
            }
            else
            {
                LOG("Error al conectarse con el servidor");
            }

            LOG("Total de actas a consultar: " + dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {

                try
                {
                    WsLecta.WebServiceEcaIrregPortTypeClient cliente = new WsLecta.WebServiceEcaIrregPortTypeClient();
                    cliente.Open();
                    LOG("Status Client WS: " + cliente.State.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        Int32 acta = Int32.Parse((String)row["acta"]);

                        LOG("Consultando Acta: " + row["acta"]);
                        WsLecta.Registro registro = null;
                        try
                        {
                            LOG(String.Format("Informacion enviada Acta {0}, fecha: {1}, delegacion: {2} ", row["acta"], (String)row["fecha"], (String)row["delegacion"]));
                            //registro = cliente.WebServiceEcaIrreg("28642525", "20210528", "Cordoba Norte");
                            registro = cliente.WebServiceEcaIrreg((String)row["acta"], (String)row["fecha"], (String)row["delegacion"]);
                        }
                        catch (WebException e)
                        {
                            LOG("Error WEBEXCEPTION. " + e.Message);
                            Novedad(acta, e.Message);
                        }
                        catch (Exception e)
                        {
                            LOG("Error. " + e.Message + " LINE: " + e.StackTrace);
                            Novedad(acta, e.Message);
                        }

                        if (registro != null)
                        {

                            LOG("No. Guia: " + registro.GUIA);
                            LOG("Fecha de Gestion: " + registro.FECHAGESTION);
                            LOG("Estado: " + registro.ESTADO);
                            LOG("Causal: " + registro.CAUSAL);
                            LOG("Ruta Imagen" + registro.RUTAIMAGEN);

                            if (registro.FECHAGESTION != null && registro.GUIA != null)
                            {
                                // Hay Gestion
                                conexion = new Datos();
                                if (conexion != null)
                                {
                                    LOG("Estado actual de entrega del acta " + acta + " es " + row["estado"]);
                                    if (row["estado"].ToString().Trim().Equals(""))
                                    {
                                        LOG("Actualizando gestion acta " + acta);
                                        UpdateGestion(acta, registro.ESTADO, registro.CAUSAL, registro.GUIA, registro.FECHAGESTION, conexion);
                                    }

                                    if (row["imagen"].ToString().Equals("0"))
                                    {
                                        if (registro.RUTAIMAGEN != null)
                                        {
                                            if (!registro.RUTAIMAGEN.Equals(""))
                                            {
                                                String filename = @"C:\TEMP\\" + (String)row["acta"] + ".tiff";
                                                WebClient webClient = new WebClient();
                                                try
                                                {
                                                    webClient.DownloadFile(registro.RUTAIMAGEN, filename);
                                                    if (File.Exists(filename))
                                                    {
                                                        FileInfo fi = new FileInfo(filename);
                                                        long size = fi.Length;
                                                        if (size == 0)
                                                        {
                                                            LOG("ERROR. Archivo de guia " + filename + " está corruto. Guia No. " + registro.GUIA);
                                                            File.Delete(filename);
                                                        }
                                                    }
                                                }
                                                catch (WebException e)
                                                {
                                                    LOG("Error. " + e.Message);
                                                    Novedad(acta, "Error al descargar archivo de imagen desde " + registro.RUTAIMAGEN);
                                                }
                                                catch (Exception e)
                                                {
                                                    LOG("Error. " + e.Message);
                                                    Novedad(acta, "Error al descargar archivo de imagen desde " + registro.RUTAIMAGEN);
                                                }

                                                if (File.Exists(filename))
                                                {
                                                    // Enviar archivo a Amazon

                                                    


                                                    
                                                    //if (ConvertirTIFtoPDF(filename, (String)row["acta"] + ".tiff"))
                                                    //{
                                                    //    File.Delete(filename); // Se borra el archivo de la imagen de la guia

                                                        if (File.Exists(filename)) {
                                                            
                                                            String keyName = System.Guid.NewGuid().ToString();
                                                            LOG("Key of file: " + keyName);
                                                            StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("hgiprofile");
                                                            s3Client = new AmazonS3Client(credentials,bucketRegion);
                                                            LOG("Send File a S3:  " + filename  + "Key Name: " + keyName);
                                                            UploadFileAsync(filename , keyName, acta).Wait();

                                                        
                                                        }
                                                       
                                                        //sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve)"
                                                        //        + " VALUES (@acta,14,'','interfaz',SYSDATETIME(),@url,1,0,'',NULL)";
                                                        //bool registrado = false;
                                                        //using (SqlCommand cmd = new SqlCommand(sql))
                                                        //{
                                                        //    cmd.Connection = conexion.getConection();
                                                        //    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                                                        //    cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "File/Guias/" + (String)row["acta"] + ".tiff" + ".pdf";
                                                        //    cmd.Prepare();

                                                        //    if (cmd.ExecuteNonQuery() > 0)
                                                        //    {
                                                        //        LOG("Archivo " + filename + ".pdf" + " asociado al Acta No. " + row["acta"]);
                                                        //        registrado = true;
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        LOG("Error al insertar el registro de documento del acta " + row["acta"] + " path " + filename + ".pdf");
                                                        //    }

                                                        //}

                                                        //if (registrado)
                                                        //{
                                                        //    UploadImagen(acta, conexion);
                                                        //}


                                                    //}
                                                    //else
                                                    //{
                                                    //    LOG("Error al convertir a PDF el archivo de imagen de la guia");
                                                    //}

                                                }
                                                else
                                                {
                                                    LOG("Error al descargar el archivo de imagen de la guia");
                                                    Novedad(acta, "Error al descargar archivo de imagen del WS LECTA");

                                                }
                                            }
                                            else
                                            {
                                                Novedad(acta, "No se obtiene ruta de archivo de imagen del WS LECTA");
                                            }
                                        }
                                        else
                                        {
                                            Novedad(acta, "No se obtiene ruta de archivo de imagen del WS LECTA");
                                        }
                                    }

                                    conexion.Close();
                                }
                                else
                                {
                                    LOG("Error al conectarse con el servidor");
                                }

                            }
                            else
                            {
                                Novedad(acta, "No se obtinen datos del WS LECTA");

                            }

                        }
                        else
                        {
                            LOG("No hay respuesta del WS del acta: " + row["acta"]);
                        }

                        UpdateContador(acta);
                    }


                    cliente.Close();
                }
                catch (Exception e)
                {
                    LOG("ERROR. " + e.Message);
                }


            }

            LOG("Proceso finalizado");



        }

        private static void insertDocumentGuia(Int32 Acta, String keyName, String filename)
        {
            Datos conexion = new Datos();
            if (conexion.getConection().State == ConnectionState.Open)
            {
                conexion.BeginTransaction();
                String sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve,DocuSAWS,docuIAWS)"
                                                                    + " VALUES (@acta,14,'','interfaz',SYSDATETIME(),@url,1,0,'',NULL,@state_AWS,@id_AWS)";
                bool registrado = false;
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Transaction = conexion.getTransaction();
                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = Acta;
                    cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "";
                    cmd.Parameters.Add("@state_AWS", SqlDbType.Int, 11).Value = 1;
                    cmd.Parameters.Add("@id_AWS", SqlDbType.VarChar, 250).Value = keyName;

                    cmd.Prepare();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        LOG("Archivo " + filename + ".pdf" + " asociado al Acta No. " + Acta);
                        registrado = true;
                    }
                    else
                    {
                        LOG("Error al insertar el registro de documento del acta " + Acta + " path " + filename + ".pdf");
                    }

                }

                if (registrado)
                {
                    sql = "UPDATE Mensajeria SET UploadImagen = 1, fechaUploadImagen=SYSDATETIME() WHERE MensActa=@acta";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Transaction = conexion.getTransaction();
                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = Acta;
                        cmd.Prepare();

                        if (cmd.ExecuteNonQuery() >= 0)
                        {

                            conexion.Commit();
                            LOG("Carga de imagen del Acta " + Acta + " actualizada correctamente");
                            Novedad(Acta, "Imagen de guia cargada desde WS LECTA");

                        }
                        else
                        {
                            conexion.Rollback();
                            LOG("Error al actualizar tabla mensajeria  " + Acta);
                        }

                    }
                }
                else
                {
                    conexion.Rollback();
                }

                conexion.Close();

            }
            
        }

        private static void Novedad(Int32 acta, String s)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "INSERT INTO AnotacionActa (AnotActa,AnotDesc,AnotEsta,AnotUsua,AnotFeSi) VALUES (@acta,@novedad,11,'interfaz',SYSDATETIME())";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                    cmd.Parameters.Add("@novedad", SqlDbType.VarChar, 250).Value = s;
                    cmd.Prepare();
                    if (cmd.ExecuteNonQuery() >= 0)
                    {
                        //conexion.Commit();
                    }
                }
                conexion.Close();
            }
            else
            {
                LOG("Error al conectarse con el servidor");
            }

        }

        private static void UploadImagen(Int32 acta, Datos conexion)
        {
            String sql = "UPDATE Mensajeria SET UploadImagen = 1, fechaUploadImagen=SYSDATETIME() WHERE MensActa=@acta";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();
                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                cmd.Prepare();

                if (cmd.ExecuteNonQuery() >= 0)
                {

                    //conexion.Commit();
                    LOG("Carga de imagen del Acta " + acta + " actualizada correctamente");
                    Novedad(acta, "Imagen de guia cargada desde WS LECTA");

                }
                else
                {
                    LOG("Error al actualizar tabla mensajeria  " + acta );
                }

            }
        }

        private static void UpdateContador(Int32 acta)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "UPDATE Mensajeria SET cntWS = cntWS + 1, fechaWS=SYSDATETIME() WHERE MensActa=@acta";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                    cmd.Prepare();

                    if (cmd.ExecuteNonQuery() >= 0)
                    {
                        LOG("Contador actualizado correctamente. Acta " + acta);
                    }
                    else
                    {
                        LOG("Error al actualizar tabla mensajeria  " + acta);
                    }

                }
                conexion.Close();
            }
            else
            {
                LOG("Error al conectarse con el servidor.");
            }
        }

        private static void UpdateGestion(Int32 acta, String estado, String causal,String guia, String fecha,  Datos conexion)
        {
            String sql = "UPDATE Mensajeria SET estadoEntrega=@estado, "
                                    + "GuiaMensajeria=@guia, "
                                    + "MensCaDe=@causal, "
                                    + "MensObDe=@observacion, "
                                    + " FechaEntregaExpe=@fecha, "
                                    + " fechaUpdate=SYSDATETIME() "
                                    + " WHERE MensActa=@acta";

            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Connection = conexion.getConection();
                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                cmd.Parameters.Add("@guia", SqlDbType.VarChar, 50).Value = guia;
                cmd.Parameters.Add("@fecha", SqlDbType.Date, 8).Value = fecha;
                if (estado.ToUpper().Contains("ENTREGA"))
                {
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 2).Value = "E";
                    cmd.Parameters.Add("@observacion", SqlDbType.VarChar, 255).Value = causal;
                    cmd.Parameters.Add("@causal", SqlDbType.Int, 11).Value = 0;
                }
                else
                {
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar, 2).Value = "D";
                    cmd.Parameters.Add("@observacion", SqlDbType.VarChar, 255).Value = causal;
                    cmd.Parameters.Add("@causal", SqlDbType.Int, 11).Value = ConvertCausal(causal);

                }
                cmd.Prepare();

                if (cmd.ExecuteNonQuery() >= 0)
                {
                    Novedad(acta, "Gestion de mensajeria actualizada desde el WS LECTA");
                }
            }
        }

        private static bool ConvertirTIFtoPDF(String path, String filename)
        {
            bool result = false;
            try
            {
                String filenamePDF = "G:\\Guias\\" + filename + ".pdf";
                iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER);
                PdfWriter writer = PdfAWriter.GetInstance(doc, new FileStream(filenamePDF, FileMode.Create));
                doc.Open();

                // Creamos la imagen y le ajustamos el tamaño
                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(path);
                imagen.BorderWidth = 0;
                imagen.Alignment = Element.ALIGN_CENTER;
                //float percentage = 0.0f;
                //percentage = 50 / imagen.Width;
                imagen.ScalePercent(30);

                // Insertamos la imagen en el documento
                doc.Add(imagen);

                // Cerramos el documento
                doc.Close();
                result = true;
            }
            catch (Exception e)
            {
                LOG("Error. " + e.Message + " Archivo: " + path);
            }
            return result;
        }

        public static int ConvertCausal(String causal) {
            int codigo = 16;
            switch (causal.Trim().ToUpper())
            {
                case "REHUSADO":
                    codigo = 4;
                    break;
                case "DIRECCION ERRADA":
                    codigo = 12;
                    break;
                case "NO RESIDE":
                    codigo = 6;
                    break;
                case "DIRECCION INCOMPLETA":
                    codigo = 14;
                    break;
                case "DESTINATARIO DESCONOCIDO":
                    codigo = 3;
                    break;
               
            }


            return codigo; 
        }

        public static void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\WS_LECTA_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }

        private static async Task UploadFileAsync(String filePath, String keyName, Int32 Acta)
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
                Console.WriteLine("Upload completed");

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


                LOG("Send File Amazon S3 OK");
                transfered = true;

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
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
                LOG("Actualizando el registro del documento.");
                insertDocumentGuia(Acta, keyName, filePath);
            }

        }


    }
}
