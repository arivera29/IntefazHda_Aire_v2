namespace InterfazHda
{
    partial class FrmUploadGuia
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
            this.gridActas = new System.Windows.Forms.DataGridView();
            this.lstImagenes = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.cmdFile = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdSalir = new System.Windows.Forms.Button();
            this.cmdActualizarGuias = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lbTotal1 = new System.Windows.Forms.Label();
            this.lbTotal2 = new System.Windows.Forms.Label();
            this.cmdSubirImagenes = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtCarpetaDestino = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbTotalGuias = new System.Windows.Forms.Label();
            this.lbTotalActas = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridActas)).BeginInit();
            this.SuspendLayout();
            // 
            // gridActas
            // 
            this.gridActas.AllowUserToAddRows = false;
            this.gridActas.AllowUserToDeleteRows = false;
            this.gridActas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridActas.Location = new System.Drawing.Point(13, 66);
            this.gridActas.Name = "gridActas";
            this.gridActas.ReadOnly = true;
            this.gridActas.Size = new System.Drawing.Size(316, 150);
            this.gridActas.TabIndex = 0;
            // 
            // lstImagenes
            // 
            this.lstImagenes.FormattingEnabled = true;
            this.lstImagenes.Location = new System.Drawing.Point(360, 66);
            this.lstImagenes.Name = "lstImagenes";
            this.lstImagenes.Size = new System.Drawing.Size(312, 147);
            this.lstImagenes.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Archivo";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(63, 36);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(232, 20);
            this.txtFilename.TabIndex = 3;
            // 
            // cmdFile
            // 
            this.cmdFile.Location = new System.Drawing.Point(301, 34);
            this.cmdFile.Name = "cmdFile";
            this.cmdFile.Size = new System.Drawing.Size(28, 23);
            this.cmdFile.TabIndex = 4;
            this.cmdFile.Text = "...";
            this.cmdFile.UseVisualStyleBackColor = true;
            this.cmdFile.Click += new System.EventHandler(this.cmdFile_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(644, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(406, 36);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(232, 20);
            this.txtFolder.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(357, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Carpeta";
            // 
            // cmdSalir
            // 
            this.cmdSalir.Location = new System.Drawing.Point(597, 344);
            this.cmdSalir.Name = "cmdSalir";
            this.cmdSalir.Size = new System.Drawing.Size(75, 23);
            this.cmdSalir.TabIndex = 8;
            this.cmdSalir.Text = "&Salir";
            this.cmdSalir.UseVisualStyleBackColor = true;
            this.cmdSalir.Click += new System.EventHandler(this.cmdSalir_Click);
            // 
            // cmdActualizarGuias
            // 
            this.cmdActualizarGuias.Location = new System.Drawing.Point(17, 239);
            this.cmdActualizarGuias.Name = "cmdActualizarGuias";
            this.cmdActualizarGuias.Size = new System.Drawing.Size(75, 23);
            this.cmdActualizarGuias.TabIndex = 9;
            this.cmdActualizarGuias.Text = "&Procesar";
            this.cmdActualizarGuias.UseVisualStyleBackColor = true;
            this.cmdActualizarGuias.Click += new System.EventHandler(this.cmdActualizarGuias_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lbTotal1
            // 
            this.lbTotal1.AutoSize = true;
            this.lbTotal1.Location = new System.Drawing.Point(13, 223);
            this.lbTotal1.Name = "lbTotal1";
            this.lbTotal1.Size = new System.Drawing.Size(71, 13);
            this.lbTotal1.TabIndex = 10;
            this.lbTotal1.Text = "Total guias: 0";
            // 
            // lbTotal2
            // 
            this.lbTotal2.AutoSize = true;
            this.lbTotal2.Location = new System.Drawing.Point(357, 223);
            this.lbTotal2.Name = "lbTotal2";
            this.lbTotal2.Size = new System.Drawing.Size(91, 13);
            this.lbTotal2.TabIndex = 11;
            this.lbTotal2.Text = "Total imagenes: 0";
            // 
            // cmdSubirImagenes
            // 
            this.cmdSubirImagenes.Location = new System.Drawing.Point(360, 286);
            this.cmdSubirImagenes.Name = "cmdSubirImagenes";
            this.cmdSubirImagenes.Size = new System.Drawing.Size(75, 23);
            this.cmdSubirImagenes.TabIndex = 12;
            this.cmdSubirImagenes.Text = "&Procesar";
            this.cmdSubirImagenes.UseVisualStyleBackColor = true;
            this.cmdSubirImagenes.Click += new System.EventHandler(this.cmdSubirImagenes_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(636, 260);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 23);
            this.button3.TabIndex = 15;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtCarpetaDestino
            // 
            this.txtCarpetaDestino.Location = new System.Drawing.Point(360, 260);
            this.txtCarpetaDestino.Name = "txtCarpetaDestino";
            this.txtCarpetaDestino.ReadOnly = true;
            this.txtCarpetaDestino.Size = new System.Drawing.Size(270, 20);
            this.txtCarpetaDestino.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(357, 244);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Carpeta Destino Guias PDF";
            // 
            // lbTotalGuias
            // 
            this.lbTotalGuias.AutoSize = true;
            this.lbTotalGuias.Location = new System.Drawing.Point(357, 324);
            this.lbTotalGuias.Name = "lbTotalGuias";
            this.lbTotalGuias.Size = new System.Drawing.Size(140, 13);
            this.lbTotalGuias.TabIndex = 16;
            this.lbTotalGuias.Text = "Total Imagenes Cargadas: 0";
            // 
            // lbTotalActas
            // 
            this.lbTotalActas.AutoSize = true;
            this.lbTotalActas.Location = new System.Drawing.Point(13, 286);
            this.lbTotalActas.Name = "lbTotalActas";
            this.lbTotalActas.Size = new System.Drawing.Size(136, 13);
            this.lbTotalActas.TabIndex = 17;
            this.lbTotalActas.Text = "Total Actas Actualizadas: 0";
            // 
            // FrmUploadGuia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 379);
            this.Controls.Add(this.lbTotalActas);
            this.Controls.Add(this.lbTotalGuias);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtCarpetaDestino);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdSubirImagenes);
            this.Controls.Add(this.lbTotal2);
            this.Controls.Add(this.lbTotal1);
            this.Controls.Add(this.cmdActualizarGuias);
            this.Controls.Add(this.cmdSalir);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdFile);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstImagenes);
            this.Controls.Add(this.gridActas);
            this.MaximizeBox = false;
            this.Name = "FrmUploadGuia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Carga de Guias HGI2";
            this.Load += new System.EventHandler(this.FrmUploadGuia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridActas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridActas;
        private System.Windows.Forms.ListBox lstImagenes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button cmdFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdSalir;
        private System.Windows.Forms.Button cmdActualizarGuias;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lbTotal1;
        private System.Windows.Forms.Label lbTotal2;
        private System.Windows.Forms.Button cmdSubirImagenes;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtCarpetaDestino;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTotalGuias;
        private System.Windows.Forms.Label lbTotalActas;
    }
}