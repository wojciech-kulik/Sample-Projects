using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IGame : IPlayable
    {
        Func<TimeSpan> GetSongCurrentTime { get; set; }

        IPlayer Player1 { get; }

        IPlayer Player2 { get; }

        bool IsMultiplayer { get; set; }

        ISong Song { get; set; }

        IMusicPlayerService MusicPlayerService { get; }

        void Reset();
    }
}
