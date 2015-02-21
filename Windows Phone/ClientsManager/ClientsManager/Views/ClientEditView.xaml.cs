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
using ClientsManager.Location;

namespace ClientsManager.Views
{
    public partial class ClientEditView : PhoneApplicationPage
    { 
        public ClientEditView()
        {
            InitializeComponent();            
        }

        private void btnCoordinatesClick(object sender, RoutedEventArgs e)
        {
            GeoLocation location = new GeoLocation();
            location.GetCurrentCoordinates(coord =>
                {
                    tbLatitude.Text = coord.Latitude.ToString();
                    tbLongitude.Text = coord.Longitude.ToString();
                });
        }

        private void btnDeletePhotoClick(object sender, RoutedEventArgs e)
        {
            photo.Source = null;
        }

        protected void btnClearBirthdateClick(object sender, EventArgs e)
        {
            dpBirthdate.Value = null;
        }

        private void tbLatitude_KeyDown(object sender, KeyEventArgs e)
        {           
            if (!((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Unknown))
                e.Handled = true;
        }
    }
}