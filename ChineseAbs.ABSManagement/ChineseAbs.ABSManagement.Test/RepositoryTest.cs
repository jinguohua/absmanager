using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChineseAbs.ABSManagement.LocalRepository;
using System.IO;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public void TestTransportFile()
        {
            var userName = "cgzhouTest";

            var bytes = File.ReadAllBytes(@"D:\RepositoryTest.txt");

            Repository repo = new Repository(userName);
            var file = repo.AddFile("新的文件名2.txt", new MemoryStream(bytes));
            
            var outFile = repo.GetFile(file.Guid);

            Assert.AreEqual(file.Name, outFile.Name);
            
            var outBytes = outFile.Stream.ToArray();
            Assert.AreEqual(bytes.Length, outBytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual(bytes[i], outBytes[i]);
            }
        }

        [TestMethod]
        public void TestTransportImage()
        {
            var userName = "cgzhouTest";

            var bytes = File.ReadAllBytes(@"D:\RepositoryTest.png");

            Repository repo = new Repository(userName);
            var image = repo.AddImage("新的图象文件名2.png", new MemoryStream(bytes));

            var outFile = repo.GetImage(image.Guid);

            Assert.AreEqual(image.Name, outFile.Name);

            var outBytes = outFile.Stream.ToArray();
            Assert.AreEqual(bytes.Length, outBytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual(bytes[i], outBytes[i]);
            }
        }
    }
}
