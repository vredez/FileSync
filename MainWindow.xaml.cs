using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using FileSync.Sync;
using IO = System.IO;

namespace FileSync
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<TargetFolder> folders;
        Synchronization sync;

        bool cancel;

        public ObservableCollection<TargetFolder> Folders
        {
            get
            {
                if (folders == null) folders = new ObservableCollection<TargetFolder>();
                return folders;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Folder button behaviour.
        /// </summary>
        void OnFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender == button_add) // Add
            {
                using (var fbd = new WinForms.FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = true;
  
                    if (fbd.ShowDialog() == WinForms.DialogResult.OK)
                    {
                        if (!Folders.Any(f => f.Path == fbd.SelectedPath))
                        {
                            Folders.Add(new TargetFolder { Path = fbd.SelectedPath });
                        }
                    }
                }
            }
            else if (sender == button_remove) // Remove
            {
                object[] selection = new object[listview_folders.SelectedItems.Count];
                listview_folders.SelectedItems.CopyTo(selection, 0);
                foreach (TargetFolder folder in selection)
                {
                    Folders.Remove(folder);
                }
            }
            else if (sender == button_clear) // Clear
            {
                Folders.Clear();
            }

            button_execute.IsEnabled = false;
        }

        void OnAnalyze(object sender, RoutedEventArgs e)
        {
            var source = new SyncSource();
            var syncTargets = new List<string>();

            foreach (var folder in Folders)
            {
                source.AddSourceFolder(folder.Path);
                if (!folder.ReadOnly)
                {
                    syncTargets.Add(folder.Path);
                }
            }

            sync = Synchronization.FromSource(source, syncTargets);
            sync.Progress += OnSyncProgress;

            listview_actions.ItemsSource = sync.Actions;

            button_execute.Content = string.Format("Execute {0} Action{1}", sync.Actions.Count, sync.Actions.Count != 1 ? "s" : string.Empty);
            button_execute.IsEnabled = true;
        }

        void OnExecute(object sender, RoutedEventArgs e)
        {
            progressbar.Value = 0;
            progressbar.Visibility = System.Windows.Visibility.Visible;
            button_execute.Content = "Cancel";
            
            button_execute.Click -= OnExecute;
            button_execute.Click += OnCancel;

            cancel = false;

            Task.Run(() =>
            {
                sync.Execute();
            });
        }

        void OnSyncProgress(object sender, ProgressEventArgs e)
        {
            e.Cancel = cancel;
            Dispatcher.Invoke(() =>
            {
                progressbar.Value = e.Progress * progressbar.Maximum;

                if (e.Progress == 1f)
                {
                    button_execute.Click -= OnCancel;
                    button_execute.Click += OnExecute;

                    button_execute.Content = "Execute";
                    button_execute.IsEnabled = false;
                    progressbar.Visibility = System.Windows.Visibility.Hidden;
                }
            });
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            cancel = true;
            button_execute.IsEnabled = false;
        }

        void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var folders = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var folder in folders)
                {
                    if (!IO.Directory.Exists(folder))
                    {
                        return;
                    }
                }
                
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
        }

        void OnDrop(object sender, DragEventArgs e)
        {
            if (((int)e.Effects & (int)DragDropEffects.Link) != (int)DragDropEffects.None)
            {
                var folders = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var folder in folders)
                {
                    if (!Folders.Any(f => f.Path == folder))
                    {
                        Folders.Add(new TargetFolder { Path = folder, ReadOnly = false });
                    }
                }
            }
        }
    }
}
