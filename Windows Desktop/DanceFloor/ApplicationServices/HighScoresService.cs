using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices
{
    //TODO: implement
    public class HighScoresService : IHighScoresService
    {
        public IList<IHighScore> GetAll()
        {
            return new List<IHighScore>();
        }

        public void Add(IHighScore highScore)
        {
        }

        public bool IsBest(int points, Difficulty difficulty, ISong song)
        {
            return true;
        }

        #region IEnumerable
        public IEnumerator<IHighScore> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
