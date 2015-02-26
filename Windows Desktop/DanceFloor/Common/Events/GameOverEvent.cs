using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GameOverEvent
    {
        public PlayerID PlayerWon { get; set; }

        public bool IsTie { get; set; }
    }
}
