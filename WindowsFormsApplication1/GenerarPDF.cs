using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generals.business.Entities;
using System.Reflection;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Web;
using System.Net;
using System.Configuration;
using System.Drawing;

namespace InterfazHda
{
    public class GenerarPdf 
    {
        // private string acta;
        //private int _number;
        //private int porce;
        static int idx = 0;
        public string Filename { get; set; }
        private const string lineaHtml = @"<table border=""1"" cellpadding=""0"" cellspacing=""0"" style=""color:#003053; ""><tr><td>&nbsp;</td></tr></table>";
        //public GenerarPSEvidente(int acta,int conseP)
        //{
        //    this._number = acta;
        //    this.porce = conseP;
        //}
       
        static BllDocumentos.Documentos Firma = new BllDocumentos.Documentos();
        static BllDocumentos.Documentos Huella = new BllDocumentos.Documentos();
        static BllActas.Actas Act = new BllActas.Actas();
        static List<BllActas.Actas> Actas = new List<BllActas.Actas>();       
        static BllDocumentos.Documentos Doc = new BllDocumentos.Documentos();
        static List<BllDocumentos.Documentos> Docu = new List<BllDocumentos.Documentos>();
        static List<BllDocumentos.Documentos> ListDocu = new List<BllDocumentos.Documentos>();
        static BllActa_Medidor.Acta_Medidor Me = new BllActa_Medidor.Acta_Medidor();
        static List<BllAC_Sellos.AC_Sellos> Sellos = new List<BllAC_Sellos.AC_Sellos>();
        static List<BllAnomalias.Anomalias> Ano = new List<BllAnomalias.Anomalias>();
        static List<BllCensoActas.CensoActas> Cen = new List<BllCensoActas.CensoActas>();
        static List<BllMaterialeses.Materiales> Mat = new List<BllMaterialeses.Materiales>();
        static List<BllTrabajosEjecutados.TrabajosEjecutados> Tra = new List<BllTrabajosEjecutados.TrabajosEjecutados>();
        static BllProcesoSimpli.ProcesoSimpli Pro = new BllProcesoSimpli.ProcesoSimpli();
        static BllActa_Liquidacion.Acta_Liquidacion Liq = new BllActa_Liquidacion.Acta_Liquidacion();
        static List<BllConsumo.Consumo> Co = new List<BllConsumo.Consumo>();
        //  static  BllProcesoSimpli.ProcesoSimpli Pro = new BllProcesoSimpli.ProcesoSimpli();
        static  string ruta1="";

        public static bool CargarDatos(int Acta)
        {
            if (Acta != null)
            {
                Act = BllActas.GetActa(Acta);
                Docu = BllDocumentos.GetCargoXActa(Act._number);
                Me =  BllActa_Medidor.GetMedidorEncontrado(Acta);
                Sellos = BllAC_Sellos.GetAC_SellosXNumeroActa((Acta ));
                Cen = BllCensoActas.CargarGridView(Acta);
                Ano = BllAnomalias.CargarGridView(Acta);
                Firma = BllDocumentos.GetFirmaOperario(Acta,10); 
                return true;
            }
            return false;
        }
        public static bool CargarDatosCarta(int Acta,int proce)
        {
            if (Acta != null)
            {
                Act = BllActas.GetActa(Acta);
                Pro = BllProcesoSimpli.GetPerson(proce);
                Ano = BllAnomalias.CargarGridView(Acta);
                return true;
            }
            return false;
        }
        public static bool CargarDatosLiqui(int Acta, int proce)
        {
            if (Acta != null)
            {
                Act = BllActas.GetActa(Acta);
                Liq = BllActa_Liquidacion.GetActa(proce);
                Co = BllConsumo.CargarGridView(Acta);
                return true;
            }
            return false;
        }

        public static string GenerarLiquidacion(int acta, int conse, string user)
        {
            try
            {
                string filename = "";
                if (CargarDatosLiqui(acta,conse))
                { 
                     Document document = new Document(PageSize.LETTER, 90, 50, 30, 65);
                    document.SetMargins(25f, 25f, 25f, 25f);
                    //  iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, Color.Black);
                    filename = "Li" + acta.ToString() + conse.ToString() + ".pdf"; ;

                    string saveAs = (@"~\File\Documentos\Liquidacion\" + filename);
                    using (System.IO.MemoryStream memoryStream1 = new System.IO.MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, new System.IO.FileStream(HttpContext.Current.Server.MapPath(saveAs), FileMode.Create));
                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = BaseColor.YELLOW;

                        document.Open();


                        //Header Table
                        table = new PdfPTable(1);
                        table.TotalWidth = 700f;
                        table.LockedWidth = true;
                        table.SetWidths(new float[] { 8f });
                        table.WidthPercentage = 100;
                        //Company Logo
                        cell = ImageCell("~/images/Liquidacion.png", 50f, PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("Nro. Consecutivo:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Liq.AcLiCodi.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Nro. Acta: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Act._number.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Fecha:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(String.Format("{0:dd/MM/yyyy}", DateTime.Now.ToShortDateString()), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Metodo de Liquidación:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Liq.DescLiqu, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Titular: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Act.nombreTitularContrato +" " +Act.apellido1TitularContrato + " " +Act.apellido2TitularContrato , FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Nic:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Act.nic, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);

                        cell = PhraseCarta(new Phrase("Valor Tarifa ($/kWh):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan =2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Math.Round(float.Parse(Act.ValorTarifa.ToString()), 3).ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("E.C.D.F. (kWh): ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan =1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        float total = (float.Parse(Act.ValorEcdf.ToString()) * float.Parse(Act.ValorTarifa.ToString()));
                        cell = PhraseCarta(new Phrase(Math.Round(float.Parse(Act.ValorEcdf.ToString()), 0).ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Total :", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(String.Format("{0:C2}", (Math.Round(total)), 2), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(6);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f});
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;

                        PdfPTable nested = new PdfPTable(2);
                        nested.SetWidths(new float[] { 10f, 10f });
                       // nested.SpacingBefore = 8f;
                        nested.WidthPercentage = 30;
                        cell = PhraseCell(new Phrase("CONSUMOS ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        cell.BackgroundColor = BaseColor.GRAY;
                        nested.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Fecha", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        nested.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Valor", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        nested.AddCell(cell);
                        foreach (var item in Co)
                        {
                            cell = PhraseCarta(new Phrase(item.ConsFech, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested.AddCell(cell);
                            cell = PhraseCarta(new Phrase(item.ConsValo.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested.AddCell(cell);
                        }
                        PdfPCell nesthousing = new PdfPCell(nested);
                        nesthousing.Padding = 0f;
                        nesthousing.Colspan = 2;
                        table.AddCell(nesthousing);

                        PdfPTable nested1 = new PdfPTable(6);
                        
                        nested1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested1.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f });
                       // nested1.SpacingBefore = 8f;
                        nested1.WidthPercentage = 70;
                        cell = PhraseCell(new Phrase("CÁLCULO DE LA E.C.D.F. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 6;
                        cell.BorderColor = BaseColor.GRAY;
                        cell.BackgroundColor = BaseColor.GRAY;
                        nested1.AddCell(cell);
                        if (Liq.AcLiMeLi == "01" || Liq.AcLiMeLi == "01" || Liq.AcLiMeLi == "03" || Liq.AcLiMeLi == "06" || Liq.AcLiMeLi == "11" || Liq.AcLiMeLi == "09")
                        {
                            cell = PhraseCarta(new Phrase("Carga Contratada (Kw)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 2;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Factor Util. (%)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Meses", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("CO(kWh)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("E.C.D.F. (kWh)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);

                            if (Liq.AcLiMeLi == "01")
                            {
                                cell = PhraseCarta(new Phrase(Liq.AcLiCaCo.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                                cell.Colspan = 2;
                                cell.BorderColor = BaseColor.GRAY;
                                nested1.AddCell(cell);
                            }
                            if (Liq.AcLiMeLi == "03")
                            {
                                cell = PhraseCarta(new Phrase(Liq.AcLiKwCe.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                                cell.Colspan = 2;
                                cell.BorderColor = BaseColor.GRAY;
                                nested1.AddCell(cell);
                            }
                            if (Liq.AcLiMeLi == "06")
                            {
                                cell = PhraseCarta(new Phrase(Liq.AcLiCoPo.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                                cell.Colspan = 2;
                                cell.BorderColor = BaseColor.GRAY;
                                nested1.AddCell(cell);
                            }
                            if (Liq.AcLiMeLi == "09")
                            {
                                cell = PhraseCarta(new Phrase(Liq.AcLiPrEs.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                                cell.Colspan = 2;
                                cell.BorderColor = BaseColor.GRAY;
                                nested1.AddCell(cell);
                            }
                            if (Liq.AcLiMeLi == "11")
                            {
                                cell = PhraseCarta(new Phrase(Liq.AcLiPoEr.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                                cell.Colspan = 2;
                                cell.BorderColor = BaseColor.GRAY;
                                nested1.AddCell(cell);
                            }

                            cell = PhraseCarta(new Phrase(Liq.AcLiPoUt.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiMese.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiCoTo.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Math.Round(float.Parse(Act.ValorEcdf.ToString()), 0).ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);


                        }
                        else
                        {
                            cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Fase 1", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Fase 2", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Fase 3", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Amperaje", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLifa1.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLifa2.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLifa3.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Voltaje", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiVolt.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiVolt2.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiVolt3.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);

                            cell = PhraseCarta(new Phrase("Carga Encontrada (Kw)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;

                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Factor Util.(%)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Nro. Horas", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("E.C.D.F. (kWh)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);

                            cell = PhraseCarta(new Phrase(Liq.AcLiCaCo.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);

                            cell = PhraseCarta(new Phrase(Liq.AcLiPoUt.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Liq.AcLiHora.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(Math.Round(float.Parse(Act.ValorEcdf.ToString()), 0).ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BorderColor = BaseColor.GRAY;
                            nested1.AddCell(cell);

                            cell = PhraseCarta(new Phrase("- ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 3;

                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("- ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase("-  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);
                            cell = PhraseCarta(new Phrase(" - ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.GRAY;
                            nested1.AddCell(cell);


                        }

                        PdfPCell nesthousing1 = new PdfPCell(nested1);
                        nesthousing1.Padding = 0f;
                        nesthousing1.Colspan = 4;
                        table.AddCell(nesthousing1);

                        document.Add(table);

                        table = new PdfPTable(6);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 8f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("MÉTODO UTILIZADO PARA EL CALCULO DE LA ENERGÍA CONSUMIDA DEJADA DE FACTURAR", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 6;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Liq.AcliDeFo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 6;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(6);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 8f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("REEMPLAZANDO LOS VALORES EN LA FORMULA MATEMÁTICA TENEMOS:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 6;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Liq.AcLiObse, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BorderColor = BaseColor.GRAY;
                        table.AddCell(cell);



                        document.Add(table);

                        cell = ImageCellPie("~/images/Electricaribe.png", 50f, PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Close();

                        byte[] bytes = memoryStream1.ToArray();
                        memoryStream1.Close();
                        GuardarDoc(acta, 13, saveAs, user);
                        return saveAs;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        protected static void GuardarDoc(int acta, int tipo, string Url,string usuario)
        {
            try
            {
                Doc.DocuActa = acta;
                Doc.DocuTiDo = tipo;
                Doc.DocuUrLo = Url;
                Doc.DocuUsCa = usuario;
                Doc.DocuFeCa = DateTime.Now;
                Doc.Insert();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static string GenerarCarta(int acta, int conse, string usuario, bool evol)
        {
            try
            {
                if (CargarDatosCarta(acta, conse))
                {
                    string filename="";
                    string irregularidades="";
                    foreach (var item in Ano)
                    {
                        irregularidades += item.AcAnDesc + ","; 
                    }
                    Document document = new Document(PageSize.LETTER, 90, 50, 30, 65);
                    document.SetMargins(25f, 25f, 25f, 25f);
                    //  iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, Color.Black);
                    if (Act.protocolo == 0)
                    {
                         filename = "PSE"+acta.ToString() + conse.ToString() + ".pdf"; ;
                    }
                    else
                    {
                        filename = "PSP" + acta.ToString() + conse.ToString() + ".pdf"; ;
                    }

                    string saveAs = (@"~\File\Documentos\ProcesoSimplificado\" + filename);
                    using (System.IO.MemoryStream memoryStream1 = new System.IO.MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, new System.IO.FileStream(HttpContext.Current.Server.MapPath(saveAs), FileMode.Create));
                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = BaseColor.YELLOW;

                        document.Open();

                        int para = 1;
                        //Header Table
                        table = new PdfPTable(1);
                        table.TotalWidth = 700f;
                        table.LockedWidth = true;
                        table.SetWidths(new float[] { 8f });
                        table.WidthPercentage = 100;
                        //Company Logo
                        cell = ImageCell("~/images/Electricaribe.png", 50f, PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        
                        document.Add(table);
                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 8f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase(Act.municipio, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 6;                       
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(", "+System.DateTime.Now.ToShortDateString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Radicación # :" + Pro.NoRaPrec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Señor(a):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                       
                        cell = PhraseCarta(new Phrase(Pro.Cliente, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        if (Act.codigoTarifa.Contains("RS") || Act.codigoTarifa.Contains("RS"))
                        {
                            cell = PhraseCarta(new Phrase("REPRESENTANTE SUSCRIPTOR COMUNITARIO BARRIO " + Act.localidad, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                        } 
                        cell = PhraseCarta(new Phrase(Pro.DireProce, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Act.nic, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(Act.municipio, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;

                        cell = PhraseCarta(new Phrase("Referencia:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Fundamento y Soportes de la factura de energía consumida dejada de facturar " + Pro.NoFaProc, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan =6;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell = PhraseCarta(new Phrase("Cordial Saludo,", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        if (Act.codigoTarifa.Contains("RS") || Act.codigoTarifa.Contains("RS"))
                        {
                            cell = PhraseCarta(new Phrase("La empresa Electricaribe S.A E.S.P., en ejercicio de las facultades conferidas por la Ley 142 de 1994, el Contrato de Condiciones Uniformes y demás normas que regulan la prestación del Servicio Público Domiciliario de Energía Eléctrica, el día "+ Act._clientCloseTs.ToShortDateString() +" realizó una revisión de las instalación eléctrica del(os) totalizador(es) del Barrio Subnormal "+Act.localidad+", ubicado en el municipio " + Act.municipio.ToUpper() +", del departamento "+Act.departamento+", identificado(s) en el sistema comercial, con el(os) NIC "+Act.nic+", así como también su punto de conexión. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            document.Add(table);

                            table = new PdfPTable(8);
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                            table.SpacingBefore = 10f;
                            table.WidthPercentage = 100;

                            cell = PhraseCarta(new Phrase(@"La revisión técnica se realizó conforme a lo establecido en el numeral 6 de la cláusula denominada “RESPONSABILIDAD ESPECIALES DE ELECTRICARIBE” y la cláusula “APLICACIÓN ACUERDO DE CONDICIONES UNIFORMES” del contrato especial de suministro de energía celebrado con el Suscriptor Comunitario del Barrio Subnormal que Ud. representa, así como lo establecido en la cláusulas 43 “VERIFICACIÓN EN SITIO DE INSTALACIÓN Y DE EQUIPOS PARA MEDICIÓN DE ENERGÍA ELÉCTRICA”  del Contrato de Condiciones Uniformes de ELECTRICARIBE y lo aplicable de la cláusula 44 “GARANTÍAS PARA LA VERIFICACIÓN EN SITIO” del mismo contrato, a cuya remisión hace la cláusula “APLICACIÓN ACUERDO DE CONDICIONES UNIFORMES” del contrato suscrito entre ELECTRICARIBE y el Suscriptor Comunitario del Barrio Subnormal que Ud. representa, en todo aquello que resulte compatible con este contrato. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Para efectos de acreditar las actuaciones desarrolladas en la vista, se levantó el acta de revisión  No. " + Act._number + " en la cual se dejó constancia de todo lo anterior y de la siguiente anomalía técnica: " + irregularidades.ToUpper() + ".  Constancia de dicha visita  fue entregada al usuario al momento de culminar la visita. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);

                            document.Add(table);
                        }
                        else
                        {
                            cell = PhraseCarta(new Phrase("La empresa Electricaribe S.A E.S.P. en ejercicio de las facultades conferidas por la Ley 142 de 1994, el Contrato de Condiciones Uniformes y demás normas que regulan la prestación " +
                            " del Servicio Público Domiciliario de Energía Eléctrica, realizó una revisión de la instalación eléctrica   el día " +
                           Act._clientCloseTs.ToShortDateString() + " en las instalaciones eléctricas del inmueble ubicado en la " + Act.direccion + " del Municipio de " + Act.municipio.ToUpper() + ",  identificado en el sistema comercial, " +
                           "con el NIC " + Act.nic + ", levantándose acta de revisión e instalación eléctrica No. " + Act._number.ToString() + "en la cual se dejó constancia de la anomalía técnica detectada y de haber comunicado al cliente/usuario el derecho que tiene de ser asistido por un técnico particular. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            document.Add(table);

                            table = new PdfPTable(8);
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                            table.SpacingBefore = 10f;
                            table.WidthPercentage = 100;
                            cell = PhraseCarta(new Phrase("La revisión técnica se realizó conforme a lo establecido en las cláusulas 43 “VERIFICACIÓN EN SITIO DE INSTALACIÓN Y DE EQUIPOS PARA MEDICIÓN DE ENERGÍA ELÉCTRICA” y 44 “GARANTÍAS PARA LA VERIFICACIÓN EN SITIO” del Contrato de Condiciones Uniformes de ELECTRICARIBE y las normas técnicas, concediéndoles incluso al usuario el derecho de ser asistido por un técnico particular. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                            cell.Colspan = 8;
                            table.AddCell(cell);

                            document.Add(table);
                        }
                       

                       
                        if (Act.protocolo != 0)
                        {
                            table = new PdfPTable(8);
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                            table.SpacingBefore = 10f;
                            table.WidthPercentage = 100;
                            cell = PhraseCarta(new Phrase("En desarrollo de esta visita técnica se realizó un censo de carga, se tomaron fotografías y filmaciones de los equipos revisados, se retiró el medidor No. "+Pro.NoMeProc+", marca "+Pro.MaMePrec+", el cual se depositó en una bolsa de seguridad para garantizar la cadena de custodia del equipo hasta ser entregado en el Laboratorio. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase("Para efectos de acreditar las actuaciones desarrolladas en la vista, se levantó el acta de revisión  No. "+Act._number+" en la cual se dejó constancia de todo lo anterior y de la siguiente anomalía técnica: "+irregularidades.ToUpper()+".  Constancia de dicha visita  fue entregada al usuario al momento de culminar la visita. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                            cell = PhraseCarta(new Phrase("El medidor retirado  fue remitido al laboratorio "+Pro.LaboProc.ToUpper() +"debidamente acreditado ante el Organismo Nacional de Acreditación de Colombia (ONAC), quién siguiendo todos los procedimientos acreditados adelantó la revisión de este equipo.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            //cell = PhraseCarta(new Phrase(Pro.AnLaProce, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            //cell.Colspan = 8;
                            //table.AddCell(cell);

                            table.AddCell(cell);
                            document.Add(table);
                        }

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 8f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("Teniendo en cuenta lo anterior, ELECTRICARIBE procedió a la valoración de las siguientes pruebas y soportes, los cuales se encuentran adjuntos al presente documento, para su conocimiento: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase(para.ToString()+".     Acta de Revisión: Dentro del expediente, se encuentra como prueba documental el Acta de Revisión con Orden de Servicio No. "+
                            Act._number +" de fecha  "+ String.Format("{0:dd/MM/yyyy}", Act._clientCloseTs.ToShortDateString()) +" en la cual se plasmaron los resultados evidenciados en la revisión técnica desarrollada en las "+
                            "instalaciones eléctricas del inmueble en mención, en está acta se consignó la anomalía consistente en: "+irregularidades.ToUpper()+" datos del cliente, datos del predio, "+
                            "censo de carga de los aparatos eléctricos susceptibles de conexión encontrándos "+Act.censoCargaInstalada+".", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        para ++;
                        document.Add(table);
                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase(para.ToString() + ".     Fotografías y/o Videos  que evidencian la Anomalía Técnica detectada: En desarrollo de la revisión se obtuvieron registros fotográficos que soportan el procedimiento adelantado el día de la revisión técnica y evidencian la anomalía detectada.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        para ++;
                        document.Add(table);
                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase(para.ToString() + ".     Formato de Liquidación: Es la cuantificación de la energía consumida dejada de facturar,  a causa de la anomalía detectada, conforme al método de liquidación establecido para este caso en particular.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);

                        document.Add(table);
                        if (Act.codigoTarifa.Contains("RS") || Act.codigoTarifa.Contains("RS"))
                        {

                            table = new PdfPTable(8);
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                            table.SpacingBefore = 8f;
                            table.WidthPercentage = 100;
                            cell = PhraseCarta(new Phrase("De la valoración de todas estas pruebas, se puede establecer la existencia de la siguiente anomalía: "+irregularidades.ToUpper()+". Esta anomalía técnica evidentemente afecta la medida, ya que le impide al medidor registrar la totalidad de la energía que efectivamente se consume en el inmueble, lo cual da lugar al cobro de un energía consumida dejada de facturar, tal y como lo establecen las cláusulas 46 y 47 del Contrato de Condiciones Uniformes de ELECTRICARIBE, aplicable por expresa remisión del contrato suscrito entre ELECTRICARIBE y el Suscriptor Comunitario del Barrio Subnormal que Ud. representa, tal y como se indicó anteriormente.  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                        }
                        else
                        {
                            //table = new PdfPTable(8);
                            //table.HorizontalAlignment = Element.ANCHOR;
                            //table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                            //table.SpacingBefore = 10f;
                            //table.WidthPercentage = 100;
                            //cell = PhraseCarta(new Phrase(para.ToString()+".     Informe Técnico: Documento que amplia y complementa lo consignado en el acta de revisión con orden de servicio No. " +
                            //    Act._number + ".", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                            //cell.Colspan = 8;
                            //table.AddCell(cell);para ++;
                            //document.Add(table);

                            if (evol)
                            {
                                table = new PdfPTable(8);
                                table.HorizontalAlignment = Element.ANCHOR;
                                table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                                table.SpacingBefore = 8f;
                                table.WidthPercentage = 100;
                                cell = PhraseCarta(new Phrase(para.ToString() + ".     Registro de Evolución de Consumos del suministro obtenido  del sistema de Gestión Comercial de Electricaribe  S.A E.S.P: El registro obtenido del sistema de gestión comercial de Electricaribe,  contiene los consumos facturados al cliente identificado con el NIC " + Act.nic + ".", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                                cell.Colspan = 8;
                                table.AddCell(cell);
                                para ++;
                                document.Add(table);
                            }
                            if (Act.protocolo != 0)
                            {
                                table = new PdfPTable(8);
                                table.HorizontalAlignment = Element.ANCHOR;
                                table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                                table.SpacingBefore = 8f;
                                table.WidthPercentage = 100;
                                cell = PhraseCarta(new Phrase("6.     Informe de calibración número " + Pro.InCaPrec + ": Documento emitido por el laboratorio " + Pro.LaboProc.ToUpper() + " debidamente acreditado por el Organismo Nacional de Acreditación en Colombia (ONAC), que describe las  condiciones técnicas que inciden en el registro de la energía consumida  en el suministro.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                                cell.Colspan = 8;
                                table.AddCell(cell);
                                document.Add(table);
                            }
                        }
                        

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("FUNDAMENTOS PARA  DETERMINAR  EL CONSUMO FACTURABLE NO MEDIDO O REGISTRADO.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);

                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("De conformidad con lo establecido en la presente comunicación, se prueba por parte de Electricaribe S.A. E.S.P. la existencia de la anomalía técnica encontrada en el inmueble en mención, la cual originó la existencia de una energía consumida dejada de facturar.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("Determinación del Consumo Facturable  No Medido o Registrado por acción u omisión del usuario:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);

                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("El Consumo Facturable no medido o registrado por causa de la anomalía detectada, se determinará por período de facturación (C2), que será la diferencia entre el consumo calculado para el inmueble en condiciones normales (C1) y el consumo medido por ELECTRICARIBE y efectivamente facturado durante el tiempo que permaneció la conducta irregular, si no se logra determinar esto último, durante los últimos cinco (5) meses, (C0), será  la  sumatoria  de los  consumos facturados  irregulares  antes  de la revisión,  según la siguiente fórmula:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("C2 = C1 – C0", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("C1= CI x FU x Número de Horas (kWh.), donde:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("CI= Carga Encontrada Medida, Censo de Carga o Carga Contratada", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCarta(new Phrase("Valoración del  Consumo No Registrado:.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);

                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("El Consumo No Registrado por acción u omisión del usuario (C2), se valorará a la tarifa vigente (VL) correspondiente al mes de detección del uso no autorizado de energía eléctrica, por el tiempo de permanencia del mismo en meses (TM), según la siguiente fórmula:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("VC = C2 x VL x TM", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("La tarifa vigente (VL) será la que corresponda al sector de consumo, incluyendo el costo del servicio y los factores aplicables según la reglamentación existente (contribuciones o subsidios).", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Si no se puede establecer el tiempo de permanencia del uso no autorizado del servicio de energía eléctrica (TM) se tomará como rango 720 horas, multiplicado por cinco (5) meses.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("De acuerdo a lo consignado en el formato de liquidación adjunto, se procede a cuantificar la energía consumida  dejada  de  facturar  en pesos  de  acuerdo a la tarifa vigente al  momento de la detección de la  irregularidad, así:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        float total = (float.Parse(Act.ValorEcdf.ToString()) * float.Parse(Act.ValorTarifa.ToString()));
                        cell = PhraseCarta(new Phrase("(C2) Energía consumida dejada de facturar=   "+ (Math.Round(float.Parse(Act.ValorEcdf.ToString()), 2)).ToString()+" kWh", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("(VL) Tarifa vigente ($/kWh)=  " + Math.Round(float.Parse(Act.ValorTarifa.ToString()), 3), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Importe del Consumo = " + (Math.Round(float.Parse(Act.ValorEcdf.ToString()), 2)).ToString() + "kWh x " + Math.Round(float.Parse(Act.ValorTarifa.ToString()), 3) + " $/kWh = $" + Math.Round(total, 3).ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Adjunto encontrará la factura con la descripción detallada de los conceptos facturados en la misma. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("La empresa Electricaribe S.A. E.S.P., conforme a sus políticas  comerciales, le ofrece  algunos planes  para que usted pueda financiar esta deuda; en virtud a ello le solicitamos se sirva hacer presencia en los centros de atención más cercano a  su  residencia o llamar la línea  de atención 115.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Según lo dispuesto en el inciso 2º del artículo 130 de la ley 142 de 1994 modificado por artículo 18 de la Ley 689 de 2001, el propietario, el suscriptor y usuario del servicio son solidarios en los derechos y obligaciones derivados del contrato de servicios públicos. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Si usted no está de acuerdo con lo registrado en la factura adjunta podrá presentar las reclamaciones por escrito que considere necesarias, los recursos de ley conforme lo establece la Ley 142 de 1.994, en el centro de atención más cercano a  su  residencia. ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase("Atentamente,", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCarta(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);

                        cell = ImageCell(@"~\FirmasDigitales\firma.jpg", 40f, PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);
                        document.Close();

                        byte[] bytes = memoryStream1.ToArray();
                        memoryStream1.Close();
                        GuardarDoc(acta, 16, saveAs, usuario);
                        return saveAs;
                    }
                }
                else
                {
                    return "Default.aspx";
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public static void GenerarActa(int number, string usuario, string ruta)
        {
            try
            {

                ruta1 = ruta;
                if (CargarDatos(number))
                {
                    Document document = new Document(PageSize.LETTER, 90, 50, 30, 65);
                    document.SetMargins(25f, 25f, 25f, 25f);
                    //  iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, Color.Black);
                    string saveAs = "";
                    string filename = number.ToString() + "a" + System.DateTime.Now.Hour.ToString() + ".pdf"; ;
                    if (ruta == "")
                    {
                         saveAs = (@"~\File\Documentos\Actas\" + filename);
                    }
                    else
                    {
                        saveAs = ruta + filename;
                    }
                    

                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        if (File.Exists(saveAs))
                        {
                            idx += 1;
                            saveAs = string.Format("{0}.{1}.pdf", saveAs, idx);
                        }
                        PdfWriter writer = PdfWriter.GetInstance(document, new System.IO.FileStream(HttpContext.Current.Server.MapPath(saveAs), FileMode.Create));
                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = BaseColor.YELLOW;

                        document.Open();


                        //Header Table
                        table = new PdfPTable(1);
                        table.TotalWidth = 700f;
                        table.LockedWidth = true;
                        table.SetWidths(new float[] { 8f });
                        table.WidthPercentage = 100;
                        //Company Logo
                        if (ruta == "")
                        {
                            cell = ImageCell(@"~\File\Documentos\Actas\Acta.png", 50f, PdfPCell.ALIGN_CENTER);
                           
                        }
                        else
                        {
                            cell = ImageCell(ruta + "Acta.png", 50f, PdfPCell.ALIGN_CENTER);
                        }
                       
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        //Separater Line                         
                        //DrawLine(writer, 25f, document.Top - 50f, document.PageSize.Width - 25f, document.Top - 50f, color);
                        //DrawLine(writer, 25f, document.Top - 49f, document.PageSize.Width - 25f, document.Top - 49f, color);
                        document.Add(table);

                        table = new PdfPTable(8);
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        table.SpacingBefore = 8f;
                        table.WidthPercentage = 100;
                        //table.TotalWidth = 0f;
                        //document.Add(table);

                        cell = PhraseCell(new Phrase("Acta de Revisión  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 6;
                        cell.BorderColor = BaseColor.GRAY;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Número OS:  " + Act._number.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BorderColor = BaseColor.GRAY;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        //document.Add(table);
                        ////Employee Details
                        //table = new PdfPTable(8);
                        //table.HorizontalAlignment = Element.ANCHOR;
                        //table.SetWidths(new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                        //table.SpacingBefore = 8f;
                        //table.WidthPercentage = 100;
                        string atendioVisita;
                        if (Act.relacionReceptorVisita == "El titular")
                        {
                            atendioVisita = Act.nombreTitularContrato + " " + Act.apellido1TitularContrato + " " + Act.apellido2TitularContrato + "con C.C. " + Act.cedulaTitularContrato;
                        }
                        else
                        {
                            atendioVisita = Act.nombreReceptorVisita + " " + Act.apellido1ReceptorVisita + " " + Act.apellido2ReceptorVisita + "con C.C." + Act.cedulaReceptorVisita;
                        }
                        cell = PhraseCell(new Phrase("A los " + Act._clientCloseTs.Day + " días del mes  " + Act._clientCloseTs.Month + " del " + Act._clientCloseTs.Year + ", siendo las " + Act._clientCloseTs.ToShortTimeString() + "  se hace presente en el inmueble identificado comercialmente con el NIC " + Act.nic +
                             " la siguiente persona " + Act.nombreOperario + " " + Act.apellido1Operario + " " + Act.apellido2Operario + " en representación de Electricaribe"+Act.codigoEmpresa +" y en presencia del señor(a) "
                            + atendioVisita + ", en calidad de clientes/usuario, con el fin de efectuar una revisión de los equipos de"
                            + " medida e instalaciones eléctricas del inmueble con el NIC indicado, cuyo resultado ha sido el siguiente:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        document.Add(table);
                        /*DATOS DEL SUMINISTRO*/
                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        //table.TotalWidth = 10f;
                        //Employee Details
                        cell = PhraseCell(new Phrase("Datos del suministro", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Nic:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.nic, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo Cliente:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.tipoCliente, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Estrato:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.estrato.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("C. Contratada(W): ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.cargaContratada.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        document.Add(table);


                        /*DATOS DEL SUMINISTRO*/
                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        //table.TotalWidth = 10f;
                        //Employee Details
                        cell = PhraseCell(new Phrase("Localización", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Departamento:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.departamento, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Municipio:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.municipio, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Localidad:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.localidad, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo Via: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.tipoVia, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Calle: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.calle, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Num. Puerta::  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.numeroPuerta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Duplicador:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.duplicador, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Piso:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.piso, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ref. Dir.: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.referenciaDireccion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Acceso : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.acceso, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Dirección : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.direccion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fotografías Fachada : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;
                        table.AddCell(cell);
                        /*titular contrato*/

                        cell = PhraseCell(new Phrase("Titular Contrato", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Nombre:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.nombreTitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Apellidos:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.apellido1TitularContrato + " " + Act.apellido2TitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Cedula: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.cedulaTitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Telefono fijo: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.telefonoFijoTitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Telefono Movil:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.cedulaTitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Email:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.emailTitularContrato, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        /*persona que atiende visita*/
                        cell = PhraseCell(new Phrase("Persona que atendio Visita", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Relación con el titular:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.relacionReceptorVisita, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Nombre : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.nombreReceptorVisita + " " + Act.apellido1ReceptorVisita + " " + Act.apellido2ReceptorVisita, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Cedula : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.cedulaReceptorVisita, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Soliccita Técnico? : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.solicitaTecnicoReceptorVisita, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Aporta Testigo? : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.aportaTestigo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 7;
                        table.AddCell(cell);


                        /*Aparatos encontrados*/
                        cell = PhraseCell(new Phrase("Aparatos", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Aparatos Encontrados", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Datos del medidor", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Revisión Medidor:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.tipoRevision, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 7;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Número medidor : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.numero, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Marca : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.marca, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.tipo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tecnología : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.tecnologia, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Lecturas", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Fecha última lectura:  ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.lecturaUltimaFecha, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Última lectura : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.lecturaUltima, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fecha lectura actual : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act._clientCloseTs.ToShortDateString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Lectura actual : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.lecturaActual, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Kh/Kd : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Kd/Kh : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.kdkh_tipo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Valor : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.kdkh_value, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Otros : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Dígitos : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.digitos, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Decimales : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.decimales, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Dígitos : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.digitos, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Decimales : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.decimales, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        //document.Add(table);
                        cell = PhraseCell(new Phrase("N° Fases : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.nFases, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V. Nominal : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltajeNominal, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Rango Min. Corriente : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.rangoCorrienteMin, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Rango Max. Corriente : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.rangoCorrienteMax, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fotografías Aparato : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        document.Add(table);


                        /*Sellos*/
                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        //table.TotalWidth = 10f;
                        //Employee Details
                        cell = PhraseCell(new Phrase("Sellos", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Sellos Encontrados", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Número Medidor", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Número Sello", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Estado", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Posición ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Color ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo Sello", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        if (Sellos.Count() > 0)
                        {
                            foreach (var item in Sellos)
                            {
                                if (item.AcSeTipo == 1)
                                {
                                    cell = PhraseCell(new Phrase(item.AcSeNuMe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeNuSe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeEsta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 2;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSePosi, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeColo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeTiSe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 2;

                                    table.AddCell(cell);
                                }
                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin información para mostrar", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;


                        }
                        cell = PhraseCell(new Phrase("Sellos Instalados", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Número Medidor", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Número Sello", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Estado", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Posición ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Color ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo Sello", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        if (Sellos.Count() > 0)
                        {
                            foreach (var item in Sellos)
                            {
                                if (item.AcSeTipo == 2)
                                {
                                    cell = PhraseCell(new Phrase(item.AcSeNuMe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeNuSe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeEsta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 2;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSePosi, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeColo, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 1;

                                    table.AddCell(cell);
                                    cell = PhraseCell(new Phrase(item.AcSeTiSe, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                    cell.Colspan = 2;

                                    table.AddCell(cell);
                                }
                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin información para mostrar", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;


                        }
                        cell = PhraseCell(new Phrase("Fotografías Sello : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Mediciones encontradas comunes", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Mediciones", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("I(Neutro):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteN_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(F+N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteFN_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(N-T):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageNT_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Fasee R", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNR_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("V(F-T):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFTR_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteR_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fasee S", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNS_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("V(F-T):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFTS_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteS_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fasee T", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNT_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("V(F-T):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFTT_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteT_mec, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fotografías  : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;
                        table.AddCell(cell);
                        /*pruebas de exactitud*/

                        cell = PhraseCell(new Phrase("Pruebas de exactitud (Alta)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo prueba", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.pruebaAlta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fase R", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNR_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteR_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Resultados (% Error):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.errorPruebaR_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fase S", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F+N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNS_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteS_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Resultados (% Error):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.errorPruebaS_alta, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fotografías Pruebas exactitud (Alta)  : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;
                        table.AddCell(cell);



                        /*pruebas de exactitud*/

                        cell = PhraseCell(new Phrase("Pruebas de exactitud (Baja)", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo prueba", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.pruebaBaja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fase R", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNR_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteR_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Resultados (% Error):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.errorPruebaR_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fase S", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F+N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNS_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteS_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Resultados (% Error):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.errorPruebaS_baja, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fotografías Pruebas exactitud (Alta)  : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;
                        table.AddCell(cell);


                        /*pruebas de exactitud*/

                        cell = PhraseCell(new Phrase("Pruebas de dosificación ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Tipo prueba", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.pruebaDosificacion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Fase R", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("V(F-N):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.voltageFNR_dosif, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("I(Fase):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.corrienteR_dosif, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Resultados (% Error):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.errorPruebaR_dosif, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Lectura inicial:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.lecturaInicialR_dosif, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Lectura final:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.lecturaFinalR_dosif, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Fotografías Pruebas exactitud (Alta)  : ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Ver Anexo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 5;

                        table.AddCell(cell);

                        /*pruebas de exactitud*/

                        cell = PhraseCell(new Phrase("Pruebas de Funcionamiento ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Pulso o giro normal?:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.giroNormal, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Continuidad:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.continuidad, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Display:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.display, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);


                        cell = PhraseCell(new Phrase("Estado conexiones:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.estadoConexiones, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Prueba Puentes:", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.pruebaPuentes, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);

                        cell = PhraseCell(new Phrase("Estado integrador: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.estadoIntegrador, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        cell = PhraseCell(new Phrase("Retirar Medidor?: ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Me.retirado, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 1;
                        table.AddCell(cell);
                        table.AddCell(cell);

                        document.Add(table);

                        /*Sellos*/
                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCell(new Phrase("Anomalias", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        if (Ano.Count() > 0)
                        {
                            foreach (var item in Ano)
                            {
                                cell = PhraseCell(new Phrase(item.AcAnDesc, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 8;
                                table.AddCell(cell);
                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin anomalia encontrada.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);
                        }
                        cell = PhraseCell(new Phrase("Observaciones", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.obsAnomalia, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;

                        table.AddCell(cell);
                        document.Add(table);


                        /*TRABAJOS EJECUTADOS*/
                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        //table.TotalWidth = 10f;
                        //Employee Details
                        cell = PhraseCell(new Phrase("Trabajos Ejecutados", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Acciones", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Codigo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Descripción", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        if (Tra.Count() > 0)
                        {
                            foreach (var item in Tra)
                            {

                                cell = PhraseCell(new Phrase(item.CodigoAccion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 4;
                                table.AddCell(cell);
                                cell = PhraseCell(new Phrase(item.DescripcionAccion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 4;

                                table.AddCell(cell);


                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin información para mostrar", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);

                        }
                        cell = PhraseCell(new Phrase(" ", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Materiales", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_CENTER);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Codigo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Descripción", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 3;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Cantidad", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        if (Mat.Count() > 0)
                        {
                            foreach (var item in Mat)
                            {

                                cell = PhraseCell(new Phrase(item.CodigoMaterial, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 3;
                                table.AddCell(cell);
                                cell = PhraseCell(new Phrase(item.Descripcion, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 3;

                                table.AddCell(cell);
                                cell = PhraseCell(new Phrase(item.Cantidad.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 2;

                                table.AddCell(cell);

                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin información para mostrar", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;
                            table.AddCell(cell);

                        }

                        document.Add(table);

                        table = new PdfPTable(8);
                        //table.LockedWidth = true;
                        table.HorizontalAlignment = Element.ANCHOR;
                        table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                        table.SpacingBefore = 10f;
                        table.WidthPercentage = 100;
                        cell = PhraseCell(new Phrase("Censo de Carga", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);

                        if (Cen.Count() > 0)
                        {
                            cell = PhraseCell(new Phrase("Item", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 4;
                            cell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(cell);
                            cell = PhraseCell(new Phrase("Cant", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 4;
                            cell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(cell);
                            foreach (var item in Cen)
                            {
                                cell = PhraseCell(new Phrase(item.AcCeItem, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 4;
                                table.AddCell(cell);
                                cell = PhraseCell(new Phrase(item.AcCeNoIt.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 4;
                                table.AddCell(cell);
                            }
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin Censo encontrado.", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 8;

                            table.AddCell(cell);
                        }
                        cell = PhraseCell(new Phrase("Tipo", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.tipoCenso, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Carga Instalada (kW):", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.censoCargaInstalada.ToString(), FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 2;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Cierre y Observaciones", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;
                        cell.BackgroundColor = BaseColor.GRAY;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase(Act.observaciones, FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 8;

                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Firma Operario", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        cell = PhraseCell(new Phrase("Firma Atendio Visita", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)), PdfPCell.ALIGN_LEFT);
                        cell.Colspan = 4;
                        table.AddCell(cell);
                        if (Firma.DocuUrLo != null)
                        {

                            if(ruta == "")
                            {
                                if (File.Exists(Firma.DocuUrLo))
                                {
                                    cell = ImageCell(Firma.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                }
                              
                            }
                            else
                            {

                                if (File.Exists(ruta + Firma.DocuUrLo))
                                {
                                    cell = ImageCell(ruta + Firma.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                }
                            }
                           
                            cell.Colspan = 4;
                            cell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin Firma en la base de datos", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 4;
                            cell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }
                        
                        Firma = BllDocumentos.GetFirmaOperario(Act._number, 9);
                        if (Firma.DocuUrLo != null)
                        {
                            if (ruta == "")
                            {
                                if (File.Exists(Firma.DocuUrLo))
                                {
                                    cell = ImageCell(Firma.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                }

                            }
                            else
                            {

                                if (File.Exists(ruta + Firma.DocuUrLo))
                                {
                                    cell = ImageCell(ruta + Firma.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                }
                            }
                            //cell = ImageCell(Firma.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 4;
                            cell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = PhraseCell(new Phrase("Sin Firma en la base de datos", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                            cell.Colspan = 4;

                            table.AddCell(cell);
                        }
                        document.Add(table);
                        document.NewPage();
                        ListDocu = Docu.Where(b => b.DocuTiDo == 1).ToList();
                        if (ListDocu.Count() > 0)
                        {
                            table = new PdfPTable(8);
                            //table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                            table.SpacingBefore = 10f;
                            table.WidthPercentage = 100;

                            ListDocu = Docu.Where(b => b.DocuTiDo == 1).ToList();
                            if (ListDocu.Count() > 0)
                            {
                                cell = PhraseCell(new Phrase("Anexo Fotografías Fachada", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 8;
                                cell.BackgroundColor = BaseColor.GRAY;
                                table.AddCell(cell);
                                foreach (var item in ListDocu)
                                {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                            }
                            document.Add(table);
                        }
                        ListDocu = Docu.Where(b => b.DocuTiDo == 3).ToList();
                     //   ListDocu =  new List<BllDocumentos.Documentos>();
                        if (ListDocu.Count() > 0)
                        {
                            table = new PdfPTable(8);
                            //table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ANCHOR;
                            table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                            table.SpacingBefore = 10f;
                            table.WidthPercentage = 100;

                            
                            if (ListDocu.Count() > 0)
                            {
                                cell = PhraseCell(new Phrase("Anexo Fotografías Aparato", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                cell.Colspan = 8;
                                cell.BackgroundColor = BaseColor.GRAY;
                                table.AddCell(cell);
                                foreach (var item in ListDocu)
                                {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                    table.AddCell(cell);
                                }
                            }
                            document.Add(table);
                        }
                        // ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 2).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                           
                             ListDocu = Docu.Where(b => b.DocuTiDo == 2).ToList();
                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Anomalia", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                        // ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 4).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                           
                             ListDocu = Docu.Where(b => b.DocuTiDo == 4).ToList();
                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Mediciones", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                        // ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 5).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                            
                             ListDocu = Docu.Where(b => b.DocuTiDo == 5).ToList();
                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Pruebas Alta", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                        // ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 6).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                             
                          
                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Pruebas Baja", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                        // ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 7).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                           
                            
                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Pruebas Dosificación", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    //cell = ImageCellFir(item.DocuUrLo, 20f, PdfPCell.ALIGN_JUSTIFIED);
                                    cell.Colspan = 4;                     
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                       //  ListDocu = new List<BllDocumentos.Documentos>();
                         ListDocu = Docu.Where(b => b.DocuTiDo == 8).ToList();
                         if (ListDocu.Count() > 0)
                         {
                             table = new PdfPTable(8);
                             //table.LockedWidth = true;
                             table.HorizontalAlignment = Element.ANCHOR;
                             table.SetWidths(new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });
                             table.SpacingBefore = 10f;
                             table.WidthPercentage = 100;
                            

                             if (ListDocu.Count() > 0)
                             {
                                 cell = PhraseCell(new Phrase("Anexo Fotografías Envio Laboratorio", FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)), PdfPCell.ALIGN_LEFT);
                                 cell.Colspan = 8;
                                 cell.BackgroundColor = BaseColor.GRAY;
                                 table.AddCell(cell);
                                 foreach (var item in ListDocu)
                                 {
                                    if (ruta == "")
                                    {
                                        if (File.Exists(item.DocuUrLo))
                                        {
                                            cell = ImageCell(item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }

                                    }
                                    else
                                    {

                                        if (File.Exists(ruta + item.DocuUrLo))
                                        {
                                            cell = ImageCell(ruta + item.DocuUrLo, 20f, PdfPCell.ALIGN_LEFT);
                                        }
                                    }
                                    cell.Colspan = 4;
                                     table.AddCell(cell);
                                 }
                             }
                             document.Add(table);
                         }
                        document.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();
                        GuardarDoc(number, 15, saveAs, usuario);

                    }

                }
            }

            catch (Exception ex)
            {
                Log.EscribirError(ex);
                throw ex;
            }
            
        }
       
        private static void DrawLine(PdfWriter writer, float x1, float y1, float x2, float y2, iTextSharp.text.BaseColor color)
        {
            PdfContentByte contentByte = writer.DirectContent;
            contentByte.SetColorStroke(color);
            contentByte.MoveTo(x1, y1);
            contentByte.LineTo(x2, y2);
            contentByte.Stroke();
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        private static PdfPCell PhraseCarta(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        private static PdfPCell ImageCell(string path, float scale, int align)
        {

            iTextSharp.text.Image image ;
            if (ruta1 == "")
            {
              image = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path));
            }
            else
            {
                image = iTextSharp.text.Image.GetInstance(path);
            }
           
            image.ScalePercent(scale);
            image.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }


        private static PdfPCell ImageCellPie(string path, float scale, int align)
        {


            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path));
            image.ScalePercent(scale);
            image.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;

            image.SetAbsolutePosition(480, 737);
           
            return cell;
        }
        private static PdfPCell ImageCellFir(string path, float scale, int align)
        {
            iTextSharp.text.Image image;
            if (ruta1 == "")
            {
                image = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path));
            }
            else
            {
                image = iTextSharp.text.Image.GetInstance(path);
            }
         
            //image.ScalePercent(scale);
            image.ScaleAbsolute(260f, 200f);
          //  image.Width = 300;
            
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 1f;
            cell.PaddingTop = 1f;
            cell.PaddingLeft = 1f;
            cell.PaddingRight = 1f;
            return cell;
        }

        class HeaderFooter : PdfPageEventHelper
        {
            /** Alternating phrase for the header. */
            Phrase[] header = new Phrase[2];
            /** Current page number (will be reset for every chapter). */
            int pagenumber;

            /**
             * Initialize one of the headers.
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onOpenDocument(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                header[0] = new Phrase("Movie history");
            }

            /**
             * Initialize one of the headers, based on the chapter title;
             * reset the page number.
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onChapter(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document, float,
             *      com.itextpdf.text.Paragraph)
             */
            public override void OnChapter(
              PdfWriter writer, Document document,
              float paragraphPosition, Paragraph title)
            {
                header[1] = new Phrase(title.Content);
                pagenumber = 1;
            }

            /**
             * Increase the page number.
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onStartPage(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                pagenumber++;
            }

            /**
             * Adds the header and the footer.
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onEndPage(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                iTextSharp.text.Rectangle rect = writer.GetBoxSize("art");
                switch (writer.PageNumber % 2)
                {
                    case 0:
                        ColumnText.ShowTextAligned(writer.DirectContent,
                          Element.ALIGN_RIGHT,
                          header[0],
                          rect.Right, rect.Top, 0
                        );
                        break;
                    case 1:
                        ColumnText.ShowTextAligned(
                          writer.DirectContent,
                          Element.ALIGN_LEFT,
                          header[1],
                          rect.Left, rect.Top, 0
                        );
                        break;
                }
                ColumnText.ShowTextAligned(
                  writer.DirectContent,
                  Element.ALIGN_CENTER,
                  new Phrase(String.Format("page {0}", pagenumber)),
                  (rect.Left + rect.Right) / 2,
                  rect.Bottom - 18, 0
                );
            }
        }
    }
}

