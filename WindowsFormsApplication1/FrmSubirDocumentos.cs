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
    public partial class FrmSubirDocumentos : Form
    {
        int contador;

        public FrmSubirDocumentos()
        {
            InitializeComponent();
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
            var files = Directory.GetFiles(currentDirName, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".PDF") || s.EndsWith(".pdf"));

            foreach (string s in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(s);
                lstImagenes.Items.Add(fi.FullName);

            }
            lstImagenes.Refresh();
            lbTotal2.Text = "Total Archivos: " + lstImagenes.Items.Count;
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

            if (cboTipoArchivo.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar el tipo de archivo");
                return;
            }

            contador = 0;

            for (int x = 0; x < lstImagenes.Items.Count; x++)
            {
                try
                {
                    String path = lstImagenes.Items[x].ToString();
                    System.IO.FileInfo fi = new System.IO.FileInfo(path);
                    String filename = fi.Name;

                    //listBox2.Items.Add(filename);
                    LOG("Extrayendo No. Acta del archivo: " + path);
                    LOG("No. de Acta: " + Path.GetFileNameWithoutExtension(path));
                    int acta = Int32.Parse(Path.GetFileNameWithoutExtension(path));
                    LOG("Procesando Acta No. " + acta);
                    ProcesarArchivo(path, filename, acta);
                }
                catch (Exception ex)
                {
                    LOG("Error procesando archivo " + lstImagenes.Items[x].ToString() + ". " + ex.Message);
                }
            }
            lstImagenes.Refresh();
            MessageBox.Show("Proceso finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ProcesarArchivo(String path, String filename, int acta)
        {
            int tipoArchivo = Int32.Parse((String)cboTipoArchivo.SelectedValue);

            Datos conexion = new Datos();
            if (conexion != null)
            {
                Boolean encontrado = true;
                LOG("Buscando Documentos tipo " + tipoArchivo + " del acta " + acta + " en el servidor");
                String sql = "SELECT DocuTiDo FROM Documentos WHERE DocuActa = @acta AND DocuTido=@tipo";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = tipoArchivo;
                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())  // No tiene un documento de este tipo.
                        {
                            encontrado = false;
                        }
                        else
                        {
                            encontrado = true;
                            LOG("El acta " + acta + " no tiene asociado el tipo de archivo " + tipoArchivo );
                        }

                    }
                }

                if (!encontrado)
                {
                    String newFile = this.CopiarArchivo(path, filename);
                    if (!newFile.Equals(""))
                    {
                        conexion.BeginTransaction();
                        sql = "INSERT INTO DOCUMENTOS (DocuActa,DocuTiDo,DocuUrRe,DocuUsca,DocuFeCa,DocuUrlo,DocuSincro,DocuVeri,DocuUsve,DocuFeve,DocuPath)"
                        + " VALUES (@acta,@tipo,'','copy',SYSDATETIME(),@url,1,0,'',NULL,@path)";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                            cmd.Parameters.Add("@tipo", SqlDbType.Int, 11).Value = tipoArchivo;
                            cmd.Parameters.Add("@url", SqlDbType.VarChar, 250).Value = "File/Documentos/" + newFile;
                            cmd.Parameters.Add("@path", SqlDbType.VarChar, 250).Value = Path.Combine(txtCarpetaDestino.Text, newFile);
                            cmd.Transaction = conexion.getTransaction();
                            cmd.Prepare();


                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                conexion.Commit();
                                LOG("Archivo " + path + " asociado al Acta No. " + acta);
                                contador++;
                            }
                            else
                            {
                                conexion.Rollback();
                                LOG("Error al insertar el registro de documento del acta " + acta + " path " + path);
                            }

                        }

                        
                    }
                    else
                    {
                        LOG("Error al copiar el archivo: " + filename);
                    }
                }


                conexion.Close();
                lbTotalGuias.Text = "Total archivos Cargadas: " + contador;



            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public String CopiarArchivo(String path, String filename)
        {
            String resultado = "";
            try
            {
                String newFilename = Guid.NewGuid().ToString() + ".pdf";
                System.IO.File.Copy(@path, Path.Combine(txtCarpetaDestino.Text, newFilename), true);
                LOG("Archivo " + newFilename + " copiado correctamente.");
                resultado = newFilename;

            }
            catch (Exception e)
            {
                LOG("Error copiando archivo en la ruta de la HGi2. Archivo: " + path);
            }
            return resultado;
        }

        public void LOG(string log)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\LOG"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\LOG");
            }

            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + @"\LOG\UPLOAD_DOCUMENTOS_HGI_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSubirDocumentos_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Descripcion");
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "SELECT TiDoCodi, TiDoDocu FROM TipoDocumento WHERE TipoEsta = 1";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())  // No tiene un documento de este tipo.
                        {
                            DataRow row = dt.NewRow();
                            row["Codigo"] = reader.GetInt32(0);
                            row["Descripcion"] = reader.GetString(1);
                            dt.Rows.Add(row);
                        }
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cboTipoArchivo.DataSource = dt;
            cboTipoArchivo.DisplayMember = "Descripcion";
            cboTipoArchivo.ValueMember = "Codigo";
            cboTipoArchivo.Refresh();

        }
    }
}
