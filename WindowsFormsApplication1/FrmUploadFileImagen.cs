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
    public partial class FrmUploadFileImagen : Form
    {
        DataTable dt;
        public FrmUploadFileImagen()
        {
            InitializeComponent();
        }

        private void cmdBuscar_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtFile.Text = openFileDialog1.FileName;
                ProcesarArchivo();

            }
            else
            {
                txtFile.Text = "";
            }
        }

        private void ProcesarArchivo()
        {
            string filename = txtFile.Text.Trim();
            dt.Rows.Clear();
            using (StreamReader sr = new StreamReader(@filename))
            {
                int cont = 0;
                int contJPG = 0;
                int contTIF = 0;
                while (!sr.EndOfStream)
                {
                    cont++;
                    try
                    {
                        string[] fila = sr.ReadLine().Split(';');
                        if (fila[4].ToLower().Contains("tif") || fila[4].ToLower().Contains("jpg"))
                        {
                            if (fila[4].ToLower().Contains("tif"))
                            {
                                contTIF++;
                            }
                            if (fila[4].ToLower().Contains("jpg"))
                            {
                                contJPG++;
                            }
                            DataRow row = dt.NewRow();
                            row["Id"] = Int32.Parse(fila[0]);
                            row["Path"] = fila[1];
                            row["Filename"] = fila[2];
                            row["RelativePath"] = fila[3];
                            row["Extension"] = fila[4];
                            row["Nic"] = fila[5];
                            row["Fr"] = fila[6];
                            row["Type"] = fila[7];
                            row["Size"] = Double.Parse(fila[8]);
                            row["Ubicacion"] = fila[9];
                            dt.Rows.Add(row);
                        }
                    }
                    catch (Exception e)
                    {
                        LOG(e.Message);
                    }

                }
                dataGridView1.Refresh();
                lbTotal.Text = "Total Archivos de Guia: " + dt.Rows.Count + " de " + cont + ". TIF: " + contTIF + ", JPG:" + contJPG;
                if (dt.Rows.Count > 0)
                {
                    btnUpload.Enabled = true;
                }
            }
        }

        private void FrmUploadFileImagen_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Id", typeof(Int32));
            dt.Columns.Add("Path", typeof(String));
            dt.Columns.Add("Filename", typeof(String));
            dt.Columns.Add("RelativePath", typeof(String));
            dt.Columns.Add("Extension", typeof(String));
            dt.Columns.Add("Nic", typeof(String));
            dt.Columns.Add("Fr", typeof(String));
            dt.Columns.Add("Type", typeof(String));
            dt.Columns.Add("Size", typeof(Double));
            dt.Columns.Add("Ubicacion", typeof(String));
            dataGridView1.DataSource = dt;

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

        public void LOG(string log)
        {
            listBox1.Items.Add(log);

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay registros en el archivo seleccionado");
                return;
            }

            if (txtCarpetaDestino.Text == "")
            {
                MessageBox.Show("Debe seleccionar la carpeta destino");
                return;
            }

            foreach (DataRow row in  dt.Rows)
            {
                String path = (String)row["Path"];
                System.IO.FileInfo fi = new System.IO.FileInfo(path);
                String filename = fi.Name;

                //listBox2.Items.Add(filename);
                String Nic = (String)row["Nic"];
                String Fr = (String)row["FR"];
                LOG("Buscando NIC / FR " + Nic + " / " + Fr);
                ProcesarRegistro(path, filename, Nic, Fr);
            }
        }

        private void ProcesarRegistro(String path, String filename, String Nic, String Fr)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                Int32 nroActa = 0;
                Boolean encontrado = false;
                String sql = "SELECT mensActa,UploadImagen FROM Mensajeria, ProcesoSimpli, Actas "
                    + " WHERE Mensajeria.MensActa= Actas._number "
                    + " AND  Mensajeria.MensActa = ProcesoSimpli.NoAcProc "
                    + " AND Actas.Nic = @Nic " 
                    + " AND ProcesoSimpli.AnLaProce = @Fr " ;
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@Nic", SqlDbType.VarChar, 20).Value = Nic;
                    cmd.Parameters.Add("@Fr", SqlDbType.VarChar, 20).Value = Fr;
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
                                LOG("FR Encontrado, imagen ya cargada!!!");
                            }
                        }
                        else
                        {
                            LOG("NIC/FR " + Nic + " / " + Fr  + " No encontrado ");
                        }

                    }
                }

                int contGuia = 0;
                if (encontrado)
                {
                    
                    if (this.ConvertirTIFtoPDF(path, filename))
                    {
                        sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve)"
                        + " VALUES (@acta,14,'','interfaz',SYSDATETIME(),@url,1,0,'',NULL)";
                        bool registrado = false;
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = nroActa;
                            cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "File/Guias/" + filename + ".pdf";
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
                                cmd.Prepare();

                                if (cmd.ExecuteNonQuery() >= 0)
                                {

                                    //conexion.Commit();
                                    LOG("Acta " + nroActa + " Actualizada correctamente");
                                    contGuia++;
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

                lbUpload.Text = "Total Guias Actualizadas: " + contGuia;
                conexion.Close();

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
    }
}
