using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ReleaseTool
{
    public class PackUtils
    {
        public PackUtils(string packagePath, string oldVersion, string newVersion, string absManagementSitePath,
            string gitPath, string versionPath)
        {
            m_packagePath = packagePath;
            m_newVersion = newVersion;
            m_oldVersion = oldVersion;
            m_absManagementSitePath = absManagementSitePath;
            m_gitPath = gitPath;
            m_absMgrDeploymentFilesPath = Path.Combine(m_gitPath, @"ChineseAbs.ABSManagement.DeploymentTool\ABSMgrDeployment\bin\Release\");
            m_versionPath = versionPath;
        }

        private bool ModifyAssemblyInfo(string oldVersion, string newVersion, string path)
        {
            Common.Log("正在修改AssemblyInfo...");
            try
            {
                if (string.IsNullOrWhiteSpace(oldVersion)
                    || string.IsNullOrWhiteSpace(newVersion))
                {
                    Common.Log("版本号不能为空 oldVersion=" + oldVersion + " newVersion=" + newVersion);
                    return false;
                }

                var text = File.ReadAllText(path);
                text = text.Replace("AssemblyFileVersion(\"" + oldVersion + "\")",
                    "AssemblyFileVersion(\"" + newVersion + "\")");

                File.WriteAllText(path, text, System.Text.Encoding.UTF8);

                Common.Log("修改AssemblyInfo成功");
                return true;
            }
            catch (Exception e)
            {
                Common.Log("修改AssemblyInfo失败", e);
                return false;
            }
        }

        private bool ModifyPubxml(string newVersion, string path)
        {
            Common.Log("正在修改版本号...");
            try
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
                                publishUrl.InnerText = m_versionPath + ' ' + newVersion;
                            }
                        }
                    }

                }
                pubxmlDoc.Save(pubxmlPath);
                Common.Log("修改版本号成功");
                return true;
            }
            catch (Exception e)
            {
                Common.Log("修改版本号失败", e);
                return false;
            }
        }

        private void CheckFolderPath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    var errorMsg = "创建文件夹失败，请检查是否有创建权限和目录是否正确：" + path;
                    Common.Assert(e, errorMsg);
                }
            }
        }


        public void Pack()
        {
            Common.Log(Environment.NewLine + "开始创建版本发布包...");

            //if (!ExcuteCmd(m_svnUpdateCmd))
            //{
            //    return;
            //}

            m_destPath = Path.Combine(m_packagePath, "ABSManager_" + m_newVersion);
            CheckFolderPath(m_destPath);

            if (!ModifyPubxml(m_newVersion, m_absManagementSitePath))
            {
                return;
            }

            if (!ModifyAssemblyInfo(m_oldVersion, m_newVersion,
                Path.Combine(m_gitPath, @"ChineseAbs.ABSManagement\Properties\AssemblyInfo.cs")))
            {
                return;
            }

            if (!ModifyAssemblyInfo(m_oldVersion, m_newVersion,
                Path.Combine(m_gitPath, @"ChineseAbs.ABSManagementSite\Properties\AssemblyInfo.cs")))
            {
                return;
            }

            Action<string, string> nugetPackage = (string packageConfigPath, string packagePath) => {
                var cdCmd = "cd " + packageConfigPath;
                var nugetCmd = Path.Combine(m_gitPath, "../nuget.exe")
                    + " install -outputDirectory "
                    + Path.Combine(packageConfigPath, packagePath);

                if (!ExcuteCmd(cdCmd + Environment.NewLine + nugetCmd))
                {
                    return;
                }
            };

            nugetPackage(m_absManagementSitePath, Path.Combine(m_absManagementSitePath, "../packages"));

            var absManagementPath = m_absManagementSitePath.Replace("ChineseAbs.ABSManagementSite", "ChineseAbs.ABSManagement");
            nugetPackage(absManagementPath, Path.Combine(absManagementPath, "../packages"));

            var deploymentToolPath = m_absManagementSitePath.Replace("ChineseAbs.ABSManagementSite", "ChineseAbs.ABSManagement.DeploymentTool\\ABSMgrDeployment");
            nugetPackage(deploymentToolPath, Path.Combine(deploymentToolPath, "../packages"));

            var msBuild = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild";
            msBuild = "\"" + msBuild + "\"";

            var publishCmd = "set publishGitRootPath=";
            publishCmd += m_gitPath + Environment.NewLine;
            publishCmd += msBuild + @" %publishGitRootPath%\ChineseAbs.ABSManagementSite\ChineseAbs.ABSManagementSite.csproj"
                + @" /p:Configuration=Release /p:DeployOnBuild=true "
                + @"/p:PublishProfile=%publishGitRootPath%\ChineseAbs.ABSManagementSite\Properties\PublishProfiles\ABSManager.pubxml /p:VisualStudioVersion=12.0";
            if (!ExcuteCmd(publishCmd))
            {
                return;
            }

            var newVersionPath = m_versionPath + ' ' + m_newVersion;
            if (!CopyWebsiteFolder(newVersionPath, Path.Combine(m_destPath, "Website")))
            {
                return;
            }

            var destSqlPath = Path.Combine(m_destPath, "SQL");
            if (!Directory.Exists(destSqlPath))
            {
                Directory.CreateDirectory(destSqlPath);
            }
            if (!CopyWebsiteFolder(Path.Combine(m_gitPath, "ChineseAbs.ABSManagementSite/SQL"), destSqlPath))
            {
                return;
            }

            var buildCmd = "set publishGitRootPath=";
            buildCmd += m_gitPath + Environment.NewLine;
            buildCmd += "call " + "\"" + "%vs120comntools%vsvars32.bat" + "\"" + Environment.NewLine;
            buildCmd += "devenv " + @"%publishGitRootPath%\ChineseAbs.ABSManagement.DeploymentTool\ABSMgrDeployment.sln /Rebuild Release";

            if (!ExcuteCmd(buildCmd))
            {
                return;
            }

            if (!CopyDeploymentFiles(Path.Combine(m_absMgrDeploymentFilesPath, @"ABSMgrDeployment.exe"),
                Path.Combine(m_destPath, "ABSMgrDeployment.exe")))
            {
                return;
            }

            if (!CopyDeploymentFiles(Path.Combine(m_absMgrDeploymentFilesPath, "ABSMgrDeployment.exe.config"),
                Path.Combine(m_destPath, "ABSMgrDeployment.exe.config")))
            {
                return;
            }

            if (!CopyDeploymentFiles(Path.Combine(m_absMgrDeploymentFilesPath, "ICSharpCode.SharpZipLib.dll"),
                Path.Combine(m_destPath, "ICSharpCode.SharpZipLib.dll")))
            {
                return;
            }

            Common.Log("正在创建压缩包...");
            FastZip zippy = new FastZip();
            ZipEntryFactory factory = new ZipEntryFactory();
            factory.IsUnicodeText = true;
            zippy.EntryFactory = factory;
            zippy.CreateZip("ABSManager_" + m_newVersion + ".zip", m_destPath, true, "");

            Common.Log("正在删除临时文件...");
            Directory.Delete(m_destPath, true);
            Common.Log("版本发布包创建成功" + Environment.NewLine);
        }

        private bool CopyDeploymentFiles(string sourcePath, string destinationPath)
        {
            Common.Log("正在复制...");
            Common.Log("从" + sourcePath + "到" + destinationPath);
            try
            {
                File.Copy(sourcePath, destinationPath, true);
                Common.Log("复制成功");
                return true;
            
            }
            catch (Exception e)
            {
                Common.Log("复制失败", e);
                return false;
            }

        }

        private bool CopyWebsiteFolder(string sourcePath, string destinationPath)
        {
            Common.Log("正在复制...");
            Common.Log("从" + sourcePath + "到" + destinationPath);
            try
            {
                var ignoreFiles = new List<string> { "web.config" };
                Common.CopyDir(sourcePath, destinationPath, ignoreFiles);

                Common.Log("复制成功");
                return true;

            }
            catch (Exception e)
            {
                Common.Log("复制失败", e);
                return false;
            }
        }

        public static bool ExcuteCmd(string cmd, bool ignoreErrMsg = false)
        {
            Common.Log("正在执行" + cmd + "...");

            var errMsg = string.Empty;

            try
            {
                string output;
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.WriteLine("exit");
                    process.StandardInput.AutoFlush = true;

                    process.WaitForExit(1000);
                    output = process.StandardOutput.ReadToEnd();
                    errMsg = process.StandardError.ReadToEnd();

                    process.Close();
                }
                
                if (ignoreErrMsg || errMsg == string.Empty)
                {
                    Common.Log(output);
                    Common.Log("执行" + cmd + "成功");
                    return true;
                }
                else
                {
                    Common.Log(output);
                    Common.Log(errMsg);
                    Common.Log("执行" + cmd + "失败");
                    return false;
                }

            }
            catch (Exception e)
            {
                Common.Log("执行" + cmd + "失败", e);
                return false;
            }
           
        }

        private string m_absMgrDeploymentFilesPath;
        private string m_absManagementSitePath;
        private string m_packagePath;

        private string m_oldVersion;
        private string m_newVersion;
        private string m_versionPath;
        private string m_gitPath;

        //目标文件夹路径
        private string m_destPath;
    }
}
