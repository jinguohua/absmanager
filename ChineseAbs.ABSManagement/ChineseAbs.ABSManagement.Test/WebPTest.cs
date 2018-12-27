using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System.Drawing;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using System.IO;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class WebPTest
    {
        [TestMethod]
        public void TestCompressToWebP()
        {
            WebPFormat webP = new WebPFormat();
            var srcPath = @"C:\Users\Public\Pictures\Sample Pictures\Jellyfish.jpg";

            //using (System.IO.FileStream fs = new System.IO.FileStream(,
            //    System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            Image img = Image.FromFile(srcPath);

                //var image = webP.Load(fs);
                
                for (int i = 10; i <= 100; i+=10 )
                {
                    webP.Quality = i;
                    webP.Save(@"C:\Users\Public\Pictures\Sample Pictures\Compress-" + i + ".webp", img, 4);
                }

                for (int i = 90; i <= 99; i += 1)
                {
                    webP.Quality = i;
                    webP.Save(@"C:\Users\Public\Pictures\Sample Pictures\90-99\Compress-" + i + ".webp", img, 4);
                }
            //}
        }

        [TestMethod]
        public void TestCompress()
        {
            for (int i = 10; i <= 100; )
            {
                var k = (i < 90 ? i += 10 : i += 1);
                var distPath = @"C:\Users\Public\Pictures\Sample Pictures\a\Jellyfish-compress-" + i + ".jpg";
                var srcPath = @"C:\Users\Public\Pictures\Sample Pictures\Jellyfish.bmp";
                byte[] photoBytes = File.ReadAllBytes(srcPath);
                // Format is automatically detected though can be changed.
                ISupportedImageFormat format = new JpegFormat { Quality = i };
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            // Load, resize, set the format and quality and save an image.
                            imageFactory.Load(inStream)
                                //.Resize(new Size(1000, 800))
                                        .Format(format)
                                        .Save(distPath);
                        }
                        // Do something with the stream.
                    }
                }
            }


        }
    }
}
