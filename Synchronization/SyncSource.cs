using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync.Synchronization
{
    public class SyncSource
    {
        /// <summary>
        /// Contains subfolders of all sublevels of this sync source. 
        /// </summary>
        List<string> folders;

        /// <summary>
        /// Contains files of all sublevels of this sync source. 
        /// </summary>
        Dictionary<string, SyncFile> files;

        /// <summary>
        /// Gets all subfolders of all sublevels of this sync source. 
        /// </summary>
        public IEnumerable<string> Folders
        {
            get { return folders; }
        }

        /// <summary>
        /// Gets all files of all sublevels of this sync source. 
        /// </summary>
        public IEnumerable<SyncFile> Files
        {
            get { return files.Values; }
        }

        public SyncSource()
        {
            folders = new List<string>();
            files = new Dictionary<string, SyncFile>();
        }

        /// <summary>
        /// Adds a folder serving as synchronization file source.
        /// </summary>
        public void AddSourceFolder(string folder)
        {
            int relativePathStartIndex = folder.Length;
            var dirInfo = new DirectoryInfo(folder);

            //add new folders
            var foundFolders = dirInfo.GetDirectories("*", SearchOption.AllDirectories);
            foreach (var foundFolder in foundFolders)
            {
                var relativeFolder = foundFolder.FullName.Substring(relativePathStartIndex);

                if (!folders.Contains(relativeFolder))
                {
                    folders.Add(relativeFolder);
                }
            }

            //add files
            var foundFiles = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var foundFile in foundFiles)
            {
                var ff = new SyncFile(foundFile, relativePathStartIndex);

                if (!files.ContainsKey(ff.RelativePath))
                {
                    files.Add(ff.RelativePath, ff);
                }
            }
        }
    }
}
