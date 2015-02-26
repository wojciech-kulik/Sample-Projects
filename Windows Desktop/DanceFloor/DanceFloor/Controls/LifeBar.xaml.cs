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
    /// Interaction logic for LifeBar.xaml
    /// </summary>
    public partial class LifeBar : UserControl
    {
        public LifeBar()
        {
            InitializeComponent();
            SizeChanged += LifeBar_SizeChanged;
        }

        void LifeBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetLife(life);
        }


        double life = 100.0;

        public void SetLife(double percent)
        {
            percent = Math.Max(0, percent);
            life = percent;
            bState.Width = ActualWidth * percent / 100;
        }
    }
}
