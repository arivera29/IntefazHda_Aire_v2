using iTextSharp.text;
using iTextSharp.text.pdf;
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

namespace InterfazHda
{
    public partial class FrmImagenGuia : Form
    {
        int contador = 0;
        public FrmImagenGuia()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = "";
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtCarpeta.Text = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                string currentDirName = txtCarpeta.Text.Trim();
                //string[] files = System.IO.Directory.GetFiles(currentDirName, "*.tif");
                var files = Directory.GetFiles(currentDirName, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".tif") || s.EndsWith(".TIF") || s.EndsWith(".jpg") || s.EndsWith(".JPG") || s.EndsWith(".png") || s.EndsWith(".PNG"));

                foreach (string s in files)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(s);
                    listBox1.Items.Add(fi.FullName);

                }
                listBox1.Refresh();
                lbTotal.Text = "Total Archivos: " + listBox1.Items.Count;

            }
        }

        private void cmdProcesar_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No hay archivos en la carpteta seleccionada");
                return;
            }

            if (txtCarpetaDestino.Text == "")
            {
                MessageBox.Show("Debe seleccionar la carpeta destino");
                return;
            }
            contador = 0;
            
            for (int x = 0; x < listBox1.Items.Count; x++)
            {
                String path = listBox1.Items[x].ToString();
                System.IO.FileInfo fi = new System.IO.FileInfo(path);
                String filename = fi.Name;

                //listBox2.Items.Add(filename);
                String nroGuia = Path.GetFileNameWithoutExtension(path);
                listBox2.Items.Add("Buscando guia No. " + nroGuia);
                ProcesarArchivo(path, filename, nroGuia);
            }
            listBox2.Refresh();
            MessageBox.Show("Proceso finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProcesarArchivo(String path,String filename, String guia)
        {

            Datos conexion = new Datos();
            if (conexion != null)
            {
                Int32 nroActa = 0;
                Boolean encontrado = false;
                String sql = "SELECT mensActa,UploadImagen FROM Mensajeria WHERE GuiaMensajeria = @guia";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@guia", SqlDbType.VarChar, 20).Value = guia;
                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetInt32(1) == 0)
                            {
                                encontrado = true;
                                nroActa = reader.GetInt32(0);
                            }
                            else
                            {
                                listBox2.Items.Add("Guia encontrada, imagen ya cargada!!!");
                            }
                        }
                        else
                        {
                            listBox2.Items.Add("Guia " + guia + " No encontrada ");
                        }

                    }
                }

                if (encontrado)
                {
                    if (this.ConvertirTIFtoPDF(path, filename))
                    {
                        conexion.BeginTransaction();
                        sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve)"
                        + " VALUES (@acta,14,'','interfaz',SYSDATETIME(),@url,1,0,'',NULL)";
                        bool registrado = false;
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                            cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "File/Guias/" + filename + ".pdf";
                            cmd.Transaction = conexion.getTransaction();
                            cmd.Prepare();
                            

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                listBox2.Items.Add("Archivo " + path + " asociado al Acta No. " + nroActa);
                                registrado = true;
                            }
                            else
                            {
                                listBox2.Items.Add("Error al insertar el registro de documento del acta " +  nroActa + " path " + path);
                            }

                        }

                        if (registrado)  // Actualizar registro de mensajeria
                        {
                            sql = "UPDATE Mensajeria SET UploadImagen = 1 WHERE MensActa=@acta";
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                                cmd.Transaction = conexion.getTransaction();
                                cmd.Prepare();

                                if (cmd.ExecuteNonQuery() >= 0)
                                {
                                    
                                    conexion.Commit();
                                    listBox2.Items.Add("Acta " + nroActa + " Actualizada correctamente");
                                    contador++;
                                }
                                else
                                {
                                    conexion.Rollback();
                                    listBox2.Items.Add("Error al actualizar tabla mensajeria  " + nroActa + " path " + path);
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
                        listBox2.Items.Add("Error al generar PDF de guia.  Archivo: " + filename);
                    }
                }
                

                conexion.Close();
                lbTotalGuias.Text = "Total Guias Cargadas: " + contador;

                

            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String filename = listBox1.Items[listBox1.SelectedIndex].ToString();
            pictureBox1.Image = new Bitmap(filename);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String filenamePDF = listBox1.Items[listBox1.SelectedIndex].ToString() + ".pdf";
            String filenameImage = listBox1.Items[listBox1.SelectedIndex].ToString();

            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER);
            PdfWriter writer = PdfAWriter.GetInstance(doc, new FileStream(filenamePDF, FileMode.Create));
            doc.Open();

            // Creamos la imagen y le ajustamos el tamaño
            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(filenameImage);
            imagen.BorderWidth = 0;
            imagen.Alignment = Element.ALIGN_CENTER;
            //float percentage = 0.0f;
            //percentage = 50 / imagen.Width;
            imagen.ScalePercent(20);

            // Insertamos la imagen en el documento
            doc.Add(imagen);

            // Cerramos el documento
            doc.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = "";
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtCarpetaDestino.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private bool ConvertirTIFtoPDF(String path,String filename)
        {
            bool result = false;
            try
            {
                String filenamePDF = txtCarpetaDestino.Text + "\\" + filename + ".pdf";
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
                listBox2.Items.Add(" Archivo: " + filename + " Convertido a PDF correctamente");
                result = true;
            }
            catch (Exception e)
            {
                listBox2.Items.Add("Error. " + e.Message + " Archivo: " + path);
            }
            return result;
        }

        private void txtCarpetaDestino_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
