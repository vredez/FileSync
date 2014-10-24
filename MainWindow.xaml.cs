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

namespace FileSync
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<TargetFolder> folders;

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
        }
    }
}
