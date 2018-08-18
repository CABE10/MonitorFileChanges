using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonitorFileChanges;

namespace MonitorTests
{
    [TestClass]
    public class CreateFileTests
    {
        [TestCleanup]
        public void Clean()
        {
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testCreate.txt";
            if (Directory.Exists(directory) && File.Exists(String.Concat(directory, @"\", fileName)))
            {
                File.Delete(String.Concat(directory, @"\", fileName));
            }
        }
        [TestMethod]
        public void CreateFileTest()
        {
            StringBuilder sb = new StringBuilder();
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testCreate.txt";
            string fullFilePath = String.Concat(directory, @"\", fileName).ToLower();
            for (int x = 0; x < 100; x++)
            {
                sb.AppendLine($"{x} Test");
            }
            File.WriteAllText(fullFilePath, sb.ToString());
            var reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath && f.NumLines == 100));
        }
    }
}
