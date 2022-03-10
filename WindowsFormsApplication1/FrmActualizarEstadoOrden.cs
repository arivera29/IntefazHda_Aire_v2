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
    public partial class FrmActualizarEstadoOrden : Form
    {
        DataTable dt;
        private List<Acta> listaActas;

        public FrmActualizarEstadoOrden()
        {
            InitializeComponent();
        }

        private void FrmCleanDocumentos_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Acta", typeof(int));
            dt.Columns.Add("Nic", typeof(string));
            dt.Columns.Add("LOG", typeof(string));
            dt.Columns.Add("Respuesta", typeof(String));
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
                        row["Nic"] = fila[1];
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
                                bool pasa = true;
                                string log = "";
                                if (radioButton1.Checked)  // Consultar estado Web Service
                                {
                                    WSOrdenes ws = new WSOrdenes();
                                    ws.Nic = (string)row["Nic"];
                                    ws.OrdenServicio =  ((int)row["Acta"]).ToString();
                                    DateTime fecha = DateTime.Now;
                                    ws.fecha = fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0');
                                    ws.CallWebService();
                                    if (!ws.Resuelta)
                                    {
                                        log = "Orden no resuelta WS";
                                        pasa = false;
                                    }
                                    
                                }

                                if (pasa)
                                {
                                    string sql = "UPDATE actas SET osResuelta=1, fechaOrden=SYSDATETIME() WHERE _number=@acta AND osResuelta=0";

                                    using (SqlCommand cmd = new SqlCommand(sql))
                                    {
                                        cmd.Connection = conexion.getConection();
                                        cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];

                                        cmd.Prepare();

                                        if (cmd.ExecuteNonQuery() >= 0)
                                        {
                                            pasa = true;
                                            log = "OK";
                                        }
                                        else
                                        {
                                            log = "No Actualizada";
                                        }

                                    }

                                    if (pasa)
                                    {
                                        sql = "INSERT INTO AnotacionActa(AnotActa,AnotDesc,AnotUsua,AnotFeSi,AnotEsta) VALUES (@acta,@obs,'interfaz',SYSDATETIME(),1)";
                                        using (SqlCommand cmd = new SqlCommand(sql))
                                        {
                                            cmd.Connection = conexion.getConection();
                                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = (int)row["Acta"];
                                            cmd.Parameters.Add("@obs", SqlDbType.VarChar, 255).Value = "Orden de servicio actualizada a Resuelta por Intefaz";
                                            cmd.Prepare();

                                            if (cmd.ExecuteNonQuery() >= 0)
                                            {
                                                log += ",OK";
                                            }

                                        }
                                    }
                                    row["LOG"] += log;
                                }

                            }
                            catch (SqlException ex)
                            {
                                row["LOG"] = ex.Message;
                            }

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
