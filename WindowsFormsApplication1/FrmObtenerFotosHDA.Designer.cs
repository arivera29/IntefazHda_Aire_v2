namespace InterfazHda
{
    partial class FrmObtenerFotosHDA
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtActa = new System.Windows.Forms.TextBox();
            this.btnObtener = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.cmdClearLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Num. Actas";
            // 
            // txtActa
            // 
            this.txtActa.Location = new System.Drawing.Point(12, 29);
            this.txtActa.Multiline = true;
            this.txtActa.Name = "txtActa";
            this.txtActa.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActa.Size = new System.Drawing.Size(184, 314);
            this.txtActa.TabIndex = 1;
            // 
            // btnObtener
            // 
            this.btnObtener.Location = new System.Drawing.Point(202, 113);
            this.btnObtener.Name = "btnObtener";
            this.btnObtener.Size = new System.Drawing.Size(84, 55);
            this.btnObtener.TabIndex = 3;
            this.btnObtener.Text = "Obtener Fotos";
            this.btnObtener.UseVisualStyleBackColor = true;
            this.btnObtener.Click += new System.EventHandler(this.btnObtener_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(292, 29);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(427, 311);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";
            // 
            // cmdClearLog
            // 
            this.cmdClearLog.Location = new System.Drawing.Point(202, 174);
            this.cmdClearLog.Name = "cmdClearLog";
            this.cmdClearLog.Size = new System.Drawing.Size(84, 55);
            this.cmdClearLog.TabIndex = 5;
            this.cmdClearLog.Text = "Limpiar LOG";
            this.cmdClearLog.UseVisualStyleBackColor = true;
            this.cmdClearLog.Click += new System.EventHandler(this.cmdClearLog_Click);
            // 
            // FrmObtenerFotosHDA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 352);
            this.Controls.Add(this.cmdClearLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnObtener);
            this.Controls.Add(this.txtActa);
            this.Controls.Add(this.label1);
            this.Name = "FrmObtenerFotosHDA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Obtener Fotos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtActa;
        private System.Windows.Forms.Button btnObtener;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button cmdClearLog;
    }
}