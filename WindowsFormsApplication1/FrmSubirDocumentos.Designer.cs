namespace InterfazHda
{
    partial class FrmSubirDocumentos
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
            this.lbTotalGuias = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.txtCarpetaDestino = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdSubirImagenes = new System.Windows.Forms.Button();
            this.lbTotal2 = new System.Windows.Forms.Label();
            this.cmdSalir = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstImagenes = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTipoArchivo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lbTotalGuias
            // 
            this.lbTotalGuias.AutoSize = true;
            this.lbTotalGuias.Location = new System.Drawing.Point(25, 340);
            this.lbTotalGuias.Name = "lbTotalGuias";
            this.lbTotalGuias.Size = new System.Drawing.Size(135, 13);
            this.lbTotalGuias.TabIndex = 27;
            this.lbTotalGuias.Text = "Total Archivos Cargadas: 0";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(304, 236);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 23);
            this.button3.TabIndex = 26;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtCarpetaDestino
            // 
            this.txtCarpetaDestino.Location = new System.Drawing.Point(28, 236);
            this.txtCarpetaDestino.Name = "txtCarpetaDestino";
            this.txtCarpetaDestino.ReadOnly = true;
            this.txtCarpetaDestino.Size = new System.Drawing.Size(270, 20);
            this.txtCarpetaDestino.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Carpeta Destino Archivos PDF";
            // 
            // cmdSubirImagenes
            // 
            this.cmdSubirImagenes.Location = new System.Drawing.Point(28, 302);
            this.cmdSubirImagenes.Name = "cmdSubirImagenes";
            this.cmdSubirImagenes.Size = new System.Drawing.Size(75, 23);
            this.cmdSubirImagenes.TabIndex = 23;
            this.cmdSubirImagenes.Text = "&Procesar";
            this.cmdSubirImagenes.UseVisualStyleBackColor = true;
            this.cmdSubirImagenes.Click += new System.EventHandler(this.cmdSubirImagenes_Click);
            // 
            // lbTotal2
            // 
            this.lbTotal2.AutoSize = true;
            this.lbTotal2.Location = new System.Drawing.Point(25, 199);
            this.lbTotal2.Name = "lbTotal2";
            this.lbTotal2.Size = new System.Drawing.Size(87, 13);
            this.lbTotal2.TabIndex = 22;
            this.lbTotal2.Text = "Total Archivos: 0";
            // 
            // cmdSalir
            // 
            this.cmdSalir.Location = new System.Drawing.Point(265, 360);
            this.cmdSalir.Name = "cmdSalir";
            this.cmdSalir.Size = new System.Drawing.Size(75, 23);
            this.cmdSalir.TabIndex = 21;
            this.cmdSalir.Text = "&Salir";
            this.cmdSalir.UseVisualStyleBackColor = true;
            this.cmdSalir.Click += new System.EventHandler(this.cmdSalir_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(312, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(74, 12);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(232, 20);
            this.txtFolder.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Carpeta";
            // 
            // lstImagenes
            // 
            this.lstImagenes.FormattingEnabled = true;
            this.lstImagenes.Location = new System.Drawing.Point(28, 42);
            this.lstImagenes.Name = "lstImagenes";
            this.lstImagenes.Size = new System.Drawing.Size(312, 147);
            this.lstImagenes.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Tipo Archivo";
            // 
            // cboTipoArchivo
            // 
            this.cboTipoArchivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoArchivo.FormattingEnabled = true;
            this.cboTipoArchivo.Location = new System.Drawing.Point(28, 276);
            this.cboTipoArchivo.Name = "cboTipoArchivo";
            this.cboTipoArchivo.Size = new System.Drawing.Size(270, 21);
            this.cboTipoArchivo.TabIndex = 29;
            // 
            // FrmSubirDocumentos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 393);
            this.Controls.Add(this.cboTipoArchivo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbTotalGuias);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtCarpetaDestino);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdSubirImagenes);
            this.Controls.Add(this.lbTotal2);
            this.Controls.Add(this.cmdSalir);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstImagenes);
            this.MaximizeBox = false;
            this.Name = "FrmSubirDocumentos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subir Documentos HGI2";
            this.Load += new System.EventHandler(this.FrmSubirDocumentos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTotalGuias;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtCarpetaDestino;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdSubirImagenes;
        private System.Windows.Forms.Label lbTotal2;
        private System.Windows.Forms.Button cmdSalir;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstImagenes;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTipoArchivo;
    }
}