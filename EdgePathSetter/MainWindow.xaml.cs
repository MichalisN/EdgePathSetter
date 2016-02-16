using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;

namespace EdgePathSetter
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private RegistryKey _edgeMain;
        private string _currentPath;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        private void FindAndSetPath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select default download directory for Microsoft Edge";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetEdgePath(dialog.SelectedPath);
                }
            }
        }

        private void SetEdgePath(string path)
        {
            try
            {
                _edgeMain.SetValue("Default Download Directory", path);
                CurrentPath = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetEdgePath()
        {
            try
            {
                _edgeMain.DeleteValue("Default Download Directory");
                CurrentPath = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var software = Registry.CurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);

                _edgeMain = software.OpenSubKey(@"Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppContainer\Storage\microsoft.microsoftedge_8wekyb3d8bbwe\MicrosoftEdge\Main", true);
                CurrentPath = _edgeMain.GetValue("Default Download Directory") as string;
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Registry value could not be found, application will close", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void ChangeEvent(object sender, RoutedEventArgs e)
        {
            FindAndSetPath();
        }

        private void ResetEvent(object sender, RoutedEventArgs e)
        {
            ResetEdgePath();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}