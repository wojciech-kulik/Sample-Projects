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
using System.Data.Linq;
namespace ClientsManager.ViewModels
{
    public class ClientsListViewModel : BaseViewModel
    {
        private string _currentSearchPhrase;
        private object _syncRoot = new object();
        public ClientsListViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
            Clients = new BindableCollection<Client>();
        }

        #region bindable properties

        private BindableCollection<Client> _clients;

        public BindableCollection<Client> Clients
        {
            get
            {
                return _clients;
            }
            set
            {
                if (_clients != value)
                {
                    _clients = value;
                    NotifyOfPropertyChange(() => Clients);
                }
            }
        }

        private string _searchPhrase;

        public string SearchPhrase
        {
            get
            {
                return _searchPhrase;
            }
            set
            {
                if (_searchPhrase != value)
                {
                    _searchPhrase = value;
                    RefreshClients(_searchPhrase);
                    NotifyOfPropertyChange(() => SearchPhrase);
                }
            }
        }

        #endregion

        #region lifecycle

        protected override void OnActivate()
        {
            base.OnActivate();
            SearchPhrase = "";
            RefreshClients();
        }

        #endregion

        #region operations

        public void CriteriaList()
        {
            _navigationService.UriFor<ClientsCriteriaViewModel>().Navigate();
        }

        public void ShowBirthdate(Client client)
        {
            MessageBox.Show(((DateTime)client.Birthdate).ToString("M/d/yyyy"));
        }

        public void SkyDriveBackup()
        {
            _navigationService.UriFor<SkyDriveFolderChooserViewModel>().Navigate();
        }

        public void ShowPhoto(Client client)
        {
            _navigationService.UriFor<PhotoViewModel>().WithParam(x => x.ClientID, client.Id).Navigate();
        }

        public void Groups()
        {
            _navigationService.UriFor<GroupsListViewModel>().Navigate();
        }

        public void Add()
        {
            _navigationService.UriFor<ClientEditViewModel>().WithParam(x => x.IsAdding, true).Navigate();
        }

        public void Edit(Client client)
        {
            _navigationService.UriFor<ClientEditViewModel>().WithParam(x => x.ClientId, client.Id).Navigate();
        }

        public void Delete(Client client)
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                var clientEntity = dataContext.Table<Client>().First(x => x.Id == client.Id);
                IsoStorageHelper.DeleteFile(clientEntity.Photo);
                dataContext.DeleteOnSubmit(clientEntity);
                dataContext.SubmitChanges();
            }
            Clients.Remove(client);
        }

        public void ClientUnchecked(Client client)
        {
            ChangeActiveStatus(client,false);
        }

        public void ClientChecked(Client client)
        {
            ChangeActiveStatus(client,true);
        }

        #endregion

        #region private

        private void RefreshClients(string searchPhrase = null)
        {           
            //potentially time consuming loading
            Task.Run(() =>
            {
                lock (_syncRoot)
                {
                    Clients.Clear();
                    _currentSearchPhrase = searchPhrase;
                }

                using (IDataContextWrapper dataContext = _dataContextLocator())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Client>(c => c.Group);
                    dataContext.LoadOptions = options;

                    BindableCollection<Client> clientsList;
                    if (searchPhrase != null)
                        clientsList = new BindableCollection<Client>(dataContext.Table<Client>()
                            .Where(x => x.Surname.Contains(searchPhrase) || x.Forename.Contains(searchPhrase))
                            .ToList()
                            .OrderBy(c => c.FullName));
                    else
                        clientsList = new BindableCollection<Client>(dataContext.Table<Client>().ToList().OrderBy(c => c.FullName));

                    lock (_syncRoot)
                    {
                        if (searchPhrase == _currentSearchPhrase)
                        {
                            Clients = clientsList;
                        }
                    }
                }
            });
        }

        private void ChangeActiveStatus(Client client, bool isActive)
        {      
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                var clientEntity = dataContext.Table<Client>().First(x => x.Id == client.Id);
                clientEntity.IsActive = isActive;
                dataContext.SubmitChanges();
            }
        }

        #endregion
    }
}
