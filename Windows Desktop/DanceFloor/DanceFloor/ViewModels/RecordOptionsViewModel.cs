using Caliburn.Micro;
using Common;
using GameLayer;
using Microsoft.Win32;
using DanceFloor.Constants;
using DanceFloor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DanceFloor.ViewModels
{
    public class RecordOptionsViewModel : BaseViewModel, IHandle<GameKeyEvent>, IHandle<ButtonsPopupEvent>
    {
        #region Song

        private ISong _song;

        public ISong Song
        {
            get
            {
                return _song;
            }
            set
            {
                if (_song != value)
                {
                    _song = value;
                    NotifyOfPropertyChange(() => Song);
                }
            }
        }
        #endregion

        #region Difficulty

        private Difficulty _difficulty;

        public Difficulty Difficulty
        {
            get
            {
                return _difficulty;
            }
            set
            {
                if (_difficulty != value)
                {
                    _difficulty = value;
                    NotifyOfPropertyChange(() => Difficulty);
                }
            }
        }
        #endregion

        IMusicPlayerService _musicPlayerService;
        ISongsService _songsService;
        RecordOptionsView _view;

        bool _difficultySelection = false;
        int _selectedIndex = 0;
        List<UIElement> _selectableControls = new List<UIElement>();        

        public RecordOptionsViewModel(IEventAggregator eventAggregator, ISongsService songsService, IMusicPlayerService musicPlayerService)
            : base(eventAggregator)
        {
            Song = new Song();
            Difficulty = Difficulty.Easy;

            _songsService = songsService;
            _musicPlayerService = musicPlayerService;
        }

        protected override void OnDeactivate(bool close)
        {
            Song = null; //to avoid memory leak
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as RecordOptionsView;

            _view.btnSong.ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;
            _view.btnBackground.ButtonBackground = GameUIConstants.PopupBtnBackground;
            _view.btnCover.ButtonBackground = GameUIConstants.PopupBtnBackground;

            _view.btnEasy.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Easy) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;
            _view.btnMedium.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Medium) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;
            _view.btnHard.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Hard) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;

            _selectableControls.Add(_view.btnSong);
            _selectableControls.Add(_view.btnBackground);
            _selectableControls.Add(_view.btnCover);
            _selectableControls.Add(_view.tbTitle);
            _selectableControls.Add(_view.tbArtist);
            _selectableControls.Add(_view.tbAuthor);
        }

        bool ValidateFields(bool saving = false)
        {
            if (String.IsNullOrWhiteSpace(Song.FilePath))
            {
                IsPopupShowing = true;
                _eventAggregator.Publish(new ShowPopupEvent()
                {
                    PopupType = PopupType.ButtonsPopup,
                    PopupSettings = (vm) => (vm as ButtonsPopupViewModel).Message = "Select a song file."
                });
                return false;
            }

            if (saving)
            {
                if (String.IsNullOrWhiteSpace(Song.Artist) || String.IsNullOrWhiteSpace(Song.Title))
                {
                    IsPopupShowing = true;
                    _eventAggregator.Publish(new ShowPopupEvent()
                    {
                        PopupType = PopupType.ButtonsPopup,
                        PopupSettings = (vm) => (vm as ButtonsPopupViewModel).Message = "Fill a title and artist."
                    });
                    return false;
                }

                if (Song.Sequences.Count == 0)
                {
                    IsPopupShowing = true;
                    _eventAggregator.Publish(new ShowPopupEvent()
                    {
                        PopupType = PopupType.ButtonsPopup,
                        PopupSettings = (vm) => (vm as ButtonsPopupViewModel).Message = "Record a sequence."
                    });
                    return false;
                }
            }

            return true;
        }

        void SetFocusOn(int index)
        {
            if (_selectableControls[index] is Controls.MenuButton)
            {
                (_selectableControls[index] as Controls.MenuButton).ButtonBackground = GameUIConstants.PopupSelectedBtnBackground;
            }
            else
            {
                _selectableControls[index].Focus();
            }
        }

        void SetFocusOn(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Common.Difficulty.Easy:
                    _view.btnEasy.ButtonBackground = GameUIConstants.GameModeSelectedBtnGradient;
                    break;
                case Common.Difficulty.Medium:
                    _view.btnMedium.ButtonBackground = GameUIConstants.GameModeSelectedBtnGradient;
                    break;
                case Common.Difficulty.Hard:
                    _view.btnHard.ButtonBackground = GameUIConstants.GameModeSelectedBtnGradient;
                    break;
            }
        }

        void LoseFocusOn(int index)
        {
            if (_selectableControls[_selectedIndex] is Controls.MenuButton)
            {
                (_selectableControls[_selectedIndex] as Controls.MenuButton).ButtonBackground = GameUIConstants.PopupBtnBackground;
            }
            else
            {
                _view.toLoseFocus.Focus();
            }
        }

        void LoseFocusOn(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Common.Difficulty.Easy:
                    _view.btnEasy.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Easy) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;
                    break;
                case Common.Difficulty.Medium:
                    _view.btnMedium.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Medium) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;
                    break;
                case Common.Difficulty.Hard:
                    _view.btnHard.ButtonBackground = Song.Sequences.ContainsKey(Difficulty.Hard) ? GameUIConstants.RecordCreatedBtnGradient : GameUIConstants.GameModeInactiveBtnGradient;
                    break;
            }   
        }

        void TryNavigateToRecordView()
        {
            if (_difficultySelection)
            {
                if (!ValidateFields())
                    return;

                IsPopupShowing = true;
                _eventAggregator.Publish(new ShowPopupEvent() 
                { 
                    PopupType = PopupType.ButtonsPopup, 
                    PopupSettings = (vm) => 
                    {
                        (vm as ButtonsPopupViewModel).Message = "Random sequence?";
                        (vm as ButtonsPopupViewModel).Buttons.AddRange(new List<string>() { "NO", "YES" });
                        (vm as ButtonsPopupViewModel).PopupId = 2;
                    }
                });
            }
        }

        void UpNavigation()
        {
            if (!_difficultySelection && _selectedIndex == 0)
            {
                _difficultySelection = true;
                LoseFocusOn(_selectedIndex);
                Difficulty = Common.Difficulty.Easy;
                SetFocusOn(Difficulty);
            }
            else if (_difficultySelection && _selectedIndex == 0)
            {
                _difficultySelection = false;
                LoseFocusOn(Difficulty);
                _selectedIndex = _selectableControls.Count - 1;
                SetFocusOn(_selectedIndex);
            }
            else if (_difficultySelection)
            {
                LoseFocusOn(Difficulty);
                _difficultySelection = false;
                SetFocusOn(_selectedIndex);
            }
            else
            {
                LoseFocusOn(_selectedIndex);
                _selectedIndex--;
                SetFocusOn(_selectedIndex);
            }
        }

        void DownNavigation()
        {
            if (!_difficultySelection && _selectedIndex == _selectableControls.Count - 1)
            {
                LoseFocusOn(_selectedIndex);
                _difficultySelection = true;
                Difficulty = Common.Difficulty.Easy;
                SetFocusOn(Difficulty);
            }
            else if (_difficultySelection && _selectedIndex == _selectableControls.Count - 1)
            {
                _difficultySelection = false;
                LoseFocusOn(Difficulty);
                _selectedIndex = 0;
                SetFocusOn(_selectedIndex);
            }
            else if (_difficultySelection)
            {
                LoseFocusOn(Difficulty);
                _difficultySelection = false;
                SetFocusOn(_selectedIndex);
            }
            else
            {                
                LoseFocusOn(_selectedIndex);
                _selectedIndex++;
                SetFocusOn(_selectedIndex);
            }
        }

        void RightNavigation()
        {
            if (_difficultySelection)
            {
                LoseFocusOn(Difficulty);
                Difficulty = (Difficulty)(((int)Difficulty + 1) % 3);
                SetFocusOn(Difficulty);
            }
        }

        void LeftNavigation()
        {
            if (_difficultySelection)
            {
                LoseFocusOn(Difficulty);
                int newVal = (int)Difficulty - 1;
                if (newVal < 0)
                    Difficulty = Difficulty.Hard;
                else
                    Difficulty = (Difficulty)newVal;
                SetFocusOn(Difficulty);
            }
        }

        void InvokeAction()
        {
            if (_difficultySelection)
            {
                TryNavigateToRecordView();
            }
            else if (_selectableControls[_selectedIndex] is Controls.MenuButton)
            {
                var ofd = new OpenFileDialog();                
                switch (_selectedIndex)
                {
                    case 0:
                        ofd.Filter = "Music files (*.mp3, *.ogg)|*.mp3;*.ogg";
                        break;
                    case 1:
                    case 2:
                        ofd.Filter = "Graphic files (*.png, *.jpg)|*.png;*.jpg;*.jpeg";
                        break;
                }

                if (ofd.ShowDialog() == true)
                {
                    switch (_selectedIndex)
                    {
                        case 0:
                            Song.Sequences = new Dictionary<Common.Difficulty, IReadOnlySequence>();
                            _view.btnEasy.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                            _view.btnMedium.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                            _view.btnHard.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;

                            Song.FilePath = ofd.FileName;
                            _musicPlayerService.FilePath = Song.FilePath;
                            while (!_musicPlayerService.HasDuration)
                            {                
                                Thread.Sleep(50);
                            }
                            Song.Duration = _musicPlayerService.Duration;
                            break;
                        case 1:
                            Song.BackgroundPath = ofd.FileName;
                            break;
                        case 2:
                            Song.CoverPath = ofd.FileName;
                            break;
                    }
                }
            }
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsActive || IsPopupShowing || message.PlayerId != PlayerID.Player1)
                return;

            switch (message.PlayerAction)
            {
                case PlayerAction.Enter:
                    InvokeAction();
                    break;

                case PlayerAction.Back:
                    IsPopupShowing = true;
                    _eventAggregator.Publish(new ShowPopupEvent() 
                    { 
                        PopupType = PopupType.ButtonsPopup, 
                        PopupSettings = (vm) =>
                        {
                            (vm as ButtonsPopupViewModel).Buttons.AddRange(new List<string>() { "Continue", "Save the song", "Leave" });
                            (vm as ButtonsPopupViewModel).Message = "What do you want to do?";
                            (vm as ButtonsPopupViewModel).PopupId = 1;
                        }
                    });
                    break;

                case PlayerAction.Right:
                    RightNavigation();
                    break;

                case PlayerAction.Left:
                    LeftNavigation();
                    break;

                case PlayerAction.Down:
                    DownNavigation();
                    break;

                case PlayerAction.Up:
                    UpNavigation();
                    break;
            }
        }

        void HandleSequencePopup(ButtonsPopupEvent message)
        {
            if (message.IsCanceled)
                return;

            if (message.SelectedButton == 1)
            {
                _eventAggregator.Publish(new NavigationExEvent()
                {
                    NavDestination = NavDestination.Record,
                    PageSettings = (vm) =>
                    {
                        (vm as RecordSequenceViewModel).Song = Song;
                        (vm as RecordSequenceViewModel).Difficulty = Difficulty;
                    }
                });
            }
            else
            {
                DebugHelpers.DebugSongHelper.GenerateSequence(new Random(), Song, Difficulty);
                NotifyOfPropertyChange(() => Song);
            }
        }

        void HandleExitPopup(ButtonsPopupEvent message)
        {
            if (message.IsCanceled)
                return;

            if (message.SelectedButton == 2 && ValidateFields(true))
            {
                Song song = new Song();
                song.Artist = Song.Artist;
                song.Author = Song.Author;
                song.Title = Song.Title;
                song.Sequences = Song.Sequences;
                song.FilePath = Song.FilePath;
                song.BackgroundPath = Song.BackgroundPath;
                song.CoverPath = Song.CoverPath;
                song.CreateDate = DateTime.Now;
                song.Duration = Song.Duration;

                song.SaveToFile();
                _songsService.AddSong(song);

                _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.MainMenu });
            }

            if (message.SelectedButton == 3)
            {
                _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.MainMenu });
            }
        }

        public void Handle(ButtonsPopupEvent message)
        {
            if (!IsActive)
                return;

            IsPopupShowing = false;

            if (message.PopupId == 1)
                HandleExitPopup(message);
            else if (message.PopupId == 2)
                HandleSequencePopup(message);
        }

        public void TextBoxKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                DownNavigation();
            }
        }
    }
}
