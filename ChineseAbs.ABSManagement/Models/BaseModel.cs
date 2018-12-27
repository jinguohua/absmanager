using System;

namespace ChineseAbs.ABSManagement.Models
{
    public abstract class BaseModel<T> : BaseDataContainer<T>
    {
        public BaseModel(T obj)
            : base(obj)
        {
        }

        public BaseModel()
            : base()
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }
    }
}
