using System.IO;

namespace ChineseAbs.ABSManagement.LocalRepository
{
    public class Repository
    {
        public Repository()
        {
            m_dbAdapter = new DBAdapter();
        }

        public RepositoryFile AddFile(string fileName, MemoryStream stream)
        {
            var file = new RepositoryFile();
            file.Name = fileName;
            file.Stream = stream;

            var record = m_dbAdapter.File.New(file.Name, string.Empty);
            file.Id = record.Id;
            file.Guid = record.Guid;
            file.CreateUserName = record.CreateUserName;
            file.CreateTime = record.CreateTime;
            record.Path = file.SaveToDisk();
            m_dbAdapter.File.Update(record);
            return file;
        }

        public RepositoryFile GetFile(int fileId, bool autoLoadFile = true)
        {
            var file = m_dbAdapter.File.GetById(fileId);
            return GetFile(file, autoLoadFile);
        }

        public RepositoryFile GetFile(string guid, bool autoLoadFile = true)
        {
            var file = m_dbAdapter.File.GetByGuid(guid);
            return GetFile(file, autoLoadFile);
        }

        private RepositoryFile GetFile(ChineseAbs.ABSManagement.Models.Repository.File file,
            bool autoLoadFile = true)
        {
            var repoFile = new RepositoryFile();
            repoFile.Name = file.Name;
            repoFile.Guid = file.Guid;
            repoFile.CreateUserName = file.CreateUserName;
            repoFile.CreateTime = file.CreateTime;
            repoFile.Id = file.Id;

            if (autoLoadFile)
            {
                repoFile.LoadFromDisk();
            }

            return repoFile;
        }

        public RepositoryImage AddImage(string imageName, MemoryStream stream)
        {
            var image = new RepositoryImage();
            image.Name = imageName;
            image.Stream = stream;

            var record = m_dbAdapter.Image.New(image.Name, string.Empty);
            image.Guid = record.Guid;
            image.CreateUserName = record.CreateUserName;
            image.CreateTime = record.CreateTime;
            record.Path = image.SaveToDisk();
            m_dbAdapter.Image.Update(record);
            return image;
        }

        public RepositoryImage GetImage(string guid, bool autoLoadImage = true)
        {
            var record = m_dbAdapter.Image.GetByGuid(guid);

            var image = new RepositoryImage();
            image.Name = record.Name;
            image.Guid = record.Guid;
            image.CreateUserName = record.CreateUserName;
            image.CreateTime = record.CreateTime;
            image.Id = record.Id;

            if (autoLoadImage)
            {
                image.LoadFromDisk();
            }

            return image;
        }

        private DBAdapter m_dbAdapter;
    }
}
