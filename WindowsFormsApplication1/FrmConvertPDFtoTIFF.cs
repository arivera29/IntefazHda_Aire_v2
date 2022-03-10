using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfazHda
{
    public partial class FrmConvertPDFtoTIFF : Form
    {
        public FrmConvertPDFtoTIFF()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Clear();
                string currentDirName = txtFolder.Text.Trim();
                string[] files = System.IO.Directory.GetFiles(currentDirName, "*.pdf");

                foreach (string s in files)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(s);
                    listBox1.Items.Add(fi.FullName);

                }
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                string tiffPath = @txtOutFile.Text.Trim();
                bool saveImg = false;
                System.Drawing.Image imagen = null;


                for (int x = 0; x < listBox1.Items.Count; x++)
                {
                    string filename = listBox1.Items[x].ToString();
                    
                    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
                    string pdfPath = @filename;
                    f.OpenPdf(pdfPath);

                    if (f.PageCount > 0)
                    {
                        f.ImageOptions.Dpi = 120;
                        byte[] tiff = f.ToMultipageTiff();
                       
                        
                        //ImageCodecInfo encoderInfo = GetEncoderInfo("image/tiff");

                        System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.SaveFlag;
                        ImageCodecInfo encoderInfo = ImageCodecInfo.GetImageEncoders().First(i => i.MimeType == "image/tiff");
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);


                        if (!saveImg)
                        {
                            // Primera imagen
                            imagen = System.Drawing.Image.FromStream(new MemoryStream(tiff));
                            imagen.Save(tiffPath,encoderInfo,encoderParameters);
                            saveImg = true;
                        }
                        else
                        {
                            encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);
                            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(tiff));
                            imagen.SaveAdd(img, encoderParameters);
                        }


                        //f.ToMultipageTiff(tiffPath);
                    }       
                }

            }
            else
            {
                MessageBox.Show("NO hay archivos para convertir");
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = Path.GetDirectoryName(txtOutFile.Text.Trim());
        }

        public static void Upload(string strServer, string strUser, string strPassword,
                           string strFileNameLocal, string strPathFTP)
        {
            FtpWebRequest ftpRequest;

            // Crea el objeto de conexión del servidor FTP
            ftpRequest = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}", strServer,
                                                                    Path.Combine(strPathFTP, Path.GetFileName(strFileNameLocal))));
            // Asigna las credenciales
            ftpRequest.Credentials = new NetworkCredential(strUser, strPassword);
            // Asigna las propiedades
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.UsePassive = true;
            ftpRequest.UseBinary = true;
            ftpRequest.KeepAlive = false;
            // Lee el archivo y lo envía
            using (FileStream stmFile = File.OpenRead(strFileNameLocal))
            { // Obtiene el stream sobre la comunicación FTP
                using (Stream stmFTP = ftpRequest.GetRequestStream())
                {
                    int cnstIntLengthBuffer = 0;
                    byte[] arrBytBuffer = new byte[cnstIntLengthBuffer];
                    int intRead;

                    // Lee y escribe el archivo en el stream de comunicaciones
                    while ((intRead = stmFile.Read(arrBytBuffer, 0, cnstIntLengthBuffer)) != 0)
                        stmFTP.Write(arrBytBuffer, 0, intRead);
                    // Cierra el stream FTP
                    stmFTP.Close();
                }
                // Cierra el stream del archivo
                stmFile.Close();
            }
        }
    }
}
