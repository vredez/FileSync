using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    /// <summary>
    /// Contains information about a sync target directory.
    /// </summary>
    public class TargetFolder
    {
        /// <summary>
        /// The path of the directory
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Flag whether just use the directory as provide-only source or include it fully into the sync process.
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}
