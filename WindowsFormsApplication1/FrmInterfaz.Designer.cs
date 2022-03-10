namespace InterfazHda
{
    partial class FrmInterfaz
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInterfaz));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.txtTiempo = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtUltimaConsulta = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.opcionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parametrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webServicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consumosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilidadArchivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actualizarTarifaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertirPDFATIFFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliminarArchivosHGIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distribuirActasRechazadasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distribuirActasSinAnomaliaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subirActasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actualizarProtocolosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distribuirActasAsignadasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actualizarOrdenesResueltasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obtenerFotosActaHDAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subirArchivoClientesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distribuirActasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mensajeriaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargaImagenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargaArchivoImagenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargaGuiasHGI2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extraerDocumentosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.txtHora = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdIniciar = new System.Windows.Forms.Button();
            this.subirDocumentosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.txtTiempo)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHora)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(81, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(394, 51);
            this.label5.TabIndex = 12;
            this.label5.Text = "Interfaz HGI - HDA";
            // 
            // txtTiempo
            // 
            this.txtTiempo.Location = new System.Drawing.Point(124, 126);
            this.txtTiempo.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtTiempo.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txtTiempo.Name = "txtTiempo";
            this.txtTiempo.Size = new System.Drawing.Size(71, 20);
            this.txtTiempo.TabIndex = 13;
            this.txtTiempo.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Tiempo (min)";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(407, 126);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(108, 20);
            this.dateTimePicker1.TabIndex = 17;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(208, 129);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(193, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Transferir solo las actas de la fecha";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtUltimaConsulta
            // 
            this.txtUltimaConsulta.AutoSize = true;
            this.txtUltimaConsulta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUltimaConsulta.Location = new System.Drawing.Point(35, 169);
            this.txtUltimaConsulta.Name = "txtUltimaConsulta";
            this.txtUltimaConsulta.Size = new System.Drawing.Size(95, 13);
            this.txtUltimaConsulta.TabIndex = 19;
            this.txtUltimaConsulta.Text = "Ultima Consulta";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opcionesToolStripMenuItem,
            this.webServicesToolStripMenuItem,
            this.mensajeriaToolStripMenuItem,
            this.documentosToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(605, 24);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // opcionesToolStripMenuItem
            // 
            this.opcionesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parametrosToolStripMenuItem});
            this.opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            this.opcionesToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.opcionesToolStripMenuItem.Text = "Opciones";
            // 
            // parametrosToolStripMenuItem
            // 
            this.parametrosToolStripMenuItem.Name = "parametrosToolStripMenuItem";
            this.parametrosToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.parametrosToolStripMenuItem.Text = "Parametros";
            this.parametrosToolStripMenuItem.Click += new System.EventHandler(this.parametrosToolStripMenuItem_Click);
            // 
            // webServicesToolStripMenuItem
            // 
            this.webServicesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consumosToolStripMenuItem,
            this.utilidadArchivoToolStripMenuItem,
            this.actualizarTarifaToolStripMenuItem,
            this.convertirPDFATIFFToolStripMenuItem,
            this.eliminarArchivosHGIToolStripMenuItem,
            this.distribuirActasRechazadasToolStripMenuItem,
            this.distribuirActasSinAnomaliaToolStripMenuItem,
            this.subirActasToolStripMenuItem,
            this.actualizarProtocolosToolStripMenuItem,
            this.distribuirActasAsignadasToolStripMenuItem,
            this.actualizarOrdenesResueltasToolStripMenuItem,
            this.obtenerFotosActaHDAToolStripMenuItem,
            this.subirArchivoClientesToolStripMenuItem,
            this.distribuirActasToolStripMenuItem});
            this.webServicesToolStripMenuItem.Name = "webServicesToolStripMenuItem";
            this.webServicesToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.webServicesToolStripMenuItem.Text = "Utilidades";
            // 
            // consumosToolStripMenuItem
            // 
            this.consumosToolStripMenuItem.Name = "consumosToolStripMenuItem";
            this.consumosToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.consumosToolStripMenuItem.Text = "Consumos";
            this.consumosToolStripMenuItem.Click += new System.EventHandler(this.consumosToolStripMenuItem_Click);
            // 
            // utilidadArchivoToolStripMenuItem
            // 
            this.utilidadArchivoToolStripMenuItem.Name = "utilidadArchivoToolStripMenuItem";
            this.utilidadArchivoToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.utilidadArchivoToolStripMenuItem.Text = "Utilidad Archivo";
            this.utilidadArchivoToolStripMenuItem.Click += new System.EventHandler(this.utilidadArchivoToolStripMenuItem_Click);
            // 
            // actualizarTarifaToolStripMenuItem
            // 
            this.actualizarTarifaToolStripMenuItem.Name = "actualizarTarifaToolStripMenuItem";
            this.actualizarTarifaToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.actualizarTarifaToolStripMenuItem.Text = "Actualizar tarifa";
            this.actualizarTarifaToolStripMenuItem.Click += new System.EventHandler(this.actualizarTarifaToolStripMenuItem_Click);
            // 
            // convertirPDFATIFFToolStripMenuItem
            // 
            this.convertirPDFATIFFToolStripMenuItem.Name = "convertirPDFATIFFToolStripMenuItem";
            this.convertirPDFATIFFToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.convertirPDFATIFFToolStripMenuItem.Text = "Convertir PDF a TIFF";
            this.convertirPDFATIFFToolStripMenuItem.Click += new System.EventHandler(this.convertirPDFATIFFToolStripMenuItem_Click);
            // 
            // eliminarArchivosHGIToolStripMenuItem
            // 
            this.eliminarArchivosHGIToolStripMenuItem.Name = "eliminarArchivosHGIToolStripMenuItem";
            this.eliminarArchivosHGIToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.eliminarArchivosHGIToolStripMenuItem.Text = "Eliminar Archivos HGI";
            this.eliminarArchivosHGIToolStripMenuItem.Click += new System.EventHandler(this.eliminarArchivosHGIToolStripMenuItem_Click);
            // 
            // distribuirActasRechazadasToolStripMenuItem
            // 
            this.distribuirActasRechazadasToolStripMenuItem.Name = "distribuirActasRechazadasToolStripMenuItem";
            this.distribuirActasRechazadasToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.distribuirActasRechazadasToolStripMenuItem.Text = "Distribuir Actas Rechazadas";
            this.distribuirActasRechazadasToolStripMenuItem.Click += new System.EventHandler(this.distribuirActasRechazadasToolStripMenuItem_Click);
            // 
            // distribuirActasSinAnomaliaToolStripMenuItem
            // 
            this.distribuirActasSinAnomaliaToolStripMenuItem.Name = "distribuirActasSinAnomaliaToolStripMenuItem";
            this.distribuirActasSinAnomaliaToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.distribuirActasSinAnomaliaToolStripMenuItem.Text = "Distribuir Actas Sin Anomalia";
            this.distribuirActasSinAnomaliaToolStripMenuItem.Click += new System.EventHandler(this.distribuirActasSinAnomaliaToolStripMenuItem_Click);
            // 
            // subirActasToolStripMenuItem
            // 
            this.subirActasToolStripMenuItem.Name = "subirActasToolStripMenuItem";
            this.subirActasToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.subirActasToolStripMenuItem.Text = "Subir Actas";
            this.subirActasToolStripMenuItem.Click += new System.EventHandler(this.subirActasToolStripMenuItem_Click);
            // 
            // actualizarProtocolosToolStripMenuItem
            // 
            this.actualizarProtocolosToolStripMenuItem.Name = "actualizarProtocolosToolStripMenuItem";
            this.actualizarProtocolosToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.actualizarProtocolosToolStripMenuItem.Text = "Actualizar Protocolos";
            this.actualizarProtocolosToolStripMenuItem.Click += new System.EventHandler(this.actualizarProtocolosToolStripMenuItem_Click);
            // 
            // distribuirActasAsignadasToolStripMenuItem
            // 
            this.distribuirActasAsignadasToolStripMenuItem.Name = "distribuirActasAsignadasToolStripMenuItem";
            this.distribuirActasAsignadasToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.distribuirActasAsignadasToolStripMenuItem.Text = "Distribuir Actas Asignadas";
            this.distribuirActasAsignadasToolStripMenuItem.Click += new System.EventHandler(this.distribuirActasAsignadasToolStripMenuItem_Click);
            // 
            // actualizarOrdenesResueltasToolStripMenuItem
            // 
            this.actualizarOrdenesResueltasToolStripMenuItem.Name = "actualizarOrdenesResueltasToolStripMenuItem";
            this.actualizarOrdenesResueltasToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.actualizarOrdenesResueltasToolStripMenuItem.Text = "Actualizar Ordenes Resueltas";
            this.actualizarOrdenesResueltasToolStripMenuItem.Click += new System.EventHandler(this.actualizarOrdenesResueltasToolStripMenuItem_Click);
            // 
            // obtenerFotosActaHDAToolStripMenuItem
            // 
            this.obtenerFotosActaHDAToolStripMenuItem.Name = "obtenerFotosActaHDAToolStripMenuItem";
            this.obtenerFotosActaHDAToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.obtenerFotosActaHDAToolStripMenuItem.Text = "Obtener fotos acta HDA";
            this.obtenerFotosActaHDAToolStripMenuItem.Click += new System.EventHandler(this.obtenerFotosActaHDAToolStripMenuItem_Click);
            // 
            // subirArchivoClientesToolStripMenuItem
            // 
            this.subirArchivoClientesToolStripMenuItem.Name = "subirArchivoClientesToolStripMenuItem";
            this.subirArchivoClientesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.subirArchivoClientesToolStripMenuItem.Text = "Subir Archivo Clientes";
            this.subirArchivoClientesToolStripMenuItem.Click += new System.EventHandler(this.subirArchivoClientesToolStripMenuItem_Click);
            // 
            // distribuirActasToolStripMenuItem
            // 
            this.distribuirActasToolStripMenuItem.Name = "distribuirActasToolStripMenuItem";
            this.distribuirActasToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.distribuirActasToolStripMenuItem.Text = "Distribuir Actas";
            this.distribuirActasToolStripMenuItem.Click += new System.EventHandler(this.distribuirActasToolStripMenuItem_Click);
            // 
            // mensajeriaToolStripMenuItem
            // 
            this.mensajeriaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cargaImagenesToolStripMenuItem,
            this.cargaArchivoImagenesToolStripMenuItem,
            this.cargaGuiasHGI2ToolStripMenuItem});
            this.mensajeriaToolStripMenuItem.Name = "mensajeriaToolStripMenuItem";
            this.mensajeriaToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.mensajeriaToolStripMenuItem.Text = "Mensajeria";
            // 
            // cargaImagenesToolStripMenuItem
            // 
            this.cargaImagenesToolStripMenuItem.Name = "cargaImagenesToolStripMenuItem";
            this.cargaImagenesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cargaImagenesToolStripMenuItem.Text = "Carga Imagenes";
            this.cargaImagenesToolStripMenuItem.Click += new System.EventHandler(this.cargaImagenesToolStripMenuItem_Click);
            // 
            // cargaArchivoImagenesToolStripMenuItem
            // 
            this.cargaArchivoImagenesToolStripMenuItem.Name = "cargaArchivoImagenesToolStripMenuItem";
            this.cargaArchivoImagenesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cargaArchivoImagenesToolStripMenuItem.Text = "Carga Archivo Imagenes";
            this.cargaArchivoImagenesToolStripMenuItem.Click += new System.EventHandler(this.cargaArchivoImagenesToolStripMenuItem_Click);
            // 
            // cargaGuiasHGI2ToolStripMenuItem
            // 
            this.cargaGuiasHGI2ToolStripMenuItem.Name = "cargaGuiasHGI2ToolStripMenuItem";
            this.cargaGuiasHGI2ToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cargaGuiasHGI2ToolStripMenuItem.Text = "Carga Guias HGI2";
            this.cargaGuiasHGI2ToolStripMenuItem.Click += new System.EventHandler(this.cargaGuiasHGI2ToolStripMenuItem_Click);
            // 
            // documentosToolStripMenuItem
            // 
            this.documentosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extraerDocumentosToolStripMenuItem,
            this.subirDocumentosToolStripMenuItem});
            this.documentosToolStripMenuItem.Name = "documentosToolStripMenuItem";
            this.documentosToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.documentosToolStripMenuItem.Text = "Documentos";
            // 
            // extraerDocumentosToolStripMenuItem
            // 
            this.extraerDocumentosToolStripMenuItem.Name = "extraerDocumentosToolStripMenuItem";
            this.extraerDocumentosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extraerDocumentosToolStripMenuItem.Text = "Extraer Documentos";
            this.extraerDocumentosToolStripMenuItem.Click += new System.EventHandler(this.extraerDocumentosToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 201);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 23);
            this.button1.TabIndex = 21;
            this.button1.Text = "Ver LOG";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // txtHora
            // 
            this.txtHora.Location = new System.Drawing.Point(407, 152);
            this.txtHora.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.txtHora.Name = "txtHora";
            this.txtHora.Size = new System.Drawing.Size(42, 20);
            this.txtHora.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(371, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Hora";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // abrirToolStripMenuItem
            // 
            this.abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            this.abrirToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.abrirToolStripMenuItem.Text = "Abrir";
            this.abrirToolStripMenuItem.Click += new System.EventHandler(this.abrirToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Interfaz HDA";
            this.notifyIcon1.Visible = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(164, 201);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 23);
            this.button2.TabIndex = 25;
            this.button2.Text = "Ocultar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // cmdStop
            // 
            this.cmdStop.Enabled = false;
            this.cmdStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStop.ForeColor = System.Drawing.Color.White;
            this.cmdStop.Image = global::InterfazHda.Properties.Resources.stop;
            this.cmdStop.Location = new System.Drawing.Point(70, 87);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(33, 33);
            this.cmdStop.TabIndex = 15;
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdIniciar
            // 
            this.cmdIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdIniciar.ForeColor = System.Drawing.Color.White;
            this.cmdIniciar.Image = global::InterfazHda.Properties.Resources.start;
            this.cmdIniciar.Location = new System.Drawing.Point(31, 87);
            this.cmdIniciar.Name = "cmdIniciar";
            this.cmdIniciar.Size = new System.Drawing.Size(33, 33);
            this.cmdIniciar.TabIndex = 4;
            this.cmdIniciar.UseVisualStyleBackColor = true;
            this.cmdIniciar.Click += new System.EventHandler(this.button2_Click);
            // 
            // subirDocumentosToolStripMenuItem
            // 
            this.subirDocumentosToolStripMenuItem.Name = "subirDocumentosToolStripMenuItem";
            this.subirDocumentosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.subirDocumentosToolStripMenuItem.Text = "Subir Documentos";
            this.subirDocumentosToolStripMenuItem.Click += new System.EventHandler(this.subirDocumentosToolStripMenuItem_Click);
            // 
            // FrmInterfaz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 245);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHora);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtUltimaConsulta);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTiempo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdIniciar);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmInterfaz";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INTERFAZ HDA";
            this.Load += new System.EventHandler(this.FrmInterfaz_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtTiempo)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHora)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdIniciar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown txtTiempo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label txtUltimaConsulta;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem opcionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parametrosToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.NumericUpDown txtHora;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem webServicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consumosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilidadArchivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actualizarTarifaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertirPDFATIFFToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem eliminarArchivosHGIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distribuirActasRechazadasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subirActasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actualizarProtocolosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distribuirActasAsignadasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distribuirActasSinAnomaliaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actualizarOrdenesResueltasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obtenerFotosActaHDAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subirArchivoClientesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distribuirActasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mensajeriaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargaImagenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargaArchivoImagenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargaGuiasHGI2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extraerDocumentosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subirDocumentosToolStripMenuItem;
    }
}