using ChineseAbs.ABSManagement.LocalRepository.Core;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Drawing;
using System.IO;

namespace ChineseAbs.ABSManagement.LocalRepository
{
    public class RepositoryImage : RepositoryElement
    {
        protected override string GetRepositoryRootPath()
        {
            return WebConfigUtils.RepositoryImagePath;
        }

        public string GetThumbnailPath()
        {
            var size = CalcThumbnailSize(GetAbsoluteFilePath(), 50);
            return Compress("Thumbnail.jpg", size);
        }

        public string GetPreviewImagePath()
        {
            return Compress("PreviewImage.jpg");
        }

        private string Compress(string compressImageName, Size? size = null)
        {
            var filePath = GetAbsoluteFilePath();
            if (filePath.ToLower().EndsWith(".gif"))
            {
                //Gif动图直接返回，不进行压缩
                return filePath;
            }

            var folder = GetCompressFolderPath();

            var nameWithOutExtension = Name.Substring(0, Name.LastIndexOf('.'));
            var thumbnailPath = Path.Combine(folder, compressImageName);

            if (!File.Exists(thumbnailPath))
            {
                if (size.HasValue)
                {
                    ImageUtils.Compress(filePath, thumbnailPath, size.Value);
                }
                else
                {
                    ImageUtils.Compress(filePath, thumbnailPath);
                }
            }

            if (File.Exists(thumbnailPath))
            {
                return thumbnailPath;
            }

            return filePath;
        }

        private Size CalcThumbnailSize(string imagePath, int height)
        {
            Image img = Image.FromFile(imagePath);
            var whPercent = (double)img.Width / (double)img.Height;
            var width = int.Parse(Math.Round((double)height * whPercent).ToString());
            return new Size(width, height);
        }

        public string GetImagePath()
        {
            return GetAbsoluteFilePath();
        }

        private string GetCompressFolderPath()
        {
            return Path.Combine(GetAbsoluteFolderPath(), "CompressImage");
        }
    }
}
