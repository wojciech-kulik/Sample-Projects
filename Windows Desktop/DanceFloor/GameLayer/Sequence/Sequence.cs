using Caliburn.Micro;
using Common;
using GameLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLayer
{
    [Serializable]
    public class Sequence : NotificableObject, ISequence
    {
        private BindableCollection<ISequenceElement> SequenceElements = new BindableCollection<ISequenceElement>();

        #region IReadOnlySequence
        public IEnumerator<ISequenceElement> GetEnumerator()
        {
            return SequenceElements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SequenceElements.GetEnumerator();
        }
        #endregion

        public void AddElement(ISequenceElement element)
        {
            SequenceElements.Add(element);
        }

        public void Clear()
        {
            SequenceElements.Clear();
        }

        public ISequenceElement GetClosestTo(TimeSpan time, SeqElemType elementType, IList<ISequenceElement> alreadyHit)
        {
            var notHitElements = SequenceElements.Except(alreadyHit).ToList(); //need to make a copy, because alreadyHit can be modified
            if (notHitElements.Count == 0)
                return null;

            var inRangeElements = notHitElements.Where(e => e.Type == elementType && Math.Abs(e.Time.TotalSeconds - time.TotalSeconds) <= GameConstants.WorstHitTime);
            if (inRangeElements.Count() == 0)
                return null;

            return inRangeElements.OrderBy(e => e.Time.TotalSeconds).First();
        }        
    }
}
