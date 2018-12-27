using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class EditProductAuthority : BaseDataContainer<TableEditProductAuthority>
    {
        public EditProductAuthority()
        {
        }

        public EditProductAuthority(TableEditProductAuthority editProductAuthority)
            : base(editProductAuthority)
        {
        }

        public int productEditAuthorityId { set; get; }

        public string Username { set; get; }

        public int CreateProductAuthority { set; get; }

        public int ModifyModelAuthority { set; get; }

        public int ModifyTaskAuthority { set; get; }

        public int ModifyProjectId { set; get; }

        public override TableEditProductAuthority GetTableObject()
        {
            var editProductAuthority = new TableEditProductAuthority();
            editProductAuthority.product_edit_authority_id = productEditAuthorityId;
            editProductAuthority.user_name = Username;
            editProductAuthority.create_product_authority = CreateProductAuthority;
            editProductAuthority.modify_model_authority = ModifyModelAuthority;
            editProductAuthority.modify_task_authority = ModifyTaskAuthority;
            editProductAuthority.modify_project_id = ModifyProjectId;
            return editProductAuthority;
        }

        public override void FromTableObject(TableEditProductAuthority editProductAuthority)
        {
            productEditAuthorityId = editProductAuthority.product_edit_authority_id;
            Username = editProductAuthority.user_name;
            CreateProductAuthority = editProductAuthority.create_product_authority;
            ModifyModelAuthority = editProductAuthority.modify_model_authority;
            ModifyTaskAuthority = editProductAuthority.modify_task_authority;
            ModifyProjectId = editProductAuthority.modify_project_id.Value;
        }
    }
}
