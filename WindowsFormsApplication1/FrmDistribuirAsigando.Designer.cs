namespace InterfazHda
{
    partial class FrmDistribuirAsigando
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnDistribuir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.lbBandejas = new System.Windows.Forms.Label();
            this.lbActasDistribuir = new System.Windows.Forms.Label();
            this.lbBandejasDistribuidas = new System.Windows.Forms.Label();
            this.lbActasAsignadas = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(31, 29);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(472, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnDistribuir
            // 
            this.btnDistribuir.Location = new System.Drawing.Point(241, 325);
            this.btnDistribuir.Name = "btnDistribuir";
            this.btnDistribuir.Size = new System.Drawing.Size(75, 23);
            this.btnDistribuir.TabIndex = 1;
            this.btnDistribuir.Text = "&Distribuir";
            this.btnDistribuir.UseVisualStyleBackColor = true;
            this.btnDistribuir.Click += new System.EventHandler(this.btnDistribuir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bandejas";
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.Location = new System.Drawing.Point(31, 186);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(72, 13);
            this.lbTotal.TabIndex = 3;
            this.lbTotal.Text = "Total actas: 0";
            // 
            // lbBandejas
            // 
            this.lbBandejas.AutoSize = true;
            this.lbBandejas.Location = new System.Drawing.Point(31, 210);
            this.lbBandejas.Name = "lbBandejas";
            this.lbBandejas.Size = new System.Drawing.Size(136, 13);
            this.lbBandejas.TabIndex = 4;
            this.lbBandejas.Text = "Bandejas Seleccionadas: 0";
            // 
            // lbActasDistribuir
            // 
            this.lbActasDistribuir.AutoSize = true;
            this.lbActasDistribuir.Location = new System.Drawing.Point(31, 234);
            this.lbActasDistribuir.Name = "lbActasDistribuir";
            this.lbActasDistribuir.Size = new System.Drawing.Size(96, 13);
            this.lbActasDistribuir.TabIndex = 5;
            this.lbActasDistribuir.Text = "Actas a distribuir: 0";
            // 
            // lbBandejasDistribuidas
            // 
            this.lbBandejasDistribuidas.AutoSize = true;
            this.lbBandejasDistribuidas.Location = new System.Drawing.Point(31, 261);
            this.lbBandejasDistribuidas.Name = "lbBandejasDistribuidas";
            this.lbBandejasDistribuidas.Size = new System.Drawing.Size(114, 13);
            this.lbBandejasDistribuidas.TabIndex = 6;
            this.lbBandejasDistribuidas.Text = "Badejas Distribuidas: 0";
            // 
            // lbActasAsignadas
            // 
            this.lbActasAsignadas.AutoSize = true;
            this.lbActasAsignadas.Location = new System.Drawing.Point(31, 289);
            this.lbActasAsignadas.Name = "lbActasAsignadas";
            this.lbActasAsignadas.Size = new System.Drawing.Size(98, 13);
            this.lbActasAsignadas.TabIndex = 7;
            this.lbActasAsignadas.Text = "Actas Asignadas: 0";
            // 
            // FrmDistribuirAsigando
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 360);
            this.Controls.Add(this.lbActasAsignadas);
            this.Controls.Add(this.lbBandejasDistribuidas);
            this.Controls.Add(this.lbActasDistribuir);
            this.Controls.Add(this.lbBandejas);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDistribuir);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FrmDistribuirAsigando";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Distribuir Actas Asignadas";
            this.Load += new System.EventHandler(this.FrmDistribuirAsigando_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnDistribuir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label lbBandejas;
        private System.Windows.Forms.Label lbActasDistribuir;
        private System.Windows.Forms.Label lbBandejasDistribuidas;
        private System.Windows.Forms.Label lbActasAsignadas;
    }
}