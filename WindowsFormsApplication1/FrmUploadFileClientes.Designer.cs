namespace InterfazHda
{
    partial class FrmUploadFileClientes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdBuscar = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbTotalErrores = new System.Windows.Forms.Label();
            this.lbTotalCargados = new System.Windows.Forms.Label();
            this.lbTotalRegistros = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmdStopProcess = new System.Windows.Forms.Button();
            this.cmdSalir = new System.Windows.Forms.Button();
            this.lbDuplicados = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdBuscar
            // 
            this.cmdBuscar.Location = new System.Drawing.Point(215, 38);
            this.cmdBuscar.Name = "cmdBuscar";
            this.cmdBuscar.Size = new System.Drawing.Size(75, 23);
            this.cmdBuscar.TabIndex = 6;
            this.cmdBuscar.Text = "Buscar";
            this.cmdBuscar.UseVisualStyleBackColor = true;
            this.cmdBuscar.Click += new System.EventHandler(this.cmdBuscar_Click);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(99, 12);
            this.txtFile.Name = "txtFile";
            this.txtFile.ReadOnly = true;
            this.txtFile.Size = new System.Drawing.Size(380, 20);
            this.txtFile.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Buscar Archivo";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(18, 168);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(117, 23);
            this.btnUpload.TabIndex = 8;
            this.btnUpload.Text = "&Iniciar";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDuplicados);
            this.groupBox1.Controls.Add(this.lbTotalErrores);
            this.groupBox1.Controls.Add(this.lbTotalCargados);
            this.groupBox1.Controls.Add(this.lbTotalRegistros);
            this.groupBox1.Location = new System.Drawing.Point(17, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 76);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resumen";
            // 
            // lbTotalErrores
            // 
            this.lbTotalErrores.AutoSize = true;
            this.lbTotalErrores.Location = new System.Drawing.Point(253, 20);
            this.lbTotalErrores.Name = "lbTotalErrores";
            this.lbTotalErrores.Size = new System.Drawing.Size(78, 13);
            this.lbTotalErrores.TabIndex = 2;
            this.lbTotalErrores.Text = "Total errores: 0";
            // 
            // lbTotalCargados
            // 
            this.lbTotalCargados.AutoSize = true;
            this.lbTotalCargados.Location = new System.Drawing.Point(7, 45);
            this.lbTotalCargados.Name = "lbTotalCargados";
            this.lbTotalCargados.Size = new System.Drawing.Size(111, 13);
            this.lbTotalCargados.TabIndex = 1;
            this.lbTotalCargados.Text = "Registros Cargados: 0";
            // 
            // lbTotalRegistros
            // 
            this.lbTotalRegistros.AutoSize = true;
            this.lbTotalRegistros.Location = new System.Drawing.Point(7, 20);
            this.lbTotalRegistros.Name = "lbTotalRegistros";
            this.lbTotalRegistros.Size = new System.Drawing.Size(90, 13);
            this.lbTotalRegistros.TabIndex = 0;
            this.lbTotalRegistros.Text = "Total Registros: 0";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 149);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(465, 11);
            this.progressBar1.TabIndex = 10;
            // 
            // cmdStopProcess
            // 
            this.cmdStopProcess.Enabled = false;
            this.cmdStopProcess.Location = new System.Drawing.Point(141, 168);
            this.cmdStopProcess.Name = "cmdStopProcess";
            this.cmdStopProcess.Size = new System.Drawing.Size(117, 23);
            this.cmdStopProcess.TabIndex = 11;
            this.cmdStopProcess.Text = "&Detener";
            this.cmdStopProcess.UseVisualStyleBackColor = true;
            this.cmdStopProcess.Click += new System.EventHandler(this.cmdStopProcess_Click);
            // 
            // cmdSalir
            // 
            this.cmdSalir.Location = new System.Drawing.Point(356, 166);
            this.cmdSalir.Name = "cmdSalir";
            this.cmdSalir.Size = new System.Drawing.Size(117, 25);
            this.cmdSalir.TabIndex = 12;
            this.cmdSalir.Text = "&Salir";
            this.cmdSalir.UseVisualStyleBackColor = true;
            this.cmdSalir.Click += new System.EventHandler(this.cmdSalir_Click);
            // 
            // lbDuplicados
            // 
            this.lbDuplicados.AutoSize = true;
            this.lbDuplicados.Location = new System.Drawing.Point(253, 45);
            this.lbDuplicados.Name = "lbDuplicados";
            this.lbDuplicados.Size = new System.Drawing.Size(99, 13);
            this.lbDuplicados.TabIndex = 3;
            this.lbDuplicados.Text = "Total Duplicados: 0";
            // 
            // FrmUploadFileClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 203);
            this.Controls.Add(this.cmdSalir);
            this.Controls.Add(this.cmdStopProcess);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.cmdBuscar);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.label1);
            this.Name = "FrmUploadFileClientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subir Archivo de Clientes";
            this.Load += new System.EventHandler(this.FrmCleanDocumentos_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdBuscar;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label lbTotalErrores;
        public System.Windows.Forms.Label lbTotalCargados;
        public System.Windows.Forms.Label lbTotalRegistros;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cmdStopProcess;
        private System.Windows.Forms.Button cmdSalir;
        public System.Windows.Forms.Label lbDuplicados;
    }
}