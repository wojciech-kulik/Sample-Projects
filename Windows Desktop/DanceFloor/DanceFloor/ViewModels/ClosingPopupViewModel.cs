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
    public class ClosingPopupViewModel : BaseViewModel, IPopup, IHandle<GameKeyEvent>
    {
        ClosingPopupView _view;
        Controls.MenuButton _selectedBtn;

        public bool IsShowing { get; set; }

        public ClosingPopupViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as ClosingPopupView;
            _selectedBtn = _view.btnYes;
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsShowing)
                return;

            if (message.PlayerAction == PlayerAction.Left || message.PlayerAction == PlayerAction.Right)
            {
                _selectedBtn.ButtonBackground = GameUIConstants.PopupBtnBackground;
                _selectedBtn = _selectedBtn == _view.btnYes ? _view.btnNo : _view.btnYes;
                _selectedBtn.ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;
            }
            else if (message.PlayerAction == PlayerAction.Enter || message.PlayerAction == PlayerAction.Back)
            {
                _eventAggregator.Publish(new ClosingPopupEvent() { YesSelected = (message.PlayerAction == PlayerAction.Enter && _selectedBtn == _view.btnYes) });
            }
        }
    }
}
