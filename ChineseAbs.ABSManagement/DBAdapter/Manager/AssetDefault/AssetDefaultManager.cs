using System;
using ChineseAbs.ABSManagement.Models;
using AssetDefaultModel = ChineseAbs.ABSManagement.Models.AssetDefault;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.AssetDefault
{
    public class AssetDefaultManager : BaseManager
    {
        public AssetDefaultManager()
        {
            m_defaultTableName = "dbo.AssetDefault";
            m_defaultPrimaryKey = "asset_default_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public AssetDefaultModel New(int assetDefaultSetId, int assetId, DateTime assetDefaultDate,
            double recoveryRate, double recoveryLag, DateTime recoveryDate)
        {
            var now = DateTime.Now;

            var assetDefault = new AssetDefaultModel();
            assetDefault.Guid = Guid.NewGuid().ToString();

            assetDefault.AssetDefaultSetId = assetDefaultSetId;
            assetDefault.AssetId = assetId;
            assetDefault.AssetDefaultDate = assetDefaultDate;
            assetDefault.RecoveryRate = recoveryRate;
            assetDefault.RecoveryLag = recoveryLag;
            assetDefault.RecoveryDate = recoveryDate;

            assetDefault.CreateTime = now;
            assetDefault.CreateUserName = UserInfo.UserName;
            assetDefault.LastModifyTime = now;
            assetDefault.LastModifyUserName = UserInfo.UserName;
            assetDefault.RecordStatus = RecordStatus.Valid;
            assetDefault.Id = Insert(assetDefault.GetTableObject());
            return assetDefault;
        }

        public AssetDefaultModel GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableAssetDefault>("asset_default_guid", guid);
            return new AssetDefaultModel(record);
        }

        public List<AssetDefaultModel> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableAssetDefault, string>("asset_default_guid", guids);
            return records.ToList().ConvertAll(x => new AssetDefaultModel(x));
        }

        public List<AssetDefaultModel> GetByAssetDefaultSetId(int assetDefaultSetId)
        {
            var records = Select<ABSMgrConn.TableAssetDefault>("asset_default_set_id", assetDefaultSetId);
            return records.ToList().ConvertAll(x => new AssetDefaultModel(x));
        }

        public int Remove(AssetDefaultModel assetDefault)
        {
            assetDefault.RecordStatus = RecordStatus.Deleted;
            assetDefault.LastModifyTime = DateTime.Now;
            assetDefault.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, assetDefault.GetTableObject());
        }

        public AssetDefaultModel GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableAssetDefault>(id);
            return new AssetDefaultModel(record);
        }
    }
}
