using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSMgrDeployment
{
    public class DeployUtils
    {
        public void Initialize(string destWebSiteFolder)
        {
            Common.Log(Environment.NewLine + Environment.NewLine + "初始化程序...");

            m_rootPath = System.Windows.Forms.Application.StartupPath;
            m_sqlFolder = Path.Combine(m_rootPath, "SQL");
            m_webSiteFolder = Path.Combine(m_rootPath, "WebSite");

            m_oldWebSiteFolder = destWebSiteFolder;
            m_oldWebConfigPath = Path.Combine(m_oldWebSiteFolder, "web.config");

            m_backupWebSiteFolder = Path.Combine(destWebSiteFolder, @"..\ABSManager-backup");

            Common.Assert(Directory.Exists(m_sqlFolder), "找不到SQL文件目录:" + m_sqlFolder);
            Common.Assert(Directory.Exists(m_webSiteFolder), "找不到新网站目录:" + m_webSiteFolder);
            Common.Assert(Directory.Exists(m_oldWebSiteFolder), "找不到待更新网站目录:" + m_oldWebSiteFolder);
            Common.Assert(File.Exists(m_oldWebConfigPath), "找不到待更新网站中的配置文件:" + m_oldWebConfigPath);

            CheckBackupFolder();
            ParseDBConfig();
            GetVersion();
        }

        private void CheckBackupFolder()
        {
            if (!Directory.Exists(m_backupWebSiteFolder))
            {
                try
                {
                    Directory.CreateDirectory(m_backupWebSiteFolder);
                }
                catch (Exception e)
                {
                    var errorMsg = "创建备份文件夹失败，请检查是否有创建权限和目录是否正确：" + m_backupWebSiteFolder;
                    Common.Assert(e, errorMsg);
                }
            }
        }

        private void ParseDBConfig()
        {
            var webConfigMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = m_oldWebConfigPath
            };
            var webConfig = ConfigurationManager.OpenMappedExeConfiguration(webConfigMap, ConfigurationUserLevel.None);
            m_setting = webConfig.ConnectionStrings.ConnectionStrings["ABSMgrConn"];
            Common.Assert(m_setting != null, "旧版本网站配置文件中，找不到ABSMgrConn节点");

            Common.Log("检测到数据库连接：Name=" + m_setting.Name
                + ";ConnectionString=" + m_setting.ConnectionString
                + ";ProviderName=" + m_setting.ProviderName);
        }

        private void GetVersion()
        {
            var oldDllPath = Path.Combine(m_oldWebSiteFolder, @"bin\ChineseAbs.ABSManagementSite.dll");
            Common.Assert(File.Exists(oldDllPath), "旧版本网站中，找不到文件：" + oldDllPath);
            m_oldWebSiteVer = FileVersionInfo.GetVersionInfo(oldDllPath).FileVersion;
            Common.Log("检测到旧版本：" + m_oldWebSiteVer);

            var dllPath = Path.Combine(m_webSiteFolder, @"bin\ChineseAbs.ABSManagementSite.dll");
            Common.Assert(File.Exists(dllPath), "新版本网站中，找不到文件：" + dllPath);
            m_webSiteVer = FileVersionInfo.GetVersionInfo(dllPath).FileVersion;
            Common.Log("检测到新版本：" + m_webSiteVer);
        }

        private bool BackupOldWebSite()
        {
            Common.Log("正在备份旧版本网站...");

            var fileName = "ABSManager " + m_oldWebSiteVer + DateTime.Now.ToString(" yyyy-MM-dd HH.mm.ss ") + ".zip";
            var filePathName = Path.Combine(m_backupWebSiteFolder, fileName);
            try
            {
                Common.ZipFileFromDirectory(m_oldWebSiteFolder, filePathName, 3);
            }
            catch (Exception e)
            {
                var errorMsg = "备份旧版本网站失败：" + filePathName;
                Common.Assert(e, errorMsg);
                return false;
            }

            Common.Log("旧版本网站备份成功");
            return true;
        }

        private bool ExecuteSQL()
        {
            Common.Log("正在同步数据库结构...");
            try
            {
                SqlConnection sqlConn = new SqlConnection(m_setting.ConnectionString);
                sqlConn.Open();

                var subfilePaths = Directory.GetFiles(m_sqlFolder);
                subfilePaths = subfilePaths.Where(x => x.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                foreach (string filePath in subfilePaths)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    Common.Log("正在执行：" + fileInfo.Name);
                    string script = File.ReadAllText(filePath);
                    script = script.Replace("GO\r\n", "")
                        .Replace("GO\r", "")
                        .Replace("GO\n", "");

                    SqlCommand command = new SqlCommand(script, sqlConn);
                    command.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
            catch (Exception e)
            {
                Common.Log("同步数据库结构失败", e);
                return false;
            }

            Common.Log("同步数据库结构成功");
            return true;
        }

        private bool UpdateNewWebSite()
        {
            Common.Log("正在部署新网站...");
            try
            {
                var ignoreFiles = new List<string> { "web.config" };
                Common.CopyDir(m_webSiteFolder, m_oldWebSiteFolder, ignoreFiles);
            }
            catch (Exception e)
            {
                Common.Log("新网站部署失败", e);
                return false;
            }

            Common.Log("新网站部署成功");
            return true;
        }

        public void Deploy()
        {
            Common.Log("启动自动部署...");

            //备份旧WebSite
            if (!BackupOldWebSite())
            {
                return;
            }

            //在目标数据库中，执行SQL
            if (!ExecuteSQL())
            {
                return;
            }

            //复制新WebSite到旧WebSite中
            if (!UpdateNewWebSite())
            {
                return;
            }

            Common.Log("自动部署结束");
        }

        public string GetOldWebSiteFolder()
        {
            return m_oldWebSiteFolder;
        }

        public string GetDBSetting()
        {
            return "Name=" + m_setting.Name
                + ";ConnectionString=" + m_setting.ConnectionString
                + ";ProviderName=" + m_setting.ProviderName;
        }

        public string GetBackupWebSiteFolder()
        {
            return m_backupWebSiteFolder;
        }

        public string GetOldWebSiteVer()
        {
            return m_oldWebSiteVer;
        }

        public string GetNewWebSiteVer()
        {
            return m_webSiteVer;
        }

        //新网站/SQL配置
        private string m_rootPath;
        private string m_sqlFolder;
        private string m_webSiteFolder;
        private string m_webSiteVer;

        //备份网站地址
        private string m_backupWebSiteFolder;

        //待更新网站配置
        private string m_oldWebSiteFolder;
        private string m_oldWebConfigPath;
        private string m_oldWebSiteVer;
        private ConnectionStringSettings m_setting;
    }
}
