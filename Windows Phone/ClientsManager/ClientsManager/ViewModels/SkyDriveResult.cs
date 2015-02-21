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

namespace ClientsManager.ViewModels
{
    public class SkyDriveResult<T>
    {
        public bool Succeded
        {
            get
            {
                return Exception == null;
            }
        }
        public Exception Exception { get; set; }
        public T Result { get; set; }
    }
}
