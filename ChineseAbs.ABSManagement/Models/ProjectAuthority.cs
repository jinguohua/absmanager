using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class ProjectAuthority: BaseDataContainer<TableProjectAuthority>
    {
        public ProjectAuthority()
        {
        }

        public ProjectAuthority(TableProjectAuthority project)
            : base(project)
        {

        }

        public int ProjectAuthorityId { get; set; }

        public int ProjectId { get; set; }

        public int EnterpriseId { get; set; }

        public override TableProjectAuthority GetTableObject()
        {
            var table = new TableProjectAuthority();
            table.project_authority_id = ProjectAuthorityId;
            table.project_id = ProjectId;
            table.enterprise_id = EnterpriseId;
            return table;
        }

        public override void FromTableObject(TableProjectAuthority project)
        {
            ProjectAuthorityId = project.project_authority_id;
            ProjectId = project.project_id;
            EnterpriseId = project.enterprise_id;
        }
    }
}
