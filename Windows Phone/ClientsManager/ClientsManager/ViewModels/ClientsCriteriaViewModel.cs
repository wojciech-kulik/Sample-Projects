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
using ClientsManager.Tombstoning;

namespace ClientsManager.ViewModels
{
    public class ClientsCriteriaViewModel : BaseViewModel
    {
        private object _syncRoot = new object();
        public ClientsCriteriaViewModel (INavigationService navigationService, Func<IDataContextWrapper> dataContextLocator)
            : base(navigationService, dataContextLocator)
        {
            LoadState();
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

        #endregion

        #region lifecycle

        protected override void LoadState()
        {
            if (TombstoningContainer.HasValue(TombstoningVariables.ClientsCriteriaBeginDate))
            {
                BeginDate = (DateTime)TombstoningContainer.GetValue(TombstoningVariables.ClientsCriteriaBeginDate);
                EndDate = (DateTime)TombstoningContainer.GetValue(TombstoningVariables.ClientsCriteriaEndDate);
                Distance = (double)TombstoningContainer.GetValue(TombstoningVariables.ClientsCriteriaDistance);
            }

        }

        protected override void SaveState()
        {
            TombstoningContainer.SetValue(TombstoningVariables.ClientsCriteriaBeginDate, BeginDate);
            TombstoningContainer.SetValue(TombstoningVariables.ClientsCriteriaEndDate, EndDate);
            TombstoningContainer.SetValue(TombstoningVariables.ClientsCriteriaDistance, Distance);
        }

        public void SaveData()
        {
            SaveState();
        }
        #endregion

        #region operations

        public void SearchClients(string MaxDistance, DateTime? BeginDate, DateTime? EndDate)
        {
            if (BeginDate.HasValue && EndDate.HasValue && BeginDate > EndDate)
            {
                MessageBox.Show("Incorrect date of birth range: first date is later then the second.");
                return;
            }

            double dMaxDistance = String.IsNullOrEmpty(MaxDistance) ? Int32.MaxValue : Double.Parse(MaxDistance);

            _navigationService.UriFor<ClientsCriteriaListViewModel>()
                .WithParam(x => x.BeginDate, BeginDate.HasValue ? BeginDate.Value.Date : BeginDate) //don't pass time
                .WithParam(x => x.EndDate, EndDate.HasValue ? EndDate.Value.Date : EndDate)
                .WithParam(x => x.Distance, dMaxDistance).Navigate();
        }

        #endregion
    }
}
