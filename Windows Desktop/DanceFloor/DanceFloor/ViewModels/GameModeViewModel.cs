using Caliburn.Micro;
using Common;
using GameLayer;
using DanceFloor.Constants;
using DanceFloor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace DanceFloor.ViewModels
{
    public class GameModeViewModel : BaseViewModel, IHandle<GameKeyEvent>
    {
        public GameModeViewModel(IEventAggregator eventAggregator, IGame game)
            :base(eventAggregator)
        {            
            Game = game;
        }

        #region Game

        private IGame _game;

        public IGame Game
        {
            get
            {
                return _game;
            }
            set
            {
                if (_game != value)
                {
                    _game = value;
                    NotifyOfPropertyChange(() => Game);
                }
            }
        }
        #endregion

        protected override void OnActivate()
        {
            Game.Song.LoadSequences();
            Difficulty lowestAvailable = Game.Song.Sequences.Keys.First();
            Game.Player1.Difficulty = lowestAvailable;
            Game.Player2.Difficulty = lowestAvailable;
            NotifyOfPropertyChange(() => Game);
        }

        protected override void OnDeactivate(bool close)
        {
            Game.Song.UnloadSequences();
        }

        protected override void OnViewAttached(object view, object context)
        {
            var v = view as GameModeView;

            if (!Game.Song.Sequences.ContainsKey(Difficulty.Easy))
            {
                v.p1Easy.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p1Easy.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
                v.p2Easy.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p2Easy.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
            }

            if (!Game.Song.Sequences.ContainsKey(Difficulty.Medium))
            {
                v.p1Medium.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p1Medium.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
                v.p2Medium.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p2Medium.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
            }

            if (!Game.Song.Sequences.ContainsKey(Difficulty.Hard))
            {
                v.p1Hard.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p1Hard.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
                v.p2Hard.ButtonBackground = GameUIConstants.GameModeInactiveBtnGradient;
                v.p2Hard.TextBlock.Foreground = GameUIConstants.GameModeInactiveBtnForeground;
            }               
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsActive)
                return;

            var player = message.PlayerId == PlayerID.Player1 ? Game.Player1 : Game.Player2;

            switch (message.PlayerAction)
            {
                case PlayerAction.Enter:
                    _eventAggregator.Publish(new NavigationExEvent() { NavDestination = NavDestination.Game, PageSettings = (vm) => (vm as GameViewModel).Game = Game });
                    break;
                case PlayerAction.Back:
                    _eventAggregator.Publish(new NavigationExEvent() { NavDestination = NavDestination.SongsList, PageSettings = (vm) => (vm as SongsListViewModel).SelectedSong = Game.Song });
                    break;
                case PlayerAction.Right:
                    Game.IsMultiplayer = true;
                    NotifyOfPropertyChange(() => Game);
                    break;
                case PlayerAction.Left:
                    Game.IsMultiplayer = false;
                    NotifyOfPropertyChange(() => Game);
                    break;
                case PlayerAction.Down:
                    do
                    {
                        player.Difficulty = (Difficulty)(((int)player.Difficulty + 1) % 3);
                    } while (!Game.Song.Sequences.ContainsKey(player.Difficulty));

                    NotifyOfPropertyChange(() => Game);
                    break;
                case PlayerAction.Up:
                {
                    do
                    {
                        int newVal = (int)player.Difficulty - 1;
                        if (newVal < 0)
                            player.Difficulty = Difficulty.Hard;
                        else
                            player.Difficulty = (Difficulty)newVal;
                    } while (!Game.Song.Sequences.ContainsKey(player.Difficulty));

                    NotifyOfPropertyChange(() => Game);
                    break;
                }
            }
        }
    }
}
