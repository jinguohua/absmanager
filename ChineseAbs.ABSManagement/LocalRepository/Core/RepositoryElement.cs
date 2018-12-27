using System;
using System.IO;

namespace ChineseAbs.ABSManagement.LocalRepository.Core
{
    public abstract class RepositoryElement
    {
        public MemoryStream Stream { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public void LoadFromDisk()
        {
            var filePath = GetAbsoluteFilePath();
            var bytes = File.ReadAllBytes(filePath);
            Stream = new MemoryStream(bytes);
        }

        public string SaveToDisk()
        {
            var folder = GetAbsoluteFolderPath();
            var filePath = GetAbsoluteFilePath();

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                var bytes = Stream.ToArray();
                fs.Write(bytes, 0, bytes.Length);
            }

            var pathInRepo = filePath.Substring(GetRepositoryRootPath().Length);
            while (pathInRepo.StartsWith("\\") || pathInRepo.StartsWith("/"))
            {
                pathInRepo = pathInRepo.Substring(1);
            }

            return pathInRepo;
        }

        //由于文件系统对文件名/路径长度的限制，存储时，将文件名长度限制为50
        private string GetLimitLengthName(string name)
        {
            return name.Length > 50 ? name.Substring(0, 50) : name;
        }

        protected string GetAbsoluteFilePath()
        {
            string folderPath = GetAbsoluteFolderPath();
            var path = Path.Combine(folderPath, Name);

            //向前兼容，如果文件已存在，使用旧规则
            //如果文件不存在，将文件名长度限制为50
            if (File.Exists(path))
            {
                return path;
            }

            var limitedName = GetLimitLengthName(Name);
            var limitedPath = Path.Combine(folderPath, limitedName);
            return limitedPath;
        }

        protected string GetAbsoluteFolderPath()
        {
            string rootPath = GetRepositoryRootPath();
            var folder = Path.Combine(rootPath, CreateUserName, Name + "_" + Guid);

            //向前兼容，如果文件夹已存在，使用旧规则
            //如果文件夹不存在，将文件夹名字中的文件名长度限制为50
            if (Directory.Exists(folder))
            {
                return folder;
            }

            var limitedName = GetLimitLengthName(Name);
            var limitedFolder = Path.Combine(rootPath, CreateUserName, limitedName + "_" + Guid);
            return limitedFolder;
        }

        protected abstract string GetRepositoryRootPath();
    }
}
