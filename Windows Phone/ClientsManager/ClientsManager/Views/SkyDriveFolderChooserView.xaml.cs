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
using ClientsManager.ViewModels;
using Caliburn.Micro;
using Microsoft.Phone.Shell;

namespace ClientsManager.Views
{
    public partial class SkyDriveFolderChooserView : PhoneApplicationPage
    {
        public SkyDriveFolderChooserView()
        {
            InitializeComponent();            
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {         
            (DataContext as SkyDriveFolderChooserViewModel).SkyDriveGoBack();
            e.Cancel = true;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnSignin.SessionChanged += (DataContext as SkyDriveFolderChooserViewModel).btnSignin_SessionChanged;            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton btnSave = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (btnSave != null)
            {
                bool loading = (sender as CheckBox).IsChecked ?? false;
                btnSave.IsEnabled = !loading;
            }
        }
    }
}