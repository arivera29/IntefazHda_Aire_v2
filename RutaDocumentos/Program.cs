using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RutaDocumentos
{
    class Program
    {

        static void Main(string[] args)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("codigo");
            dt.Columns.Add("ruta");
            long contador = 0;

            Datos conexion = new Datos();
            if (conexion != null)
            {
                do
                {
                    System.Console.WriteLine("Consultando documentos sin ruta en la BD...");
                    dt.Rows.Clear();

                    String sql = "SELECT TOP(1000) DocuCodi,DocuUrLo FROM Documentos WHERE DocuPath IS NULL or DocuPath='' and DocuTiDo=14";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Prepare();
                        try
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    DataRow row = dt.NewRow();
                                    row["codigo"] = reader.GetInt32(0).ToString();
                                    row["ruta"] = reader.GetString(1);
                                    dt.Rows.Add(row);

                                }


                            }
                        }
                        catch (SqlException ex)
                        {
                            System.Console.WriteLine("Error SQL " + ex.Message);
                        }
                    }
                    System.Console.WriteLine("Documentos consultados: " + dt.Rows.Count);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            System.Console.WriteLine("Procesando documento id " + (String)row["codigo"] + " ruta " + (String)row["ruta"]);
                            String path = getLocalPath((String)row["ruta"]);
                            System.Console.WriteLine("Conversion ruta: " + path);
                            int fileExiste = 0;
                            long size = 0;
                            if (File.Exists(path))
                            {

                                fileExiste = 1;
                                size = new System.IO.FileInfo(path).Length;
                                System.Console.WriteLine("Ruta " + path + " encontrada. Size " + size);


                            }
                            else
                            {
                                System.Console.WriteLine("Ruta " + path + " no existe");
                            }



                            sql = "UPDATE Documentos SET DocuPath = @path, DocuSize=@size, DocuExis=@existe WHERE DocuCodi=@Codigo";
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                cmd.Parameters.Add("@path", SqlDbType.VarChar, 200).Value = path;
                                cmd.Parameters.Add("@size", SqlDbType.Int, 11).Value = size;
                                cmd.Parameters.Add("@existe", SqlDbType.Int, 11).Value = fileExiste;
                                cmd.Parameters.Add("@Codigo", SqlDbType.Int, 11).Value = (String)row["codigo"];
                                cmd.Prepare();
                                try
                                {
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        System.Console.WriteLine("Documento id " + (String)row["codigo"] + " actualizado correctamente");
                                        contador++;
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Error al actualizar ruta del documento id " + (String)row["codigo"]);
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    System.Console.WriteLine("Ruta " + path + " no existe");
                                }
                            }

                        }


                    }
                    else
                    {
                        System.Console.WriteLine("No se encontraron archivos sin ruta en la base de datos");
                    }
                } while (dt.Rows.Count > 0);

                System.Console.WriteLine("Proceso finalizado. Documentos actualizados: " + contador);


                conexion.Close();
            }
            else
            {
                System.Console.WriteLine("Error al conectarse a la base de datos");
            }




        }

        public static String getLocalPath(String path)
        {
            String filename = Path.GetFileName(path);
            String ruta = path.Trim();
            ruta = ruta.Replace("~\\", "");
            ruta = ruta.Replace("~/", "");
            if (ruta.Contains("File/Documentos/"))
            {
                ruta = ruta.Replace("File/Documentos/", "I:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }
            if (ruta.Contains("File\\Documentos\\"))
            {
                ruta = ruta.Replace("File\\Documentos\\", "I:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File/Documentos2/"))
            {
                ruta = ruta.Replace("File/Documentos2/", "F:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }
            if (ruta.Contains("File\\Documentos2\\"))
            {
                ruta = ruta.Replace("File\\Documentos2\\", "F:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File/Documentos3/"))
            {
                ruta = ruta.Replace("File/Documentos3/", "H:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File\\Documentos3\\"))
            {
                ruta = ruta.Replace("File\\Documentos3\\", "H:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File/Documentos4/"))
            {
                ruta = ruta.Replace("File/Documentos4/", "E:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File\\Documentos4\\"))
            {
                ruta = ruta.Replace("File\\Documentos4\\", "E:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File/Documentos5/"))
            {
                ruta = ruta.Replace("File/Documentos5/", "J:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File\\Documentos5\\"))
            {
                ruta = ruta.Replace("File\\Documentos5\\", "J:\\HGI2\\Documentos\\");
                ruta = ruta.Replace("/", "\\");
            }

            if (ruta.Contains("File/Guias/"))
            {
                ruta = ruta.Replace("File/Guias/", "G:\\Guias\\");
                ruta = ruta.Replace("/", "\\");
            }
            
            if (ruta.Contains("File\\Guias\\"))
            {
                ruta = ruta.Replace("File\\Guias\\", "G:\\Guias\\");
                ruta = ruta.Replace("/", "\\");
            }

            return ruta;
        }
    }
}
