using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace ClientsManager.Storage
{
    public interface IDataContextWrapper : IDisposable
    {
        List<T> Table<T>() where T : class;
        void DeleteAllOnSubmit<T>(IEnumerable<T> entities) where T : class;
        void DeleteOnSubmit<T>(T entity) where T : class;
        void InsertOnSubmit<T>(T entity) where T : class;
        void SubmitChanges();

        DataLoadOptions LoadOptions { get; set; }
    }
}
