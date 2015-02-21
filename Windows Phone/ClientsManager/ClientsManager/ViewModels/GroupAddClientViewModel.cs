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

namespace ClientsManager.ViewModels
{
    public class GroupAddClientViewModel : BaseViewModel
    {
        public GroupAddClientViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
        }

        #region navigation properties

        public long GroupId { get; set; }

        #endregion

        #region lifecycle

        protected override void OnCreated()
        {
            base.OnCreated();

            using (IDataContextWrapper dataContext = _dataContextLocator())
                GroupName = dataContext.Table<Group>().First(g => g.Id == GroupId).GroupName;

            RefreshClients();
            SelectedClient = ClientsList.FirstOrDefault();
        }

        protected override void LoadState()
        {
            GroupName = (string)TombstoningContainer.GetValue(TombstoningVariables.GroupAddClientGroupName);
            RefreshClients();
            long id = (long)TombstoningContainer.GetValue(TombstoningVariables.GroupAddClientSelectedClient);
            SelectedClient = ClientsList.FirstOrDefault(c => c.Id == id);
        }

        protected override void SaveState()
        {
            TombstoningContainer.SetValue(TombstoningVariables.GroupAddClientGroupName, GroupName);
            TombstoningContainer.SetValue(TombstoningVariables.GroupAddClientSelectedClient, SelectedClient.Id);
        }

        #endregion

        #region bindable properties

        private Client _selectedClient;

        public Client SelectedClient
        {
            get
            {
                return _selectedClient;
            }
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    NotifyOfPropertyChange(() => SelectedClient);
                }
            }
        }

        private string _groupName;

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    NotifyOfPropertyChange(() => GroupName);
                }
            }
        }

        private BindableCollection<Client> _clientsList;

        public BindableCollection<Client> ClientsList
        {
            get
            {
                return _clientsList;
            }
            set
            {
                if (_clientsList != value)
                {
                    _clientsList = value;
                    NotifyOfPropertyChange(() => ClientsList);
                }
            }
        }

        #endregion

        #region operations

        public void Add()
        {
            if (SelectedClient != null)
            {                
                using (IDataContextWrapper dataContext = _dataContextLocator())
                {
                    Group group = dataContext.Table<Group>().First(g => g.Id == GroupId);
                    dataContext.Table<Client>().First(c => c.Id == SelectedClient.Id).Group = group;                  
                    dataContext.SubmitChanges();
                }
                GoBack();
            }
            else
            {
                MessageBox.Show("Client must be selected.");
            }
        }

        public void Cancel()
        {
            GoBack();
        }

        public void RefreshClients()
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
                ClientsList = new BindableCollection<Client>(dataContext.Table<Client>().Where(c => c.GroupId == null).ToList().OrderBy(c => c.FullName));
        }

        #endregion

    }
}
