using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ClientsManager.Storage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ClientsManager.FilesAccess;
using Microsoft.Live;
using Microsoft.Live.Controls;
using System.IO.IsolatedStorage;

namespace ClientsManager.ViewModels
{
    public class SkyDriveFolder : EntityBase
    {
        #region Name

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        #endregion

        #region ID

        private string _ID;

        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    NotifyPropertyChanging("ID");
                    _ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }


        #endregion
    }

    public class SkyDriveFolderChooserViewModel : BaseViewModel
    {
        private SkyDriveManager _skyDriveManager;
        Stack<string> _navHistory = new Stack<string>();
        private string _currentFolderID;

        public SkyDriveFolderChooserViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
            Folders = new BindableCollection<SkyDriveFolder>();
            _skyDriveManager = new SkyDriveManager();
            SignInRequired = false;
            NoFolders = false;
        }


        #region redirection to SkyDriveCurrentStateManager

        public void btnSignin_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (_skyDriveManager.RegisterSessionChange(sender, e))
            {
                FolderPath = "/SkyDrive/";
                SignInRequired = false;
                NoFolders = false;
                _skyDriveManager.GetFolderAsync("/me/skydrive", GetFolderCallback);
                _currentFolderID = "/me/skydrive";
            }
            else
            {
                _navHistory.Clear();

                RestartNavInfo();
                FolderPath = "/SkyDrive/";
                Folders.Clear();
                SignInRequired = true;
                NoFolders = false;
            }
        }

        #endregion

        #region bindable properties

        private BindableCollection<SkyDriveFolder> _folders;

        public BindableCollection<SkyDriveFolder> Folders
        {
            get
            {
                return _folders;
            }
            set
            {
                if (_folders != value)
                {
                    _folders = value;
                    NotifyOfPropertyChange(() => Folders);
                }
            }
        }


        private bool _showLoading;

        public bool ShowLoading
        {
            get
            {
                return _showLoading;
            }
            set
            {
                if (_showLoading != value)
                {
                    _showLoading = value;
                    NotifyOfPropertyChange(() => ShowLoading);
                }
            }
        }

        private string _folderPath;

        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
            set
            {
                if (_folderPath != value)
                {
                    _folderPath = value;
                    NotifyOfPropertyChange(() => FolderPath);
                }
            }
        }

        private bool _signInRequired;

        public bool SignInRequired
        {
            get
            {
                return _signInRequired;
            }
            set
            {
                if (_signInRequired != value)
                {
                    _signInRequired = value;
                    NotifyOfPropertyChange(() => SignInRequired);
                }
            }
        }

        private bool _noFolders;

        public bool NoFolders
        {
            get
            {
                return _noFolders;
            }
            set
            {
                if (_noFolders != value)
                {
                    _noFolders = value;
                    NotifyOfPropertyChange(() => NoFolders);
                }
            }
        }
        #endregion

        #region operations

        private bool GoingBack;
        private string NavFolderName;
        internal void RestartNavInfo()
        {
            GoingBack = false;
            NavFolderName = "";
        }

        internal void UpdateFolderPath()
        {
            if (GoingBack)
            {
                string folder = FolderPath.Substring(0, FolderPath.Length - 1);
                FolderPath = folder.Substring(0, folder.LastIndexOf('/') + 1);
            }
            else
                if (!String.IsNullOrEmpty(NavFolderName))
                    FolderPath += NavFolderName + "/";
        }

        public void SkyDriveNavigate(SkyDriveFolder skyFile)
        {
            if (_skyDriveManager.GetFolderAsync(skyFile.ID, GetFolderCallback))
            {
                _navHistory.Push(_currentFolderID);
                GoingBack = false;
                NavFolderName = skyFile.Name;

                _currentFolderID = skyFile.ID;
            }
        }

        public void SkyDriveGoBack()
        {
            if (!_skyDriveManager.IsWorking)
            {
                if (_navHistory.Count == 0)
                    GoBack();
                else
                {
                    GoingBack = true;
                    _currentFolderID=_navHistory.Pop();
                    _skyDriveManager.GetFolderAsync(_currentFolderID, GetFolderCallback);
                }
            }
        }

        private void GetFolderCallback(SkyDriveResult< List<SkyDriveFolder>> result)
        {
            if (result.Succeded)
            {
                Folders.Clear();
                UpdateFolderPath();
                Folders.AddRange(result.Result);
            }
            else
            {
                MessageBox.Show("Exception occured while getting folder");
            }
            ShowLoading = false;
            NoFolders = Folders.Count == 0;
        }

        public void Cancel()
        {
            GoBack();
        }

        private IsolatedStorageFileStream dbStream;
        public void SaveBackup()
        {
            if (!_skyDriveManager.Logged)
            {
                MessageBox.Show("You must be logged to SkyDrive to backup your data.", "Not logged", MessageBoxButton.OK);
            }
            else if (String.IsNullOrEmpty(_currentFolderID))
            {
                MessageBox.Show("None of the folders have been chosen.");
            }
            else if (MessageBox.Show("Do you want to upload backup to:\n" + FolderPath + " ?", "Backup", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                dbStream = IsoStorageHelper.GetDatabaseStream();
                _skyDriveManager.Upload(_currentFolderID, "ClientsManager.sdf", dbStream, OverwriteOption.Rename, UploadCallback);
                ShowLoading = true;
            }
        }

        void UploadCallback(SkyDriveResult<bool> result)
        {
            ShowLoading = false;
            dbStream.Close();
            IsoStorageHelper.DeleteFile("ClientsManager_backup.sdf");

            if (result.Succeded)
            {                
                GoBack();
                MessageBox.Show("Backup has been uploaded.");
            }
            else
                MessageBox.Show("Error occured while uploading backup: \n" + result.Exception.Message);
        }
     
        #endregion
    }
}
