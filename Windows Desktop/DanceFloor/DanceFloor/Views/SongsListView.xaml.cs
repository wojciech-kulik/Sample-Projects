using Caliburn.Micro;
using DanceFloor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DanceFloor.Views
{
    /// <summary>
    /// Interaction logic for SongsListView.xaml
    /// </summary>
    public partial class SongsListView : UserControl
    {
        public SongsListView()
        {
            InitializeComponent();
            Loaded += SongsListView_Loaded;
            Unloaded += SongsListView_Unloaded;
        }

        void SongsListView_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.PreviewKeyDown -= songsList.SongsList_HandleKeyDown;
            Application.Current.MainWindow.PreviewKeyUp -= songsList.SongsList_HandleKeyUp;
        }

        void SongsListView_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.PreviewKeyDown += songsList.SongsList_HandleKeyDown;
            Application.Current.MainWindow.PreviewKeyUp += songsList.SongsList_HandleKeyUp;
        }

    }
}
