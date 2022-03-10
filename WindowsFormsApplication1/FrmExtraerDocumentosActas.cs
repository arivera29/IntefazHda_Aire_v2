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
    public partial class FrmExtraerDocumentosActas : Form
    {
        DataTable dt;
        public FrmExtraerDocumentosActas()
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

        private void FrmExtraerDocumentosActas_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Acta");

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
            String filename = Environment.CurrentDirectory + @"\LOG\EXTRAER_DOCUMENTOS_HGI_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
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

        private void CrearDirectorios()
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    String acta = (String)row["Acta"];
                    String folder = txtCarpetaDestino.Text.Trim() + "\\" + acta.Trim();
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                }
            }
        }

        private void cmdExtraer_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("NO hay actas para extraer", "Extraer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtCarpetaDestino.Text == "")
            {
                MessageBox.Show("Debe seleccionar la carpeta destino", "Extraer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CrearDirectorios();
            Datos conexion = new Datos();
            if (conexion != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    String acta = (String)row["Acta"];
                    String sql = "SELECT DocuUrLo "
                        + " FROM Documentos,TipoDocumento "
                        + " WHERE DocuActa = @acta "
                        + " AND TipoDocumento.TiDoCodi=Documentos.DocuTiDo "
                        + " AND (TipoDocumento.TiDoMens=1 OR TipoDocumento.TidoCodi IN(14) )"
                        + " ORDER BY TiDoOrMe";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Parameters.Add("@acta", SqlDbType.Int, 20).Value = acta;
                        cmd.Prepare();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int cont = 0;
                            while (reader.Read())
                            {
                                String path = reader.GetString(0);
                                String origen = getLocalPath(path);
                                String filename = Path.GetFileName(origen);
                                LOG("Acta " + acta + " Ruta " + origen);
                                if (File.Exists(origen))
                                {
                                    System.IO.File.Copy(@origen, Path.Combine(txtCarpetaDestino.Text + "\\" + acta, filename), true);
                                    LOG("Archivo " + filename + " copiado correctamente");
                                }
                                else
                                {
                                    LOG("Archivo " + origen + " no existe");
                                }
                                
                                cont++;
                            }

                            if (cont == 0)
                            {
                                LOG("Acta " + acta + " no tiene archivos");
                            }

                        }
                    }


                }
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor de base de datos", "Extraer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Proceso finalizado", "Fin", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private String getLocalPath(String path)
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

            if (ruta.Contains("File/Guias/"))
            {
                ruta = ruta.Replace("File/Guias/", "G:\\Guias\\");
                ruta = ruta.Replace("/", "\\");
            }


            return ruta;
        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
