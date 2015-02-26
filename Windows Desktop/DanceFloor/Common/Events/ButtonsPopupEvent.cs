using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ButtonsPopupEvent : ClosePopupEvent
    {
        public int SelectedButton { get; set; }

        public bool IsCanceled { get; set; }

        public int PopupId { get; set; }
    }
}
