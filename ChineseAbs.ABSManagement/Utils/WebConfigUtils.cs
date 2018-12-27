using System.Configuration;
using System.IO;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class WebConfigUtils
    {
        public static string RootFolder = GetSetting("Root_Folder");

        public static string DocumentFolderPath = Path.Combine(RootFolder, "Document");

        public static bool LocalDeployed
        {
            get
            {
                bool isLocal = false;
                var localDeployedSetting = GetSetting("LocalDeployed");
                if (localDeployedSetting != null)
                {
                    bool.TryParse(localDeployedSetting, out isLocal);
                }
                return isLocal;
            }
        }

        public static string WatermarkTitle
        {
            get
            {
                var prefix = GetSetting("WatermarkTitle");
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    prefix = "www.cn-abs.com";
                }
                return prefix;
            }
        }

        public static string RsaPublicKey = GetSetting("RsaPublicKey");

        public static string RsaPrivateKey = GetSetting("RsaPrivateKey");


        public static string PatternFileFolder = System.IO.Path.Combine(RootFolder, "Document", "FilePattern");

        public static string EnterpriseName = GetSetting("EnterpriseName");

        

        public static string RepositoryFilePath = Path.Combine(RootFolder, "Repository", "File");

        public static string RepositoryImagePath = Path.Combine(RootFolder, "Repository", "Image");

        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
