using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class SongsService : ISongsService
    {
        class StrComparer: StringComparer
        {
            public override int Compare(string x, string y)
            {
                if (x.Length < y.Length)
                    return -1;
                else if (x.Length == y.Length)
                    return x.CompareTo(y);
                else
                    return 1;
            }

            public override bool Equals(string x, string y)
            {
                return x == y;
            }

            public override int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        private List<ISong> _songs = null;
        Func<ISong> _songsFactory;

        public SongsService(Func<ISong> songsFactory)
        {
            _songsFactory = songsFactory;
        }

        public IReadOnlyCollection<ISong> GetAllSongs()
        {
            if (_songs == null)
            {
                _songs = new List<ISong>();
                var songsList = Directory.GetDirectories(GameConstants.SongsDir).OrderBy(s => s, new StrComparer());

                foreach (var s in songsList)
                {
                    var song = _songsFactory();
                    song.LoadFromFile(s);
                    _songs.Add(song);
                }
            }

            return _songs;
        }

        public void AddSong(ISong song)
        {
            if (_songs == null)
                GetAllSongs();

            if (_songs.FirstOrDefault(s => s.FilePath == song.FilePath) != null)
                return;

            _songs.Add(song);
            _songs = _songs.OrderBy(s => s.Artist, new StrComparer()).ToList();
        }
    }
}
