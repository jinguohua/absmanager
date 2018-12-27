using System;
using Aspose.Words;
using System.Drawing;
using Aspose.Words.Drawing;
using System.ComponentModel;
using System.IO;
using Aspose.Words.Saving;

namespace ChineseAbs.ABSManagement.Utils
{
    /******Word转pdf*********************************************/

    public class PdfUtils
    {
        /// <summary>
        /// 水印文字
        /// </summary>
        public string m_waterMarkText;

        [DefaultValue(-45)]
        public double m_rotation = -45;

        private PdfSaveOptions m_pdfSaveOptions;
        private static Aspose.Words.License m_license;
        public WaterMarkMultiText m_waterMarkMultiText;

        public PdfPermissionEnum PdfPermission { get; set; }

        /// <summary>
        /// 保存pdf相关权限
        /// </summary>
        [Flags]
        public enum PdfPermissionEnum
        {
            DisallowAll = 0,
            Printing = 4,
            ModifyContents = 8,
            ContentCopy = 16,
            ModifyAnnotations = 32,
            FillIn = 256,
            ContentCopyForAccessibility = 512,
            DocumentAssembly = 1024,
            HighResolutionPrinting = 2052,
            AllowAll = 65535
        }

        public PdfUtils()
        {
            SetLisence();
        }

        public PdfUtils(string waterMarkText)
        {
            SetLisence();
            m_waterMarkText = waterMarkText;
        }

        public PdfUtils(WaterMarkMultiText WaterMarkMultiText)
        {
            SetLisence();
            m_waterMarkMultiText = WaterMarkMultiText;
            // _rotation = 0;
        }

        protected static void SetLisence()
        {
            if (m_license == null || !m_license.IsLicensed)
            {
                m_license = new Aspose.Words.License();
                m_license.SetLicense("Aspose.Words.lic");
            }
        }

        protected void DocToPdf(Document doc, string outFilepath)
        {
            DealDoc(doc);
            doc.Save(outFilepath, m_pdfSaveOptions);
        }

        protected void DealDoc(Document doc)
        {
            if (m_waterMarkText != null)
            {
                InsertWatermarkText(doc);
            }
            else if (m_waterMarkMultiText != null)
            {
                InsertWatermarkMultiText(doc);
            }
            PdfSaveOptions saveOption = new PdfSaveOptions();
            saveOption.SaveFormat = Aspose.Words.SaveFormat.Pdf;
            PdfEncryptionDetails encryptionDetails = new PdfEncryptionDetails(string.Empty, "PasswordHere", PdfEncryptionAlgorithm.RC4_128);
            encryptionDetails.Permissions = (PdfPermissions)PdfPermission;

            saveOption.EncryptionDetails = encryptionDetails;
            m_pdfSaveOptions = saveOption;
            var ms = new MemoryStream();
        }

        public MemoryStream WordToPdfMemoryStream(MemoryStream memoryStream)
        {
            Document doc = new Document(memoryStream);
            return DealDocGetStream(doc);
        }

        public MemoryStream WordToPdfMemoryStream(string filePath)
        {
            Document doc = new Document(filePath);
            return DealDocGetStream(doc);
        }

        private MemoryStream DealDocGetStream(Document doc)
        {
            DealDoc(doc);
            MemoryStream ms = new MemoryStream();
            doc.Save(ms, m_pdfSaveOptions);
            return ms;
        }

        public void WordToPdf(string filePath)
        {
            Document doc = new Document(filePath);
            string pdfPath = filePath.Replace(".docx", ".pdf");
            DocToPdf(doc, pdfPath);
        }

        public void MemoryStreamToPdf(Stream stream, string outFilePath)
        {
            Document doc = new Document(stream);
            DocToPdf(doc, outFilePath);
        }

        void SetParagraphsBackgroundNoColor(Document doc)
        {
            NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
            foreach (Paragraph paragraph in paragraphs)
            {
                paragraph.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            }
        }

        private void InsertWatermarkText(Aspose.Words.Document doc)
        {
            SetParagraphsBackgroundNoColor(doc);
            var watermark = SetWatermarkFormat(doc, m_waterMarkText);
            watermark.Width = 500;
            watermark.Height = 100;
            //角度
            watermark.Rotation = m_rotation;

            Aspose.Words.Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        private void InsertWatermarkMultiText(Aspose.Words.Document doc)
        {
            SetParagraphsBackgroundNoColor(doc);

            GroupShape groupShape = new GroupShape(doc);
            groupShape.AllowOverlap = false;
            groupShape.BehindText = true;
            groupShape.Width = 300;
            groupShape.Height = 200;

            groupShape.Rotation = m_rotation;
            groupShape.WrapType = WrapType.None;
            // groupShape.WrapSide = WrapSide.Default;

            var watermark = SetWatermarkFormat(doc, m_waterMarkMultiText.BigText);
            // watermark.Bounds = m_rotation == 0 ? new RectangleF(13, 232, 400, 150) : new RectangleF(-90, 150, 400, 200);
             watermark.Bounds = m_rotation == 0 ? new RectangleF(0, 0, 400, 150) : new RectangleF(0, 0, 400, 200);
            groupShape.AppendChild(watermark);

            watermark = SetWatermarkFormat(doc, m_waterMarkMultiText.SmallText);
            // watermark.Bounds = m_rotation == 0 ? new RectangleF(13, 352, 400, 20) : new RectangleF(-90, 310, 400, 20);
             watermark.Bounds = m_rotation == 0 ? new RectangleF(0, 140, 400, 20) : new RectangleF(0, 181, 400, 20);
            groupShape.AppendChild(watermark);

            groupShape.CoordSize = new System.Drawing.Size(300, 200);


            groupShape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            groupShape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            groupShape.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            groupShape.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;


            Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(groupShape);

            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        private Shape SetWatermarkFormat(Document doc, string watermarkText)
        {
            var watermark = new Shape(doc, ShapeType.TextPlainText);
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Microsoft YaHei";

            //填充色和边线色
            watermark.Fill.Color = System.Drawing.Color.LightGray;
            watermark.StrokeColor = System.Drawing.Color.LightGray;

            //使居中
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;

            watermark.WrapType = WrapType.None;
            watermark.BehindText = true;

            return watermark;
        }

        private static void InsertWatermarkIntoHeader(Aspose.Words.Paragraph watermarkPara, Aspose.Words.Section sect, HeaderFooterType headerType)
        {
            Aspose.Words.HeaderFooter header = sect.HeadersFooters[headerType];

            if (header == null)
            {
                header = new Aspose.Words.HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }
            header.AppendChild(watermarkPara.Clone(true));
        }

        /// <summary>
        /// 使用office组件导出
        /// </summary>
        /// <param name="filename"></param>
        //public void WordToPdfWithCom(string filename)
        //{
        //    var word = new Microsoft.Office.Interop.Word.Application();
        //    object oMissing = Type.Missing;

        //    Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //        ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        //    doc.Activate();

        //    object outputFileName = filename.Replace(".docx", "COM.pdf");
        //    object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

        //    doc.SaveAs(ref outputFileName,
        //       ref fileFormat, ref oMissing, ref oMissing,
        //       ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //       ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //       ref oMissing, ref oMissing, ref oMissing, ref oMissing);

        //    object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
        //    ((Microsoft.Office.Interop.Word._Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
        //    doc = null;
        //}
    }

    public class WaterMarkMultiText
    {
        public string BigText { get; set; }
        public string SmallText { get; set; }
    }
}
