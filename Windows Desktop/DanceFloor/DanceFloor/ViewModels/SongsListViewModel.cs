using Caliburn.Micro;
using Common;
using System.Windows.Input;
using GameLayer;
using DanceFloor.Views;

namespace DanceFloor.ViewModels
{
    public class SongsListViewModel : BaseViewModel, IHandle<GameKeyEvent>
    {
        #region SelectedSong

        private ISong _selectedSong;

        public ISong SelectedSong
        {
            get
            {
                return _selectedSong;
            }
            set
            {
                if (_selectedSong != value)
                {
                    _selectedSong = value;
                    NotifyOfPropertyChange(() => SelectedSong);
                }
            }
        }
        #endregion

        #region Songs

        private BindableCollection<ISong> _songs;

        public BindableCollection<ISong> Songs
        {
            get
            {
                return _songs;
            }
            set
            {
                if (_songs != value)
                {
                    _songs = value;
                    NotifyOfPropertyChange(() => Songs);
                }
            }
        }
        #endregion

        public SongsListViewModel(IEventAggregator eventAggregator, ISongsService songsService)
            : base(eventAggregator)
        {
            Songs = new BindableCollection<ISong>(songsService.GetAllSongs());
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsActive)
                return;

            if (message.PlayerAction == PlayerAction.Back)
            {
                _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.MainMenu });
            }
            else if (message.PlayerAction == PlayerAction.Enter && SelectedSong != null)
            {
                SelectedSong.IsSelected = false;
                _eventAggregator.Publish(new NavigationExEvent() { NavDestination = NavDestination.GameMode, PageSettings = (vm) => (vm as GameModeViewModel).Game.Song = SelectedSong });
            }
        }   
    }
}
