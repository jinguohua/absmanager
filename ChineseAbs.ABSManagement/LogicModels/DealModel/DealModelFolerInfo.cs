
namespace ChineseAbs.ABSManagement.LogicModels.DealModel
{
    public class DealModelFolerInfo
    {
        public DealModelFolerInfo(string ymlFolder, string dsFolder)
        {
            YmlFolder = ymlFolder;
            DsFolder = dsFolder;
        }

        public string YmlFolder { get; set; }

        public string DsFolder { get; set; }
    }
}
