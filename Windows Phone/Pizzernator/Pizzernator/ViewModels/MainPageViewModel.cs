using Caliburn.Micro;
using Pizzernator.GoogleAPI;
using Pizzernator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Windows;
using Pizzernator.Storage;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Pizzernator.Views;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;

namespace Pizzernator.ViewModels
{
    public class MainPageViewModel : BaseViewModel, IHandle<PlacesReceivedEvent>
    {
        IGoogleApiService _googleApiService;
        Func<PizzernatorDataContext> _dataContextLocator;
        INavigationService _navigationService;
        MainPageView _view;
        GeoCoordinate _gpsLocation;

        public MainPageViewModel() { }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IGoogleApiService googleApiService, Func<PizzernatorDataContext> dataContextLocator)
            : base(eventAggregator)
        {
            Restaurants = new BindableCollection<Restaurant>();
            _googleApiService = googleApiService;
            _dataContextLocator = dataContextLocator;
            _navigationService = navigationService;

            ProgressBarVisibility = Visibility.Collapsed;

            LoadRestaurants();
            LoadFavourites();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _view = view as MainPageView;
        }

        #region bindable properties
        #region ProgressBarVisibility

        private Visibility _progressBarVisibility;

        public Visibility ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                if (_progressBarVisibility != value)
                {
                    _progressBarVisibility = value;
                    NotifyOfPropertyChange(() => ProgressBarVisibility);
                }
            }
        }
        #endregion

        #region Restaurants

        private BindableCollection<Restaurant> _restaurants;

        public BindableCollection<Restaurant> Restaurants
        {
            get
            {
                return _restaurants;
            }
            set
            {
                if (_restaurants != value)
                {
                    _restaurants = value;
                    NotifyOfPropertyChange(() => Restaurants);
                }
            }
        }
        #endregion

        #region Favourites

        private BindableCollection<Restaurant> _favourites;

        public BindableCollection<Restaurant> Favourites
        {
            get
            {
                return _favourites;
            }
            set
            {
                if (_favourites != value)
                {
                    _favourites = value;
                    NotifyOfPropertyChange(() => Favourites);
                }
            }
        }
        #endregion
        #endregion

        public void ShowPreview(Restaurant restaurant)
        {
            string picture = restaurant.ImageSource.Replace("maxwidth=70", "maxwidth=460").Replace("maxheight=70", "");
            _navigationService.UriFor<RestaurantPreviewViewModel>().WithParam(vm => vm.Picture, picture).Navigate();
        }

        public void LoadRestaurants()
        {
            ClearMap();
            ProgressBarVisibility = Visibility.Visible;
            Restaurants.Clear();

            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                Geolocator geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 100;

                try
                {
                    Geoposition geoPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60));
                    _gpsLocation = new GeoCoordinate(geoPosition.Coordinate.Latitude, geoPosition.Coordinate.Longitude);

                    _googleApiService.SendGetPlacesRequest(geoPosition.Coordinate.Latitude, geoPosition.Coordinate.Longitude, 2000, "pizza,pizzeria");
                }
                catch (Exception)
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                    MessageBox.Show("Błąd podczas pobierania lokalizacji. Sprawdź ustawienia telefonu.");
                }
            });
        }

        #region Favourites operations

        public Task LoadFavourites()
        {
            return Task.Run(() =>
            {
                using (var dataContext = _dataContextLocator())
                {
                    Favourites = new BindableCollection<Restaurant>(dataContext.Favourites.ToList());
                }
            });
        }

        public Task AddToFavourite(Restaurant restaurant)
        {
            return Task.Run(() =>
            {
                using (var dataContext = _dataContextLocator())
                {
                    if (!dataContext.Favourites.Any(r => r.Id == restaurant.Id))
                    {
                        dataContext.Favourites.InsertOnSubmit(restaurant);
                        dataContext.SubmitChanges();
                    }
                }

                LoadFavourites();
            });
        }

        public Task RemoveFavourite(Restaurant restaurant)
        {
            return Task.Run(() =>
            {
                using (var dataContext = _dataContextLocator())
                {
                    var toRemove = dataContext.Favourites.FirstOrDefault(r => r.Id == restaurant.Id);
                    if (toRemove != null)
                    {
                        dataContext.Favourites.DeleteOnSubmit(toRemove);
                        dataContext.SubmitChanges();
                    }
                }

                LoadFavourites();
            });
        }

        #endregion

        #region IHandle

        public void Handle(PlacesReceivedEvent message)
        {
            ProgressBarVisibility = Visibility.Collapsed;

            if (!message.Success)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania restauracji.");
                return;
            }

            message.Restaurants.Apply(r => r.Distance = _gpsLocation.GetDistanceTo(r.GeoCoordinate));
            message.Restaurants.Sort((r1, r2) => r1.Distance.CompareTo(r2.Distance));
            Restaurants = new BindableCollection<Restaurant>(message.Restaurants);
            UpdateMap();
        }

        #endregion

        #region Map

        private void UpdateMap()
        {
            Map map = _view.Map;
            map.SetView(_gpsLocation, 15);

            map.Layers.Clear();
            MapLayer layer = new MapLayer();            

            foreach (var r in Restaurants)
            {
                MapOverlay overlay = new MapOverlay();                
                overlay.Content = new Pushpin() { Content = r.Name };
                overlay.GeoCoordinate = r.GeoCoordinate;    
            
                layer.Add(overlay);                
            }

            layer.Add(new MapOverlay() { Content = new UserLocationMarker(), GeoCoordinate = _gpsLocation });
            map.Layers.Add(layer);
        }

        private void ClearMap()
        {
            if (_view != null)
                _view.Map.Layers.Clear();
        }

        #endregion
    }
}
