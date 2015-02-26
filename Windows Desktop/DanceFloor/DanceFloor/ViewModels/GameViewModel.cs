using Caliburn.Micro;
using Common;
using GameLayer;
using DanceFloor.Constants;
using DanceFloor.DebugHelpers;
using DanceFloor.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DanceFloor.ViewModels
{
    public class GameViewModel : BaseViewModel, 
        IHandle<PlayerHitEvent>, IHandle<PlayerMissedEvent>, IHandle<GameKeyEvent>, 
        IHandle<GameOverPopupEvent>, IHandle<GameOverEvent>,
        IHandle<CountdownPopupEvent>, IHandle<ButtonsPopupEvent>
    {
        GameView _view;
        Storyboard _p1Animation, _p2Animation;

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

                    if (_game != null)
                    {
                        _game.GetSongCurrentTime = () => _p1Animation.GetCurrentTime();
                    }

                    NotifyOfPropertyChange(() => Game);
                }
            }
        }
        #endregion

        public GameViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }        

        protected override void OnViewAttached(object view, object context)
        {           
            _view = view as GameView;
            _p1Animation = _view.Resources.Values.OfType<Storyboard>().First() as Storyboard;
            _p2Animation = _view.Resources.Values.OfType<Storyboard>().Skip(1).First() as Storyboard;

            PrepareUI();
        }

        protected override void OnActivate()
        {
            Game.Song.LoadSequences();
            IsPopupShowing = true;
            _eventAggregator.Publish(new ShowPopupEvent() { PopupType = PopupType.CountdownPopup, PopupSettings = (vm) => (vm as CountdownPopupViewModel).Message = "Game starts in..." });
        }

        protected override void OnDeactivate(bool close)
        {
            Game.Song.UnloadSequences();
        }

        void LoadNotes(ISong song, IPlayer player)
        {
            var notes = player.PlayerID == PlayerID.Player1 ? _view.p1Notes.Children : _view.p2Notes.Children;
            var animation = player.PlayerID == PlayerID.Player1 ? _p1Animation : _p2Animation;
            notes.Clear();

            //set animation
            animation.Children.First().Duration = song.Duration;
            (animation.Children.First() as DoubleAnimation).From = -GameUIConstants.ArrowWidthHeight;
            (animation.Children.First() as DoubleAnimation).To = -(GameUIConstants.PixelsPerSecond * song.Duration.TotalSeconds + GameUIConstants.ArrowWidthHeight);

            //create arrows for each sequence element
            foreach (var seqElem in song.Sequences[player.Difficulty])
            {
                //load and prepare image
                string imagePath = player.PlayerID == PlayerID.Player1 ? GameUIConstants.P1ArrowImage : GameUIConstants.P2ArrowImage;
                if (seqElem.IsBomb)
                    imagePath = GameUIConstants.BombImage;
                var bitmap = new BitmapImage(new Uri(imagePath));
                Image img = new Image() { Width = GameUIConstants.ArrowWidthHeight, Height = GameUIConstants.ArrowWidthHeight, Source = bitmap, Tag = seqElem };

                //set top position of arrow according to time
                double top = seqElem.Time.TotalSeconds * GameUIConstants.PixelsPerSecond;
                Canvas.SetTop(img, top);

                //rotate arrow and set in proper place
                switch (seqElem.Type)
                {
                    case SeqElemType.LeftArrow:
                        if (!seqElem.IsBomb)
                            img.RenderTransform = new RotateTransform(90, GameUIConstants.ArrowWidthHeight / 2.0, GameUIConstants.ArrowWidthHeight / 2.0);
                        Canvas.SetLeft(img, GameUIConstants.LeftArrowX);
                        break;
                    case SeqElemType.DownArrow:
                        Canvas.SetLeft(img, GameUIConstants.DownArrowX);
                        break;
                    case SeqElemType.UpArrow:
                        if (!seqElem.IsBomb)
                            img.RenderTransform = new RotateTransform(180, GameUIConstants.ArrowWidthHeight / 2.0, GameUIConstants.ArrowWidthHeight / 2.0);
                        Canvas.SetLeft(img, GameUIConstants.UpArrowX);
                        break;
                    case SeqElemType.RightArrow:
                        if (!seqElem.IsBomb)
                            img.RenderTransform = new RotateTransform(-90, GameUIConstants.ArrowWidthHeight / 2.0, GameUIConstants.ArrowWidthHeight / 2.0);
                        Canvas.SetLeft(img, GameUIConstants.RightArrowX);
                        break;
                }

                //add image to the canvas
                notes.Add(img);

                DebugSongHelper.AddTimeToNotes(notes, seqElem.Type, top);
            }  
        }

        void PrepareUI()
        {
            //set points & life
            _view.p1PointsBar.Points = "0";
            _view.p1Health.SetLife(GameConstants.FullLife);
            _view.p2PointsBar.Points = "0";
            _view.p2Health.SetLife(GameConstants.FullLife);

            //load notes
            LoadNotes(Game.Song, Game.Player1);
            DebugSongHelper.ShowCurrentTimeInsteadPoints(_p1Animation, _game.MusicPlayerService, _view);

            //set background
            if (Game.Song.BackgroundPath != null && File.Exists(Game.Song.BackgroundPath))
                _view.Background = new ImageBrush(new BitmapImage(new Uri(Game.Song.BackgroundPath)));
            else
                _view.Background = new ImageBrush(new BitmapImage(new Uri(GameUIConstants.DefaultGameBackground)));

            //set view according to number of players
            _view.mainGrid.ColumnDefinitions.Clear();
            _view.mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            if (_game.IsMultiplayer)
            {
                LoadNotes(Game.Song, Game.Player2);
                _view.mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

                _view.p2Playboard.Visibility = System.Windows.Visibility.Visible;
                _view.p2LifePanel.Visibility = System.Windows.Visibility.Visible;
                _view.p2PointsBar.Visibility = System.Windows.Visibility.Visible;                
            }
            else
            {
                _view.p2Playboard.Visibility = System.Windows.Visibility.Hidden;
                _view.p2LifePanel.Visibility = System.Windows.Visibility.Hidden;
                _view.p2PointsBar.Visibility = System.Windows.Visibility.Hidden;   
            }
        }

        public void StartGame()
        {
            _p1Animation.Begin();
            if (_game.IsMultiplayer)
                _p2Animation.Begin();
            _game.Start();            
        }

        public void ResumeGame()
        {
            //sometimes _game.MusicPlayerService.CurrentTime returns 0  o_O  so we have to wait for a normal value
            TimeSpan currentTime;
            while ((currentTime = _game.MusicPlayerService.CurrentTime).TotalSeconds == 0) ; //TODO: possible deadloop, but shouldn't happen

            _p1Animation.Resume();
            _p1Animation.Seek(currentTime); //synchronize animation with music (difference will be about <= 0.05s, which is enough)
            if (_game.IsMultiplayer)
            {
                _p2Animation.Resume();
                _p2Animation.Seek(currentTime);
            }

            _game.Resume();                        
        }

        public void PauseGame()
        {
            _p1Animation.Pause();
            if (_game.IsMultiplayer)
                _p2Animation.Pause();
            _game.Pause();
        }

        public void StopGame()
        {
            _p1Animation.Stop();
            if (_game.IsMultiplayer)
                _p2Animation.Stop();
            _game.Stop();
        }

        void PlayAgain()
        {
            PrepareUI();
            Game.Reset();
            _p1Animation.Seek(new TimeSpan(0));
            _p2Animation.Seek(new TimeSpan(0));

            IsPopupShowing = true;
            _eventAggregator.Publish(new ShowPopupEvent() { PopupType = PopupType.CountdownPopup, PopupSettings = (vm) => (vm as CountdownPopupViewModel).Message = "Game starts in..." });
        }

        void ExitGame()
        {
            StopGame();
            _eventAggregator.Publish(new NavigationEvent() { NavDestination = NavDestination.MainMenu });
        }





        public void Handle(PlayerHitEvent message)
        {
            if (!IsActive)
                return;

            var notesPanel = message.PlayerID == PlayerID.Player1 ? _view.p1Notes     : _view.p2Notes;
            var pointsBar  = message.PlayerID == PlayerID.Player1 ? _view.p1PointsBar : _view.p2PointsBar;
            var healthBar  = message.PlayerID == PlayerID.Player1 ? _view.p1Health    : _view.p2Health;

            //set player status
            pointsBar.Points = message.Points.ToString();
            healthBar.SetLife(message.Life);
            DebugSongHelper.ShowHitTimeDifferenceInsteadPoints(_view, _p1Animation);

            //remove hit element
            var toRemove = notesPanel.Children.OfType<Image>().FirstOrDefault(img => img.Tag == message.SequenceElement);
            if (toRemove != null)
                notesPanel.Children.Remove(toRemove);
        }

        public void Handle(PlayerMissedEvent message)
        {
            if (!IsActive)
                return;

            var pointsBar = message.PlayerID == PlayerID.Player1 ? _view.p1PointsBar : _view.p2PointsBar;
            var healthBar = message.PlayerID == PlayerID.Player1 ? _view.p1Health    : _view.p2Health;

            pointsBar.Points = message.Points.ToString();
            healthBar.SetLife(message.Life);

            //todo: play miss sound
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsActive || IsPopupShowing)
                return;

            if (DebugSongHelper.HandleKeyPressed(this, message))
                return;

            if (message.PlayerAction == PlayerAction.Back)
            {
                PauseGame();
                IsPopupShowing = true;
                _eventAggregator.Publish(new ShowPopupEvent()
                {
                    PopupType = PopupType.ButtonsPopup,
                    PopupSettings = (vm) =>
                    {
                        (vm as ButtonsPopupViewModel).Buttons.AddRange(new List<string>() { "Resume", "Play again", "Go back" });
                        (vm as ButtonsPopupViewModel).Message = "Pause";
                    }
                });
            }
        }

        public void Handle(ButtonsPopupEvent message)
        {
            if (!IsActive)
                return;

            IsPopupShowing = false;

            if (message.IsCanceled)
            {
                ResumeGame();
                return;
            }

            switch (message.SelectedButton)
            {
                case 1:
                    ResumeGame();
                    break;

                case 2:
                    PlayAgain();
                    break;

                case 3:
                    ExitGame();
                    break;
            }
        }

        public void Handle(GameOverPopupEvent message)
        {
            if (!IsActive)
                return;

            IsPopupShowing = false;

            if (message.PlayAgainSelected)
            {
                PlayAgain();
            }
            else
            {
                ExitGame();
            }
        }

        public void Handle(GameOverEvent message)
        {
            if (!IsActive)
                return;

            PauseGame();

            string msg;
            if (Game.IsMultiplayer)
            {
                if (message.IsTie)
                {
                    msg = "Draw!";
                }
                else
                {
                    msg = "Player " + (message.PlayerWon == PlayerID.Player1 ? "1 won!" : "2 won!"); 
                }
            }
            else
            {
                msg = "Score: " + Game.Player1.Points.ToString();             
            }

            IsPopupShowing = true;
            _eventAggregator.Publish(new ShowPopupEvent() 
            { 
                PopupType = PopupType.GameOverPopup, 
                PopupSettings = (vm) =>
                {
                    (vm as GameOverPopupViewModel).Message = msg;
                }
            });
        }

        public void Handle(CountdownPopupEvent message)
        {
            if (!IsActive)
                return;

            IsPopupShowing = false;
            StartGame();
        }
    }
}
