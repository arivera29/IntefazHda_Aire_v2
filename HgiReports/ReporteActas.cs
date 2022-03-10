using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HgiReports
{
    class ReporteActas
    {
        public String Separator { get; set; }
        public String Path { set; get; }
        public String Filename { set; get; }
        public String FilenameLog { set; get; }
        public String FilenameZip { set; get; }

        public int Tipo { set; get; } // 1- informe diaria
                                         // 2 - informe anual

        private Datos conex;

        public ReporteActas(int tipo)
        {
            this.Tipo = tipo;  // Informe diario
            this.Filename = this.generateFilename();
            this.FilenameLog = this.generateFilenameLog();
            this.FilenameZip = Filename + ".zip";
            

            conex = new Datos();
        }


        private String generateFilename()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\REPORTS"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\REPORTS");
            }
            return Environment.CurrentDirectory + @"\REPORTS\RP_ACTAS_" + (Tipo == 1 ? "DIARIO_" : "ANUAL_") + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

        }

        private String generateFilenameLog()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\REPORTS"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\REPORTS");
            }
            return Environment.CurrentDirectory + @"\LOG\RP_ACTAS_" + (Tipo==1?"DIARIO_":"ANUAL_") + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";

        }

        public void GenerarReporte()
        {

            this.RemoveFile();
            System.Console.WriteLine("Ejecutando consulta en el servidor...");
            String sql = "SELECT * FROM INFORME_ACTAS ";
            if (Tipo == 1)
            {
                sql += " WHERE CONVERT(DATE,FechaUltimaModificacion) = CONVERT(DATE,SYSDATETIME()) ";
            }

            if (Tipo == 2)
            {
                sql += " WHERE YEAR(FechaUltimaModificacion) = YEAR(SYSDATETIME()) ";
            }

            sql += " ORDER BY FechaUltimaModificacion";

            System.Console.WriteLine("Query: " + sql);

            Datos conexion = new Datos();
            if (conexion != null)
            {

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        System.Console.WriteLine("Consulta ejecutada");
                        System.Console.WriteLine("Generando encabezado del archivo de salida");
                        String header = "";
                        for (int x = 0; x < reader.FieldCount; x++)
                        {
                            header += reader.GetName(x) + this.Separator;
                        }
                        this.WriteLine(header);
                        System.Console.WriteLine("Generando encabezado cuerpo del archivo");
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
                                    case "Single":
                                        linea += reader.GetSqlSingle(x).ToString() + this.Separator;
                                        break;
                                    case "Decimal":
                                        linea += reader.GetSqlDecimal(x).ToString() + this.Separator;
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
                conexion.Close();
            }
            else
            {

                this.WriteLineLog("Error al conectarse con el servidor de datos");
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
            using (StreamWriter outfile = new StreamWriter(@FilenameLog, true,Encoding.UTF8))
            {
                outfile.Write(linea + "\r\n");
            }
        }

        public Boolean Comprimir()
        {
            if (File.Exists(@Filename))
            {
                try
                {
                    System.Console.WriteLine("Creando archivo: " + FilenameZip);
                    FileStream fsOut = File.Create(FilenameZip);
                    ZipOutputStream zip = new ZipOutputStream(fsOut);

                    System.Console.WriteLine("Agregando archivo al ZIP: " + Filename);
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
                    System.Console.WriteLine("Error Comprimiendo Archivo. " + e.StackTrace);
                    this.WriteLineLog("Error Comprimiendo Archivo. " + e.StackTrace);
                    return false;
                }

            }
            else
            {
                return false;
            }

            return true;
        }

        private void RemoveFile()
        {
            if (File.Exists(@Filename))
            {
                File.Delete(@Filename);
            }
        }

        private String parseValue(String cadena)
        {
            cadena = cadena.Replace(this.Separator, "_");
            cadena = cadena.Replace("\n", "");
            cadena = cadena.Replace("\r", "");
            cadena = cadena.Replace("\t", "");

            return cadena;
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

    }
}
