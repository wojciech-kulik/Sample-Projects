using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzernator.ViewModels
{
    public class RestaurantPreviewViewModel : BaseViewModel
    {
        public RestaurantPreviewViewModel() { }

        public RestaurantPreviewViewModel(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public string Picture { get; set; }
    }
}
