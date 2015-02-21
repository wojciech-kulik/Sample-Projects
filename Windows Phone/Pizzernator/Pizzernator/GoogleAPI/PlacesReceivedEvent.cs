using Caliburn.Micro;
using Pizzernator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzernator.GoogleAPI
{
    public class PlacesReceivedEvent
    {
        public bool Success { get; set; }
        public List<Restaurant> Restaurants { get; set; }
    }
}
