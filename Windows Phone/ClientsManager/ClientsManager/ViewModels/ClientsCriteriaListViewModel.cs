using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ClientsManager.Storage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Device.Location;
using ClientsManager.Location;

namespace ClientsManager.ViewModels
{
    public class ClientsCriteriaListViewModel : BaseViewModel
    {
        private object _syncRoot = new object();
        private GeoLocation geoLocation = new GeoLocation();
        private GeoCoordinate currentPosition;

        public ClientsCriteriaListViewModel(INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
            Clients = new BindableCollection<Client>();
        }

        #region bindable properties

        #region BeginDate

        private DateTime _beginDate;

        public DateTime BeginDate
        {
            get
            {
                return _beginDate;
            }
            set
            {
                if (_beginDate != value)
                {
                    _beginDate = value;
                    NotifyOfPropertyChange(() => BeginDate);
                }
            }
        }
        #endregion

        #region EndDate

        private DateTime _endDate;

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    NotifyOfPropertyChange(() => EndDate);
                }
            }
        }
        #endregion

        #region Distance

        private double _distance;

        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    NotifyOfPropertyChange(() => Distance);
                }
            }
        }
        #endregion

        #region Clients

        private BindableCollection<Client> _clients;

        public BindableCollection<Client> Clients
        {
            get
            {
                return _clients;
            }
            set
            {
                if (_clients != value)
                {
                    _clients = value;
                    NotifyOfPropertyChange(() => Clients);
                }
            }
        }

        #endregion

        #endregion

        #region lifecycle

        protected override void OnActivate()
        {
            base.OnActivate();
            RefreshCoordinatesAndClients();
        }

        #endregion

        #region operations

        public void RefreshCoordinatesAndClients(System.Action callback = null)
        {
            geoLocation.GetCurrentCoordinates(coord => 
            {
                currentPosition = coord;
                RefreshClients(() =>
                    {
                        if (callback != null) callback();
                    });
            });
        }

        private void RefreshClients(System.Action callback = null)
        {
            //potentially time consuming loading
            Task.Run(() =>
            {
                lock (_syncRoot)
                {
                    Clients.Clear();
                }

                using (IDataContextWrapper dataContext = _dataContextLocator())
                {
                    var clientsList = dataContext.Table<Client>().AsEnumerable();

                    clientsList.Apply(c =>
                    {
                        if (c.Longitude != null && c.Latitude != null)
                            c.Distance = geoLocation.CalculateDistance(currentPosition, (double)c.Latitude, (double)c.Longitude);
                    });

                    if (BeginDate.Ticks > 0)
                        clientsList = clientsList.Where(c => c.Birthdate >= BeginDate);
                    if (EndDate.Ticks > 0)
                        clientsList = clientsList.Where(c => c.Birthdate <= EndDate);
                    if (Distance < Int32.MaxValue)
                        clientsList = clientsList.Where(c => c.Distance <= Distance);

                    lock (_syncRoot)
                    {
                        Clients = new BindableCollection<Client>(clientsList);
                        if (callback != null) callback();
                    }
                }
            });
        }

        #endregion

    }
}
