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
using System.Windows.Media.Imaging;
using ClientsManager.FilesAccess;

namespace ClientsManager.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        public PhotoViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
        }

        public long ClientID { get; set; }

        #region bindable properties

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

        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    NotifyOfPropertyChange(() => FullName);
                }
            }
        }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                Client client = dataContext.Table<Client>().FirstOrDefault(x => x.Id == ClientID);
                if (client != null)
                {
                    FullName = client.FullName;
                    Photo = IsoStorageHelper.PhotoFromFile(client.Photo);
                }
            }
        }
    }
}
