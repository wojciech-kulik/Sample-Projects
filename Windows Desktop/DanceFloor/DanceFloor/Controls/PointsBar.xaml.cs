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

namespace DanceFloor.Controls
{
    /// <summary>
    /// Interaction logic for PointsBar.xaml
    /// </summary>
    public partial class PointsBar : UserControl
    {
        public PointsBar()
        {
            InitializeComponent();
        }



        public string Points
        {
            get { return (string)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(string), typeof(PointsBar), new PropertyMetadata("0"));


    }
}
