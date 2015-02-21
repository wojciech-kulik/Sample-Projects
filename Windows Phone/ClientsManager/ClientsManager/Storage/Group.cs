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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace ClientsManager.Storage
{
    [Table]
    public class Group : EntityBase, IAutoIdentity
    {
        public Group() : base()
        {
            _clientRefs = new EntitySet<Client>(OnClientAdded, OnClientRemoved);
        }

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

        #region GroupName

        private string _groupName;

        [Column]
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != value)
                {
                    NotifyPropertyChanging("GroupName");
                    _groupName = value;
                    NotifyPropertyChanged("GroupName");
                }
            }
        }
        #endregion

        #region Clients

        private EntitySet<Client> _clientRefs;

        [Association(Name = "FK_Group_Clients", Storage = "_clientRefs", ThisKey = "Id", OtherKey = "GroupId")]
        public EntitySet<Client> Clients
        {
            get { return _clientRefs; }
        }

        private void OnClientAdded(Client client)
        {
            client.Group = this;
        }

        private void OnClientRemoved(Client client)
        {
            client.Group = null;
        }

        #endregion
    }
}
