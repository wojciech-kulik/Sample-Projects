using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IMusicPlayerService : IPlayable
    {
        TimeSpan CurrentTime { get; }

        TimeSpan Duration { get; }

        string FilePath { get; set; }

        bool IsRunning { get; }

        bool HasDuration { get; }

        void Reset();
    }
}
