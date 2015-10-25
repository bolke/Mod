using ModBase.Configuration.Properties;
using ModBase.Modules;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces
{
  public interface IPipe<T> : ILockable, IConfigurable, IList<T>, IObjectPipe
  {
    [Configure(InitType = typeof(List<>))]
    IList<T> Data
    {
      get;
      set;
    }

    T Pop();
    bool Push(T element);
  }
}
