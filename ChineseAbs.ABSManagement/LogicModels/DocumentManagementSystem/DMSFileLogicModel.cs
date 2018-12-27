using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DMSFileLogicModel : BaseLogicModel
    {
        public DMSFileLogicModel(ProjectLogicModel projectLogicModel)
            : base(projectLogicModel.UserName, projectLogicModel)
        {
            AllVerFiles = new List<DMSFile>();
        }

        public List<DMSFile> AllVerFiles { get; set; }

        public DMSFileSeries FileSeries { get; set; }

        public DMSFile LatestVerFile { get { return AllVerFiles.First(); } }

        public DMSFile OriginVerFile { get { return AllVerFiles.Last(); } }
    }
}
