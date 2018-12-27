using System;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Comparison;
using System.Drawing;
using Aspose.Words.Saving;
using System.IO;
using System.ComponentModel;

namespace ChineseAbs.ABSManagement.Utils
{
    public class ComparisonUtil
    {
        private string m_saveDirectory;

        /// <summary>
        /// 比较结果中增加的个数
        /// </summary>
        public int AddCount { get; private set; }

        /// <summary>
        /// 比较结果中删除的个数
        /// </summary>
        public int DeleteCount { get; private set; }

        /// <summary>
        ///  比较结果中格式变化的个数
        /// </summary>
        public int FormatChangeCount { get; private set; }

        /// <summary>
        /// 生成图片的个数
        /// </summary>
        public int FileCount { get; private set; }

        public string FilePath { get; private set; }

        [DefaultValue("~/Temp")]
        public string TempfilePath { get; set; }

        public string TempfileName { get; set; }

        public SaveType DocSaveType { get; set; }

        public ComparisonUtil(string resultFilePath, string tempFileName, SaveType saveType)
        {
            TempfilePath = resultFilePath;
            TempfileName = tempFileName;
            FilePath = TempfileName;
            DocSaveType = saveType;
            m_saveDirectory = string.Format(@"{0}\{1}", TempfilePath, TempfileName);
        }

        /// <summary>
        /// 是否忽略格式改变
        /// </summary>
        public bool IgnoreFormatting { get; set; }

        public void Compare2Doc(string doc1Path, string doc2Path)
        {
            var doc1 = Compare(new Document(doc1Path), new Document(doc2Path));

            if (!Directory.Exists(m_saveDirectory))
            {
                ChooseSaveMethod(doc1);
            }
            else
            {
                DirectoryInfo TheFolder = new DirectoryInfo(m_saveDirectory);
                var allFiles = TheFolder.GetFiles();
                FileCount = allFiles.Where(x => (x.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden).Count();
                int imageCount = allFiles.Where(x => x.Extension == "png").Count();
                int htmlCount = allFiles.Where(x => x.Extension == "html").Count();
                FileCount = DocSaveType == SaveType.Image ? imageCount : htmlCount;
                if (FileCount == 0)
                {
                    ChooseSaveMethod(doc1);
                }
            }
        }

        private void ChooseSaveMethod(Document doc1)
        {
            switch (DocSaveType)
            {
                case SaveType.Html: SaveDocHtml(doc1); break;
                case SaveType.Image: SaveDocImg(doc1); break;
                default: SaveDocImg(doc1); break;
            }
        }

        protected Document Compare(Document doc1, Document doc2)
        {
            DocumentBuilder builder = new DocumentBuilder(doc1);
            var compOption = new CompareOptions();
            compOption.IgnoreFormatting = IgnoreFormatting;
            compOption.IgnoreHeadersAndFooters = true;
            doc1.Compare(doc2, "no author", DateTime.Now, compOption);

            foreach (Revision revision in doc1.Revisions)
            {
                switch (revision.RevisionType)
                {
                    case RevisionType.Insertion:
                        if (revision.ParentNode.GetType() == typeof(Run))
                        {
                            var run = (Run)revision.ParentNode;
                            run.Font.Color = Color.Blue;
                          //  run.Font.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(25, 0, 255, 0);
                            AddCount++;
                        }
                        else if (revision.ParentNode.GetType() == typeof(Paragraph))
                        {
                            //var parag = (Paragraph)revision.ParentNode;
                            //parag.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(50, 255, 0, 0);
                        }
                        break;
                    case RevisionType.Deletion:
                        if (revision.ParentNode.GetType() == typeof(Run))
                        {
                            var run = (Run)revision.ParentNode;
                            run.Font.Color = Color.Red;
                           // run.Font.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(25, 255, 0, 0);
                            DeleteCount++;
                        }
                        else if (revision.ParentNode.GetType() == typeof(Paragraph))
                        {
                            //var parag = (Paragraph)revision.ParentNode;
                            //parag.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(50, 0, 255, 0);
                        }
                        break;
                    case RevisionType.FormatChange:
                        if (revision.ParentNode.GetType() == typeof(Run))
                        {
                            var run = (Run)revision.ParentNode;
                            run.Font.Color = Color.Green;
                            FormatChangeCount++;
                        }
                        break;
                }
            }

            return doc1;
        }

        protected void SaveDocImg(Document doc)
        {
            if (!Directory.Exists(TempfilePath))
            {
                Directory.CreateDirectory(TempfilePath);
            }

            ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Jpeg);

            for (int i = 0; i < doc.PageCount; i++)
            {
                options.PageIndex = i;
                doc.Save(string.Format(@"{0}\{1}\{2}.png", TempfilePath, TempfileName, i + 1), options);
            }
            FileCount = doc.PageCount;
        }

        protected void SaveDocHtml(Document doc)
        {
            if (!Directory.Exists(TempfilePath))
            {
                Directory.CreateDirectory(TempfilePath);
            }
            HtmlFixedSaveOptions options = new HtmlFixedSaveOptions();
            options.ExportEmbeddedCss = true;
            options.ExportEmbeddedFonts = true;
            options.ExportEmbeddedImages = true;
            options.ExportEmbeddedSvg = true;
            doc.Save(string.Format(@"{0}\{1}\{2}.html", TempfilePath, TempfileName, 1), options);
            FileCount = doc.PageCount;
        }

        public enum SaveType
        {
            Html,
            Image,
        }

    }
}
