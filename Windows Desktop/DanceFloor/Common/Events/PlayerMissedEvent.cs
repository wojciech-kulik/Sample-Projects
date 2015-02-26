using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PlayerMissedEvent
    {
        public PlayerID PlayerID { get; set; }

        public int Points { get; set; }

        public int Life { get; set; }

        public MissReason Reason { get; set; }
    }
}
