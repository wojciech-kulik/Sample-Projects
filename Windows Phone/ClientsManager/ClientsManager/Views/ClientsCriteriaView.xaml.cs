using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using ClientsManager.ViewModels;

namespace ClientsManager.Views
{
    public partial class ClientsCriteriaView : PhoneApplicationPage
    {
        public ClientsCriteriaView()
        {
            InitializeComponent();
        }

        protected void btnClearDatesClick(object sender, EventArgs e)
        {
            dpBeginDate.Value = null;
            dpEndDate.Value = null;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            (DataContext as ClientsCriteriaViewModel).SaveData();
            base.OnNavigatedFrom(e);
        }
    }
}