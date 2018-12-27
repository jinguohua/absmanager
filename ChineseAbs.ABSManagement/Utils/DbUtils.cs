using System;
using System.Linq;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Utils
{
    public class DbUtils
    {
        public static string GetGuidById(int id, string tableName, string idColumnName)
        {
            var db = ABSMgrConnDB.GetInstance();
            if (!idColumnName.EndsWith("_id"))
            {
                throw new ApplicationException("id column name doesn't match the rule: ends with _id");
            }
            var guidColumn = idColumnName.Substring(0, idColumnName.Length - 3) + "_guid";
            var rt = db.FirstOrDefault<string>("select top 1 " + guidColumn + " from "
                + tableName
                + " where "
                + idColumnName + " = @0"
                , id);
            return rt;
        }

        public static int GetIdByGuid(string guid, string tableName, string guidColumnName)
        {
            guidColumnName = guidColumnName.ToLower();
            var db = ABSMgrConnDB.GetInstance();
            if (!guidColumnName.EndsWith("_guid"))
            {
                throw new ApplicationException("guid column name doesn't match the rule: ends with _guid");
            }
            var idColumn = guidColumnName.Substring(0, guidColumnName.Length - 5) + "_id";
            var rt = db.FirstOrDefault<int>("select top 1 " + idColumn + " from "
                + tableName
                + " where "
                + guidColumnName + " = @0"
                , guid);
            return rt;
        }

        public static string CamelCaseToUnderScore(string str)
        {
            string result = string.Concat(str.Select((x, i) => i > 0 
                && (char.IsUpper(x) || (char.IsNumber(x) 
                && !char.IsNumber(str[i - 1]))) 
                ? "_" + x.ToString() : x.ToString()));

            return result.ToLower();
        }
    }
}
