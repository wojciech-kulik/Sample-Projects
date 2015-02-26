using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceFloor.ViewModels
{
    public class BaseViewModel : Screen
    {
        protected IEventAggregator _eventAggregator;
        public BaseViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        protected bool IsPopupShowing { get; set; }
    }
}
