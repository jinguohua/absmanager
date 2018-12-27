using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.Repository
{
    public class Image : BaseDataContainer<TableImage>
    {
        public Image()
        {
        }

        public Image(TableImage image)
            : base(image)
        {
        }

        public int Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserName { get; set; }
        public DateTime LastModifyTime { get; set; }
        public string LastModifyUserName { get; set; }

        public override void FromTableObject(TableImage image)
        {
            Id = image.image_id;
            Guid = image.image_guid;
            Name = image.name;
            Path = image.path;
            CreateTime = image.create_time;
            CreateUserName = image.create_user_name;
            LastModifyTime = image.last_modify_time;
            LastModifyUserName = image.last_modify_user_name;
        }

        public override TableImage GetTableObject()
        {
            var image = new TableImage();
            image.image_id = Id;
            image.image_guid = Guid;
            image.name = Name;
            image.path = Path;
            image.create_time = CreateTime;
            image.create_user_name = CreateUserName;
            image.last_modify_time = LastModifyTime;
            image.last_modify_user_name = LastModifyUserName;
            return image;
        }
    }
}
