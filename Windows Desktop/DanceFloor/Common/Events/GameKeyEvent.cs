using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GameKeyEvent
    {
        public PlayerID PlayerId { get; set; }
        public PlayerAction PlayerAction { get; set; }
    }
}
