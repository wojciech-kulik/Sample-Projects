using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLayer
{
    public class Player : NotificableObject, IPlayer
    {
        public Player()
        {
            IsGameOver = false;
            Life = GameConstants.FullLife;
            Points = 0;
        }

        #region PlayerID
        private PlayerID _playerID;

        public PlayerID PlayerID
        {
            get
            {
                return _playerID;
            }
            set
            {
                if (_playerID != value)
                {
                    _playerID = value;
                    NotifyPropertyChanged("PlayerID");
                }
            }
        }
        #endregion

        #region Points

        private int _point;

        public int Points
        {
            get
            {
                return _point;
            }
            set
            {
                if (_point != value)
                {
                    _point = value;
                    NotifyPropertyChanged("Points");
                }
            }
        }
        #endregion

        #region Life

        private int _life;

        public int Life
        {
            get
            {
                return _life;
            }
            set
            {
                if (_life != value)
                {
                    _life = value;
                    NotifyPropertyChanged("Life");
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
                    NotifyPropertyChanged("Difficulty");
                }
            }
        }
        #endregion

        #region IsGameOver

        private bool _isGameOver;

        public bool IsGameOver
        {
            get
            {
                return _isGameOver;
            }
            set
            {
                if (_isGameOver != value)
                {
                    _isGameOver = value;
                    NotifyPropertyChanged("IsGameOver");
                }
            }
        }
        #endregion
    }
}
