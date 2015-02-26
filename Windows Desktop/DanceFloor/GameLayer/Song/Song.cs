using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace GameLayer
{
    [Serializable]
    public class Song : NotificableObject, ISong
    {
        public Song()
        {
            Sequences = new Dictionary<Difficulty, IReadOnlySequence>();
        }

        public Dictionary<Difficulty, IReadOnlySequence> Sequences { get; set; }

        #region Title

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        #endregion

        #region Artist

        private string _artist;

        public string Artist
        {
            get
            {
                return _artist;
            }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    NotifyPropertyChanged("Artist");
                }
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
                    _filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }
        #endregion

        #region Author

        private string _author;

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }
        #endregion

        #region CreateDate

        private DateTime _createDate;

        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                if (_createDate != value)
                {
                    _createDate = value;
                    NotifyPropertyChanged("CreateDate");
                }
            }
        }
        #endregion

        #region BackgroundPath

        private string _backgroundPath;

        public string BackgroundPath
        {
            get
            {
                return _backgroundPath;
            }
            set
            {
                if (_backgroundPath != value)
                {
                    _backgroundPath = value;
                    NotifyPropertyChanged("BackgroundPath");
                }
            }
        }
        #endregion

        #region CoverPath

        private string _coverPath;

        public string CoverPath
        {
            get
            {
                return _coverPath;
            }
            set
            {
                if (_coverPath != value)
                {
                    _coverPath = value;
                    NotifyPropertyChanged("CoverPath");
                }
            }
        }
        #endregion

        #region Duration

        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    NotifyPropertyChanged("Duration");
                }
            }
        }
        #endregion

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        #endregion

        public ISequenceElement GetClosestTo(Difficulty difficulty, TimeSpan time, SeqElemType elementType, IList<ISequenceElement> alreadyHit)
        {
            return Sequences[difficulty].GetClosestTo(time, elementType, alreadyHit);
        }

        private string _fileName;
        public void LoadSequences()
        {
            if (Sequences.Count > 0)
                return;

            using (var stream = File.OpenRead(_fileName + "\\" + GameConstants.SongObjectFileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Sequences = (formatter.Deserialize(stream) as ISong).Sequences;
            }
        }

        public void UnloadSequences()
        {
            if (Sequences == null)
                return;

            Sequences.Clear();
        }

        public void LoadFromFile(string fileName, bool loadeSequences = false)
        {
            if (!File.Exists(fileName + "\\" + GameConstants.SongObjectFileName))
                throw new ArgumentException("File doesn't exist.");

            _fileName = fileName;
            
            ISong song;
            using (var stream = File.OpenRead(fileName + "\\" + GameConstants.SongObjectFileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                song = (ISong)formatter.Deserialize(stream);
            }

            if (loadeSequences)
                Sequences = song.Sequences;
            Title = song.Title;
            Artist = song.Artist;
            FilePath = Environment.CurrentDirectory + "\\" + song.FilePath;
            Author = song.Author;
            CreateDate = song.CreateDate;
            BackgroundPath = Environment.CurrentDirectory + "\\" + song.BackgroundPath;
            CoverPath = Environment.CurrentDirectory + "\\" + song.CoverPath;
            Duration = song.Duration;

            if (!File.Exists(BackgroundPath))
                BackgroundPath = "../Images/game_background.jpg";

            if (!File.Exists(CoverPath))
                CoverPath = "../Images/cover.png";
        }

        public void SaveToFile()
        {
            if (String.IsNullOrWhiteSpace(Title) || String.IsNullOrWhiteSpace(Artist))
                throw new InvalidOperationException("Title or artist wasn't filled.");

            if (!File.Exists(FilePath))
                throw new InvalidOperationException("File doesn't exist.");


            //create dir
            string dir = GameConstants.SongsDir + Artist + " - " + Title + "\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);     

            //copy music file
            string songPath = dir + GameConstants.SongFileName + Path.GetExtension(FilePath);
            File.Copy(FilePath, songPath);
            FilePath = songPath;

            //copy cover
            if (!String.IsNullOrWhiteSpace(CoverPath))
            {
                string coverPath = dir + GameConstants.CoverFileName + Path.GetExtension(CoverPath);
                File.Copy(CoverPath, coverPath);
                CoverPath = coverPath;
            }

            //copy background
            if (!String.IsNullOrWhiteSpace(BackgroundPath))
            {
                string backgroundPath = dir + GameConstants.BackgroundFileName + Path.GetExtension(BackgroundPath);
                File.Copy(BackgroundPath, backgroundPath);
                BackgroundPath = backgroundPath;
            }

            //save object
            using (var stream = File.Create(dir + GameConstants.SongObjectFileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }

            LoadFromFile(GameConstants.SongsDir + Artist + " - " + Title); //to set paths correctly
        }
    }
}
