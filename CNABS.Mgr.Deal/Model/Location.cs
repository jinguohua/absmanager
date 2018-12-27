using ChineseAbs.ABSManagement.Utils;
using System.IO;

namespace CNABS.Mgr.Deal.Model
{
    public class Location
    {
        public Location(string ymlFolder, string dsFolder)
        {
            YmlFolder = ymlFolder;
            DsFolder = dsFolder;
            Yml = Path.Combine(ymlFolder, "Script.yml");
            FutureVarCsv = Path.Combine(dsFolder, "FutureVariables.csv");
            CurrentVarCsv = Path.Combine(dsFolder, "CurrentVariables.csv");
            PastVarCsv = Path.Combine(dsFolder, "PastVariables.csv");
            CollateralCsv = Path.Combine(dsFolder, "collateral.csv");
            CombinedVarCsv = Path.Combine(dsFolder, "CombinedVariables.csv");
        }

        public void Validate()
        {
            CommUtils.AssertExistFolder(YmlFolder);
            CommUtils.AssertExistFolder(DsFolder);
            CommUtils.AssertExistFile(Yml);

            try
            {
                CommUtils.AssertExistFile(CombinedVarCsv);
            }
            catch
            {
                CommUtils.AssertExistFile(FutureVarCsv);
                CommUtils.AssertExistFile(CurrentVarCsv);
                CommUtils.AssertExistFile(PastVarCsv);
            }

            
            CommUtils.AssertExistFile(CollateralCsv);
        }

        public string FutureVarCsv { get; private set; }

        public string CurrentVarCsv { get; private set; }

        public string PastVarCsv { get; private set; }

        public string CollateralCsv { get; private set; }

        public string YmlFolder { get; private set; }

        public string Yml { get; private set; }

        public string DsFolder { get; private set; }

        public string CombinedVarCsv { get; set; }
    }
}
