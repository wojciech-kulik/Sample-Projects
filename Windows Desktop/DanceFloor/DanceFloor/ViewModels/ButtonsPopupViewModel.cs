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
    public class ButtonsPopupViewModel : BaseViewModel, IPopup, IHandle<GameKeyEvent>
    {
        ButtonsPopupView _view;
        int _selectedIndex = 0;
        List<MenuButton> controls = new List<MenuButton>();

        public bool IsShowing { get; set; }

        public int PopupId { get; set; }

        public bool CanCancel { get; set; }

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

        #region Buttons

        private List<string> _buttons;

        public List<string> Buttons
        {
            get
            {
                return _buttons;
            }
            set
            {
                if (_buttons != value)
                {
                    _buttons = value;
                    NotifyOfPropertyChange(() => Buttons);
                }
            }
        }
        #endregion        

        public ButtonsPopupViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            Buttons = new List<string>();
            CanCancel = true;
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as ButtonsPopupView;

            for (int i = 0; i < Buttons.Count; i++)
            {
                var btn = new Controls.MenuButton() { Text = Buttons[i] };
                _view.btnsContainer.Children.Add(btn);
                controls.Add(btn);
            }

            if (controls.Count > 0)
                controls.First().ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;

            _view.Height = 120 + Buttons.Count * 70;
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsShowing)
                return;

            if ((message.PlayerAction == PlayerAction.Up || message.PlayerAction == PlayerAction.Down) && controls.Count > 0)
            {
                controls[_selectedIndex].ButtonBackground = GameUIConstants.PopupBtnBackground;

                if (message.PlayerAction == PlayerAction.Up)
                {
                    _selectedIndex--;
                    if (_selectedIndex < 0)
                        _selectedIndex = controls.Count - 1;
                }
                else
                {
                    _selectedIndex = (_selectedIndex + 1) % controls.Count;
                }

                controls[_selectedIndex].ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;
            }
            else if (message.PlayerAction == PlayerAction.Enter)
            {
                _eventAggregator.Publish(new ButtonsPopupEvent() 
                {
                    SelectedButton = _selectedIndex + 1,
                    PopupId = this.PopupId
                });
            }
            else if (message.PlayerAction == PlayerAction.Back && CanCancel)
            {
                _eventAggregator.Publish(new ButtonsPopupEvent() { IsCanceled = true, PopupId = this.PopupId });
            }
        }
    }
}
