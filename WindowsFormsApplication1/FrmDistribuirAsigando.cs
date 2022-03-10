using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmDistribuirAsigando : Form
    {
        DataTable dt;
        public FrmDistribuirAsigando()
        {
            InitializeComponent();
        }

        private void FrmDistribuirAsigando_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Asignar");

            dt.Columns[0].DataType = typeof(Int32);
            dt.Columns[1].DataType = typeof(string);
            dt.Columns[2].DataType = typeof(bool);

            

            Datos conexion = new Datos();
            if (conexion != null)
            {
                string sql = "select bandcodi,banddesc from bandejas order by banddesc";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();

                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["Codigo"] = reader.GetInt32(0);
                            row["Nombre"] = reader.GetString(1);
                            row["ASignar"] = false;

                            dt.Rows.Add(row);
                        }
                    }


                }



                conexion.Close();
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }

            dataGridView1.DataSource = dt;
            dataGridView1.Refresh();
        }

        private void btnDistribuir_Click(object sender, EventArgs e)
        {
            int actasAsignadas = 0;
            List<Int32> bandejas = new List<Int32>();
            
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows) {

                    if ((bool)row["Asignar"] == true)
                    {
                        bandejas.Add((Int32)row["Codigo"]);
                    }

                }

                if (bandejas.Count > 0)
                {
                    lbBandejas.Text = "Bandejas seleccionadas: " + bandejas.Count;
                    Datos conexion = new Datos();
                    if (conexion != null)
                    {
                        long total = 0;
                        string sql = "select count(*) from actas where Actas.EstadoActa in(1,2) and osResuelta=1 and protocolo in (0,2)";
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetInt32(0) > 0)
                                    {
                                        total = reader.GetInt32(0);
                                        lbTotal.Text = "Total Actas: " + total;
                                    }
                                }
                             }

                        }

                        if (total > 0)
                        {
                            List<Int32> actas = new List<Int32>();
                            
                            int reg = (int)(total / bandejas.Count);
                            lbActasDistribuir.Text = "Actas a distribuir: " + reg;

                            sql = "select _number from actas where Actas.EstadoActa in(1,2) and osResuelta=1 and protocolo in (0,2)";
                            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql))
                            {
                                cmd.Connection = conexion.getConection();
                                using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        actas.Add(reader.GetInt32(0));
                                    }
                                }

                                if (actas.Count > 0)
                                {
                                    int contador = 0;
                                    int indice = 0;
                                    foreach (Int32 acta in actas)
                                    {
                                        sql = "update Actas set Bandeja=@bandeja  where Actas.EstadoActa in(1,2) and osResuelta=1 and protocolo in (0,2) and _number=@acta ";
                                        using (System.Data.SqlClient.SqlCommand cmd2 = new System.Data.SqlClient.SqlCommand(sql))
                                        {
                                            cmd2.Connection = conexion.getConection();
                                            cmd2.Parameters.Add("@acta", SqlDbType.Int, 32).Value = acta;
                                            cmd2.Parameters.Add("@bandeja", SqlDbType.VarChar, 20).Value = bandejas[indice];
                                            cmd2.ExecuteNonQuery();
                                            actasAsignadas++;
                                            contador++;
                                            if (contador > reg)
                                            {
                                                contador = 0;
                                                indice++;
                                            }

                                        }

                                    }

                                    lbBandejasDistribuidas.Text = "Bandejas distribuidas: " + indice;

                                }

                            }



                        }


                    }
                    else
                    {
                        MessageBox.Show("Error al conectarse con el servidor");
                    }


                }
                else
                {
                    MessageBox.Show("No ha seleccionado ninguna bandeja");
                }

            }
            else
            {
                MessageBox.Show("No hay Registro de bandejas");
            }
            lbActasAsignadas.Text = "Actas asignadas: " + actasAsignadas;

            MessageBox.Show("Proceso finalizado");
        }
    }
}
