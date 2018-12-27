using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ABSMgrDeployment
{
    public static class Common
    {
        private static RichTextBox m_richTextBoxLog = null;

        public static void Init(RichTextBox richTextBox)
        {
            m_richTextBoxLog = richTextBox;
        }

        private static void UpdateLogTxt(string message)
        {
            if (m_richTextBoxLog != null)
            {
                m_richTextBoxLog.Text += message;
                m_richTextBoxLog.SelectionStart = m_richTextBoxLog.Text.Length;
                m_richTextBoxLog.ScrollToCaret();
                m_richTextBoxLog.Update();
            }
        }

        public static void Log(string message, Exception e = null)
        {
            UpdateLogTxt(message + Environment.NewLine);
            if (e != null)
            {
                Log(e);
            }
        }

        public static void Log(Exception e)
        {
            UpdateLogTxt(e.Message + Environment.NewLine + e.StackTrace);
        }

        public static void Assert(bool value, string errorMsg)
        {
            if (!value)
            {
                throw new ApplicationException(errorMsg);
            }
        }

        public static void Assert(Exception e, string errorMsg = "")
        {
            errorMsg += Environment.NewLine + "Exception:" + e.Message + Environment.NewLine + e.StackTrace;
            throw new ApplicationException(errorMsg);
        }

        public static void ZipFileFromDirectory(string rootPath, string destinationPath, int compressLevel)
        {
            List<string> files = new List<string>();
            List<string> paths = new List<string>();

            GetAllDirectories(rootPath, files, paths);

            string rootMark = rootPath + "\\";
            Crc32 crc = new Crc32();
            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(destinationPath));
            outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression
            foreach (string file in files)
            {
                var fileName = Path.GetFileName(file);
                if (UnBackupFiles.Contains(fileName))
                {
                    continue;
                }
                FileStream fileStream = File.OpenRead(file);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file.Replace(rootMark, string.Empty));
                entry.DateTime = DateTime.Now;
                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                outPutStream.PutNextEntry(entry);
                outPutStream.Write(buffer, 0, buffer.Length);
            }

            foreach (string emptyPath in paths)
            {
                ZipEntry entry = new ZipEntry(emptyPath.Contains(rootMark)? emptyPath.Replace(rootMark, string.Empty) + "/" :
                    emptyPath.Replace(rootPath, string.Empty) + "/");
                outPutStream.PutNextEntry(entry);
            }

            outPutStream.Finish();
            outPutStream.Close();
            GC.Collect();
        }

        private static void GetAllDirectories(string rootPath, List<string> files, List<string> paths)
        {
            string[] subPaths = Directory.GetDirectories(rootPath);
            foreach (string path in subPaths)
            {
                GetAllDirectories(path, files, paths);
            }
            string[] subfiles = Directory.GetFiles(rootPath);
            foreach (string file in subfiles)
            {
                files.Add(file);
            }
            if (subPaths.Length == subfiles.Length && subfiles.Length == 0)
            {
                paths.Add(rootPath);
            }
        }
       
        public static void DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo fileOrFolder in fileinfo)
            {
                if (fileOrFolder is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(fileOrFolder.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(fileOrFolder.FullName);
                }
            }
        }

        public static void CopyDir(string sourcePath, string destinationPath, List<string> ignoreFiles = null)
        {
            if (!Directory.Exists(destinationPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(destinationPath);
                directoryInfo.Create();
            }

            string[] files = Directory.GetFiles(sourcePath);
            foreach (string fromFileName in files)
            {
                string fileName = Path.GetFileName(fromFileName);
                if (ignoreFiles != null
                    && ignoreFiles.Any(x => x.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                string toFileName = Path.Combine(destinationPath, fileName);
                File.Copy(fromFileName, toFileName, true);
            }

            string[] fromDirs = Directory.GetDirectories(sourcePath);
            foreach (string fromDirName in fromDirs)
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(destinationPath, dirName);
                CopyDir(fromDirName, toDirName);
            }
        }

        private static List<string> UnBackupFiles = new List<string>()
        {
            "FrameworkLog.txt",
            "framework-rolling-log.txt"
        };
    }
}
