using Pizzernator.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzernator.Storage
{
    public class PizzernatorDataContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/Pizzernator.sdf";

        public PizzernatorDataContext() : base(DBConnectionString)
        {
            if (!DatabaseExists())
                CreateDatabase();
        }

        public Table<Restaurant> Favourites;
    }
}
