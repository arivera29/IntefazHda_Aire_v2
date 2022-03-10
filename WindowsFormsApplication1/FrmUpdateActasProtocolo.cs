using Newtonsoft.Json.Linq;
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
    public partial class FrmUpdateActasProtocolo : Form
    {
        DataTable dt;
        private List<Acta> listaActas;

        public FrmUpdateActasProtocolo()
        {
            InitializeComponent();
        }

        private void FrmCleanDocumentos_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Acta", typeof(int));
            dt.Columns.Add("Error", typeof(double));
            dt.Columns.Add("Observacion1", typeof(String));
            dt.Columns.Add("Observacion2", typeof(String));
            dt.Columns.Add("Observacion3", typeof(String));
            dt.Columns.Add("LOG", typeof(String));
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
                        row["Acta"] = fila[0];
                        String error = fila[1].Replace('.', ',');
                        if (error == "")
                        {
                            error = "0";
                        }
                        row["Error"] = error;
                        row["Observacion1"] = fila[2];
                        row["Observacion2"] = fila[3];
                        row["Observacion3"] = fila[4];
                        dt.Rows.Add(row);
                    }
                    catch (Exception e)
                    {
                        LOG(e.Message);
                    }

                }
                dataGridView1.Refresh();
            }
        }

        

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                if (MessageBox.Show("Esta seguro de actualizar las actas", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Datos conexion = new Datos();
                    listaActas = new List<Acta>();
                    if (conexion != null)
                    {
                       
                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                string sql = "UPDATE actas SET protocolo=2, porcentajeError=@error WHERE _number=@acta AND protocolo=1";
                                bool pasa = false;
                                string log = "";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];
                                    cmd.Parameters.Add("@error", SqlDbType.Decimal).Value = (double)row["Error"];
                                    cmd.Parameters["@error"].Precision = 12;
                                    cmd.Parameters["@error"].Scale = 2;

                                    cmd.Prepare();

                                    if (cmd.ExecuteNonQuery() >= 0)
                                    {
                                        pasa = true;
                                        log = "OK";
                                    }

                                }

                                if (pasa)
                                {
                                    sql = "INSERT INTO AnotacionActa(AnotActa,AnotDesc,AnotUsua,AnotFeSi,AnotEsta) VALUES (@acta,@obs,'interfaz',SYSDATETIME(),1)";
                                    using (SqlCommand cmd = new SqlCommand(sql))
                                    {
                                        cmd.Connection = conexion.getConection();
                                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];
                                        cmd.Parameters.Add("@obs", SqlDbType.VarChar, 255).Value = (string)row["Observacion1"];
                                        cmd.Prepare();

                                        if (cmd.ExecuteNonQuery() >= 0)
                                        {
                                            log += ",OK";
                                        }

                                    }

                                    sql = "INSERT INTO AnotacionActa(AnotActa,AnotDesc,AnotUsua,AnotFeSi,AnotEsta) VALUES (@acta,@obs,'interfaz',SYSDATETIME(),1)";
                                    using (SqlCommand cmd = new SqlCommand(sql))
                                    {
                                        cmd.Connection = conexion.getConection();
                                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];
                                        cmd.Parameters.Add("@obs", SqlDbType.VarChar, 255).Value = (string)row["Observacion2"];
                                        cmd.Prepare();

                                        if (cmd.ExecuteNonQuery() >= 0)
                                        {
                                            log += ",OK";
                                        }

                                    }

                                    sql = "INSERT INTO AnotacionActa(AnotActa,AnotDesc,AnotUsua,AnotFeSi,AnotEsta) VALUES (@acta,@obs,'interfaz',SYSDATETIME(),1)";
                                    using (SqlCommand cmd = new SqlCommand(sql))
                                    {
                                        cmd.Connection = conexion.getConection();
                                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];
                                        cmd.Parameters.Add("@obs", SqlDbType.VarChar, 255).Value = (string)row["Observacion3"];
                                        cmd.Prepare();

                                        if (cmd.ExecuteNonQuery() >= 0)
                                        {
                                            log += ",OK";
                                        }

                                    }
                                }
                                row["LOG"] += log;

                            }
                            catch (SqlException ex)
                            {
                                row["LOG"] = ex.Message;
                            }
                        }

                        

                        if (checkBox1.Checked) {

                            GestionActa gestion = new GestionActa();
                            gestion.conexion = conexion;
                            gestion.DistribuirActas();
                        
                        }

                        conexion.Close();

                        dataGridView1.Refresh();
                        MessageBox.Show("Proceso finalizado");
                    }
                    else
                    {
                        MessageBox.Show("Error al conectarse con el servidor");
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay registros para procesar");
            }
        }


        private bool ExisteActa(String _number, Datos conexion)
        {
            bool resultado = false;
            if (conexion != null)
            {
                String sql = "SELECT _number FROM Actas WHERE _number = @_number";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@_number", SqlDbType.VarChar, 20).Value = _number;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado = true;
                        }
                    }

                }

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
            String filename = Environment.CurrentDirectory + @"\LOG\UPLOAD_ACTAS_HGI_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
        }
    }
}
