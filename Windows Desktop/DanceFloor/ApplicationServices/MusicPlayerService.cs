using Caliburn.Micro;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ApplicationServices
{
    public class MusicPlayerService : NotificableObject, IMusicPlayerService
    {
        IEventAggregator _eventAggregator;
        MediaPlayer _mediaPlayer = new MediaPlayer();

        public MusicPlayerService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
        }

        void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            IsRunning = false;
            _eventAggregator.Publish(new MusicEndedEvent() { FilePath = this.FilePath });
        }

        #region CurrentTime

        public TimeSpan CurrentTime
        {
            get
            {
                return _mediaPlayer.Position;
            }
        }
        #endregion

        #region Duration

        public TimeSpan Duration
        {
            get
            {
                return _mediaPlayer.NaturalDuration.TimeSpan;
            }
        }
        #endregion

        #region HasDuration

        public bool HasDuration
        {
            get
            {
                return _mediaPlayer.NaturalDuration.HasTimeSpan;
            }
        }
        #endregion

        #region FilePath

        private string _filePath;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (_filePath != value)
                {
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        if (!File.Exists(value))
                        {
                            throw new ArgumentException("Podany plik muzyczny nie istnieje:\n" + FilePath);
                        }
                        _mediaPlayer.Open(new Uri(value, UriKind.Relative));
                    }

                    _filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }
        #endregion

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
            _mediaPlayer.Play();
            IsRunning = true;
        }

        public void Resume()
        {
            _mediaPlayer.Play();
            IsRunning = true;
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            IsRunning = false;
        }

        public void Reset()
        {
            _mediaPlayer.Stop();
            IsRunning = false;
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Close();
            IsRunning = false;
        }
    }
}
