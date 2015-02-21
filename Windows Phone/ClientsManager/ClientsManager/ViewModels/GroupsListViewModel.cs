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
using System.Data.Linq;

namespace ClientsManager.ViewModels
{
    public class GroupsListViewModel : BaseViewModel
    {
        private object _syncRoot = new object();
        public GroupsListViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
            Groups = new BindableCollection<Group>();            
        }

        #region bindable properties

        private BindableCollection<Group> _groups;

        public BindableCollection<Group> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                if (_groups != value)
                {
                    _groups = value;
                    NotifyOfPropertyChange(() => Groups);
                }
            }
        }
        #endregion

        #region lifecycle

        protected override void OnActivate()
        {
            base.OnActivate();
            RefreshGroups();
        }

        #endregion

        #region operations

        public void Add()
        {
            _navigationService.UriFor<GroupAddViewModel>().Navigate();
        }

        public void DeleteGroup(Group group)
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                Group groupRecord = dataContext.Table<Group>().FirstOrDefault(x => x.Id == group.Id);
                dataContext.DeleteOnSubmit(groupRecord);
                dataContext.Table<Client>().Apply(x => { if (x.GroupId == group.Id) x.Group = null; });
                dataContext.SubmitChanges();
            }

            Groups.Remove(group);
        }

        public void AddClientToGroup(Group group)
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
                if (dataContext.Table<Client>().Where(c => c.GroupId == null).Count() == 0)
                {
                    MessageBox.Show("There are no clients without a group.");
                    return;
                }

            _navigationService.UriFor<GroupAddClientViewModel>().WithParam(g => g.GroupId, group.Id).Navigate();
        }

        public void RemoveClientFromGroup(Client client)
        {
            using (IDataContextWrapper dataContext = _dataContextLocator())
            {
                var clientRecord = dataContext.Table<Client>().FirstOrDefault(x => x.Id == client.Id);
                clientRecord.Group.Clients.Remove(clientRecord); //automatically detach client from group
                dataContext.SubmitChanges();
            }

            Groups.First(x => x.Id == client.GroupId).Clients.Remove(client);
        }


        #endregion

        #region private

        private void RefreshGroups()
        {
            Task.Run(() =>
            {
                lock (_syncRoot)
                {
                    using (IDataContextWrapper dataContext = _dataContextLocator())
                    {
                        Groups.Clear();

                        DataLoadOptions options = new DataLoadOptions();
                        options.LoadWith<Group>(g => g.Clients);
                        dataContext.LoadOptions = options;

                        dataContext.Table<Group>()
                            .OrderBy(x => x.GroupName)
                            .ToList()
                            .ForEach(g => Groups.Add(g));
                    }
                }
            });
        }
        #endregion
    }
}
