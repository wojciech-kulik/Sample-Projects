using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Device.Location;
using Pizzernator.Models;
using System.Globalization;

namespace Pizzernator.GoogleAPI
{
    public class GoogleApiService : IGoogleApiService
    {
        #error Set Google Api key first:
        //You need to get a key from: https://developers.google.com/places/documentation/index#Introduction
        private const string ApiKey = "YourKey";
        private const string GooglePlaceApiUrl = "https://maps.googleapis.com/maps/api/place/";
        
        IEventAggregator _eventAggregator;

        public GoogleApiService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void SendGetPlacesRequest(double latitude, double longitude, double radious, string keyWords)
        {                 
            string url = String.Format(CultureInfo.InvariantCulture,
                                       GooglePlaceApiUrl + "nearbysearch/json?location={0},{1}&radius={2}&types=food|restaurant&name={3}&sensor=true&key={4}",
                                       latitude, longitude, radious, keyWords, ApiKey);

            WebRequest request = HttpWebRequest.Create(url);  
            request.BeginGetResponse(ReadWebRequestCallBack, request);
        }

        private string GetPhotoUrl(string photoReference)
        {
            return String.Format(GooglePlaceApiUrl + "photo?maxwidth=70&maxheight=70&photoreference={0}&sensor=true&key={1}", photoReference, ApiKey);
        }

        private void ReadWebRequestCallBack(IAsyncResult callBackResult)
        {
            List<Restaurant> restaurants = new List<Restaurant>();
            bool success = true;

            try
            {
                var myRequest = callBackResult.AsyncState as WebRequest;
                var response = (HttpWebResponse)myRequest.EndGetResponse(callBackResult);

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    dynamic data = JObject.Parse(sr.ReadToEnd());

                    if (data.status == "OK")
                    {
                        foreach (dynamic r in data.results)
                        {
                            GeoCoordinate coords = new GeoCoordinate((double)r.geometry.location.lat, (double)r.geometry.location.lng);
                            restaurants.Add(new Restaurant()
                            {
                                Id = r.id,
                                Name = r.name,
                                Latitude = coords.Latitude,
                                Longitude = coords.Longitude,
                                Address = r.vicinity,
                                ImageSource = r.photos != null ? GetPhotoUrl((string)r.photos[0].photo_reference) : r.icon                                
                            });
                        }
                    }
                    else if (data.status != "ZERO_RESULTS")
                    {
                        success = false;
                    }                    
                }
            }
            catch(Exception)
            {
                success = false;
            }

            _eventAggregator.Publish(new PlacesReceivedEvent() { Success = success, Restaurants = restaurants });
        }
    }
}
