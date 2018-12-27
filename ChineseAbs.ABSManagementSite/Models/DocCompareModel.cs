using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class DocCompareModel
    {
        public int PageCount { get; set; }

        public Dictionary<int, string> PageImages { get; set; }

        public List<DocumentPage> Pages { get; set; }
    }

    public class DocumentPage
    {
        public int PageNumber { get; set; }

        public int Added { get; set; }

        public int Deleted { get; set; }
    }

    public class Base64Image
    {
        public string ContentType { get; set; }

        public byte[] FileContents { get; set; }

        public override string ToString()
        {
            return string.Format("data:{0};base64,{1}", ContentType, Convert.ToBase64String(FileContents));
        }
    }
}