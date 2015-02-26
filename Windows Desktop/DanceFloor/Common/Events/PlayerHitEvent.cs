using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PlayerHitEvent
    {
        public bool IsBomb { get; set; }

        public int Points { get; set; }

        public int Life { get; set; }

        public PlayerID PlayerID { get; set; }

        public ISequenceElement SequenceElement { get; set; }
    }
}
