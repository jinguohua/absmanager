using System;
using System.Collections.Generic;
using System.IO;

namespace ChineseAbs.ABSManagement.Utils
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        Excel,
        Word,
        PDF,
        RTF,
        TXT,
        ZIP,
        RAR,
        PPT,
        Image,
        Script,
        Others
    }

    public enum ImageFileType
    {
        BMP = 1,
        //PCX = 2,
        //TIFF = 3,
        GIF = 4,
        JPEG = 5,
        //TGA = 6,
        //EXIF = 7,
        //FPX = 8,
        //SVG = 9,
        //PSD = 10,
        //CDR = 11,
        //PCD = 12,
        //DXF = 13,
        //UFO = 14,
        //EPS = 15,
        //AI = 16,
        PNG = 17,
        //HDRI = 18,
        //RAW = 19,
        //WMF = 20,
        //LIC = 21,
        //EMF = 22,
        JPG = 23
    }

    static public class FileUtils
    {
        static public readonly string PathSeparator = "/";

        static public string FormatSize(long totalByte)
        {
            var sizeUnits = new [] { "B", "KB", "MB", "GB", "TB" };
            var indexOfUnit = 0;
            while (totalByte > 1024 && indexOfUnit < sizeUnits.Length)
            {
                totalByte /= 1024;
                ++indexOfUnit;
            }

            return totalByte.ToString("#.## ") + sizeUnits[indexOfUnit];
        }

        static public void CheckExtension(string filePathName)
        {
            var extension = Path.GetExtension(filePathName);
            CommUtils.Assert(!string.IsNullOrEmpty(extension), "文件[{0}]中不包含扩展名（文件类型）", filePathName);
        }

        //解析文件扩展名
        static public string GetExtension(string filePathName, bool withCheck = false)
        {
            var extension = Path.GetExtension(filePathName);
            if (withCheck)
            {
                CommUtils.Assert(!string.IsNullOrEmpty(extension), "文件[{0}]中不包含扩展名（文件类型）", filePathName);
            }
            return extension.ToLower();
        }

        static public string CombineExtension(string filePathName, string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return filePathName;
            }

            if (extension.StartsWith("."))
            {
                return filePathName + extension;
            }

            return filePathName + "." + extension;
        }

        static public string GetFileNameWithoutExtension(string filePathName)
        {
            return Path.GetFileNameWithoutExtension(filePathName);
        }

        static public string GetFileName(string filePathName)
        {
            return Path.GetFileName(filePathName);
        }

        static public string InsertTimeStamp(string filePathName)
        {
            var timestamp = DateTime.Now.ToString("_yyyy-MM-dd HH.mm.ss");
            var index = filePathName.LastIndexOf('.');
            if (index > 0)
            {
                filePathName = filePathName.Substring(0, index) + timestamp
                    + '.' + filePathName.Substring(index + 1);
            }
            else
            {
                filePathName += timestamp;
            }

            return filePathName;
        }

        static public string GetMIME(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            if (m_dictMime.ContainsKey(extension))
            {
                return m_dictMime[extension];
            }

            //任意的二进制数据  http://baike.baidu.com/view/160611.htm
            return "application/octet-stream";
        }

        static public FileType GetFileType(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            if (m_dictFileType.ContainsKey(extension))
            {
                return m_dictFileType[extension];
            }

            return FileType.Others;
        }

        static public void Copy(string srcFilePath, string destFilePath, bool checkIsExist = true)
        {
            if (checkIsExist)
            {
                CommUtils.Assert(File.Exists(srcFilePath), "Can't find file [" + srcFilePath + "]");
            }
            else
            {
                if (!File.Exists(srcFilePath))
                {
                    return;
                }
            }

            try
            {
                File.Copy(srcFilePath, destFilePath, true);
            }
            catch (Exception e)
            {
                CommUtils.Assert(false, "Copy [{0}] from [{1}] failed! Exception: {2}", destFilePath, srcFilePath, e.Message);
            }
        }

        static public string ConvertFileExtension(string fileName, FileType newFileType)
        {
            var index = fileName.LastIndexOf('.');
            if (index >= 0)
            {
                fileName = fileName.Substring(0, index);
            }

            switch (newFileType)
            {
                case FileType.Word:
                    return fileName + ".docx";
                case FileType.PDF:
                    return fileName + ".pdf";
                case FileType.Excel:
                    return fileName + ".xlsx";
                case FileType.ZIP:
                    return fileName + ".zip";
                case FileType.RAR:
                    return fileName + ".rar";
                case FileType.TXT:
                    return fileName + ".txt";
            }

            return fileName;
        }

        static private Dictionary<string, FileType> m_dictFileType = new Dictionary<string, FileType> {
            { "doc", FileType.Word },
            { "docx", FileType.Word },
            { "rtf", FileType.RTF },
            { "xls", FileType.Excel },
            { "xlsx", FileType.Excel },
            { "ppt", FileType.PPT },
            { "pptx", FileType.PPT },
            { "pps", FileType.PPT },
            { "ppsx", FileType.PPT },
            { "pdf", FileType.PDF },
            { "rar", FileType.RAR },
            { "zip", FileType.ZIP },
            { "z", FileType.ZIP },
            { "7z", FileType.ZIP },
            { "bmp", FileType.Image },
            { "gif", FileType.Image },
            { "png", FileType.Image },
            { "jpe", FileType.Image },
            { "jpeg", FileType.Image },
            { "jpg", FileType.Image },
            { "txt", FileType.TXT },
            { "xml", FileType.Script },
            { "html", FileType.Script },
            { "css", FileType.Script },
            { "js", FileType.Script }
        };

        static private Dictionary<string, string> m_dictMime = new Dictionary<string, string> {
            { "doc", "application/msword" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "rtf", "application/rtf" },
            { "xls", "application/vnd.ms-excel	application/x-excel" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "ppt", "application/vnd.ms-powerpoint" },
            { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "pps", "application/vnd.ms-powerpoint" },
            { "ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            { "pdf", "application/pdf" },
            { "swf", "application/x-shockwave-flash" },
            { "dll", "application/x-msdownload" },
            { "exe", "application/octet-stream" },
            { "msi", "application/octet-stream" },
            { "chm", "application/octet-stream" },
            { "cab", "application/octet-stream" },
            { "ocx", "application/octet-stream" },
            { "rar", "application/octet-stream" },
            { "tar", "application/x-tar" },
            { "tgz", "application/x-compressed" },
            { "zip", "application/x-zip-compressed" },
            { "z", "application/x-compress" },
            { "wav", "audio/wav" },
            { "wma", "audio/x-ms-wma" },
            { "wmv", "video/x-ms-wmv" },
            { "mp3", "audio/mpeg" },
            { "mp2", "audio/mpeg" },
            { "mpe", "audio/mpeg" },
            { "mpeg", "audio/mpeg" },
            { "mpg", "audio/mpeg" },
            { "rm", "application/vnd.rn-realmedia" },
            { "mid", "audio/mid" },
            { "midi", "audio/mid" },
            { "rmi", "audio/mid" },
            { "bmp", "image/bmp" },
            { "gif", "image/gif" },
            { "png", "image/png" },
            { "tif", "image/tiff" },
            { "tiff", "image/tiff" },
            { "jpe", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "jpg", "image/jpeg" },
            { "txt", "text/plain" },
            { "xml", "text/xml" },
            { "html", "text/html" },
            { "css", "text/css" },
            { "js", "text/javascript" },
            { "mht", "message/rfc822" },
            { "mhtml", "message/rfc822" }
        };
    }
}
