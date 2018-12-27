using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class DocumentType : BaseDataContainer<TableDocumentType>
    {
        public DocumentType(TableDocumentType documentType) : base(documentType)
        { }

        public int DocumentTypeId { get; set; }

        public string TypeName { get; set; }

        public override void FromTableObject(TableDocumentType obj)
        {
            DocumentTypeId = obj.id;
            TypeName = obj.name;
        }

        public override TableDocumentType GetTableObject()
        {
            TableDocumentType documentType = new TableDocumentType {
                id = DocumentTypeId,
                name = TypeName
            };
            return documentType;
        }
    }
}
