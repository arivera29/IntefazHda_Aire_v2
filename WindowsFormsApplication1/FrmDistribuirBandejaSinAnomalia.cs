using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmDistribuirBandejaSinAnomalia : Form
    {
        public FrmDistribuirBandejaSinAnomalia()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de distribuir las actas sin Anomalia?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Datos conexion = new Datos();
                if (conexion != null)
                {
                    List<Int32> Actas = new List<Int32>();
                    List<Int32> Bandejas = new List<Int32>();
                    String sql = "Select _number From Actas with(nolock) Where EstadoActa=15";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Prepare();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Actas.Add(reader.GetInt32(0));
                            }

                        }
                    }

                    label1.Text = "Total Actas: " + Actas.Count;

                    sql = "Select BandCodi From Bandejas Where BandEsta=1 And BandTiba=5";
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        cmd.Connection = conexion.getConection();
                        cmd.Prepare();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Bandejas.Add(reader.GetInt32(0));
                            }

                        }
                    }
                    label3.Text = "Total Bandejas: " + Bandejas.Count;


                    int cntActas = (int)(Actas.Count / Bandejas.Count);
                    int contador = 0;
                    int index = 0;
                    int cntUpdate = 0;
                    foreach (Int32 acta in Actas)
                    {
                        label4.Text = "Index: " + index;
                        sql = "UPDATE Actas set Bandeja=@bandeja Where _number=@acta And EstadoActa=15";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@bandeja", SqlDbType.Int, 11).Value = Bandejas[index];
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta;
                            cmd.Prepare();

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                cntUpdate++;
                                label2.Text = "Total Actas Actualizadas: " + cntUpdate;
                            }

                        }


                        contador++;
                        if (cntActas == contador)
                        {
                            contador = 0;
                            if ((index + 1) < Bandejas.Count)
                            {
                                index++;
                            }
                        }

                    }



                    conexion.Close();
                }
                else
                {
                    MessageBox.Show("Error al conectarse con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
