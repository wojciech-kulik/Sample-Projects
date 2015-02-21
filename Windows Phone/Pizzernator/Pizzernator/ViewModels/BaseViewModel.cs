using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Pizzernator.ViewModels
{
    public class BaseViewModel: Screen
    {
        protected IEventAggregator _eventAggregator;

        public BaseViewModel() { }

        public BaseViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
    }
}
