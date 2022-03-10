using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValidacionHgi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "SELECT DocuUrLo FROM Documentos WHERE DocuActa=@acta";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@acta", SqlDbType.VarChar, 20).Value = textBox1.Text.Trim();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String path = reader.GetString(0);
                            String filename = Path.GetFileName(path);
                            String DirDocumentos = @"C:\inetpub\wwwroot\HGI\File\Documentos";
                            filename = DirDocumentos + @"\" + filename;
                            String cadena = "OK";
                            if (!File.Exists(filename))
                            {
                                cadena = "NO EXISTE";
                            }

                            listBox1.Items.Add(Path.GetFileName (path) + " " + cadena);
                            

                        }
                    }
                }


            }
            else
            {
                MessageBox.Show("Error de conexion");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String path = @"C:\Users\Aimer\Documents\HDA\21476187a15.pdf";
            SplitePDF(path);
        }

        void SplitePDF(string filepath)
        {

            string outfile = Path.GetDirectoryName(filepath) + Path.GetFileNameWithoutExtension(filepath) + ".tif";
            iTextSharp.text.pdf.PdfReader reader = null;
            int currentPage = 1;
            int pageCount = 0;
            //string filepath_New = filepath + "\\PDFDestination\\";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //byte[] arrayofPassword = encoding.GetBytes(ExistingFilePassword);
            reader = new iTextSharp.text.pdf.PdfReader(filepath);
            reader.RemoveUnusedObjects();
            pageCount = reader.NumberOfPages;
            string ext = System.IO.Path.GetExtension(filepath);
            for (int i = 1; i <= pageCount; i++)
            {
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(filepath);
                //string outfile = filepath.Replace((System.IO.Path.GetFileName(filepath)), (System.IO.Path.GetFileName(filepath).Replace(".pdf", "") + "_" + i.ToString()) + ext);
                reader1.RemoveUnusedObjects();
                iTextSharp.text.Document doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(currentPage));
                iTextSharp.text.pdf.PdfCopy pdfCpy = new iTextSharp.text.pdf.PdfCopy(doc, new System.IO.FileStream(outfile, System.IO.FileMode.Create));
                doc.Open();
                for (int j = 1; j <= 1; j++)
                {
                    iTextSharp.text.pdf.PdfImportedPage page = pdfCpy.GetImportedPage(reader1, currentPage);
                    
                    //pdfCpy.SetFullCompression();
                    pdfCpy.AddPage(page);
                    currentPage += 1;
                }
                doc.Close();
                pdfCpy.Close();
                reader1.Close();
                reader.Close();
            }
        }
    }
}
