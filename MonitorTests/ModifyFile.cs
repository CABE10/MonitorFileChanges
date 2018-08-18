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
    public class ModifyFileTests
    {
        [TestCleanup]
        public void Clean()
        {
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testModify.txt";
            if (Directory.Exists(directory) && File.Exists(String.Concat(directory, @"\", fileName)))
            {
                File.Delete(String.Concat(directory, @"\", fileName));
            }
        }
        [TestMethod]
        public void AddLinesTest()
        {
            StringBuilder sb = new StringBuilder();
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testModify.txt";
            string fullFilePath = String.Concat(directory, @"\", fileName).ToLower();
            for (int x = 0; x < 10; x++)
            {
                sb.AppendLine($"{x} Test");
            }
            File.WriteAllText(fullFilePath, sb.ToString());
            var reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath && f.NumLines == 10));
            StringBuilder sb2 = new StringBuilder();
            for (int x = 0; x < 5; x++)
            {
                sb2.AppendLine($"{x} Test");
            }
            File.AppendAllText(fullFilePath, sb2.ToString());
            reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath && f.NumLines == 15));
        }
        [TestMethod]
        public void RemoveLinesTest()
        {
            StringBuilder sb = new StringBuilder();
            string directory = ConfigurationManager.AppSettings["TestDirectory"];
            string fileName = "testModify.txt";
            string fullFilePath = String.Concat(directory, @"\", fileName).ToLower();
            for (int x = 0; x < 10; x++)
            {
                sb.AppendLine($"{x} Test");
            }
            File.WriteAllText(fullFilePath, sb.ToString());
            var reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath && f.NumLines == 10));
            StringBuilder sb2 = new StringBuilder();
            for (int x = 0; x < 5; x++)
            {
                sb2.AppendLine($"{x} Test");
            }
            File.WriteAllText(fullFilePath, sb2.ToString()); //overwrites
            reqs = Program.GetFileChangeReqs(directory, "*.txt");
            Assert.IsTrue(reqs.FileChanges.Any(f => f.FileName.ToLower() == fullFilePath && f.NumLines == 5)); //5 not 15 this time.
        }
    }
}
