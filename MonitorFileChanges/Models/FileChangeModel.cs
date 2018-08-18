using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorFileChanges.Models
{
    public class FileChangeModel
    {
        public DateTime DateModified { get; set; }
        public string FileName { get; set; } //includes location and extension.
        public int NumLines { get; set; }
    }
}
