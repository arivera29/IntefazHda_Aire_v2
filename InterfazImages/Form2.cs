using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazImages
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            txtRuta.Text = Properties.Settings.Default.ruta_doc;
            txtConexion.Text = Properties.Settings.Default.conexion;
            txtServerFtp.Text = Properties.Settings.Default.serverFtp;
            txtUserFtp.Text = Properties.Settings.Default.userFtp;
            txtClaveFtp.Text = Properties.Settings.Default.claveFtp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ruta_doc = txtRuta.Text.Trim();
            Properties.Settings.Default.conexion = txtConexion.Text.Trim();
            Properties.Settings.Default.serverFtp = txtServerFtp.Text.Trim();
            Properties.Settings.Default.userFtp = txtUserFtp.Text.Trim();
            Properties.Settings.Default.claveFtp = txtClaveFtp.Text.Trim();
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
