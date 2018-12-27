using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public abstract class BaseModelManager<T, TBase> : BaseManager where T : BaseModel<TBase>, new()
    {
        public BaseModelManager()
        {
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        private T Convert(TBase baseObj)
        {
            var obj = new T();
            obj.FromTableObject(baseObj);
            return obj;
        }

        public T GetById(int id)
        {
            var record = SelectSingle<TBase>(id);
            return Convert(record);
        }

        public List<T> GetByIds(IEnumerable<int> ids)
        {
            var records = Select<TBase, int>(ids);
            return records.ToList().ConvertAll(x => Convert(x));
        }

        public T GetByGuid(string guid)
        {
            var record = SelectSingle<TBase>(m_defalutFieldPrefix + "guid", guid);
            return Convert(record);
        }

        public List<T> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<TBase>(m_defalutFieldPrefix + "guid", guids);
            return records.Select(x => Convert(x)).ToList();
        }

        public List<T> New(IEnumerable<T> records)
        {
            return records.Select(x => New(x)).ToList();
        }

        public T New(T obj)
        {
            obj.Guid = System.Guid.NewGuid().ToString();

            var now = DateTime.Now;
            obj.CreateTime = now;
            obj.CreateUserName = UserInfo.UserName;
            obj.LastModifyTime = now;
            obj.LastModifyUserName = UserInfo.UserName;
            obj.RecordStatus = RecordStatus.Valid;

            obj.Id = Insert(obj.GetTableObject());
            return obj;
        }

        public int Update(T obj)
        {
            obj.LastModifyTime = DateTime.Now;
            obj.LastModifyUserName = UserInfo.UserName;

            var tableObj = obj.GetTableObject();
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, tableObj, obj.Id);
        }

        public int Delete(T obj)
        {
            obj.RecordStatus = RecordStatus.Deleted;
            obj.LastModifyTime = DateTime.Now;
            obj.LastModifyUserName = UserInfo.UserName;
            return Update(obj);
        }

        public int Delete(IEnumerable<T> obj)
        {
            if (obj.Count() == 0)
            {
                return 0;
            }

            var ids = string.Join(", ", obj.Select(x => x.Id).ToList());

            var sql = "UPDATE " + m_defaultTableName
                + " SET record_status_id = "
                + (int)RecordStatus.Deleted
                + " WHERE " + m_defaultPrimaryKey + " in (" + ids + ")";
            
            return m_db.Execute(sql);
        }

        protected string m_defalutFieldPrefix = string.Empty;
    }
}
