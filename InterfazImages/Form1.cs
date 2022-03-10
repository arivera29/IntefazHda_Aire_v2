using Ghostscript.NET.Rasterizer;
using Pdf2Image;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazImages
{
    public partial class Form1 : Form
    {
        bool Start = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void configuracionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtTiempo.Value = 60;
            cboDPI.SelectedIndex = 0;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (!Start)
            {
                timer1.Interval = (int)txtTiempo.Value *1000* 60 ;
                timer1.Enabled = true;
                btnIniciar.Text = "Detener";
                txtTiempo.Enabled = false;
                cboDPI.Enabled = false;
                Start = true;
                LOG("Iniciando");
                this.ConvertirDocumentos();
            }
            else
            {
                Start = false;
                txtTiempo.Enabled = true;
                cboDPI.Enabled = true;
                btnIniciar.Text = "Iniciar";
                LOG("Finalizando");
            }
        }

        public void LOG(string log)
        {

            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }
            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\INTERFAZ_IMAGES_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

        }

        public void ConvertirDocumentos()
        {
            //String sql = "Select DocuUrlo From Documentos, TipoDocumento Where TidoCodi=DocuTido And TidoMens=true and  DocuActa in ( Select TOP 500 _number FROM Actas WHERE EstadoActa = 11) ";

            List<Acta> lista = new List<Acta>();

            String sql = "Select _number,nic,ProcesoSimpli.AnLaProce,Actas.Delegacion "
                + " FROM Actas with(nolock), Mensajeria, ProcesoSimpli "
                + " WHERE EstadoActa = 11 "
                + " And _number = Mensajeria.MensActa "
                + " And CONVERT (date,Mensajeria.MensFesi) = CONVERT (date, GETDATE()) "
                + " AND _number = NoAcProc  "
                + " ORDER BY MensFesi DESC";
            Datos conexion = new Datos();
            if (conexion != null)
            {

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    //cmd.Parameters.Add("@contrata", SqlDbType.VarChar, 100).Value = codigoContrata;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Acta acta = new Acta();
                            acta.NumActa = reader.GetInt32(0);
                            acta.Nic = reader.GetString(1);
                            acta.Fr = reader.GetString(2);
                            acta.Delegacion = reader.GetString(3);
                            lista.Add(acta);
                        }
                    }

                }

                if (lista.Count > 0)
                {


                    sql = "Select DocuUrlo,DocuTido From Documentos with(nolock), TipoDocumento Where TidoCodi=DocuTido And TidoMens = 1 and  DocuActa=@acta";
                    foreach (Acta acta in lista)
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta.NumActa;
                                LOG("Consultando archivo Acta " + acta.NumActa + " Nic " + acta.Nic);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    List<string> archivos = new List<string>();
                                    while (reader.Read())
                                    {
                                        String path = reader.GetString(0); // La Url del documento
                                        String fileName = Path.GetFileName(path);
                                        switch (reader.GetInt32(1))
                                        {
                                            case 15:  // Acta
                                                path = Properties.Settings.Default.ruta_doc + @"Actas\" + fileName;
                                                break;

                                            case 13:  // Formato de Liquidacion
                                                path = Properties.Settings.Default.ruta_doc + @"Liquidacion\" + fileName;
                                                break;

                                            case 16:  // Carta Proceso Simplificado
                                                path = Properties.Settings.Default.ruta_doc + @"ProcesoSimplificado\" + fileName;
                                                break;

                                            case 21:  // Aviso
                                                path = Properties.Settings.Default.ruta_doc + fileName;
                                                break;
                                            case 22:  // Formato de Guia
                                                path = Properties.Settings.Default.ruta_doc + @"Actas\" + fileName;
                                                break;

                                            default:
                                                path = Properties.Settings.Default.ruta_doc + fileName;
                                                break;

                                        }

                                        if (File.Exists(path))
                                        {
                                            LOG("Procesando archivo: " + path);
                                            archivos.Add(path);
                                        }
                                        else
                                        {
                                            LOG("Error. Archivo no existe: " + path);
                                        }

                                    }

                                    if (archivos.Count > 0)
                                    {
                                        LOG("Iniciando conversion archivos del acta " + acta.NumActa + " Nic " + acta.Nic);
                                        this.ConvertirPDFtoTIF(acta.Fr, acta.Delegacion, acta.Nic, archivos);
                                    }
                                }

                            }
                        }
                        catch (SqlException e)
                        {
                            LOG("Error: " + e.Message);
                        }

                    }


                }

                conexion.Close();
            }
            else
            {
                LOG("Error al conectarse con el Servidor");
            }
        }

        private class Acta
        {
            public Int32 NumActa { set; get; }
            public string Nic { set; get; }
            public String Fr { set; get; }
            public String Delegacion { set; get; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.ConvertirDocumentos();
        }

        private void ConvertirPDFtoTIF(String acta, String Delegacion,string nic, List<string> Files)
        {
            string tiffPath = @"C:\Temp\" + acta + "_" + nic + ".tif";
            bool saveImg = false;
            System.Drawing.Image imagen = null;
            int desired_x_dpi = 300;
            int desired_y_dpi = 300;



            foreach (string filename in Files)
            {
                LOG("Conversión archivo " + filename);
                //Pdf2ImageConverter p2i = new Pdf2ImageConverter(@filename);
                //int pages = p2i.GetPageCount();

                //SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
                //string pdfPath = @filename;
                //f.OpenPdf(pdfPath);
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(@filename);
                    int pages = rasterizer.PageCount;

                    //if (f.PageCount > 0)
                    if (pages > 0)
                    {
                        //f.ImageOptions.Dpi = 120;
                        //for (int page = 1; page <= f.PageCount; page++)

                        //int width, height;
                        //p2i.GetPageSize(1, out width, out height);

                        for (int page = 1; page <= pages; page++)
                        {
                            //Bitmap bm = p2i.GetImage(page, 500, width, height, Pdf2Image.Pdf2ImageFormat.PNG);
                            //byte[] tiff = f.ToImage(page);
                            ImageConverter converter = new ImageConverter();
                            //byte[] tiff = (byte[])converter.ConvertTo(bm, typeof(byte[]));
                            Image bm = rasterizer.GetPage(desired_x_dpi, desired_y_dpi, page);
                            byte[] tiff = (byte[])converter.ConvertTo(bm, typeof(byte[]));
                            //ImageCodecInfo encoderInfo = GetEncoderInfo("image/tiff");

                            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.SaveFlag;
                            ImageCodecInfo encoderInfo = ImageCodecInfo.GetImageEncoders().First(i => i.MimeType == "image/tiff");
                            EncoderParameters encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);


                            if (!saveImg)
                            {
                                // Primera imagen
                                //LOG("Convirtiendo primer archivo " + filename);
                                imagen = System.Drawing.Image.FromStream(new MemoryStream(tiff));
                                imagen.Save(tiffPath, encoderInfo, encoderParameters);
                                saveImg = true;
                            }
                            else
                            {
                                //LOG("Convirtiendo otros archivo " + filename);
                                encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);
                                System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(tiff));
                                imagen.SaveAdd(img, encoderParameters);
                            }

                        }
                        //f.ToMultipageTiff(tiffPath);
                        //f.ClosePdf();

                        LOG("Conversión Finalizada");
                    }
                }
            }

            if (imagen != null)
            {
                imagen.Dispose();
                if (this.SubirArchivoAFTP("ftp://181.48.232.226:2221", "ext.arivera", "Applus2016#", tiffPath, "/" + Delegacion, Path.GetFileName(tiffPath)))
                {
                    System.IO.File.Delete(tiffPath);

                }

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private bool SubirArchivoAFTP(string server, string user, string pass, string origen, string rutadestino, string nombredestino)
        {
            LOG("Iniciando Carga FTP");    
            try
                {
                    LOG("Subiendo Archivo: " + server + rutadestino + "/" + nombredestino);
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(server + rutadestino + "/" + nombredestino);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(user, pass);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = true;
                    FileStream stream = File.OpenRead(origen);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Flush();
                    reqStream.Close();
                    LOG("Carga Completa...");
                    return true;
                }
                catch(Exception ex)
                {
                    LOG("Error: " + ex.Message);
                    return false;
                }

}

        
    }
}


