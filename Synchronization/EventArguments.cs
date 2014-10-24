using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Synchronization
{
    /// <summary>
    /// Arguments provding progress infomation and cancellation functionality. 
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// If set to true, the process will be cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// The progress from 0 to 1 of the process.
        /// </summary>
        public float Progress { get; private set; }

        public ProgressEventArgs(float progress) :
            base()
        {
            Progress = progress;
            Cancel = false;
        }
    }

    /// <summary>
    /// Arguments providing exception information.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception ex)
            : base()
        {
            Exception = ex;
        }
    }
}
