using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }

        public string Username { get; set; }

        public string DocumentName { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocumentTypeName { get; set; }

        public int Version { get; set; }

        public string UploadTime { get; set; }
    }

    public class DocumentHistoryViewModel
    {
        public int DocumentId { get; set; }

        public string Username { get; set; }

        public string UploadTime { get; set; }

        public int Version { get; set; }

        public string DocumentTypeName { get; set; }

        public string Comment { get; set; }
    }

    public class DocumentManagerViewModel
    {
        public List<object> Projects { get; set; }
        public List<object> DocumentTypes { get; set; }
    }
}
