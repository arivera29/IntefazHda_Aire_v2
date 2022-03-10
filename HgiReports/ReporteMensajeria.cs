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
    class ReporteMensajeria
    {
        public String Separator { get; set; }
        public String Path { set; get; }
        public String Filename { set; get; }
        public String FilenameLog { set; get; }
        public String FilenameZip { set; get; }

        private Datos conex;

        public ILog log { get; set; }

        public Boolean debug { get; set; }

        public ReporteMensajeria(ILog log)
        {
            this.Filename = this.generateFilename();
            this.FilenameLog = this.generateFilenameLog();
            this.FilenameZip = Filename + ".zip";
            this.log = log;
            this.debug = false;
            conex = new Datos();
        }

        private String parseValue(String cadena)
        {
            cadena = cadena.Replace(this.Separator, "_");
            cadena = cadena.Replace("\n", "");
            cadena = cadena.Replace("\r", "");
            cadena = cadena.Replace("\t", "");

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
            return Environment.CurrentDirectory + @"\REPORTS\RP_MENSAJERIA_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

        }

        private String generateFilenameLog()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\REPORTS"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\REPORTS");
            }
            return Environment.CurrentDirectory + @"\LOG\RP_MENSAJERIA_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";

        }

        public Boolean AgregarRegistroBD(String filename, String path, String url, long size)
        {
            Boolean resultado = false;
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "INSERT INTO FileReporte (Fecha,Usuario,Filename,Path,Url,Size) VALUES (SYSDATETIME(),'interfaz',@filename, @path,@url,@size)";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@filename", System.Data.SqlDbType.VarChar, 100).Value = filename;
                    cmd.Parameters.Add("@path", System.Data.SqlDbType.VarChar, 100).Value = path;
                    cmd.Parameters.Add("@url", System.Data.SqlDbType.VarChar, 100).Value = url;
                    cmd.Parameters.Add("@size", System.Data.SqlDbType.Int, 11).Value = size;

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }

                }

            }
            return resultado;
        }

        public void GenerarReporte()
        {

            this.RemoveFile();
            String sql = "SELECT * FROM REPORTE_MENSAJERIA ORDER BY FECHA_CARGA_ACTA_HGI2";
            log.Info("Conectandose a la base de datos");
            Datos conexion = new Datos();
            if (conexion != null)
            {
                log.Info("Conexion OK");
                log.Info("Ejecutando consulta");
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        log.Info("Consulta OK");
                        log.Info("Generenado reporte mensajeria.");
                        cmd.Connection = conexion.getConection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            String header = "";
                            for (int x = 0; x < reader.FieldCount; x++)
                            {
                                header += reader.GetName(x) + this.Separator;
                            }
                            this.WriteLine(header);
                            while (reader.Read())
                            {
                                String linea = "";
                                for (int x = 0; x < reader.FieldCount; x++)
                                {
                                    //System.Console.WriteLine("Campo: " + reader.GetName(x) + " Type: " + reader.GetFieldType(x).Name);
                                    switch (reader.GetFieldType(x).Name)
                                    {
                                        case "Int32":
                                            linea += reader.GetInt32(x) + this.Separator;
                                            break;
                                        case "DateTime":
                                            if (!reader.IsDBNull(x))
                                            {
                                                linea += reader.GetDateTime(x).ToString("dd/MM/yyyy HH:mm") + this.Separator;
                                            }
                                            else
                                            {
                                                linea += "" + this.Separator;
                                            }
                                            break;
                                        case "Byte":
                                            linea += reader.GetByte(x).ToString() + this.Separator;
                                            break;
                                        default:
                                            if (!reader.IsDBNull(x))
                                            {
                                                linea += parseValue(reader.GetString(x).Replace("\t", "")) + this.Separator;
                                            }
                                            else
                                            {
                                                linea += "" + this.Separator;
                                            }
                                            break;
                                    }

                                }


                                this.WriteLine(linea);

                            }


                        }

                    }
                }
                catch (SqlException ex)
                {
                    log.Error("Error DB: " + ex.Message);
                }
                log.Info("Cerrando conexion BD");
                conexion.Close();
                log.Info("Generación archivo de reporte de mensajeria finalizada");
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
                using (StreamWriter outfile = new StreamWriter(@FilenameLog, true, Encoding.UTF8))
                {
                    outfile.Write(linea + "\r\n");
                }
            }
        }

        public Boolean Comprimir()
        {
            log.Info("Comprimiendo archivo " + Filename);
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
                    log.Info("Archivo comprimido: " + FilenameZip);
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

    }
}
