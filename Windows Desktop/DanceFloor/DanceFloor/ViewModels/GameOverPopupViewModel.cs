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
    public class GameOverPopupViewModel : BaseViewModel, IPopup, IHandle<GameKeyEvent>
    {
        GameOverPopupView _view;
        Controls.MenuButton _selectedBtn;

        public bool IsShowing { get; set; }

        #region Message

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }
        #endregion

        public GameOverPopupViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as GameOverPopupView;
            _selectedBtn = _view.btnPlayAgain;
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsShowing)
                return;

            if (message.PlayerAction == PlayerAction.Up || message.PlayerAction == PlayerAction.Down)
            {
                _selectedBtn.ButtonBackground = GameUIConstants.PopupBtnBackground;
                _selectedBtn = _selectedBtn == _view.btnPlayAgain ? _view.btnExit : _view.btnPlayAgain;
                _selectedBtn.ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;
            }
            else if (message.PlayerAction == PlayerAction.Enter)
            {
                _eventAggregator.Publish(new GameOverPopupEvent() { PlayAgainSelected = _selectedBtn == _view.btnPlayAgain });
            }
        }
    }
}
