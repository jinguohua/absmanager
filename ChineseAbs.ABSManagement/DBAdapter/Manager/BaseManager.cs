using ABS.Core.Services;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public abstract class BaseManager : SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        public BaseManager()
        {
            m_logger = GetLogger();
        }

        public UserService UserService { get; set; }

        protected ABSMgrConn.ABSMgrConnDB m_db
        {
            get { return ABSMgrConn.ABSMgrConnDB.GetInstance(); }
        }

        protected int Insert(object recordObject)
        {
            return Insert(m_defaultTableName, m_defaultPrimaryKey, recordObject);
        }

        protected int Insert(string tableName, string key, object recordObject)
        {
            var newId = m_db.Insert(tableName, key, true, recordObject);
            return (int)newId;
        }

        protected T SelectSingle<T>(object value)
        {
            return SelectSingle<T>(m_defaultTableName, m_defaultPrimaryKey, value);
        }

        protected T SelectSingle<T>(string key, object value)
        {
            return SelectSingle<T>(m_defaultTableName, key, value);
        }

        protected T SelectSingle<T>(string tableName, string key, object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                CommUtils.Assert(false, "SelectSingle传入参数错误，key={0},value=null", key);
            }

            var records = Select<T>(tableName, key, value);

            if (records.Count() == 0)
            {
                var errorMsg = string.Format("获取 {0} [{1}={2}] 失败, 找不到相关数据，请刷新后再试。",
                    tableName, key, value.ToString());
                throw new ApplicationException(errorMsg);
            }
            else if (records.Count() > 1)
            {
                var errorMsg = string.Format("Get {0} [{1}={2}] failed, multiply records have been found.",
                    tableName, key, value.ToString());
                throw new ApplicationException(errorMsg);
            }

            return records.Single();
        }

        protected IEnumerable<T> Select<T>(string tableName, string key, object value)
        {
            IEnumerable<T> records = null;

            var sql = "SELECT * FROM " + tableName + " where " + key + " = @0";
            if (m_defaultHasRecordStatusField)
            {
                sql += " AND record_status_id <> @1";
                sql += m_defaultOrderBy;
                records = m_db.Query<T>(sql, value, (int)RecordStatus.Deleted);
            }
            else
            {
                sql += m_defaultOrderBy;
                records = m_db.Query<T>(sql, value);
            }

            return records;
        }

        protected IEnumerable<T> Select<T>(string key, object value)
        {
            return Select<T>(m_defaultTableName, key, value);
        }

        protected IEnumerable<T> Select<T, TKey>(IEnumerable<TKey> values)
        {
            return Select<T, TKey>(m_defaultTableName, m_defaultPrimaryKey, values, m_defaultOrderBy);
        }

        protected IEnumerable<T> Select<T, TKey>(string key, IEnumerable<TKey> values)
        {
            return Select<T, TKey>(m_defaultTableName, key, values, m_defaultOrderBy);
        }

        protected IEnumerable<T> Select<T, TKey>(string tableName, string key, IEnumerable<TKey> values, string orderBy)
        {
            if (values.Count() == 0)
            {
                return new List<T>();
            }

            var sql = "SELECT * FROM " + tableName + " WHERE " + key + " IN (@values) ";

            IEnumerable<T> records = null;
            if (m_defaultHasRecordStatusField)
            {
                sql += " AND record_status_id <> @recordStatusDeleted";
                sql += orderBy;
                var recordStatusDeleted = (int)RecordStatus.Deleted;
                records = m_db.Query<T>(sql, new { values, recordStatusDeleted });
            }
            else
            {
                sql += orderBy;
                records = m_db.Query<T>(sql, new { values });
            }

            return records;
        }

        protected bool Exists<T>(string tableName, string key, object value)
        {
            return Select<T>(tableName, key, value).Count() > 0;
        }

        protected bool Exists<T>(string key, object value)
        {
            return Select<T>(key, value).Count() > 0;
        }

        protected abstract AbstractLogger GetLogger();

        protected UserInfo UserInfo
        {
            get
            {
                return new UserInfo() { UserName = SAFS.Core.Context.ApplicationContext.Current.Operator.Name };
            }
        }

        protected AbstractLogger m_logger;

        //当该Manager表示一个具体Table时，缺省TableName、PrimaryKey、OrderBy
        protected string m_defaultTableName;
        protected string m_defaultPrimaryKey;
        protected string m_defaultOrderBy;
        protected bool m_defaultHasRecordStatusField;
    }

    public enum ELoggerType
    {
        Generic,
        UserLogin,
        UserLogout,
        Account
    }
}
