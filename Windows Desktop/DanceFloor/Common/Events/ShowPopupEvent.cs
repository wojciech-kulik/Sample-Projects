using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ShowPopupEvent
    {
        public PopupType PopupType { get; set; }

        public Action<object> PopupSettings { get; set; }
    }
}
