using MonitorFileChanges.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorFileChanges
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args != null && args.Count() == 2)
            {
                RulesCheck(args[0], args[1]);
                FileChangeReqs reqs = GetFileChangeReqs(args[0], args[1]);

                ParseEngine(reqs);
            }
            else
            {
                //Normally, I would keep my exception strings in the resource file.
                Exception argsException = new Exception("Invalid number of arguments supplied.",
                    new Exception("Expected two arguments: a directory and a file pattern."));
                throw argsException;
            }
        }
        private static void RulesCheck(string directory, string pattern)
        {
            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                throw new Exception("Directory does not exist.");
            }
            if (String.IsNullOrEmpty(pattern) || !pattern.Contains("*."))
            {
                Exception patternException = new Exception("Invalid pattern supplied.",
                    new Exception("Pattern must contain '*.'"));
                throw patternException;
            }
            int seconds; //milliseconds from our app configuration. Didn't want to hardcode.
            int.TryParse(ConfigurationManager.AppSettings["NumSeconds"], out seconds);
            if (seconds <= 0)
            {
                throw new Exception("Invalid milliseconds specified.");
            }
        }

        public static FileChangeReqs GetFileChangeReqs(string directory, string pattern)
        {
            int seconds = int.Parse(ConfigurationManager.AppSettings["NumSeconds"]);
            FileChangeReqs fcr = new FileChangeReqs(seconds, directory, pattern);
            string[] arrFiles = Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly); //no subdirectories.
            if (arrFiles != null)
            {
                Parallel.ForEach(arrFiles, (file) =>
                {
                    FileChangeModel fcm = new FileChangeModel();
                    fcm.FileName = file;
                    fcm.NumLines = GetFileLinesCount(file);
                    fcm.DateModified = GetFileDateModified(file);
                    fcr.FileChanges.Add(fcm);
                });
            }
            return fcr;
        }

        private static void ParseEngine(FileChangeReqs reqs)
        {
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan spanTime = TimeSpan.FromSeconds(reqs.Seconds);
            System.Threading.Timer timer = new System.Threading.Timer((e) =>
            {
                Parse(ref reqs);
            }, null, startTime, spanTime);
            Console.ReadLine(); //so the console doesn't close.
        }

        private static void Parse(ref FileChangeReqs reqsBefore)
        {
            StringBuilder sb = new StringBuilder();
            FileChangeReqs reqsAfter = GetFileChangeReqs(reqsBefore.Directory, reqsBefore.FilePattern);
            foreach (FileChangeModel m in reqsAfter.FileChanges)
            {
                var reqBefore = reqsBefore.FileChanges.FirstOrDefault(f => f.FileName.ToLower() == m.FileName.ToLower());
                if (reqBefore == null)
                {
                    sb.AppendLine($"Created File: {Path.GetFileName(m.FileName)} | Lines: {m.NumLines}");
                }
                //Check for modifications.
                else if (reqBefore.NumLines > m.NumLines)
                {
                    sb.AppendLine($"Modified File: -{reqBefore.NumLines - m.NumLines} | {Path.GetFileName(m.FileName)}");
                }
                else if (reqBefore.NumLines < m.NumLines)
                {
                    sb.AppendLine($"Modified File: +{m.NumLines - reqBefore.NumLines} | {Path.GetFileName(m.FileName)}");
                }
            }
            foreach (FileChangeModel m in reqsBefore.FileChanges)
            {
                if (!reqsAfter.FileChanges.Any(f => f.FileName.ToLower() == m.FileName.ToLower()))
                {
                    sb.AppendLine($"Deleted File: {Path.GetFileName(m.FileName)}");
                }
            }
            if (sb.ToString().Length > 0)
            {
                Console.WriteLine(sb.ToString());
            }
            reqsBefore = reqsAfter;
        }

        private static int GetFileLinesCount(string file)
        {
            //I could have done File.ReadLines("").Count() but I thought this way (below) would consume less memory.
            int result = 0;
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                while (sr.ReadLine() != null)
                {
                    result++;
                }
            }
            return result;
        }
        private static DateTime GetFileDateModified(string file)
        {
            DateTime result = DateTime.MinValue;
            result = File.GetLastWriteTime(file);
            return result;
        }

    }
}
