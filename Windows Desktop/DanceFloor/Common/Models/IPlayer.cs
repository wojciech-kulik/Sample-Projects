using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IPlayer : IBaseModel
    {
        PlayerID PlayerID { get; set; }

        int Points { get; set; }

        int Life { get; set; }

        Difficulty Difficulty { get; set; }

        bool IsGameOver { get; set; }
    }
}
