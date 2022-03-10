namespace InterfazHda
{
    partial class FrmExtraerDocumentosActas
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
            this.cmdExtraer = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtCarpetaDestino = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbTotal1 = new System.Windows.Forms.Label();
            this.gridActas = new System.Windows.Forms.DataGridView();
            this.cmdFile = new System.Windows.Forms.Button();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cmdSalir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridActas)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdExtraer
            // 
            this.cmdExtraer.Location = new System.Drawing.Point(21, 334);
            this.cmdExtraer.Name = "cmdExtraer";
            this.cmdExtraer.Size = new System.Drawing.Size(75, 23);
            this.cmdExtraer.TabIndex = 0;
            this.cmdExtraer.Text = "&Extraer";
            this.cmdExtraer.UseVisualStyleBackColor = true;
            this.cmdExtraer.Click += new System.EventHandler(this.cmdExtraer_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(305, 265);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtCarpetaDestino
            // 
            this.txtCarpetaDestino.Location = new System.Drawing.Point(67, 267);
            this.txtCarpetaDestino.Name = "txtCarpetaDestino";
            this.txtCarpetaDestino.ReadOnly = true;
            this.txtCarpetaDestino.Size = new System.Drawing.Size(232, 20);
            this.txtCarpetaDestino.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 270);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Destino";
            // 
            // lbTotal1
            // 
            this.lbTotal1.AutoSize = true;
            this.lbTotal1.Location = new System.Drawing.Point(15, 236);
            this.lbTotal1.Name = "lbTotal1";
            this.lbTotal1.Size = new System.Drawing.Size(73, 13);
            this.lbTotal1.TabIndex = 20;
            this.lbTotal1.Text = "Total Actas: 0";
            // 
            // gridActas
            // 
            this.gridActas.AllowUserToAddRows = false;
            this.gridActas.AllowUserToDeleteRows = false;
            this.gridActas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridActas.Location = new System.Drawing.Point(15, 39);
            this.gridActas.Name = "gridActas";
            this.gridActas.ReadOnly = true;
            this.gridActas.Size = new System.Drawing.Size(316, 190);
            this.gridActas.TabIndex = 19;
            // 
            // cmdFile
            // 
            this.cmdFile.Location = new System.Drawing.Point(305, 10);
            this.cmdFile.Name = "cmdFile";
            this.cmdFile.Size = new System.Drawing.Size(28, 23);
            this.cmdFile.TabIndex = 23;
            this.cmdFile.Text = "...";
            this.cmdFile.UseVisualStyleBackColor = true;
            this.cmdFile.Click += new System.EventHandler(this.cmdFile_Click);
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(67, 12);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(232, 20);
            this.txtFilename.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Archivo";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // cmdSalir
            // 
            this.cmdSalir.Location = new System.Drawing.Point(102, 334);
            this.cmdSalir.Name = "cmdSalir";
            this.cmdSalir.Size = new System.Drawing.Size(75, 23);
            this.cmdSalir.TabIndex = 24;
            this.cmdSalir.Text = "&Salir";
            this.cmdSalir.UseVisualStyleBackColor = true;
            this.cmdSalir.Click += new System.EventHandler(this.cmdSalir_Click);
            // 
            // FrmExtraerDocumentosActas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 369);
            this.Controls.Add(this.cmdSalir);
            this.Controls.Add(this.cmdFile);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbTotal1);
            this.Controls.Add(this.gridActas);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtCarpetaDestino);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdExtraer);
            this.MaximizeBox = false;
            this.Name = "FrmExtraerDocumentosActas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Extraer Documentos Actas";
            this.Load += new System.EventHandler(this.FrmExtraerDocumentosActas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridActas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdExtraer;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtCarpetaDestino;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTotal1;
        private System.Windows.Forms.DataGridView gridActas;
        private System.Windows.Forms.Button cmdFile;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button cmdSalir;
    }
}