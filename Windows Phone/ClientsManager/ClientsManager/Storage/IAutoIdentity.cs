using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientsManager.Storage
{
    public interface IAutoIdentity
    {
        long Id { get; set; }
    }
}
