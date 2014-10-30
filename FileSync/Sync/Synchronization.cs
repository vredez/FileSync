using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync.Sync
{
    class Synchronization
    {
        /// <summary>
        /// The necessary sync actions.
        /// </summary>
        List<SyncAction> actions;

        /// <summary>
        /// Gets all necessary synchronization actions.
        /// </summary>
        public IReadOnlyList<ISyncAction> Actions
        {
            get { return actions; }
        }

        /// <summary>
        /// This event reports the synchronization progress and provides cancellation functionality.
        /// </summary>
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// This event reports errors as they occur.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorOccured;

        void OnProgress(ProgressEventArgs args)
        {
            if (Progress != null) Progress(this, args);
        }

        void OnErrorOccured(ErrorEventArgs args)
        {
            if (ErrorOccured != null) ErrorOccured(this, args);
        }

        private Synchronization(SyncSource source, IEnumerable<string> targetDirectories)
        {
            actions = new List<SyncAction>();

            //prepare folder sync
            foreach (var folder in source.Folders)
            {
                foreach (var target in targetDirectories)
                {
                    var newFolder = Path.Combine(target, folder);

                    if (!Directory.Exists(newFolder))
                    {
                        actions.Add(new SyncAction(newFolder));
                    }
                }
            }
            
            //prepare file sync
            foreach (var file in source.Files)
            {
                foreach (var target in targetDirectories)
                {
                    var newFile = new FileInfo(Path.Combine(target, file.RelativePath));
                    
                    if(newFile.Exists)
                    {
                        if(newFile.LastWriteTimeUtc != file.NewestVersion.LastWriteTimeUtc)
                        {
                            actions.Add(new SyncAction(file.NewestVersion.FullName, newFile.FullName, true));
                        }
                    }
                    else
                    {
                        actions.Add(new SyncAction(file.NewestVersion.FullName, newFile.FullName, false));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new synchronzation object.
        /// </summary>
        /// <param name="source">The source filetree of the synchronization.</param>
        /// <param name="targetDirectories">The directories the source filetree shall be applied to.</param>
        /// <returns></returns>
        public static Synchronization FromSource(SyncSource source, IEnumerable<string> targetDirectories)
        {
            return new Synchronization(source, targetDirectories);
        }

        /// <summary>
        /// Executes the synchronization.
        /// </summary>
        public void Execute()
        {
            float currentStep = 0f;
            float maxSteps = actions.Count;

            foreach (var action in actions)
            {
                try
                {
                    action.Execute();
                }
                catch (Exception ex)
                {
                    OnErrorOccured(new ErrorEventArgs(ex));
                }

                //report progress
                var progress = new ProgressEventArgs(++currentStep / maxSteps);
                OnProgress(progress);

                //check cancellation request
                if (progress.Cancel) break;
            }

            //report completion
            OnProgress(new ProgressEventArgs(1f));
        }
    }
}
