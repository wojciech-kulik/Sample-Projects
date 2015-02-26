using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IHighScore : IBaseModel
    {
        ISong Song { get; set; }

        string Player { get; set; }

        DateTime Date { get; set; }

        Difficulty Difficulty { get; set; }

        int Points { get; set; }
    }
}
