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
            string svnPath, string versionPath)
        {
            m_packagePath = packagePath;
            m_newVersion = newVersion;
            m_oldVersion = oldVersion;
            m_absManagementSitePath = absManagementSitePath;
            m_svnPath = svnPath;
            m_absMgrDeploymentFilesPath = Path.Combine(m_svnPath, @"chineseabs\app\ChineseAbs.ABSManagement.DeploymentTool\ABSMgrDeployment\bin\Release\");
            m_svnUpdateCmd = @"svn update " + m_svnPath;
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

            string errMsg = string.Empty;
            if (!ExcuteCmd(m_svnUpdateCmd, ref errMsg))
            {
                return;
            }

            m_destPath = Path.Combine(m_packagePath, "ABSManager_" + m_newVersion);
            CheckFolderPath(m_destPath);

            if (!ModifyPubxml(m_newVersion, m_absManagementSitePath))
            {
                return;
            }

            if (!ModifyAssemblyInfo(m_oldVersion, m_newVersion,
                Path.Combine(m_svnPath, @"chineseabs\app\ChineseAbs.ABSManagement\Properties\AssemblyInfo.cs")))
            {
                return;
            }

            if (!ModifyAssemblyInfo(m_oldVersion, m_newVersion,
                Path.Combine(m_svnPath, @"chineseabs\app\ChineseAbs.ABSManagementSite\Properties\AssemblyInfo.cs")))
            {
                return;
            }

            var publishCmd = "set svnRootPath=";
            publishCmd += m_svnPath + Environment.NewLine;
            publishCmd += "call " + "\"" + "%vs120comntools%vsvars32.bat" + "\"" + Environment.NewLine;
            publishCmd += "msbuild " + @"%svnRootPath%\chineseabs\app\ChineseAbs.ABSManagementSite\ChineseAbs.ABSManagementSite.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=%svnRootPath%\chineseabs\app\ChineseAbs.ABSManagementSite\Properties\PublishProfiles\ABSManager.pubxml /p:VisualStudioVersion=12.0";
            if (!ExcuteCmd(publishCmd, ref errMsg))
            {
                return;
            }

            var newVersionPath = m_versionPath + ' ' + m_newVersion;
            if (!CopyWebsiteFolder(newVersionPath, Path.Combine(m_destPath, "Website")))
            {
                return;
            }

            m_svnExportCmd += Path.Combine(m_destPath, "SQL") + " --force";
            if (!ExcuteCmd(m_svnExportCmd, ref errMsg))
            {
                return;
            }

            var buildCmd = "set svnRootPath=";
            buildCmd += m_svnPath + Environment.NewLine;
            buildCmd += "call " + "\"" + "%vs120comntools%vsvars32.bat" + "\"" + Environment.NewLine;
            buildCmd += "devenv " + @"%svnRootPath%\chineseabs\app\ChineseAbs.ABSManagement.DeploymentTool\ABSMgrDeployment.sln /Rebuild Release";

            if (!ExcuteCmd(buildCmd, ref errMsg))
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
            (new FastZip()).CreateZip("ABSManager_" + m_newVersion + ".zip", m_destPath, true, "");

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

        private bool ExcuteCmd(string cmd, ref string errMsg)
        {
            Common.Log("正在执行" + cmd + "...");
            try
            {
                errMsg = string.Empty;
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
                
                if (errMsg == string.Empty)
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

        private string m_svnExportCmd = "svn export svn://10.1.1.26/2010/chineseabs/app/ChineseAbs.ABSManagementSite/SQL ";
        private string m_absMgrDeploymentFilesPath;
        private string m_svnUpdateCmd;
        private string m_absManagementSitePath;
        private string m_packagePath;

        private string m_oldVersion;
        private string m_newVersion;
        private string m_versionPath;
        private string m_svnPath;

        //目标文件夹路径
        private string m_destPath;
    }
}
