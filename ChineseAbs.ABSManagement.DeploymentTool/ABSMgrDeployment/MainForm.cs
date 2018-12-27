using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ABSMgrDeployment
{
    public partial class MainForm : Form
    {
        private readonly static string assemblyConfigFile = Assembly.GetEntryAssembly().Location;

        Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);

        private void UpdateAppConfig(string strKey, string value)
        {
            var node = config.AppSettings.Settings[strKey];
            if (node != null)
            {
                config.AppSettings.Settings.Remove(strKey);
            }

            config.AppSettings.Settings.Add(strKey, value);
            config.Save(ConfigurationSaveMode.Modified);
        }

        private string LoadAppConfig(string key)
        {
            var setting = config.AppSettings.Settings[key];
            if (setting == null)
            {
                return null;
            }

            return setting.Value;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonStartDeployment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxDestinationWebsitePath.Text))
            {
                MessageBox.Show("请选择目标网站。");
                return;
            }

            UpdateAppConfig(m_configKeyWebSitePath, textBoxDestinationWebsitePath.Text);

            m_deployUtils = new DeployUtils();

            try
            {
                m_deployUtils.Initialize(textBoxDestinationWebsitePath.Text);
            }
            catch (ApplicationException exception)
            {
                Common.Log(exception);
                MessageBox.Show(exception.Message);
                return;
            }

            var message = "请确认核对以下信息：" + Environment.NewLine;
            message += "  目标网站版本：" + m_deployUtils.GetOldWebSiteVer() + Environment.NewLine;
            message += "  新网站版本：" + m_deployUtils.GetNewWebSiteVer() + Environment.NewLine + Environment.NewLine;
            message += "  目标网站路径：" + m_deployUtils.GetOldWebSiteFolder() + Environment.NewLine + Environment.NewLine;
            message += "  网站备份路径：" + m_deployUtils.GetBackupWebSiteFolder() + Environment.NewLine + Environment.NewLine;
            message += "  目标数据库：" + m_deployUtils.GetDBSetting() + Environment.NewLine + Environment.NewLine;
            message += "是否继续部署？";
            if (MessageBox.Show(message, "确认部署", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                Common.Log("用户取消部署");
                return;
            }

            Common.Log("用户确认部署");
            m_deployUtils.Deploy();
        }

        private void buttonDestinationWebsitePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            textBoxDestinationWebsitePath.Text = dialog.SelectedPath;
        }

        private DeployUtils m_deployUtils;

        private void MainForm_Load(object sender, EventArgs e)
        {
            Common.Init(richTextBoxLog);

            var path = LoadAppConfig(m_configKeyWebSitePath);
            if (!string.IsNullOrEmpty(path))
            {
                textBoxDestinationWebsitePath.Text = path;
            }
        }

        private static readonly string m_configKeyWebSitePath = "webSitePath";

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Text = string.Empty;
        }
    }
}
