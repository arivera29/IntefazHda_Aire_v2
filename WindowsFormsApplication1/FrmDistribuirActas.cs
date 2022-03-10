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
    public partial class FrmDistribuirActas : Form
    {
        public FrmDistribuirActas()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                
                ga.DistribuirActas();

                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                ga.DistribuirLiquidacionAnticipadaActas();

                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                ga.DistribuirActasManuales();

                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                ga.DistribuirActasSubnormal();

                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                ga.ActualizarEstadoOrdenServicio();
                ga.ActualizarTarifaOrdenServicio();
                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Datos conexion = new Datos();
            if (conexion != null)
            {

                GestionActa ga = new GestionActa();
                ga.conexion = conexion;

                ga.DistribuirActasSinAnomalia();
                conexion.Close();
                MessageBox.Show("Proceso Finalizado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                MessageBox.Show("Error al conectarse con el servidor");
            }
        }
    }
}
