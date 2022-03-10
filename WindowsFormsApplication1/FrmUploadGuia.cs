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
    public partial class FrmUploadGuia : Form
    {
        DataTable dt;
        int contador;
        public FrmUploadGuia()
        {
            InitializeComponent();
        }

        private void cmdFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtFilename.Text = openFileDialog1.FileName;
                PopulateGrid(txtFilename.Text);

            }
            lbTotal1.Text = "Total Guias: " + dt.Rows.Count;
        }

        private void FrmUploadGuia_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Acta");
            dt.Columns.Add("Guia");

            gridActas.DataSource = dt;
            gridActas.Refresh();

        }

        private void PopulateGrid(String filename)
        {

            dt.Rows.Clear();
            using (StreamReader sr = new StreamReader(@filename))
            {
                int cont = 0;
                while (!sr.EndOfStream)
                {
                    cont++;
                    try
                    {
                        string[] fila = sr.ReadLine().Split('\t');
                        DataRow row = dt.NewRow();
                        row["Acta"] = fila[0];
                        row["Guia"] = fila[1];
                        dt.Rows.Add(row);
                    }
                    catch (Exception e)
                    {
                        LOG(e.Message);
                    }

                }
                gridActas.Refresh();
            }

        }

        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\UPLOAD_GUIAS_HGI_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
                PopulateListBox(txtFolder.Text);

            }
        }

        private void PopulateListBox(String folder)
        {
            lstImagenes.Items.Clear();
            string currentDirName = folder;
            var files = Directory.GetFiles(currentDirName, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".tif") || s.EndsWith(".TIF") || s.EndsWith(".TIFF") || s.EndsWith(".tiff") || s.EndsWith(".jpg") || s.EndsWith(".JPG") || s.EndsWith(".png") || s.EndsWith(".PNG"));

            foreach (string s in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(s);
                lstImagenes.Items.Add(fi.FullName);

            }
            lstImagenes.Refresh();
            lbTotal2.Text = "Total Imagenes: " + lstImagenes.Items.Count;
        }

        private void cmdSubirImagenes_Click(object sender, EventArgs e)
        {
            if (lstImagenes.Items.Count == 0)
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

            for (int x = 0; x < lstImagenes.Items.Count; x++)
            {
                String path = lstImagenes.Items[x].ToString();
                System.IO.FileInfo fi = new System.IO.FileInfo(path);
                String filename = fi.Name;

                //listBox2.Items.Add(filename);
                String nroGuia = Path.GetFileNameWithoutExtension(path);
                LOG("Buscando guia No. " + nroGuia);
                ProcesarArchivo(path, filename, nroGuia);
            }
            lstImagenes.Refresh();
            MessageBox.Show("Proceso finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProcesarArchivo(String path, String filename, String guia)
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
                                LOG("Guia encontrada, imagen ya cargada!!!");
                            }
                        }
                        else
                        {
                            LOG("Guia " + guia + " No encontrada ");
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
                                cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                                cmd.Transaction = conexion.getTransaction();
                                cmd.Prepare();

                                if (cmd.ExecuteNonQuery() >= 0)
                                {

                                    conexion.Commit();
                                    LOG("Acta " + nroActa + " Actualizada correctamente");
                                    contador++;
                                }
                                else
                                {
                                    conexion.Rollback();
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


                conexion.Close();
                lbTotalGuias.Text = "Total Imagenes Cargadas: " + contador;



            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool ConvertirTIFtoPDF(String path, String filename)
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
                LOG(" Archivo: " + filename + " Convertido a PDF correctamente");
                result = true;
            }
            catch (Exception e)
            {
                LOG("Error. " + e.Message + " Archivo: " + path);
            }
            return result;
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

        private void cmdActualizarGuias_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    contador = 0;
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        conexion.BeginTransaction();
                        String sql = "UPDATE Mensajeria SET GuiaMensajeria=@guia WHERE MensActa = @acta AND uploadImagen=0 ";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@guia", SqlDbType.VarChar, 20).Value = (String)row["Guia"];
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (String)row["Acta"];
                            cmd.Transaction = conexion.getTransaction();
                            cmd.Prepare();
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                conexion.Commit();
                                LOG("Acta " + (String)row["Acta"] + " con guia " + (String)row["Guia"] + " Actualizada correctamente");
                                contador++;
                            }
                            else
                            {
                                conexion.Rollback();
                                LOG("Acta " + (String)row["Acta"] + " con guia " + (String)row["Guia"] + " No se pudo actualizar");
                            }

                        }

                    }

                    conexion.Close();
                    lbTotalActas.Text = "Total Actas Actualizadas: " + contador;
                    MessageBox.Show("Proceso finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
