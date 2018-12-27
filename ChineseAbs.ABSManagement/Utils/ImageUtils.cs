using System.IO;
using ImageProcessor;
using System.Drawing;
using ImageProcessor.Imaging.Formats;

namespace ChineseAbs.ABSManagement.Utils
{
    public class ImageUtils
    {
        public static void Compress(Stream inImageStream, Stream outImageStream)
        {
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(inImageStream).Format(m_defaultFormat).Save(outImageStream);
            }
        }

        public static void Compress(Stream inImageStream, Stream outImageStream, Size size)
        {
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(inImageStream).Resize(size).Format(m_defaultFormat).Save(outImageStream);
            }
        }

        public static void Compress(string inImagePath, string outImagePath, Size size)
        {
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(inImagePath).Resize(size).Format(m_defaultFormat).Save(outImagePath);
            }
        }

        public static void Compress(string inImagePath, string outImagePath)
        {
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(inImagePath).Format(m_defaultFormat).Save(outImagePath);
            }
        }

        //默认为JPG格式，图片质量压缩90%
        private static ISupportedImageFormat m_defaultFormat = new JpegFormat { Quality = 90 };
    }
}
