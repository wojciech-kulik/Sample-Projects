using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NavigationExEvent
    {
        public NavDestination NavDestination { get; set; }

        public Action<object> PageSettings { get; set; }
    }
}
