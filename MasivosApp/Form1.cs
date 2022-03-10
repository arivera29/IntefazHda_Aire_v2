using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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

namespace MasivosApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtNic.Text != "")
            {
                List<int> lista = ReadPdfFile(txtFile.Text, txtNic.Text.Trim());
                if (lista.Count > 0)
                {
                    System.Console.WriteLine("Paginas: " + lista.Count);
                    foreach (int page in lista)
                    {
                        System.Console.WriteLine("Pag. encontrada: " + page);
                        crearPdf(txtFile.Text, page, txtNic.Text.Trim() + ".pdf");

                    }
                }
            }
        }

        public List<int> ReadPdfFile(string fileName, String searthText)
        {
            List<int> pages = new List<int>();
            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                    string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    if (currentPageText.Contains(searthText))
                    {
                        pages.Add(page);
                    }
                }
                pdfReader.Close();
            }
            return pages;
        }

        public void crearPdf(String filename, int page, String newFilename)
        {
            Document document = new Document();
            Stream target = new FileStream(newFilename, FileMode.Create);

            using (PdfCopy pdfCopy = new PdfCopy(document, target))
            {

                document.Open();
                pdfCopy.SetLinearPageMode();

                PdfReader pdfReader = new PdfReader(filename);
                pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, page));
                pdfReader.Close();
            }
            
            
            document.Close();

        }
    }
}
