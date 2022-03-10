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
    public partial class FrmUpdateTarifa : Form
    {
        public FrmUpdateTarifa()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<actas> lista = new List<actas>();
            int contador1 = 0;
            int contador2 = 0;
            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "select nic,_number,_clientCloseTs From Actas Where convert(date,fechaCarga) Between @fecha1 and @fecha2";
                if (checkBox1.Checked)
                {
                    sql += " AND EstadoActa = 6";
                }
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    cmd.Parameters.Add("@fecha1", SqlDbType.Date, 20).Value = dateTimePicker1.Value.Date;
                    cmd.Parameters.Add("@fecha2", SqlDbType.Date, 11).Value = dateTimePicker2.Value.Date;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WSTarifa ws = new WSTarifa();
                            ws.nic = reader.GetString(0);
                            DateTime fecha = reader.GetDateTime(2);
                            ws.fecha = fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0');
                            ws.CallWebService();
                            if (ws.Tarifa != null)
                            {
                                try
                                {
                                    actas acta = new actas();
                                    acta.Acta = reader.GetInt32(1);
                                    acta.Tarifa = Double.Parse(ws.Tarifa.ValorTarifa.Replace(".",","));
                                    lista.Add(acta);
                                    contador1++;
                                }
                                catch (Exception)
                                {

                                }
                            }


                        }
                    }
                }

                if (lista.Count > 0)
                {
                    foreach (actas acta in lista)
                    {
                        sql = "UPDATE Actas SET ValorTarifa=@tarifa WHERE Actas._number = @acta";
                        using (SqlCommand cmd = new SqlCommand(sql))
                        {
                            cmd.Connection = conexion.getConection();
                            cmd.Parameters.Add("@tarifa", SqlDbType.Decimal, 20).Value = acta.Tarifa;
                            cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = acta.Acta;

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                contador2++;
                                LOG("Actualizando acta No. " + acta.Acta + " con Tarifa " + acta.Tarifa);
                            }
                        }

                    }

                }
                label3.Text = "Procesadas: " + contador1;
                label4.Text = "Actualizadas: " + contador2;

                conexion.Close();
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        public void LOG(string log)
        {
            string fecha = DateTime.Now.ToString();
            String filename = Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }

            //listBox2.Items.Add(fecha + " " + log);
        }

        private class actas
        {
            public  Int32 Acta { set; get; }
            public double Tarifa { set; get; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
