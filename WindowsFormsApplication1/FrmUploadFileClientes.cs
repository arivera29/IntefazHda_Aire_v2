using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmUploadFileClientes : Form
    {
        private string[] fields;
        private int cntCargados = 0;
        private int cntError = 0;
        private int cntTotal = 0;
        private int cntDuplicado = 0;
        private bool runProcess = false;

        public FrmUploadFileClientes()
        {
            InitializeComponent();
        }

        private void FrmCleanDocumentos_Load(object sender, EventArgs e)
        {
            String cadena = "NIS_RAD,NIC,TIPO_CLIENTE,NOM_CLI,APE1_CLI,APE2_CLI,TIPO_VIA,NOM_CALLE,DUPLICADOR,NUM_PUERTA,CGV_CLI,REF_DIR,NOM_CALLE_1,DUPLICADOR_1,NUM_PUERTA_1,CGV_SUM,REF_DIR_1,DEPARTAMENTO,MINICIPIO,LOCALIDAD,NUM_APA,MARCA_APA,COD_TAR,POT";
            fields = cadena.Split(',');
        }

        private void cmdBuscar_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtFile.Text = openFileDialog1.FileName;
                

            }
            else
            {
                txtFile.Text = "";
            }
        }

        private String cadenaINSERT()
        {
            string sql = "INSERT INTO Clientes (";
            foreach (string fieldname in fields)
            {

                sql += fieldname + ",";
            }
            sql += "FECHA_CARGA) VALUES (";
            foreach (string fieldname in fields)
            {
                sql += "@" + fieldname + ",";
            }
            sql += "SYSDATETIME())";

            return sql;
        }

        private bool InsertarRegistro(Datos conexion, string[] fila)
        {
            FileLOG.LOG("Insertando fila");
            String cadenaSQL = cadenaINSERT();
            String cFila = "";
            bool respuesta = false;
            if (conexion != null)
            {
                using (SqlCommand cmd = new SqlCommand(cadenaSQL))
                {
                    cmd.Connection = conexion.getConection();
                    for (int x = 0; x < fila.Length; x++) // Se recorren las columnas
                    {
                        cmd.Parameters.Add("@" + fields[x], SqlDbType.VarChar, 100).Value = fila[x].Trim();
                        cFila += fila[x].Trim() + "\t";
                    }

                    try
                    {
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            // Registro guardado
                            cntCargados++;
                            FileLOG.LOG("Fila cargada a la base de datos");
                            respuesta = true;
                        }
                        else
                        {
                            cntError++;
                            FileLOG.LOG("Error al cargar fila");
                        }

                    }
                    catch (SqlException ex)
                    {
                        System.Console.WriteLine("Error. " + ex.Message);
                        FileLOG.LOG("Error. " + ex.Message);
                        cntError++;
                        if (ex.ErrorCode == 2601)
                        {
                            cntDuplicado++;
                            FileLOG.LOG("Fila Duplicada " + cFila);
                        }
                    }

                }


            }

            return respuesta;
        }


        private void ProcesarArchivo()
        {
            
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de Subir Archivo?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;
                cmdStopProcess.Enabled = true;
                btnUpload.Enabled = false;
                FileLOG.LOG("Iniciando carga de archivo");
                backgroundWorker1.RunWorkerAsync();

            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            runProcess = true;
            cntDuplicado = 0;
            cntError = 0;
            cntCargados = 0;
            string filename = txtFile.Text.Trim();
            FileLOG.LOG("Procesando archivo " + filename);
            Datos conexion = new Datos();
            if (conexion != null)
            {
                using (StreamReader sr = new StreamReader(@filename))
                {
                    cntTotal = 0;
                    string[] fila = null;
                    while (!sr.EndOfStream)
                    {
                        cntTotal++;
                        backgroundWorker1.ReportProgress(cntTotal);
                        try
                        {
                            FileLOG.LOG("Leyendo fila " + cntTotal);
                            fila = sr.ReadLine().Split('\t');

                            InsertarRegistro(conexion, fila);
                            if (backgroundWorker1.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error. " + ex.Message);
                            FileLOG.LOG("Error. " + ex.Message);

                        }

                    }

                }
                conexion.Close();
            }
            else
            {
                System.Console.WriteLine("Error al conectarse con el servidor ");
                FileLOG.LOG("Error al conectarse con el servidor ");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.MarqueeAnimationSpeed = 0;
            if (e.Cancelled)
            {
                // The user canceled the operation.
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                MessageBox.Show(msg);
            }
            else
            {
                // The operation completed normally.
                string msg = String.Format("Registros cargados = {0}", cntCargados);
                MessageBox.Show(msg);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbTotalCargados.Text = String.Format("Total Cargados: {0}", cntCargados);
            lbTotalErrores.Text = String.Format("Total Errores: {0}", cntError);
            lbTotalRegistros.Text = String.Format("Total Registros: {0}", cntTotal);
            lbDuplicados.Text = String.Format("Total Duplicados: {0}", cntDuplicado);
        }

        private void cmdStopProcess_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de detener el proceso?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                btnUpload.Enabled = true;
                cmdStopProcess.Enabled = false;
                if (runProcess)
                {
                    backgroundWorker1.CancelAsync();
                    runProcess = false;
                }
            }
            
        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            if (runProcess)
            {
                if (MessageBox.Show("Se está ejecutando un proceso, desea detener el proceso?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    backgroundWorker1.CancelAsync();
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }


    }
}
