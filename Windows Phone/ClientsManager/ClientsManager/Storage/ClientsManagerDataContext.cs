using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Linq;
using System.Collections.Generic;

namespace ClientsManager.Storage
{
    public class ClientsManagerDataContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/ClientsManager.sdf";

        public ClientsManagerDataContext() : base(ClientsManagerDataContext.DBConnectionString)
        {
           
        }

        public Table<Client> Clients;

        public Table<Group> Groups;
    }
}
