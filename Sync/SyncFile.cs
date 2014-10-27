using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync.Sync
{
    /// <summary>
    /// Contains information about a file in the synchronization context.
    /// </summary>
    public class SyncFile
    {
        /// <summary>
        /// The filepath relative to the sync folder.
        /// </summary>
        public string RelativePath { get; private set; }

        /// <summary>
        /// The file information about the newest version of this file available.
        /// </summary>
        public FileInfo NewestVersion { get; private set; }

        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="fileInfo">The file.</param>
        /// <param name="relativePathStartIndex">The index marking the beginning of the relative part of the filepath.</param>
        public SyncFile(FileInfo fileInfo, int relativePathStartIndex)
        {
            NewestVersion = fileInfo;
            RelativePath = fileInfo.FullName.Substring(relativePathStartIndex);
        }

        /// <summary>
        /// Updates the NewestVersion of the SyncFile if the comparison file is newer.
        /// </summary>
        public void UpdateSource(FileInfo comparisonFile)
        {
            if (comparisonFile.LastWriteTimeUtc > NewestVersion.LastWriteTimeUtc)
            {
                NewestVersion = comparisonFile;
            }
        }
    }
}
