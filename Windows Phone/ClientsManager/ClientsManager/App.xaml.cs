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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ClientsManager.Storage;

namespace ClientsManager
{
    public partial class App : Application
    {

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            using (ClientsManagerDataContext database = new ClientsManagerDataContext())
            {
                if (!database.DatabaseExists())
                {
                    database.CreateDatabase();
                }
            }
            // Standard Silverlight initialization
            InitializeComponent();            
        }
    }
}