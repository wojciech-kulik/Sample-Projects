using Caliburn.Micro;
using Common;
using GameLayer;
using DanceFloor.Constants;
using DanceFloor.Controls;
using DanceFloor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DanceFloor.ViewModels
{
    public class MenuViewModel : BaseViewModel, IHandle<GameKeyEvent>, IHandle<ClosingPopupEvent>
    {
        MenuView _view;
        int _activeButton = 0;
        int _buttonsCount;

        public MenuViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as MenuView;

            _buttonsCount = _view.menuPanel.Children.OfType<MenuButton>().Count();
            _activeButton = 0;
            ActivateButton(_activeButton);
        }

        private void ActivateButton(int index)
        {
            foreach (var b in _view.menuPanel.Children.OfType<MenuButton>())
            {
                b.ButtonBackground = GameUIConstants.MainMenuBtnGradient;
            }

            var button = _view.menuPanel.Children.OfType<MenuButton>().Skip(index).First();
            button.ButtonBackground = GameUIConstants.MainMenuSelectedBtnGradient;
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsActive || IsPopupShowing)
                return;
            
            if (message.PlayerAction == PlayerAction.Enter)
            {
                switch(_activeButton)
                {
                    case 0:
                        _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.SongsList });
                        break;
                    case 1:
                        _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.RecordOptions });
                        break;
                    case 2:
                        CloseGame();
                        break;
                }
            }
            else if (message.PlayerAction == PlayerAction.Back)
            {
                CloseGame();
            }
            else if (message.PlayerAction == PlayerAction.Down)
            {
                _activeButton++;
                if (_activeButton >= _buttonsCount)
                    _activeButton = 0;
                ActivateButton(_activeButton);
            }
            else if (message.PlayerAction == PlayerAction.Up)
            {
                _activeButton--;
                if (_activeButton < 0)
                    _activeButton = _buttonsCount - 1;
                ActivateButton(_activeButton);
            }
        }

        private void CloseGame()
        {
            IsPopupShowing = true;
            _eventAggregator.Publish(new ShowPopupEvent() { PopupType = PopupType.ClosingPopup});
        }

        public void Handle(ClosingPopupEvent message)
        {
            if (!IsActive)
                return;

            IsPopupShowing = false;
            if (message.YesSelected)
                Application.Current.MainWindow.Close();
        }
    }
}
