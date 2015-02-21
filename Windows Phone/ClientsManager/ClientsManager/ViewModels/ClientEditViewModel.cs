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
using System.Linq;
using ClientsManager.Tombstoning;
using Microsoft.Phone.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using ClientsManager.FilesAccess;

namespace ClientsManager.ViewModels
{
    public class ClientEditViewModel : BaseViewModel
    {
        public ClientEditViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator) 
            : base(navigationService, dataContextLocator)
        {            
        }

        #region navigation properties

        public bool IsAdding { get; set; }
        public long ClientId { get; set; }

        #endregion

        #region lifecycle

        protected override void OnCreated()
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                Group noneGroup = new Group() { Id = -1, GroupName = "None" };
                GroupsList = new BindableCollection<Group>(dataContext.Table<Group>().OrderBy(g => g.GroupName).ToList());
                GroupsList.Insert(0, noneGroup);

                if (!IsAdding)
                {
                    var client = dataContext.Table<Client>().First(x => x.Id == ClientId);
                    Forename = client.Forename;
                    Surname = client.Surname;
                    Birthdate = client.Birthdate;
                    Latitude = client.Latitude;
                    Longitude = client.Longitude;
                    IsActive = client.IsActive;
                    Group = client.Group ?? noneGroup;
                    Photo = IsoStorageHelper.PhotoFromFile(client.Photo);
                }
                else
                {
                    IsActive = true;
                    Group = noneGroup;
                }
            }
        }

        protected override void LoadState()
        {

            Forename = (string)TombstoningContainer.GetValue(TombstoningVariables.ClientEditForename);
            Surname = (string)TombstoningContainer.GetValue(TombstoningVariables.ClientEditSurname);
            Birthdate = (DateTime?)TombstoningContainer.GetValue(TombstoningVariables.ClientEditBirthdate);
            Longitude = (double?)TombstoningContainer.GetValue(TombstoningVariables.ClientEditLongitude);
            Latitude = (double?)TombstoningContainer.GetValue(TombstoningVariables.ClientEditLatitude);
            IsActive = (bool)TombstoningContainer.GetValue(TombstoningVariables.ClientEditIsActive);
            lastPhotoName = (string)TombstoningContainer.GetValue(TombstoningVariables.ClientEditLastPhotoName);
            Photo = ImageConverter.ConvertToImage((byte[])TombstoningContainer.GetValue(TombstoningVariables.ClientEditPhoto));

            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                Group noneGroup = new Group() { Id = -1, GroupName = "None" };
                GroupsList = new BindableCollection<Group>(dataContext.Table<Group>().OrderBy(g => g.GroupName).ToList());
                GroupsList.Insert(0, noneGroup);

                long id = (long)TombstoningContainer.GetValue(TombstoningVariables.ClientEditGroupId);
                Group = GroupsList.FirstOrDefault(x => x.Id == id) ?? noneGroup;
            }
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += getPhotoComplete;

        }

        protected override void SaveState()
        {
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditForename, Forename);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditSurname, Surname);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditBirthdate, Birthdate);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditLongitude, Longitude);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditLatitude, Latitude);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditIsActive, IsActive);
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditGroupId, (Group != null ? Group.Id : -1));
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditPhoto, ImageConverter.ConvertToBytes(Photo));
            TombstoningContainer.SetValue(TombstoningVariables.ClientEditLastPhotoName, lastPhotoName);
        }

        #endregion

        #region bindable properties

        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyOfPropertyChange(() => IsActive);
                }
            }
        }
        

        private string _forename;

        public string Forename
        {
            get
            {
                return _forename;
            }
            set
            {
                if (_forename != value)
                {
                    _forename = value;
                    NotifyOfPropertyChange(() => Forename);
                }
            }
        }

        private string _surname;

        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    NotifyOfPropertyChange(() => Surname);
                }
            }
        }

        private DateTime? _birthdate;

        public DateTime? Birthdate
        {
            get
            {
                return _birthdate;
            }
            set
            {
                if (_birthdate != value)
                {
                    _birthdate = value;
                    NotifyOfPropertyChange(() => Birthdate);
                }
            }
        }


        private double? _longitude;

        public double? Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    NotifyOfPropertyChange(() => Longitude);
                }
            }
        }


        private double? _latitude;

        public double? Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    NotifyOfPropertyChange(() => Latitude);
                }
            }
        }


        private Group _group;

        public Group Group
        {
            get
            {
                return _group;
            }
            set
            {
                if (_group != value)
                {
                    _group = value;
                    NotifyOfPropertyChange(() => Group);
                }
            }
        }

        private BindableCollection<Group> _groupsList;

        public BindableCollection<Group> GroupsList
        {
            get
            {
                return _groupsList;
            }
            set
            {
                if (_groupsList != value)
                {
                    _groupsList = value;
                    NotifyOfPropertyChange(() => GroupsList);
                }
            }
        }

        private BitmapImage _photo;

        public BitmapImage Photo
        {
            get
            {
                return _photo;
            }
            set
            {
                if (_photo != value)
                {
                    _photo = value;
                    NotifyOfPropertyChange(() => Photo);
                }
            }
        }

        #endregion

        #region operations

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(Forename) || !string.IsNullOrWhiteSpace(Surname))
            {
                if (Latitude != null && (Latitude < -90.0 || Latitude > 90.0))
                {
                    MessageBox.Show("Incorrect latitude.");
                    return;
                }
                if (Longitude != null && (Longitude < -180.0 || Longitude > 180.0))
                {
                    MessageBox.Show("Incorrect longitude.");
                    return;
                }

                using (IDataContextWrapper dataContext = _dataContextLocator())
                {
                    Group selectedGroup = dataContext.Table<Group>().FirstOrDefault(x => x.Id == Group.Id);
                    DateTime? onlyDate = Birthdate.HasValue ? new DateTime?(Birthdate.Value.Date) : Birthdate;

                    if (IsAdding)
                    {
                        Client newClient = new Client() { 
                            Forename = this.Forename, 
                            Surname = this.Surname,
                            Birthdate = onlyDate, 
                            Longitude = this.Longitude,
                            Latitude = this.Latitude,
                            IsActive = this.IsActive, 
                            Group = selectedGroup 
                        };
                        dataContext.InsertOnSubmit(newClient);
                        if (!String.IsNullOrEmpty(lastPhotoName) && Photo != null)
                        {
                            dataContext.SubmitChanges();
                            newClient.Photo = IsoStorageHelper.SavePhoto(Photo, newClient.Id, lastPhotoName);
                        }
                    }
                    else
                    {
                        Client clientToEdit = dataContext.Table<Client>().FirstOrDefault(x => x.Id == ClientId);
                        if (clientToEdit != null)
                        {
                            clientToEdit.Surname = Surname;
                            clientToEdit.Forename = Forename;
                            clientToEdit.Birthdate = onlyDate;
                            clientToEdit.Longitude = Longitude;
                            clientToEdit.Latitude = Latitude;
                            clientToEdit.IsActive = IsActive;
                            clientToEdit.Group = selectedGroup;
                            if (Photo == null)
                            {
                                IsoStorageHelper.DeleteFile(clientToEdit.Photo);  //delete old photo
                                clientToEdit.Photo = null;
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(lastPhotoName)) //if new photo were chosen
                                {
                                    IsoStorageHelper.DeleteFile(clientToEdit.Photo); //delete old photo
                                    clientToEdit.Photo = IsoStorageHelper.SavePhoto(Photo, clientToEdit.Id, lastPhotoName);
                                }
                            }
                        }
                    }

                    dataContext.SubmitChanges();
                }
                GoBack();
            }
            else
                MessageBox.Show("Client must have Forename or Surname");
        }

        public void Cancel()
        {
            GoBack();
        }

        #endregion

        #region Set Photo

        private string lastPhotoName;
        private void getPhotoComplete(object sender, PhotoResult result)
        {
            ((PhotoChooserTask)sender).Completed -= getPhotoComplete;
            if (result.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(result.ChosenPhoto);
                Photo = bmp;

                lastPhotoName = System.IO.Path.GetFileName(result.OriginalFileName);
            }
        }

        public void PhotoFromGallery()
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += getPhotoComplete;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Show(); 
        }

        #endregion
    }
}
