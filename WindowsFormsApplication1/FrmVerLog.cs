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
    public partial class FrmVerLog : Form
    {
        public FrmVerLog()
        {
            InitializeComponent();
        }

        private void FrmVerLog_Load(object sender, EventArgs e)
        {
            string currentDirName = Environment.CurrentDirectory;
            string[] files = System.IO.Directory.GetFiles(currentDirName, "*.txt");

            foreach (string s in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(s);
                listBox1.Items.Add(fi.Name);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = Environment.CurrentDirectory + @"\\" + listBox1.Items[listBox1.SelectedIndex].ToString();
            try
            {
                richTextBox1.LoadFile(filename,RichTextBoxStreamType.PlainText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
