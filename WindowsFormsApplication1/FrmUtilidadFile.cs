using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmUtilidadFile : Form
    {
        public FrmUtilidadFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string currentDirName = textBox1.Text.Trim();
            string[] files = System.IO.Directory.GetFiles(currentDirName, "Documentos*.jpg");

            foreach (string s in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(s);
                string newFile = fi.Name.Replace("Documentos", "");
                File.Move(fi.FullName, textBox1.Text.Trim() + "\\" + newFile);
                
            }
        }
    }
}
