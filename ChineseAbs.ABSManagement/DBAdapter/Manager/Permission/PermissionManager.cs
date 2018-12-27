using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public class PermissionManager : BaseManager
    {
        public PermissionManager()
        {
            m_defaultTableName = "dbo.Permission";
            m_defaultPrimaryKey = "permission_id";
            m_defaultOrderBy = "";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public Permission NewPermission(string userName, string objectUid, PermissionObjectType objectType, PermissionType type)
        {
            var permission = new Permission() {
                UserName = userName,
                ObjectUniqueIdentifier = objectUid,
                Type = type,
                ObjectType = objectType
            };

            return NewPermission(permission);
        }

        public Permission NewPermission(Permission permission)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name = @0 and permission_object_unique_identifier = @1 and permission_type = @2",
                permission.UserName, permission.ObjectUniqueIdentifier, (int)permission.Type);

            if (records.Count > 0)
            {
                return new Permission(records.First());
            }

            var newId = Insert(permission.GetTableObject());
            permission.Id = newId;
            return permission;
        }

        public Permission GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TablePermission>(id);
            return new Permission(record);
        }

        public List<Permission> GetByIds(IEnumerable<int> ids)
        {
            var records = Select<ABSMgrConn.TablePermission, int>(ids);
            return records.ToList().ConvertAll(x => new Permission(x));
        }

        public List<Permission> GetByObjectUid(string objectUid, PermissionObjectType objectType, PermissionType type)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where permission_object_unique_identifier = @0 and permission_object_type_id = @1 and permission_type = @2",
                objectUid, (int)objectType, (int)type);
            return records.ToList().ConvertAll(x => new Permission(x));
        }

        public List<Permission> GetByObjectUid(string objectUid)
        {
            var records = Select<ABSMgrConn.TablePermission>("permission_object_unique_identifier", objectUid);
            return records.ToList().ConvertAll(x => new Permission(x));
        }

        public List<Permission> GetByObjectUid(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TablePermission, string>("permission_object_unique_identifier", guids);
            return records.ToList().ConvertAll(x => new Permission(x));
        }

        public List<string> GetObjectUids(string userName, PermissionObjectType objectType, PermissionType type)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name = @0 and permission_object_type_id = @1 and permission_type = @2",
                userName, (int)objectType, (int)type);
            return records.ConvertAll(x => x.permission_object_unique_identifier);
        }

        public List<Permission> GetAllPermission(string userName, string objectUid)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name = @0 and permission_object_unique_identifier = @1",
                userName, objectUid);
            return records.ConvertAll(x => new Permission(x));
        }

        public List<Permission> GetAllPermission(List<string> userNameList, List<string> objectUid)
        {
            if (userNameList.Count == 0 || objectUid.Count == 0)
            {
                return new List<Permission>();
            }

            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name IN (@userNameList) and permission_object_unique_identifier IN (@objectUid)",
                new { userNameList, objectUid });
            var permissions = records.ConvertAll(x => new Permission(x));
            return permissions;
        }

        public Dictionary<string, List<Permission>> GetAllPermission(List<string> userNameList, string objectUid)
        {
            if (userNameList.Count == 0)
            {
                return new Dictionary<string, List<Permission>>();
            }

            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name IN (@userNameList) and permission_object_unique_identifier = @objectUid",
                new { userNameList, objectUid });
            var permissions = records.ConvertAll(x => new Permission(x));
            
            var dict = new Dictionary<string, List<Permission>>();
            foreach (var permission in permissions)
            {
                if (!dict.Keys.Contains(permission.UserName, StringComparer.OrdinalIgnoreCase))
                {
                    dict[permission.UserName.ToLower()] = new List<Permission>();
                }
                dict[permission.UserName.ToLower()].Add(permission);
            }

            return dict;
        }

        public List<String> GetAllUserNameByUid(string objectUid)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT DISTINCT user_name FROM " + m_defaultTableName
                + " where permission_object_unique_identifier = @0",
                objectUid);

            return records.ConvertAll(x => x.user_name);
        }

        public Permission GetPermission(string userName, string objectUid, PermissionType type)
        {
            var records = m_db.Fetch<ABSMgrConn.TablePermission>(
                "SELECT * FROM " + m_defaultTableName
                + " where user_name = @0 and permission_object_unique_identifier = @1 and permission_type = @2",
                userName, objectUid, (int)type);

            if (records.Count > 0)
            {
                return new Permission(records.First());
            }
            return null;
        }

        public int DeletePermission(Permission permission)
        {
            return m_db.Delete(m_defaultTableName, m_defaultPrimaryKey, permission.GetTableObject());
        }

        public int DeletePermission(List<Permission> permissions)
        {
            if (permissions.Count == 0)
            {
                return 0;
            }

            CommUtils.Assert(permissions.Select(x => x.ObjectType).Distinct().Count() == 1,
                "不能一次删除不同权限类型的数据");

            var permissionIds = permissions.Select(x => x.Id);
            var sql = "delete from " + m_defaultTableName + " where " + m_defaultPrimaryKey + " in (@permissionIds)";
            return m_db.Execute(sql, new { permissionIds = permissionIds });
        }

        public bool HasPermission(Permission permission)
        {
            return HasPermission(permission.UserName, permission.ObjectUniqueIdentifier, permission.Type);
        }

        public bool HasPermission(string userName, string objectUid, PermissionType type)
        {
            var records = GetPermission(userName, objectUid, type);
            return records != null;
        }
    }
}
