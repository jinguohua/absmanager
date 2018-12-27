using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ReleaseTool
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
    }
}
