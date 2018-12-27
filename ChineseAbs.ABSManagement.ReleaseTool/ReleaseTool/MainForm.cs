using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;


namespace ReleaseTool
{
    public partial class MainForm : Form
    {
        private readonly static string startupPath = System.Windows.Forms.Application.StartupPath;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonStartDeployment_Click(object sender, EventArgs e)
        {
            m_packagePath = textBoxPackagePath.Text;
           
            if (string.IsNullOrWhiteSpace(m_packagePath))
            {
                MessageBox.Show("请选择目标目录。");
                return;
            }
            m_svnPath = textBoxSvnPath.Text;
            if (string.IsNullOrWhiteSpace(m_svnPath))
            {
                MessageBox.Show("请选择目标目录。");
                return;
            }

            m_absManagementSitePath = Path.Combine(m_svnPath, @"chineseabs\app\ChineseAbs.ABSManagementSite\");
            Common.Assert(Directory.Exists(m_absManagementSitePath), "SVN路径不存在");
            Common.Assert(GetVersion(m_absManagementSitePath), "旧版本信息获取失败");

            ModifyVersionForm modifyVersionForm = new ModifyVersionForm(m_oldVersion);
            if (modifyVersionForm.ShowDialog() == DialogResult.OK)
            {
                string newVersion = modifyVersionForm.GetNewVersion();
                PackUtils packUtils = new PackUtils(m_packagePath, m_oldVersion, newVersion, 
                    m_absManagementSitePath, m_svnPath, m_versionPath);
                packUtils.Pack();
            }
            else 
            {
                Common.Log("用户取消");
                return;
            }
            modifyVersionForm.Close();

        }

        private void buttonDestinationWebsitePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            textBoxPackagePath.Text = dialog.SelectedPath;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Common.Init(richTextBoxLog);
            textBoxPackagePath.Text = startupPath;

            var svnRootPath = ConfigurationManager.AppSettings["svnRootPath"];
            if (!string.IsNullOrWhiteSpace(svnRootPath))
            {
                this.textBoxSvnPath.Text = svnRootPath;
            }
        }

        private bool GetVersion(string path)
        {
            var pubxmlPath = Path.Combine(path, @"Properties\PublishProfiles\ABSManager.pubxml");
            var pubxmlDoc = new XmlDocument();
            pubxmlDoc.Load(pubxmlPath);
            var root = pubxmlDoc.DocumentElement;
            var projectNodes = root.ChildNodes;
            foreach (var projectNode in projectNodes)
            {
                XmlElement propertyGroup = (XmlElement)projectNode;
                if (propertyGroup.Name == "PropertyGroup")
                {
                    var propertyGroupNodes = propertyGroup.ChildNodes;
                    foreach (var propertyGroupNode in propertyGroupNodes)
                    {
                        XmlElement publishUrl = (XmlElement)propertyGroupNode;
                        if (publishUrl.Name == "publishUrl")
                        {
                            var publishPath = publishUrl.InnerText;
                            m_versionPath = publishPath.Split(' ')[0];
                            m_oldVersion = publishPath.Split(' ')[1];
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Text = string.Empty;
        }

        private void buttonSvnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            textBoxSvnPath.Text = dialog.SelectedPath;
        }

        private string m_absManagementSitePath;
        private string m_packagePath;
        private string m_svnPath;
        private string m_oldVersion;
        private string m_versionPath;
    }
}
