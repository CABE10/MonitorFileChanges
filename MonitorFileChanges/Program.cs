using MonitorFileChanges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorFileChanges
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }
        private static void RulesCheck(string directory, string pattern)
        {

        }

        public static FileChangeReqs GetFileChangeReqs(string directory, string pattern)
        {
            throw new NotImplementedException();
        }

        private static void ParseEngine(FileChangeReqs reqs) { }

        private static void Parse(ref FileChangeReqs reqsBefore) { }

        private static int GetFileLinesCount(string file)
        {
            throw new NotImplementedException();
        }
        private static DateTime GetFileDateModified(string file)
        {
            throw new NotImplementedException();

        }

    }
}
