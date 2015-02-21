using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzernator.GoogleAPI
{
    public interface IGoogleApiService
    {
        void SendGetPlacesRequest(double latitude, double longitude, double radious, string keyWords);
    }
}
