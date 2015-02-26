using Common;
using GameLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLayer
{
    public class HighScore : NotificableObject, IHighScore
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
                    NotifyPropertyChanged("Song");
                }
            }
        }
        #endregion

        #region Player

        private string _player;

        public string Player
        {
            get
            {
                return _player;
            }
            set
            {
                if (_player != value)
                {
                    _player = value;
                    NotifyPropertyChanged("Player");
                }
            }
        }
        #endregion

        #region Date

        private DateTime _date;

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    NotifyPropertyChanged("Date");
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

        #region Points

        private int _points;

        public int Points
        {
            get
            {
                return _points;
            }
            set
            {
                if (_points != value)
                {
                    _points = value;
                    NotifyPropertyChanged("Points");
                }
            }
        }
        #endregion
    }
}
