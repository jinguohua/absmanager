using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class LocalDeployedUserProfile: BaseDataContainer<TableLocalDeployedUserProfile>
    {
        public LocalDeployedUserProfile()
        {
        }

        public LocalDeployedUserProfile(TableLocalDeployedUserProfile obj)
            : base(obj)
        {

        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        public string Email { get; set; }

        public string Cellphone { get; set; }

        public override TableLocalDeployedUserProfile GetTableObject()
        {
            var obj = new TableLocalDeployedUserProfile();
            obj.user_profile_id = Id;
            obj.user_profile_guid = Guid;
            obj.user_name = UserName;
            obj.real_name = RealName;
            obj.company = Company;
            obj.department = Department;
            obj.email = Email;
            obj.cellphone = Cellphone;
            return obj;
        }

        public override void FromTableObject(TableLocalDeployedUserProfile obj)
        {
            Id = obj.user_profile_id;
            Guid = obj.user_profile_guid;
            UserName = obj.user_name;
            RealName = obj.real_name;
            Company = obj.company;
            Department = obj.department;
            Email = obj.email;
            Cellphone = obj.cellphone;
        }
    }
}
