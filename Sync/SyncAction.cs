using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync.Sync
{
    /// <summary>
    /// Different types of synchronizaton actions.
    /// </summary>
    public enum SyncActionType
    {
        CreateDirectory = 0,
        CopyFile = 1,
        OverwriteFile = 2
    }

    /// <summary>
    /// Interface providing information about a synchronization action.
    /// </summary>
    public interface ISyncAction
    {
        SyncActionType Type { get; }
        String Source { get; }
        string Destination { get; }
        string Description { get; }
    }

    /// <summary>
    /// Represents a synchronization action.
    /// </summary>
    public class SyncAction : ISyncAction
    {
        string source;
        string destination;

        public string Source
        {
            get { return source; }
        }

        public string Destination
        {
            get { return destination; }
        }

        public string Description
        {
            get
            {
                switch (Type)
                {
                    case SyncActionType.CreateDirectory:
                        return string.Format("Creates the directory \"{0}\"", destination);
                    case SyncActionType.OverwriteFile:
                        return string.Format("Overwrites the file \"{0}\" with \"{1}\"", destination, source);
                    case SyncActionType.CopyFile:
                        return string.Format("Copies the file \"{0}\" to \"{1}\"", source, destination);
                }

                return ToString();
            }
        }


        /// <summary>
        /// The type of the sync action.
        /// </summary>
        public SyncActionType Type { get; private set; }

        /// <summary>
        /// Instanciates an directory creation action.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        public SyncAction(string directory)
        {
            destination = directory;
            Type = SyncActionType.CreateDirectory;
        }

        /// <summary>
        /// Instanciates a file copy or overwrite action.
        /// </summary>
        /// <param name="source">The source file.</param>
        /// <param name="destination">The file destination.</param>
        /// <param name="overwrite">True if the action is overwriting.</param>
        public SyncAction(string source, string destination, bool overwrite = true)
        {
            this.source = source;
            this.destination = destination;
            Type = overwrite ? SyncActionType.OverwriteFile : SyncActionType.CopyFile;
        }
        
        /// <summary>
        /// Executes the action.
        /// </summary>
        public void Execute()
        {
            switch (Type)
            {
                case SyncActionType.CreateDirectory:
                    Directory.CreateDirectory(destination);
                    return;
                default:
                    File.Copy(source, destination, true);
                    return;
            }
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
