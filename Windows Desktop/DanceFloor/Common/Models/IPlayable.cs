using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IPlayable : IBaseModel
    {
        void Start();

        void Resume();

        void Pause();

        void Stop();
    }
}
