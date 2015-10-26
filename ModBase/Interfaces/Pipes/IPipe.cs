using ModBase.Configuration.Properties;
using ModBase.Modules;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBase.Interfaces.Pipes
{
  public interface IPipe<T> : IQueuePipe<T>, IList<T>
  {
    [Configure(InitType = typeof(List<>))]
    IList<T> Data
    {
      get;
      set;
    }
  }
}
