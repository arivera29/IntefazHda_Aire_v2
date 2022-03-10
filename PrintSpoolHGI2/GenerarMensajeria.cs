using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using itextsharp.pdfa;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;

namespace PrintSpoolHGI2
{
    public class GenerarMensajeria
    {
        public ILog log { set; get; }
        public String PATH_FILE_OUT { set; get; }

        private void WriteTextToDocument(BaseFont bf, iTextSharp.text.Rectangle tamPagina, PdfContentByte over, PdfGState gs, string texto)
        {

            over.SetGState(gs);

            over.SetRGBColorFill(33, 31, 31);

            over.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_STROKE);

            over.SetFontAndSize(bf, 46);

            Single anchoDiag =

                (Single)Math.Sqrt(Math.Pow((tamPagina.Height - 120), 2)

                + Math.Pow((tamPagina.Width - 60), 2));

            Single porc =

                (Single)100 * (anchoDiag / bf.GetWidthPoint(texto, 46));

            over.SetHorizontalScaling(porc);

            double angPage = (-1)

            * Math.Atan((tamPagina.Height - 60) / (tamPagina.Width - 60));

            over.SetTextMatrix((float)Math.Cos(angPage),

                        (float)Math.Sin(angPage),

                        (float)((-1F) * Math.Sin(angPage)),

                        (float)Math.Cos(angPage),

                        30F,

                        (float)tamPagina.Height - 60);

            over.ShowText(texto);


        }
        public string GenerarMensajeriaActa(int acta)
        {

            //Lista de archivos para concatenar 
            List<byte[]> lista = new List<byte[]>();

            List<Documento> Docs = ObtenerDocumentosActa(acta);
            
            foreach (var item in Docs)
            {
                lista.Add(ServiceAWS.ReadObjectDataHgi(item.KeyName));
            }

            //‘ Nombre del documento resultante;
            string filename = "MS" + acta.ToString() + ".pdf"; ;

            //    string saveAs = (@"~\File\Documentos\" + filename);
            string sFileJoin = PATH_FILE_OUT + filename;

            Document Doc = new Document();

            try
            {

                FileStream fs = new FileStream(sFileJoin, FileMode.Create, FileAccess.Write, FileShare.None);

                PdfCopy copy = new PdfCopy(Doc, fs);
                //Marca de agua


                Doc.Open();

                PdfReader Rd;


                int n;

                foreach (var file in lista)
                {

                    Rd = new PdfReader(file);

                    PdfStamper stamp = null;

                    stamp = new PdfStamper(Rd, fs);

                    BaseFont bf = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.CP1252, true);

                    PdfGState gs = new PdfGState();

                    gs.FillOpacity = 0.35F;

                    gs.StrokeOpacity = 0.35F;

                    for (int nPag = 1; nPag <= Rd.NumberOfPages; nPag++)
                    {

                        iTextSharp.text.Rectangle tamPagina = Rd.GetPageSizeWithRotation(nPag);

                        PdfContentByte over = stamp.GetOverContent(nPag);

                        over.BeginText();

                        WriteTextToDocument(bf, tamPagina, over, gs, acta.ToString() + "Pag " + nPag.ToString() + "De" + Rd.NumberOfPages.ToString());

                        over.EndText();

                    }
                    n = Rd.NumberOfPages;


                    int page = 0;

                    while (page < n)
                    {

                        page += 1;

                        copy.AddPage(copy.GetImportedPage(Rd, page));

                    }

                    copy.FreeReader(Rd);

                    Rd.Close();

                }


            }
            catch (Exception ex)
            {

                log.Info(ex);
                log.Info("Error al generar documentos de mensajeria para el acta" + acta.ToString());
                return "";
            }
            finally
            {

                // ‘ Cerramos el documento;

                Doc.Close();

            }
            return sFileJoin;
        }
        static int i;
        protected List<byte[]> CrearPaginacion(int IdMensajeria)
        {
            try
            {
                i = 1;

                ResultData<List<DocumentosDto>> Doc1 = new ResultData<List<DocumentosDto>>();
                List<byte[]> ListNew = new List<byte[]>();//Guarda los nuevos pdfs creados
                List<byte[]> Lista = new List<byte[]>();
                MensajeriaServices mensajeriaServices = new MensajeriaServices();
                DocumentosServices documentosServices = new DocumentosServices();
                DocumentosMensajeriaServices documentosMensajeriaServices = new DocumentosMensajeriaServices();
                var Men = mensajeriaServices.GetById(IdMensajeria).Result;
                log.Info($"Consultando Acta Mensajeria {Men.MensCodi}");
                //Se consultan los documentos que pertenecen a este registro de mensajeria
                var documentosToPrint = documentosMensajeriaServices.ListDocumentosMensajeria(IdMensajeria);
                List<int> listaDocs = new List<int>();
                if (documentosToPrint.Result != null)
                {
                    foreach (var item in documentosToPrint.Result)
                    {
                        listaDocs.Add(item.DocumentoId);
                    }
                }
                else
                {
                    log.Info($"Sin documentos para mensajeria nro {IdMensajeria}");
                    return null;
                }

                //Doc1 = documentosServices.GetAllDocumentosActaPrint(Men.MensActa);
                //Solo se consultan los registros que vienen de documentosMensajeria en documentos 
                Doc1 = documentosServices.GetAllDocumentosPrint(listaDocs);
                log.Info("Consultando acta");
                foreach (var D in Doc1.Result)
                {
                    //  ListNew.Add(HttpContext.Current.Server.MapPath(D.DocuUrLo));
                    if (D.DocuSAWS == 1)
                    {
                        log.Info($"Consultando documento key {D.DocuIAWS}");
                        ListNew.Add(ServiceAWS.ReadObjectDataHgi(D.DocuIAWS));
                        D.Impreso++;
                        documentosServices.Put(D);
                    }
                    else
                    {
                        string url = D.DocuPath;
                        var doc = File.ReadAllBytes(url);
                        D.Impreso++;
                        documentosServices.Put(D);
                        ListNew.Add(doc);
                    }

                }

                int All = 0;
                foreach (var con in ListNew)
                {
                    PdfReader reader = new PdfReader(con);
                    All += reader.NumberOfPages;
                }

                foreach (var D in ListNew)
                {
                    Lista.Add(AgregarPaginacion(D, Men.MensActa.ToString(), i.ToString(), All.ToString()));

                }
                Men.MensNoFo = Byte.Parse(All.ToString());
                Men.MensCaDe = 0;
                mensajeriaServices.Put(Men);
                return Lista;
            }
            catch (Exception ex)
            {
                log.Info($"Error creando paginación {ex}");
                log.Info(ex);
                return null;
            }
        }
        public string GenerarMensajeriaActa(List<int> acta, string Usuario)
        {

            //Lista de archivos para concatenar 
            List<byte[]> lista = new List<byte[]>();
            List<byte[]> ListNew = new List<byte[]>();//Guarda los nuevos pdfs creados
            log.Info("Creando Mensajeria");
            foreach (var item in acta)
            {
                log.Info("Creando Mensajeria  de  :" + item);
                ListNew = CrearPaginacion(item);
                foreach (var item1 in ListNew)
                {
                    lista.Add(item1);
                }
            }
            //‘ Nombre del documento resultante;
            string filename = "MS" + Usuario + Guid.NewGuid() +
                ".pdf"; ;

            //    string saveAs = (@"~\File\Documentos\" + filename);
            string urlReturn = PATH_FILE_OUT + filename;
            string sFileJoin = urlReturn;

            Document Doc = new Document();

            try
            {

                FileStream fs = new FileStream(sFileJoin, FileMode.Create, FileAccess.Write, FileShare.None);

                PdfCopy copy = new PdfCopy(Doc, fs);

                Doc.Open();

                PdfReader Rd;

                int n;

                foreach (var file in lista)
                {




                    Rd = new PdfReader(file);
                    PdfStamper stamp = null;

                    stamp = new PdfStamper(Rd, fs);

                    BaseFont bf = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.CP1252, true);

                    PdfGState gs = new PdfGState();

                    gs.FillOpacity = 0.35F;

                    gs.StrokeOpacity = 0.35F;

                    for (int nPag = 1; nPag <= Rd.NumberOfPages; nPag++)
                    {

                        iTextSharp.text.Rectangle tamPagina = Rd.GetPageSizeWithRotation(nPag);

                        PdfContentByte over = stamp.GetOverContent(nPag);

                        over.BeginText();

                        WriteTextToDocument(bf, tamPagina, over, gs, acta.ToString() + "Pag " + nPag.ToString() + "De" + Rd.NumberOfPages.ToString());

                        over.EndText();

                    }

                    n = Rd.NumberOfPages;


                    int page = 0;

                    while (page < n)
                    {

                        page += 1;

                        copy.AddPage(copy.GetImportedPage(Rd, page));

                    }

                    copy.FreeReader(Rd);
                    Rd.Close();


                }
                string[] files = Directory.GetFiles(@"C:\Temp\FilesTemp\");
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        File.Delete(item);
                    }
                }
                urlReturn = urlReturn.Replace("\\", "/");
                return urlReturn;

            }
            catch (Exception ex)
            {

                log.Info(ex);
                log.Info("Error al generar documentos de mensajeria para el acta" + acta.ToString());
                return "";
            }
            finally
            {

                // ‘ Cerramos el documento;

                Doc.Close();

            }

        }

        protected byte[] AgregarPaginacion(byte[] file, string acta, string nro, string nrofin)
        {
            try
            {
                /*Agregar marca de paginacion*/
                byte[] oldFile = file;
                var fileName = Guid.NewGuid();
                var url = @"C:\Temp\FilesTemp\";
                string dest = Path.Combine(url, fileName.ToString() + ".pdf");


                // open the reader
                PdfReader reader = new PdfReader(oldFile);
                Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                //FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(dest), FileMode.Create, FileAccess.Write);
                FileStream fs1 = new FileStream((dest), FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, fs1);
                document.Open();

                float offset = 0;
                for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
                {
                    // the pdf content
                    PdfContentByte cb = writer.DirectContent;

                    // select the font properties
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb.SetColorFill(BaseColor.DARK_GRAY);
                    cb.SetFontAndSize(bf, 8);

                    // write the text in the pdf content
                    cb.BeginText();
                    string text = acta + " Pag.  " + i + " de " + nrofin;
                    // put the alignment and coordinates here
                    cb.SetTextMatrix(10, document.PageSize.GetTop(25));
                    //cb.ShowTextAligned(0, text, 500, 780, 0);
                    cb.ShowText(text);

                    cb.EndText();
                    PdfImportedPage page1 = writer.GetImportedPage(reader, pageIndex);
                    offset = (document.PageSize.Width - page1.Width) / 2;
                    cb.AddTemplate(page1, offset, 0);
                    document.NewPage();
                    i++;
                }

                //while (page < n)
                //{
                //    PdfImportedPage page1;
                //    page += 1;

                //   page1  = writer.GetImportedPage(reader, page);
                //    cb.AddTemplate(page1, 0, 0);

                //}
                // create the new page and add it to the pdf

                // System.IO.File.Delete(dest);
                // close the streams and voilá the file should be changed :)
                document.Close();
                fs1.Close();
                writer.Close();
                reader.Close();
                /*Fin */
                return oldFile;
            }
            catch (Exception ex)
            {

                log.Info(ex);
                return null;
            }
        }

        public string GenerarMensajeriaFtp(List<int> acta, string Usuario, string Delegacion)
        {

            //Lista de archivos para concatenar 
            List<byte[]> lista = new List<byte[]>();
            List<byte[]> ListNew = new List<byte[]>();//Guarda los nuevos pdfs creados

            foreach (var item in acta)
            {

                ListNew = CrearPaginacion(item);
                foreach (var item1 in ListNew)
                {
                    lista.Add(item1);
                }
            }


            //‘ Nombre del documento resultante;
            string filename = "MSFTP" + Usuario + Guid.NewGuid() + ".pdf"; ;
            string sourcePath = @"";
            //    string saveAs = (@"~\File\Documentos\" + filename);
            string targetPath = PATH_FILE_OUT;

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            string dir = targetPath + @"\" + Delegacion;
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            string sFileJoin = (PATH_FILE_OUT + filename);

            Document Doc = new Document();

            try
            {

                FileStream fs = new FileStream(sFileJoin, FileMode.Create, FileAccess.Write, FileShare.None);

                PdfCopy copy = new PdfCopy(Doc, fs);

                Doc.Open();

                PdfReader Rd;

                int n;

                foreach (var file in lista)
                {
                    Rd = new PdfReader(file);


                    n = Rd.NumberOfPages;


                    int page = 0;

                    while (page < n)
                    {

                        page += 1;

                        copy.AddPage(copy.GetImportedPage(Rd, page));

                    }

                    copy.FreeReader(Rd);
                    Rd.Close();


                }
                Doc.Close();
                string sourceFile = System.IO.Path.Combine(sourcePath, acta.ToString());
                String fileExtension = System.IO.Path.GetFileName(sFileJoin);
                string destFile = System.IO.Path.Combine(dir, fileExtension);
                System.IO.File.Copy(sFileJoin, destFile, true);

                return destFile;

            }
            catch (Exception ex)
            {

                log.Info(ex);
                log.Info("Error al generar documentos de mensajeria para el acta" + acta.ToString());
                return "";
            }
            finally
            {

                // ‘ Cerramos el documento;

                Doc.Close();

            }

        }

        public List<Documento> ObtenerDocumentosActa(int acta)
        {
            List<Documento> docs = new List<Documento>();

            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "SELECT DocuIAWS, DocuCodi FROM Documentos WHERE DocuActa = @acta";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Documento doc = new Documento();
                            doc.Id = reader.GetInt32(1);
                            doc.KeyName = reader.GetString(0);

                            docs.Add(doc);

                        }
                    }
                }

                    conexion.Close();
            }

            return docs;
        }

        public List<Documento> ObtenerDocumentosMensajeriaActa(int idMensajeria)
        {
            List<Documento> docs = new List<Documento>();

            Datos conexion = new Datos();
            if (conexion != null)
            {
                String sql = "SELECT DocuIAWS, DocuCodi FROM Documentos WHERE DocuActa = @acta";
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = conexion.getConection();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Documento doc = new Documento();
                            doc.Id = reader.GetInt32(1);
                            doc.KeyName = reader.GetString(0);

                            docs.Add(doc);

                        }
                    }
                }

                conexion.Close();
            }

            return docs;
        }
    }
}
