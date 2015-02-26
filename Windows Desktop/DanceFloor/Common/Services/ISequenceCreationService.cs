using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ISequenceCreationService : IPlayable, IEnumerable<ISequence>
    {
        ISong Song { get; set; }

        Difficulty Difficulty { get; set; }

        void SaveSequenceToFile(string path);
    }
}
