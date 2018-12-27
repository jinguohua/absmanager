using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class ZipUtils
    {
        public static void ExtractFiles(Stream zipStream, Dictionary<string, string> fileDict, string extractFolder)
        {
            Stream stream = new MemoryStream();
            zipStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var zipInputStream = new ZipInputStream(stream))
            {
                ZipEntry entry = null;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile && fileDict.ContainsKey(entry.Name))
                    {
                        byte[] buffer = new byte[4096];
                        var filePath = Path.Combine(extractFolder, fileDict[entry.Name]);
                        System.IO.File.Delete(filePath);
                        using (FileStream streamWriter = System.IO.File.Open(filePath, FileMode.OpenOrCreate))
                        {
                            StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                        }
                    }
                }
            }
        }

        public static List<string> GetZipFileNames(Stream zipStream)
        {
            Stream stream = new MemoryStream();
            zipStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            List<string> fileNames = new List<string>();

            using (var zipInputStream = new ZipInputStream(stream))
            {
                ZipEntry entry = null;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile)
                    {
                        fileNames.Add(entry.Name);
                    }
                }
            }

            return fileNames;
        }

        public static List<string> GetZipFolderNames(Stream zipStream)
        {
            Stream stream = new MemoryStream();
            zipStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            List<string> folderNames = new List<string>();

            using (var zipInputStream = new ZipInputStream(stream))
            {
                ZipEntry entry = null;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    var fileNameStartIndex = entry.Name.LastIndexOf('/');
                    if (fileNameStartIndex >= 0)
                    {
                        var names = entry.Name.Remove(fileNameStartIndex).Split('/');
                        folderNames.AddRange(names);
                    }
                }
            }

            return folderNames.Distinct().ToList();
        }

        public static void Compress(string destZipFile, string srcFolder)
        {
            (new FastZip()).CreateZip(destZipFile, srcFolder, true, "");
        }

        public static void CompressFiles(string folder, List<string> fileNames, MemoryStream ms)
        {
            CommUtils.Assert(Directory.Exists(folder), "Compress failed, can not find folder [" + folder + "] .");

            ZipOutputStream zipStream = new ZipOutputStream(ms);
            zipStream.SetLevel(3);

            foreach (var file in fileNames)
            {
                var filePath = Path.Combine(folder, file);
                if (!File.Exists(filePath))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(filePath);

                string entryName = ZipEntry.CleanName(file);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fileInfo.LastWriteTime;
                newEntry.Size = fileInfo.Length;
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filePath))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            zipStream.Flush();
            zipStream.Finish();

            ms.Seek(0, SeekOrigin.Begin);
        }

        public static void Compress(string folder, MemoryStream ms)
        {
            CommUtils.Assert(Directory.Exists(folder), "Compress failed, can not find folder [" + folder + "] .");

            ZipOutputStream zipStream = new ZipOutputStream(ms);
            zipStream.SetLevel(3);

            int folderOffset = folder.Length + (folder.EndsWith("\\") ? 0 : 1);
            CompressFolder(folder, zipStream, folderOffset);

            zipStream.Flush();
            zipStream.Finish();

            ms.Seek(0, SeekOrigin.Begin);
        }

        private static void CompressFolder(string folder, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string filename in files)
            {
                FileInfo fileInfo = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset);
                entryName = ZipEntry.CleanName(entryName);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fileInfo.LastWriteTime;
                newEntry.Size = fileInfo.Length;
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            string[] folders = Directory.GetDirectories(folder);
            foreach (string subFolder in folders)
            {
                CompressFolder(subFolder, zipStream, folderOffset);
            }
        }
    }
}
