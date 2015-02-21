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
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ClientsManager.Storage
{
    public class DataContextWrapper<T> : IDataContextWrapper where T : DataContext, new()
    {
        private readonly T db;
        private bool _disposed;

        public DataContextWrapper()
        {
            var t = typeof(T);
            db = (T)Activator.CreateInstance(t);
        }

        #region IDataContextWrapper Members

        public List<TableName> Table<TableName>() where TableName : class
        {
            var table = (Table<TableName>)db.GetTable(typeof(TableName));

            return table.ToList();
        }

        public void DeleteAllOnSubmit<Entity>(IEnumerable<Entity> entities) where Entity : class
        {
            db.GetTable(typeof(Entity)).DeleteAllOnSubmit(entities);
        }

        public void DeleteOnSubmit<Entity>(Entity entity) where Entity : class
        {
            db.GetTable(typeof(Entity)).DeleteOnSubmit(entity);
        }

        public void InsertOnSubmit<Entity>(Entity entity) where Entity : class
        {
            db.GetTable(typeof(Entity)).InsertOnSubmit(entity);
        }

        public void SubmitChanges()
        {
            db.SubmitChanges();
        }

        public DataLoadOptions LoadOptions
        {
            get
            {
                return db.LoadOptions;
            }
            set
            {
                db.LoadOptions = value;
            }
        }

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                db.Dispose();
                _disposed = true;
            }
        }
    }
}
