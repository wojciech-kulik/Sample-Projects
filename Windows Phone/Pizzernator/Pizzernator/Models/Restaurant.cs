using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzernator.Models
{
    [Table]
    public class Restaurant : EntityBase
    {
        #region Id

        private string _id;

        [Column(IsPrimaryKey=true)]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        #endregion

        #region Name

        private string _name;

        [Column]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        #endregion

        #region Address

        private string _address;

        [Column]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (_address != value)
                {
                    NotifyPropertyChanging("Address");
                    _address = value;
                    NotifyPropertyChanged("Address");
                }
            }
        }
        #endregion

        #region ImageSource

        private string _imageSource;

        [Column]
        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (_imageSource != value)
                {
                    NotifyPropertyChanging("ImageSource");
                    _imageSource = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }
        #endregion


        //not stored in database
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
                    NotifyPropertyChanging("Distance");
                    _distance = value;
                    NotifyPropertyChanged("Distance");
                }
            }
        }
        #endregion

        #region Latitude

        private double _latitude;

        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    NotifyPropertyChanging("GeoCoordinate");
                    NotifyPropertyChanging("Latitude");
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                    NotifyPropertyChanged("GeoCoordinate");
                }
            }
        }
        #endregion

        #region Longitude

        private double _longitude;

        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    NotifyPropertyChanging("GeoCoordinate");
                    NotifyPropertyChanging("Longitude");
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                    NotifyPropertyChanged("GeoCoordinate");
                }
            }
        }
        #endregion

        #region GeoCoordinate

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return new GeoCoordinate(Latitude, Longitude);
            }

            set
            {
            }
        }
        #endregion
    }
}
