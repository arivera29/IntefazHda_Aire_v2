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
    public partial class FrmWsConsumos : Form
    {
        public FrmWsConsumos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtNic.Text == "" || txtFecha.Text == "")
            {
                MessageBox.Show("Debe ingresar los datos de NIC y Fecha");
                return;
            }

            lstConsumos.Items.Clear();
            WSConsumo ws = new WSConsumo();
            ws.nic = txtNic.Text.Trim();
            ws.fecha = txtFecha.Text.Trim();

            ws.CallWebService();

            if (ws.ListaConsumos.Count > 0)
            {
                for (int x = 0; x < ws.ListaConsumos.Count; x++)
                {
                    lstConsumos.Items.Add(ws.ListaConsumos[x].fecha + " --> " + ws.ListaConsumos[x].consumo);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtNic.Text == "" || txtFecha.Text == "" || txtActa.Text == "")
            {
                MessageBox.Show("Debe ingresar los datos de NIC y Fecha");
                return;
            }
            Datos conexion = new Datos();
            if (conexion != null)
            {
                GestionActa ga = new GestionActa();
                ga.conexion = conexion;
                ga.ActualizarConsumosActa(txtActa.Text.Trim(), txtNic.Text.Trim(), txtFecha.Text.Trim(),true);
                conexion.Close();
                MessageBox.Show("Proceso finalizado, Verificar los consumos en la HGI");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtActas.Text != "")
            {
                String actas = txtActas.Text.Trim();
                String[] lista = actas.Split(',');
                int contador = 0;
                if (lista.Length > 0)
                {
                    Datos conexion = new Datos();
                    if (conexion != null)
                    {
                        foreach (String acta in lista)
                        {
                            if (acta != "")
                            {
                                String sql = "SELECT _number,nic FROM Actas with(nolock) WHERE _number=@acta";
                                String nic = "-1";
                                using (SqlCommand cmd = new SqlCommand(sql))
                                {
                                    cmd.Connection = conexion.getConection();
                                    cmd.Parameters.Add("@acta", SqlDbType.Int, 11).Value = Int32.Parse(acta);
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            nic = reader.GetString(1);
                                        }
                                    }


                                }

                                if (nic != "-1")
                                {
                                    GestionActa ga = new GestionActa();
                                    ga.conexion = conexion;
                                    String fecha = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                                    ga.ActualizarConsumosActa(acta, nic, fecha, true);
                                    contador++;
                                }
                            }
                        }
                        conexion.Close();
                        MessageBox.Show("Proceso Finalizado.  Actas procesadas " + contador);
                        
                    }
                    else
                    {
                        MessageBox.Show("Error al conectarse a la base de datos");

                    }
                }
                else
                {
                    MessageBox.Show("No se encontraron Actas");
                }
            }
            else
            {
                MessageBox.Show("Debe ingresar las actas a procesar");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {
                GestionActa gestion = new GestionActa();
                gestion.conexion = conexion;
                gestion.ActualizarConsumosOrdenServicio();
                conexion.Close();
                MessageBox.Show("Proceso finalizado");
            }
            else
            {
                MessageBox.Show("Error al conectase con el servidor");
            }
        }
    }
}
