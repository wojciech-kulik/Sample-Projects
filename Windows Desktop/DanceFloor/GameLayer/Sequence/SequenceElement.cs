using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLayer
{
    [Serializable]
    public class SequenceElement : NotificableObject, ISequenceElement
    {
        #region Time

        private TimeSpan _time;

        public TimeSpan Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
					NotifyPropertyChanged("Time");
                }
            }
        }
        #endregion

        #region Type

        private SeqElemType _type;

        public SeqElemType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
					NotifyPropertyChanged("Type");
                }
            }
        }
        #endregion

        #region IsBomb

        private bool _isBomb;

        public bool IsBomb
        {
            get
            {
                return _isBomb;
            }
            set
            {
                if (_isBomb != value)
                {
                    _isBomb = value;
                    NotifyPropertyChanged("IsBomb");
                }
            }
        }
        #endregion
    }
}
