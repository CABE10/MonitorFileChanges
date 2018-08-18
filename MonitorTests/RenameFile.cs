using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonitorFileChanges;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorTests
{
    [TestClass]
    public class RenameFileTests
    {
        [TestCleanup]
        public void Clean()
        {
            //probably no need to clean in this class as we've already deleted the file.
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testRename1.txt";
            if (Directory.Exists(directory) && File.Exists(String.Concat(directory, @"\", fileName)))
            {
                File.Delete(String.Concat(directory, @"\", fileName));
            }
            fileName = "testRename2.txt";
            if (Directory.Exists(directory) && File.Exists(String.Concat(directory, @"\", fileName)))
            {
                File.Delete(String.Concat(directory, @"\", fileName));
            }
        }
        [TestMethod]
        public void RenameFileTest()
        {
            StringBuilder sb = new StringBuilder();
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testRename1.txt";
            string fullFilePath = String.Concat(directory, @"\", fileName).ToLower();
            for (int x = 0; x < 5; x++)
            {
                sb.AppendLine($"{x} Test");
            }
            File.WriteAllText(fullFilePath, sb.ToString());
            var reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath));
            File.Move(fullFilePath, String.Concat(directory, @"\", "testRename2.txt").ToLower());
            reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(!reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath));
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == String.Concat(directory, @"\", "testRename2.txt").ToLower()));
        }
    }
}
