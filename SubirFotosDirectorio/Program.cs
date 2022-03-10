using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirFotosDirectorio
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                LOG("Error de parametros...");
                return;
            }

            String directorio = args[0];
            String archivo = args[1];
            

            Datos conexion = new Datos();
            if (conexion.getConection().State == ConnectionState.Open)
            {
                
                List<String> filas = parserFileText(Path.Combine(directorio,archivo));
                int contador = 1;
                LOG("Filas leidas del archivo: " + filas.Count);

                foreach (String fila in filas)
                {
                    try
                    {
                        
                        LOG("Leyendo fila " + contador);
                        if (contador > 1)
                        {
                            String[] campos = fila.Split('\t');
                            LOG("Campos leidos: " + campos.Length);
                            if (campos.Length >= 6)
                            {
                                
                                String acta = campos[0];
                                String estado = campos[1];
                                String causal = campos[2];
                                String fecha = campos[3];
                                String guia = campos[4];
                                String imagen = campos[5];
                                LOG("Procesando acta " + acta);

                                if (acta == "" || estado == "" || causal == "" || fecha == "" || guia == "" || imagen == "")
                                {
                                    LOG("Fila " + contador + " no válida. Campo(s) vacío(s)");
                                    continue;
                                }

                                conexion.BeginTransaction();

                                if (agregarDocumento(conexion, Path.Combine(directorio, imagen), imagen, acta))
                                {
                                    String sql = "UPDATE Mensajeria SET FechaEntregaExpe=@fecha, "
                                        + " GuiaMensajeria=@guia, "
                                        + " FechaUpdate = SYSDATETIME(),"
                                        + " EstadoEntrega = @estado,"
                                        + " MensObDe = @observacion,"
                                        + " MensCaDe = @causal "
                                        + " WHERE MensActa=@acta ";
                                    using (SqlCommand cmd = new SqlCommand(sql))
                                    {
                                        cmd.Connection = conexion.getConection();
                                        cmd.Transaction = conexion.getTransaction();
                                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                                        cmd.Parameters.Add("@fecha", SqlDbType.Date, 8).Value = fecha;
                                        cmd.Parameters.Add("@guia", SqlDbType.VarChar, 100).Value = guia;
                                        cmd.Parameters.Add("@estado", SqlDbType.VarChar, 10).Value = estado;

                                        if (estado.Trim().Equals("D"))
                                        {
                                            cmd.Parameters.Add("@observacion", SqlDbType.VarChar, 100).Value = causal;
                                        }
                                        else
                                        {
                                            cmd.Parameters.Add("@observacion", SqlDbType.VarChar, 100).Value = "";
                                        }

                                        cmd.Parameters.Add("@causal", SqlDbType.Int, 11).Value = ConvertCausal(causal);

                                        cmd.Prepare();
                                        if (cmd.ExecuteNonQuery() > 0)
                                        {
                                            conexion.TrasactionCommit();
                                            LOG("Acta " + acta + " Procesada correctamente");

                                        }
                                        else
                                        {
                                            conexion.TransactionRollback();
                                            LOG("Error al procesa el acta No. " + acta);
                                        }


                                    }


                                }
                                else
                                {
                                    conexion.TransactionRollback();
                                    LOG("Error al agregar documento al acta");

                                }


                            }
                            else
                            {
                                LOG("Fila " + contador + " No valida");
                            }


                        }
                    }
                    catch (SqlException e)
                    {
                        conexion.TransactionRollback();
                        LOG("Error: " + e.Message);
                    }
                    contador++;

                }

                conexion.Close();
                LOG("Proceso finalizado");
            }
            else
            {
                LOG("Error al conectarse con el servidor de base de datos");
            }

        }

        static List<String> parserFileText(String archivo)
        {
            LOG("Parseando archivo " + archivo);
            StreamReader objReader = new StreamReader(archivo);
            string sLine = "";
            List<String> filas = new List<String>();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                    filas.Add(sLine);
            }
            objReader.Close();

            return filas;
        }

        static bool agregarDocumento(Datos conexion,String path, String filename, String nroActa)
        {
            bool resultado = false;
            if (conexion.getConection().State == System.Data.ConnectionState.Open)
            {

                if (ConvertirTIFtoPDF(path, filename))
                {
                    String sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve, DocuPath)"
                    + " VALUES (@acta,14,'','interfaz',SYSDATETIME(),@url,1,0,'',NULL,@path)";
                    bool registrado = false;
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        
                        cmd.Connection = conexion.getConection();
                        cmd.Transaction = conexion.getTransaction();
                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                        cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "File/Guias/" + filename + ".pdf";
                        cmd.Parameters.Add("@path", SqlDbType.VarChar, 250).Value = ConfigVars.FolderGuiasHGI2() + Path.DirectorySeparatorChar + filename + ".pdf";
                        cmd.Prepare();

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            LOG("Archivo " + path + " asociado al Acta No. " + nroActa);
                            registrado = true;
                        }
                        else
                        {
                            LOG("Error al insertar el registro de documento del acta " + nroActa + " path " + path);
                        }

                    }

                    if (registrado)  // Actualizar registro de mensajeria
                    {
                        sql = "UPDATE Mensajeria SET UploadImagen = 1 WHERE MensActa=@acta";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            
                            cmd.Connection = conexion.getConection();
                            cmd.Transaction = conexion.getTransaction();
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                            cmd.Prepare();

                            if (cmd.ExecuteNonQuery() >= 0)
                            {

                                //conexion.Commit();
                                LOG("Acta " + nroActa + " Actualizada correctamente");
                                resultado = true;
                            }
                            else
                            {
                                LOG("Error al actualizar tabla mensajeria  " + nroActa + " path " + path);
                            }

                        }
                    }
                    else
                    {
                        conexion.Rollback();
                    }
                }
                else
                {
                    LOG("Error al generar PDF de guia.  Archivo: " + filename);
                }
            }
            else
            {
                LOG("Error al conectarse con la base de datos");
            }
            return resultado;
        }

        static bool ConvertirTIFtoPDF(String path, String filename)
        {
            bool result = false;
            try
            {
                String filenamePDF = ConfigVars.FolderGuiasHGI2() + "\\" + filename + ".pdf";
                iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER);
                PdfWriter writer = PdfAWriter.GetInstance(doc, new FileStream(filenamePDF, FileMode.Create));
                doc.Open();

                // Creamos la imagen y le ajustamos el tamaño
                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(path);
                imagen.BorderWidth = 0;
                imagen.Alignment = Element.ALIGN_CENTER;
                //float percentage = 0.0f;
                //percentage = 50 / imagen.Width;
                imagen.ScalePercent(20);

                // Insertamos la imagen en el documento
                doc.Add(imagen);

                // Cerramos el documento
                doc.Close();
                LOG(" Archivo: " + filename + " Convertido a PDF correctamente");
                result = true;
            }
            catch (Exception e)
            {
                LOG("Error. " + e.Message + " Archivo: " + path);
            }
            return result;
        }

        public static int ConvertCausal(String causal)
        {
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
            String filename = Environment.CurrentDirectory + @"\LOG\IMAGEN_GUIA_FTP_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }


    }
}
