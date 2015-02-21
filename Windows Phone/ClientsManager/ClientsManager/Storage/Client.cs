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
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace ClientsManager.Storage
{
    [Table]
    public class Client : EntityBase, IAutoIdentity
    {
        #region Id
        private long _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
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

        #region Forename

        private string _forename;

        [Column]
        public string Forename
        {
            get
            {
                return _forename;
            }
            set
            {
                if (_forename != value)
                {
                    NotifyPropertyChanging("Forename");
                    _forename = value;
                    NotifyPropertyChanged("Forename");
                }
            }
        }
        #endregion

        #region Surname

        private string _surname;

        [Column]
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                if (_surname != value)
                {
                    NotifyPropertyChanging("Surname");
                    _surname = value;
                    NotifyPropertyChanged("Surname");
                }
            }
        }
        #endregion

        #region IsActive

        private bool _isActive;

        [Column(DbType = "bit DEFAULT 0 NOT NULL")]
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    NotifyPropertyChanging("IsActive");
                    _isActive = value;
                    NotifyPropertyChanged("IsActive");
                }
            }
        }
        #endregion

        #region Birthdate

        private DateTime? _birthdate;

        [Column(CanBeNull=true, DbType="datetime")]
        public DateTime? Birthdate
        {
            get
            {
                return _birthdate;
            }
            set
            {
                if (_birthdate != value)
                {
                    NotifyPropertyChanging("Birthdate");
                    _birthdate = value;
                    NotifyPropertyChanged("Birthdate");
                }
            }
        }
        #endregion

        #region Longitude

        private double? _longitude;

        [Column]
        public double? Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    NotifyPropertyChanging("Longitude");
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }
        #endregion

        #region Latitude

        private double? _latitude;

        [Column]
        public double? Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    NotifyPropertyChanging("Latitude");
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }
        #endregion

        #region FullName

        public string FullName
        {
            get
            {
                return string.Join(" ", Forename, Surname);
            }
        }

        #endregion

        #region GroupId

        private long? _groupId;

        [Column]
        public long? GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                if (_groupId != value)
                {
                    NotifyPropertyChanging("GroupId");
                    _groupId = value;
                    NotifyPropertyChanged("GroupId");
                }
            }
        }
        #endregion

        #region Group

        private EntityRef<Group> _groupRef = new EntityRef<Group>();

        [Association(Name = "FK_Group_Clients", Storage = "_groupRef", ThisKey = "GroupId", OtherKey = "Id", IsForeignKey = true)]
        public Group Group
        {
            get
            {
                return _groupRef.Entity;
            }
            set
            {
                Group previousValue = _groupRef.Entity;
                if (((previousValue != value) || (_groupRef.HasLoadedOrAssignedValue == false)))
                {
                    _groupRef.Entity = value;

                    //remove client from previous group
                    if (previousValue != null)
                        previousValue.Clients.Remove(this);                    

                    //add client to the new group
                    if ((value != null))
                    {
                        value.Clients.Add(this);
                        this.GroupId = value.Id;
                    }
                    else
                        this.GroupId = default(Nullable<long>);
                }
            }
        }
        #endregion

        #region Photo

        private string _photo;

        [Column]
        public string Photo
        {
            get
            {
                return _photo;
            }
            set
            {
                if (_photo != value)
                {
                    NotifyPropertyChanging("Photo");
                    _photo = value;
                    NotifyPropertyChanged("Photo");
                }
            }
        }
        #endregion

        #region Distance

        private double? _distance;

        public double? Distance
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

    }
}
