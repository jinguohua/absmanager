using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Manager.AssetDefault
{
    public class AssetDefaultSetManager : BaseManager
    {
        public AssetDefaultSetManager()
        {
            m_defaultTableName = "dbo.AssetDefaultSet";
            m_defaultPrimaryKey = "asset_default_set_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public AssetDefaultSet New(int projectId, DateTime paymentDate, string name)
        {
            var now = DateTime.Now;

            var assetDefaultSet = new AssetDefaultSet();
            assetDefaultSet.Guid = Guid.NewGuid().ToString();

            assetDefaultSet.ProjectId = projectId;
            assetDefaultSet.PaymentDate = paymentDate;
            assetDefaultSet.Name = name;

            assetDefaultSet.CreateTime = now;
            assetDefaultSet.CreateUserName = UserInfo.UserName;
            assetDefaultSet.LastModifyTime = now;
            assetDefaultSet.LastModifyUserName = UserInfo.UserName;
            assetDefaultSet.RecordStatus = RecordStatus.Valid;
            assetDefaultSet.Id = Insert(assetDefaultSet.GetTableObject());
            return assetDefaultSet;
        }

        public AssetDefaultSet GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableAssetDefaultSet>("asset_default_set_guid", guid);
            return new AssetDefaultSet(record);
        }

        public AssetDefaultSet GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableAssetDefaultSet>(id);
            return new AssetDefaultSet(record);
        }
    }
}
