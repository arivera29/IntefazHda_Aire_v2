using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmCleanDocumentos : Form
    {
        DataTable dt;

        public FrmCleanDocumentos()
        {
            InitializeComponent();
        }

        private void FrmCleanDocumentos_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Ruta", typeof(String));
            dt.Columns.Add("Resultado", typeof(String));
            dataGridView1.DataSource = dt;

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
                while (!sr.EndOfStream)
                {
                    cont++;
                    try
                    {
                        string[] fila = sr.ReadLine().Split('\t');
                        DataRow row = dt.NewRow();
                        row["Ruta"] = fila[0];
                        dt.Rows.Add(row);
                    }
                    catch (Exception e)
                    {
                        
                    }

                }
                dataGridView1.Refresh();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de eliminar los documentos", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataRow row in dt.Rows)
                {
                    String ruta = (String)row["Ruta"];
                    String path = @"F:\HGI2\Documentos\" + System.IO.Path.GetFileName(ruta);
                    try
                    {
                        System.IO.File.Delete(path);
                        row["Resultado"] = "ARCHIVO ELIMINADO. " + path;
                    }
                    catch (System.IO.IOException ex)
                    {
                        row["Resultado"] = ex.Message;
                    }

                }
            }
        }
    }
}
