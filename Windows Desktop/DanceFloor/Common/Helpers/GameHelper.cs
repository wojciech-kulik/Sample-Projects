using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GameHelper
    {
        public static SeqElemType PlayerActionToSeqElemType(PlayerAction playerAction)
        {
            return (SeqElemType)playerAction;
        }
    }
}
