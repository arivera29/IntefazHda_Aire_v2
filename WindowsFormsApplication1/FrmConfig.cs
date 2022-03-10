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
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            txtUrlHDA.Text = Properties.Settings.Default.url_hda;
            txtUserHda.Text  = Properties.Settings.Default.user_hda;
            txtPasswordHDA.Text  = Properties.Settings.Default.pass_hda;
            txtUrlHdaDocu.Text = Properties.Settings.Default.url_hda_docu;
            txtUrlWSEca.Text = Properties.Settings.Default.url_ws;
            txtUserWS.Text = Properties.Settings.Default.user_ws;
            txtPasswordWS.Text = Properties.Settings.Default.pass_ws;
            txtUrlHDAFotos.Text = Properties.Settings.Default.url_hda_fotos;
            txtUrlHDAFirma.Text = Properties.Settings.Default.url_hda_firma;
            txtFotos.Text = Properties.Settings.Default.dir_imagenes;
            txtConexion.Text = Properties.Settings.Default.conexion;
        }

        private void cmdAceptar_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.url_hda = txtUrlHDA.Text;
            Properties.Settings.Default.user_hda = txtUserHda.Text;
            Properties.Settings.Default.pass_hda = txtPasswordHDA.Text;
            Properties.Settings.Default.url_hda_docu = txtUrlHdaDocu.Text;
            Properties.Settings.Default.url_ws = txtUrlWSEca.Text;
            Properties.Settings.Default.user_ws = txtUserWS.Text;
            Properties.Settings.Default.pass_ws = txtPasswordWS.Text;
            Properties.Settings.Default.url_hda_fotos = txtUrlHDAFotos.Text;
            Properties.Settings.Default.url_hda_firma = txtUrlHDAFirma.Text;
            Properties.Settings.Default.dir_imagenes = txtFotos.Text;
            Properties.Settings.Default.conexion = txtConexion.Text;

            Properties.Settings.Default.Save();
            this.Close();

        }

        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
