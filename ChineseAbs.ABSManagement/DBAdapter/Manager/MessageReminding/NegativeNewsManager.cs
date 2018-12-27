
using ABSMgrConn;
using ChineseAbs.ABSManagement.Framework;
using ChineseAbs.ABSManagement.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.MessageReminding
{
    public class NegativeNewsManager
        : BaseModelManager<NegativeNews, ABSMgrConn.TableNegativeNews>
    {
        DBAdapter m_dbAdapter;
        public NegativeNewsManager()
        {
            m_defaultTableName = "dbo.NegativeNews";
            m_defaultPrimaryKey = "negative_news_id";
            m_defalutFieldPrefix = "negative_news_";
            m_dbAdapter = new DBAdapter();
        }

        public NegativeNews New(int projectId, string currentUsername)
        {
            var negativeNews = new NegativeNews();

            if (GetByProjIdUser(projectId, currentUsername) == null)
            {
                negativeNews.Guid = Guid.NewGuid().ToString();
                negativeNews.ProjectId = projectId;
                negativeNews.Username = currentUsername;
                negativeNews.SubscribeTime = DateTime.Now;
                negativeNews = New(negativeNews);
            }
            return negativeNews;
        }

        public NegativeNews GetByProjIdUser(int projectId, string currentUsername)
        {
            var negativeNews = m_db.Query<TableNegativeNews>(new Sql("where [project_id] =@0 and username=@1 and  [record_status_id]!=@2", projectId, currentUsername, Models.RecordStatus.Deleted)).Select(x => new NegativeNews(x)).FirstOrDefault();
            return negativeNews;
        }

        public bool HasNegativeNews(int projectId, string currentUsername)
        {
            return GetByProjIdUser(projectId, currentUsername)!=null;
        }

        public bool CheckProjectsPermission(List<string> projectGuids)
        {
            var allHasPermission = true;
            projectGuids.ForEach(x =>
           {
               if (!CheckProjectPermission(x))
               {
                   allHasPermission = false;
                   return;
               }
           });
            return allHasPermission;
        }

        public bool CheckProjectPermission(string projectGuid)
        {
            var pm = new ProjectManager();
            var projectId = pm.GetProjectByGuid(projectGuid).ProjectId;
            return m_dbAdapter.Authority.GetAuthorizedProjectIds(Platform.UserName).Contains(projectId);
        }

    }
}
