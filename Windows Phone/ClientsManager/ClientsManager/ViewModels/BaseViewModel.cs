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
using ClientsManager.Tombstoning;

namespace ClientsManager.ViewModels
{
    public class BaseViewModel : Screen
    {
        protected INavigationService _navigationService;
        protected Func<IDataContextWrapper> _dataContextLocator;
        bool _creating = true;
        public BaseViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
        {
            _navigationService = navigationService;
            _dataContextLocator = dataContextLocator;
        }

        protected override void OnActivate()
        {
            bool resurecting = ClientsManagerBootStrapper.Resurecting;

            ClientsManagerBootStrapper.Resurecting = false;
            base.OnActivate();
            if (resurecting)
            {
                LoadState();
            }
            else if (_creating)
            {
                OnCreated();
                _creating = false;
            }

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (!close)
            {
                SaveState();
            }
        }

        protected virtual void LoadState()
        {

        }

        protected virtual void SaveState()
        {
            
        }

        protected virtual void OnCreated()
        {

        }

        protected void GoBack()
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }
}
