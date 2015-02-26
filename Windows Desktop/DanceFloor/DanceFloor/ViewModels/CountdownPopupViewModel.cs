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
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DanceFloor.ViewModels
{
    public class CountdownPopupViewModel : BaseViewModel, IPopup
    {
        public bool IsShowing { get; set; }

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

        #region TimeLeft

        private int _timeLeft;

        public int TimeLeft
        {
            get
            {
                return _timeLeft;
            }
            set
            {
                if (_timeLeft != value)
                {
                    _timeLeft = value;
                    NotifyOfPropertyChange(() => TimeLeft);
                }
            }
        }
        #endregion

        Timer timer = new Timer(1000);

        public CountdownPopupViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            TimeLeft = GameUIConstants.CountDownDuration;
            timer.Elapsed += timer_Elapsed;            
        }

        protected override void OnViewLoaded(object view)
        {
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeLeft--;
            if (TimeLeft == 0)
            {
                timer.Stop();
                _eventAggregator.Publish(new CountdownPopupEvent());
            }
        }
    }
}
