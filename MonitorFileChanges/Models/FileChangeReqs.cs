using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorFileChanges.Models
{
    /// <summary>
    /// File Change Requirements or Prerequirements.
    /// </summary>
    public class FileChangeReqs
    {
        public FileChangeReqs(int seconds, string directory, string pattern)
        {
            this.FileChanges = new ConcurrentBag<FileChangeModel>();
            this.Seconds = seconds;
            this.Directory = directory;
            this.FilePattern = pattern;
        }
        public int Seconds { get; set; }

        public ConcurrentBag<FileChangeModel> FileChanges { get; set; }

        public string Directory { get; set; }

        public string FilePattern { get; set; } //.txt
    }
}
