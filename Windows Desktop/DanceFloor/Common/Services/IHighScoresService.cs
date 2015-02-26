using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IHighScoresService : IEnumerable<IHighScore>
    {
        IList<IHighScore> GetAll();

        void Add(IHighScore highScore);

        bool IsBest(int points, Difficulty difficulty, ISong song);
    }
}
