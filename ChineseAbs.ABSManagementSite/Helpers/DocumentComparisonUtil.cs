using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Aspose.Words;
using Aspose.Words.Layout;
using Aspose.Words.Saving;
using ChineseAbs.ABSManagementSite.Models;

namespace ChineseAbs.ABSManagementSite.Helpers
{
    public class DocumentComparisonUtil
    {
        public static DocCompareModel CompareDocuments(string originalDocumentPath, string currentDocumentPath)
        {
            var original = new Document(originalDocumentPath);   
            var current = new Document(currentDocumentPath);
            return Compare(original, current);
        }

        public static DocCompareModel CompareDocuments(Stream originalDocument, Stream currentDocument)
        {
            var original = new Document(originalDocument);
            var current = new Document(currentDocument);
            return Compare(original, current);
        }

        private static DocCompareModel Compare(Document original, Document current)
        {
            var docCompareModel = new DocCompareModel();

            var pages = new List<DocumentPage>();

            original.Compare(current, "Apose", DateTime.Now);
            LayoutCollector layout = new LayoutCollector(original);

            foreach (Revision revision in original.Revisions)
            {
                if (revision.RevisionType == RevisionType.Insertion)
                {
                    var index = layout.GetStartPageIndex(revision.ParentNode);

                    if (index == 0) continue;

                    if (pages.Where(i => i.PageNumber == index).ToList().Any())
                    {
                        var page = pages.Find(i => i.PageNumber == index);
                        page.Added++;
                    }
                    else
                    {
                        pages.Add(new DocumentPage
                        {
                            PageNumber = index,
                            Added = 1
                        });
                    }
                    continue;
                }

                if (revision.RevisionType == RevisionType.Deletion)
                {
                    var index = layout.GetStartPageIndex(revision.ParentNode);

                    if (index == 0) continue;

                    if (pages.Where(i => i.PageNumber == index).ToList().Any())
                    {
                        var page = pages.Find(i => i.PageNumber == index);
                        page.Deleted++;
                    }
                    else
                    {
                        pages.Add(new DocumentPage
                        {
                            PageNumber = index,
                            Deleted = 1
                        });
                    }
                }
            }
            docCompareModel.Pages = pages;

            using (var stream = new MemoryStream())
            {
                original.Save(stream, SaveFormat.Docx);
                var doc = new Document(stream);

                ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Jpeg) { PageCount = 1 };
                var urls = new Dictionary<int, string>();
                docCompareModel.PageCount = doc.PageCount;

                for (int i = 0; i < doc.PageCount; i++)
                {
                    options.PageIndex = i;
                    using (var imgStream = new MemoryStream())
                    {
                        doc.Save(imgStream, options);
                        var base64Image = new Base64Image
                        {
                            FileContents = imgStream.ToArray(),
                            ContentType = "image/png"
                        };

                        urls.Add(i, base64Image.ToString());
                    }
                }
                docCompareModel.PageImages = urls;
            }
            return docCompareModel;
        }
    
    }
}