using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;
using System.IO;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class PdfUtilsTest
    {
        [TestMethod]
        public void PdfUtilsComTest()
        {
            //  var pdfUtil = new PdfUtils();
            //  pdfUtil.WordToPdfWithCom("D:\\wordToPdf\\公司简介.docx");
        }

        [TestMethod]
        public void PdfUtilsAsposeTest()
        {
            //   var pdfUtil = new PdfUtils();
            //  pdfUtil.WordToPdfWithAspose("D:\\wordToPdf\\公司简介.docx");

        }

        [TestMethod]
        public void PdfUtilsAsposeWordTest()
        {
            var pdfUtil = new PdfUtils(new WaterMarkMultiText() { BigText = "中国资产证券分析网", SmallText = "下载人" + DateTime.Now.ToString() });


            pdfUtil.PdfPermission = PdfUtils.PdfPermissionEnum.ContentCopy | PdfUtils.PdfPermissionEnum.Printing;

            pdfUtil.PdfPermission = PdfUtils.PdfPermissionEnum.Printing;
            // pdfUtil.m_rotation = 0;  
            pdfUtil.WordToPdf("D:\\wordToPdf\\公司简介横.docx");


            //pdfUtil.m_rotation = 0;
            //pdfUtil.WordToPdf("D:\\wordToPdf\\公司简介横.docx");
            //   Stream stream = File.OpenRead("D:/wordToPdf/公司简介.docx");
            // pdfUtil.MemoryStreamToPdf(stream, "D:/wordToPdf/Memory.pdf");
            //  pdfUtil.WordToPdfMemoryStream("D:/wordToPdf/公司简介.docx");

            //  var pdfUtil2 = new PdfUtils();

            //  pdfUtil2.WordToPdf("D:\\wordToPdf\\公司简介横.docx");
        }


        [TestMethod]
        public void ComparisonUtilsTest()
        {
            // var compareUtil = new ComparisonUtil();
            //compareUtil.Compare2Doc("D:\\test.docx", "D:\\test11.docx", "D:\\result.docx");
        }


        [TestMethod]
        public void Excel2HtmlTest()
        {

            // ViewOnline.Excel2Html("D:\\viewOnline\\存续期-开发计划.xlsx");

            //ViewOnline.ExcelToHtml("D:\\viewOnline\\存续期-开发计划.xlsx");
        }

        [TestMethod]
        public void Doc2HtmlTest()
        {
            //new ViewOnline().Doc2Html("D:\\wordToPdf\\Demo建元模板 (49).docx");


        }


        [TestMethod]
        public void pdf2Html()
        {
          //  ViewOnline.ExcelToHtml("D:\\viewOnline\\存续期-开发计划.xlsx");


        }





    }
}
