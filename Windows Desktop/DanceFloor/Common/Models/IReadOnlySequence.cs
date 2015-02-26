using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IReadOnlySequence : IBaseModel, IEnumerable<ISequenceElement>
    {
        ISequenceElement GetClosestTo(TimeSpan time, SeqElemType elementType, IList<ISequenceElement> alreadyHit);
    }
}
