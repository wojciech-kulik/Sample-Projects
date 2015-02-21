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
    public class GroupAddViewModel : BaseViewModel
    {
        public GroupAddViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
        }

        #region lifecycle

        protected override void LoadState()
        {
            GroupName = (string)TombstoningContainer.GetValue(TombstoningVariables.GroupAddGroupName);
        }

        protected override void SaveState()
        {
            TombstoningContainer.SetValue(TombstoningVariables.GroupAddGroupName, GroupName);
        }

        #endregion

        #region bindable properties

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

        #endregion

        #region operations

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(GroupName))
            {                
                using (IDataContextWrapper dataContext = _dataContextLocator())
                {
                    dataContext.InsertOnSubmit(new Group() { GroupName = this.GroupName });                   
                    dataContext.SubmitChanges();
                }
                GoBack();
            }
            else
            {
                MessageBox.Show("Group must have GroupName");
            }
        }

        public void Cancel()
        {
            GoBack();
        }

        #endregion

    }
}
