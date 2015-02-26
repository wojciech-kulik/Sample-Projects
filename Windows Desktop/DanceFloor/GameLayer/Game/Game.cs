using Caliburn.Micro;
using Common;
using GameLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GameLayer
{
    public class Game : NotificableObject, IGame, IHandle<GameKeyEvent>, IHandle<MusicEndedEvent>
    {
        List<ISequenceElement> _p1AlreadyHit = new List<ISequenceElement>();
        List<ISequenceElement> _p2AlreadyHit = new List<ISequenceElement>();
        IEventAggregator _eventAggregator;
        Timer _missedTimer = new Timer(250);

        public Game(IEventAggregator eventAggregator, IMusicPlayerService musicPlayerService)
        {
            MusicPlayerService = musicPlayerService;

            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);

            _missedTimer.Elapsed += lookForMissedNotes_Tick;

            Player1 = new Player() { PlayerID = PlayerID.Player1 };
            Player2 = new Player() { PlayerID = PlayerID.Player2 };
        }

        //NEEDS TO BE SET BY VIEWMODEL (for example you can get this from animation.GetCurrentTime())
        public Func<TimeSpan> GetSongCurrentTime { get; set; }

        public IMusicPlayerService MusicPlayerService { get; private set; }

        #region Player1

        private IPlayer _player1;

        public IPlayer Player1
        {
            get
            {
                return _player1;
            }
            set
            {
                if (_player1 != value)
                {
                    _player1 = value;
                    NotifyPropertyChanged("Player1");
                }
            }
        }
        #endregion

        #region Player2

        private IPlayer _player2;

        public IPlayer Player2
        {
            get
            {
                return _player2;
            }
            set
            {
                if (_player2 != value)
                {
                    _player2 = value;
                    NotifyPropertyChanged("Player2");
                }
            }
        }
        #endregion

        #region IsMultiplayer

        private bool _isMultiplayer;

        public bool IsMultiplayer
        {
            get
            {
                return _isMultiplayer;
            }
            set
            {
                if (_isMultiplayer != value)
                {
                    _isMultiplayer = value;
                    NotifyPropertyChanged("IsMultiplayer");
                }
            }
        }
        #endregion

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
                    MusicPlayerService.FilePath = value != null ? value.FilePath : null;
                    _song = value;
                    NotifyPropertyChanged("Song");
                }
            }
        }
        #endregion

        private void CheckIfGameOver()
        {
            if (Player1.IsGameOver || (IsMultiplayer && Player2.IsGameOver))
            {
                bool tie = IsMultiplayer && Player1.IsGameOver && Player2.IsGameOver && Player1.Points == Player2.Points;
                _eventAggregator.Publish(new GameOverEvent() { IsTie = tie, PlayerWon = Player1.IsGameOver ? PlayerID.Player2 : PlayerID.Player1 });
            }
        }

        private void lookForMissedNotes_Tick(object sender, ElapsedEventArgs e)
        {
            LookForMissedNotes(Player1);
            if (IsMultiplayer)
                LookForMissedNotes(Player2);
            CheckIfGameOver();
        }

        private void LookForMissedNotes(IPlayer player)
        {
            var currenTime = GetSongCurrentTime().TotalSeconds;            
            var alreadyHit = player.PlayerID == PlayerID.Player1 ? _p1AlreadyHit : _p2AlreadyHit;
            //this collection is beeing modified and we access it from thread, so we should make a copy first
            var copyOfAlreadyHit = alreadyHit.ToList();
            
            var missed = Song.Sequences[player.Difficulty]
                .Except(copyOfAlreadyHit)
                .Where(e => !e.IsBomb && e.Time.TotalSeconds - currenTime < -GameConstants.WorstHitTime)
                .ToList();

            if (missed.Count == 0)
                return;

            alreadyHit.AddRange(missed);
            SetLifePoints(player, -GameConstants.MissLifePoints * missed.Count);

            foreach(var missedElem in missed)
            {
                _eventAggregator.Publish(new PlayerMissedEvent()
                {
                    PlayerID = player.PlayerID,
                    Points = player.Points,
                    Life = player.Life,
                    Reason = MissReason.NotHit
                });
            }   
        }

        private void SetLifePoints(IPlayer player, int deltaLifePoints)
        {
            player.Life = Math.Min(GameConstants.FullLife, Math.Max(0, player.Life + deltaLifePoints));
            if (player.Life == 0)
            {
                player.IsGameOver = true;                
            }
        }

        //returns null if nothing hit
        //synchronized with UI (if animation time attached to GetSongCurrentTime)
        private ISequenceElement SetPoints(IPlayer player, TimeSpan hitTime, PlayerAction playerAction)
        {
            var alreadyHit = player.PlayerID == PlayerID.Player1 ? _p1AlreadyHit : _p2AlreadyHit;

            SeqElemType type = GameHelper.PlayerActionToSeqElemType(playerAction);
            ISequenceElement hitElement = Song.GetClosestTo(player.Difficulty, hitTime, type, alreadyHit);

            if (hitElement == null)
            {
                SetLifePoints(player, -GameConstants.WrongMomentOrActionLifePoints);
                return null;
            }                

            if (!hitElement.IsBomb)
            {
                double diff = Math.Abs(hitElement.Time.TotalSeconds - hitTime.TotalSeconds);

                if (diff <= GameConstants.BestHitTime)
                    player.Points += GameConstants.BestHitPoints;
                else if (diff <= GameConstants.MediumHitTime)
                    player.Points += GameConstants.MediumHitPoints;
                else if (diff <= GameConstants.WorstHitTime)
                    player.Points += GameConstants.WorstHitPoints;

                SetLifePoints(player, GameConstants.HitLifePoints);
            }
            else
            {
                SetLifePoints(player, -GameConstants.BombLifePoints);
            }

            alreadyHit.Add(hitElement);
            return hitElement;
        }

        public void Handle(GameKeyEvent message)
        {
            if (!IsRunning)
                return;

            IPlayer player = message.PlayerId == PlayerID.Player1 ? Player1 : Player2;
            if  (player == null || 
                (player.PlayerID == PlayerID.Player2 && !IsMultiplayer) || 
                 message.PlayerAction == PlayerAction.Enter || message.PlayerAction == PlayerAction.Back)
                return;

            ISequenceElement hitElem = SetPoints(player, GetSongCurrentTime(), message.PlayerAction);
            if (hitElem != null)
            {
                _eventAggregator.Publish(new PlayerHitEvent()
                {
                    PlayerID = player.PlayerID,                    
                    Life = player.Life,
                    Points = player.Points,
                    IsBomb = hitElem.IsBomb,
                    SequenceElement = hitElem
                });
            }
            else
            {
                _eventAggregator.Publish(new PlayerMissedEvent()
                {
                    PlayerID = player.PlayerID,
                    Points = player.Points,
                    Life = player.Life,
                    Reason = MissReason.WrongMomentOrAction
                });
            }
        }

        public void Handle(MusicEndedEvent message)
        {
            if (!IsRunning)
                return;

            if (!IsMultiplayer)
            {
                _eventAggregator.Publish(new GameOverEvent() { IsTie = false, PlayerWon = PlayerID.Player1 });
                return;
            }

            if (Player1.Points > Player2.Points)
                _eventAggregator.Publish(new GameOverEvent() { IsTie = false, PlayerWon = PlayerID.Player1 });
            else if (Player1.Points < Player2.Points)
                _eventAggregator.Publish(new GameOverEvent() { IsTie = false, PlayerWon = PlayerID.Player2 });
            else
                _eventAggregator.Publish(new GameOverEvent() { IsTie = true });
        }

        public void Reset()
        {
            _p1AlreadyHit.Clear();
            _p2AlreadyHit.Clear();

            Player1.Life = GameConstants.FullLife;
            Player2.Life = GameConstants.FullLife;

            Player1.Points = 0;
            Player2.Points = 0;

            Player1.IsGameOver = false;
            Player2.IsGameOver = false;

            MusicPlayerService.Reset();
        }

        #region IPlayable

        #region IsRunning

        private bool _isRunning;

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            protected set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    NotifyPropertyChanged("IsRunning");
                }
            }
        }
        #endregion

        public void Start()
        {
            MusicPlayerService.Start();
            _missedTimer.Start();
            IsRunning = true;
        }

        public void Resume()
        {
            if (IsRunning)
                return;

            MusicPlayerService.Resume();
            _missedTimer.Start();
            IsRunning = true;
        }

        public void Pause()
        {
            MusicPlayerService.Pause();
            _missedTimer.Stop();
            IsRunning = false;
        }

        public void Stop()
        {
            MusicPlayerService.Stop();
            _missedTimer.Stop();
            IsRunning = false;
        }
        #endregion
    }
}
